using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaAcademicoMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //protege la ruta
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}