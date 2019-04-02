using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OASYS.Models;

namespace OASYS.Controllers
{
    public class InicioColabController : Controller
    {
        BD_OASYS db = new BD_OASYS();
        public ActionResult Index()
        {
            ViewBag.TipoUsuario = "COLAB";
            return View(db.Noticias.ToList());
        }
    }
}