using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Owin;
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
        internal static IContainer Container { get; set; }

        private static ISessionModel SetupSession(IComponentContext context)
        {
            context.TryResolve<ISessionStore>(out var store);

            ISessionModel session = null;
            var cookie = HttpContext.Current.Request.Cookies["iocHUB"]?.Value;

            if (!System.Guid.TryParse(cookie, out var sessionID))
            {
                session = CreateSession(store);
            }
            else
            {
                session = store.Contains(sessionID) 
                    ? store.Get(sessionID)
                    : CreateSession(store);
            }

            return session;
        }

        private static ISessionModel CreateSession(ISessionStore store)
        {
            var session = new SessionModel(store);
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("iocHUB", $"{session.Id}") 
            {
                Secure = false, 
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
            builder.Register(ctx => SetupSession(ctx)).As<ISessionModel>().ExternallyOwned();
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