using System.Web.Mvc;
using web_ioc.components;
using web_ioc.Models;

namespace web_ioc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILegendService _service;

        public HomeController(ILegendService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            _service.Touched = true;


            return View();
        }

        public ActionResult Logoff()
        {
            Session.Abandon();

            return View("Index");
        }
    }
}
