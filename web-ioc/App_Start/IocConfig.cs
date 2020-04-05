using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Owin;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using web_ioc.Hubs;
using web_ioc.Models;

namespace web_ioc
{
    public static class IocConfig
    {
        private static string _cookiename;
        internal static IContainer Container { get; set; }

        private static ISessionModel SetupSession<T>(IComponentContext context) where T : class, ISessionModel, new()
        {
            context.TryResolve<ISessionStore>(out var store);

            var cookie = HttpContext.Current.Request.Cookies[SessionName]?.Value;

            ISessionModel session;
            if (!System.Guid.TryParse(cookie, out var sessionID))
            {
                session = CreateSession<T>(store);
            }
            else
            {
                session = store.Contains(sessionID)
                    ? store.Get(sessionID)
                    : CreateSession<T>(store);
            }

            return session;
        }
        private static string SessionName
        {
            get
            {
                if (string.IsNullOrEmpty(_cookiename))
                {
                    _cookiename = ConfigurationManager.AppSettings["sessionCookieName"];
                }
                return _cookiename;
            }
        }
        private static ISessionModel CreateSession<T>(ISessionStore store) where T : class, ISessionModel, new()
        {
            var session = new T();

            store.Set(session);

            if (HttpContext.Current?.Response == null) return null;

            HttpContext.Current.Response.Cookies.Add(new HttpCookie(SessionName, $"{session.Id}")
            {
                Secure = HttpContext.Current.Request.IsSecureConnection,
                Shareable = false,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
            return session;
        }

        private static void SetupMvc(ContainerBuilder builder)
        {
            // register all controllers
            builder.RegisterControllers(typeof(IocConfig).Assembly).InstancePerRequest();

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(typeof(IocConfig).Assembly);
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();
        }
        private static void SetupWebApi(ContainerBuilder builder)
        {
            // Register all API controller
            builder.RegisterApiControllers(typeof(IocConfig).Assembly).InstancePerRequest();
        }
        private static void SetupHubs(ContainerBuilder builder)
        {
            // Register your SignalR hubs.
            builder.RegisterHubs(typeof(IocConfig).Assembly).ExternallyOwned();
        }
        private static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterInstance(GlobalHost.ConnectionManager).As<IConnectionManager>();
            builder.Register(ctx => SetupSession<SessionModel>(ctx)).As<ISessionModel>().ExternallyOwned();
            builder.RegisterType<SessionStore>().As<ISessionStore>().SingleInstance();
            //Hub does not support InstancePerRequest...
            builder.RegisterType<LegendService>().As<ILegendService>().InstancePerLifetimeScope();
        }

        internal static void SetupContainer(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            SetupMvc(builder);
            SetupWebApi(builder);
            SetupHubs(builder);
            RegisterTypes(builder);

            Container = builder.Build();

            DependencyResolver.SetResolver(new Autofac.Integration.Mvc.AutofacDependencyResolver(Container));

            var config = GlobalConfiguration.Configuration;
            config.DependencyResolver = new AutofacWebApiDependencyResolver(Container);

            app.UseAutofacMiddleware(Container);
            //app.UseAutofacMvc();

            var hubconfig = new HubConfiguration
            {
                Resolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(Container)
            };

            GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(Container);

            app.MapSignalR("/signalr", hubconfig);
        }
    }
}