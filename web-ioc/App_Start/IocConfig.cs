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

        private static I SetupSession<T, I>(IComponentContext context) where T : I, new() where I : ISessionModel
        {
            context.TryResolve<ISessionStore>(out var store);

            var cookie = HttpContext.Current.Request.Cookies[SessionName]?.Value;

            I session;
            if (!System.Guid.TryParse(cookie, out var sessionID))
            {
                session = CreateSession<T, I>(store);
            }
            else
            {
                session = store.Contains(sessionID)
                    ? (I)store.Get(sessionID)
                    : CreateSession<T, I>(store);
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
        private static I CreateSession<T, I>(ISessionStore store) where T : I, new() where I : ISessionModel
        {
            var session = new T();

            store.Set(session);

            if (HttpContext.Current?.Response == null) return default(I);

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
            // in online we have to register the IsessionModel because all the online Components will use this interface.
            // but when we create the session we habe to create it with a GribSession / IGribSession to ensure the
            // controllers / services can use the Grib specific parts.
            builder.Register(ctx => SetupSession<GribSession, IGribSession>(ctx)).As<ISessionModel>().ExternallyOwned();
            // in grib we have to register the IGribSession because all the grib components will use this interface.
            builder.Register(ctx => SetupSession<GribSession, IGribSession>(ctx)).As<IGribSession>().ExternallyOwned();
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