using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using web_ioc.Models;

namespace web_ioc
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }
        protected void Application_Start(object sender, EventArgs e)
        {

        }
        protected void Application_End(object sender, EventArgs e)
        {
            IocConfig.Container.Dispose();
            IocConfig.Container = null;
        }
  
        public void Session_OnStart()
        {
        }
        public void Session_OnEnd()
        {
            (Session["GV::Session"] as ISessionModel)?.Dispose();
            Session["GV::Session"] = null;
        }
    }
}
