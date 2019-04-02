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
    public class BusquedaAdminController : Controller
    {
        private BD_OASYS db = new BD_OASYS();

        public ViewResult Index(string searchString)
        {
           
            var persona = from s in db.Persona
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                persona = persona.Where(s => s.nombre.Contains(searchString)
                                       || s.correo.Contains(searchString));
            }
     
            return View(persona.ToList());
        }
    }
}
