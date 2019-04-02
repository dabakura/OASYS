using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OASYS.Models;
using System.Data.Entity;
using OASYS.Data;
using System.Net;

namespace OASYS.Controllers
{
    public class AulaController : Controller
    {
        private BD_OASYS db = new BD_OASYS();
        // GET: Aulas
        public ActionResult AulaIndex()
        {
            
            return View(db.Aula.ToList());
        }

        public ActionResult AulaCrear()
        {
            ViewBag.ListaAula = AulaData.ListAula();
            ViewBag.TipoUsuario = "ADMIN";
            return View();
        }

        [HttpPost]
        public ActionResult CreaAula([Bind(Include = "IDaula,nombre,descripcion,usuarioCrea,fechaModifica,usuarioModifica,FechaRegistro,indicador_activo")] Aula aula)
        {
            aula.FechaRegistro = DateTime.Now;
            if (ModelState.IsValid)
            {
                AulaData.CreaAula(aula);
                return RedirectToAction("AulaIndex");
            }

            return View(aula);
        }

        public ActionResult AulaEdita(int id)
        {
            ViewBag.TipoUsuario = "ADMIN";
            Aula aula = AulaData.TraerAula(id);
            if (aula==null)
            {
                HttpNotFound();
            }
            return View(aula);
            
        }

        [HttpPost]
        public ActionResult EditarAula(Aula aula)
        {
            aula.fechaModifica = DateTime.Now;
            if (ModelState.IsValid)
            {
                AulaData.EditaAula(aula);
            }
            return RedirectToAction("AulaIndex");
        }

    }
}