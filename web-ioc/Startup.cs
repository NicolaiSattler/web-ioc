using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

[assembly: OwinStartup(typeof(web_ioc.Startup))]
namespace web_ioc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //AuthConfig.Setup(app);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Any connection or hub wire up and configuration should go here
            IocConfig.SetupContainer(app);
        }
    }
}