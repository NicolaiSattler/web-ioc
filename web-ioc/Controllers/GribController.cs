using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web_ioc.Models;

namespace web_ioc.Controllers
{
    public class GribController : Controller
    {
        private IGribService _service;

        public GribController(IGribService gribService)
        {
            _service = gribService;
        }

        // GET: Grib
        public ActionResult Index()
        {
            return View();
        }

        // GET: Grib/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Grib/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Grib/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Grib/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Grib/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Grib/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Grib/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
