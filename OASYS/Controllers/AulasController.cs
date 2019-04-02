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
    public class AulasController : Controller
    {
        private BD_OASYS db = new BD_OASYS();

        // GET: Aulas
        public ActionResult Index()
        {
            return View(db.Aula.ToList());
        }

        // GET: Aulas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Aulas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDaula,nombre,descripcion,usuarioCrea,fechaModifica,usuarioModifica,FechaRegistro,indicador_activo")] Aula aula)
        {
            aula.indicador_activo = true;
            aula.FechaRegistro = DateTime.Now;
            aula.usuarioCrea = (int)Session["IdUsuario"];
            if (ModelState.IsValid)
            {          
                db.Aula.Add(aula);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aula);
        }

        // GET: Aulas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aula aula = db.Aula.Find(id);
            if (aula == null)
            {
                return HttpNotFound();
            }
            return View(aula);
        }

        // POST: Aulas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDaula,nombre,descripcion,usuarioCrea,fechaModifica,usuarioModifica,FechaRegistro,indicador_activo")] Aula aula)
        {
            aula.usuarioModifica= (int)Session["IdUsuario"];
            aula.fechaModifica = DateTime.Now;
            if (ModelState.IsValid)
            {
               
                db.Entry(aula).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aula);
        }
    }
}
