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
    public class CursosController : Controller
    {
        private BD_OASYS db = new BD_OASYS();

        // GET: Cursos
        public ActionResult Index()
        {
            var curso = db.Curso.Include(c => c.Nivel);
            return View(curso.ToList());
        }

        // GET: Cursos/Create
        public ActionResult Create()
        {
            ViewBag.Id_Nivel = new SelectList(db.Nivel, "Id_Nivel", "nivel1");
            return View();
        }

        // POST: Cursos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDcurso,nombre,descripcion,precio,usuarioCrea,fechaModifica,usuarioModifica,Id_Nivel,Indicador_Activo,FechaRegistro")] Curso curso)
        {
            curso.Indicador_Activo = true;
            curso.fechaModifica = DateTime.Now;
            curso.usuarioCrea= (int)Session["IdUsuario"];

            if (ModelState.IsValid)
            {
                db.Curso.Add(curso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_Nivel = new SelectList(db.Nivel, "Id_Nivel", "nivel1", curso.Id_Nivel);
            return View(curso);
        }

        // GET: Cursos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Curso curso = db.Curso.Find(id);
            if (curso == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_Nivel = new SelectList(db.Nivel, "Id_Nivel", "nivel1", curso.Id_Nivel);
            return View(curso);
        }

        // POST: Cursos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDcurso,nombre,descripcion,precio,usuarioCrea,fechaModifica,usuarioModifica,Id_Nivel,Indicador_Activo,FechaRegistro")] Curso curso)
        {
            curso.usuarioModifica= (int)Session["IdUsuario"];
            curso.fechaModifica = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(curso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_Nivel = new SelectList(db.Nivel, "Id_Nivel", "nivel1", curso.Id_Nivel);
            return View(curso);
        }

        protected override void Dispose(bool disposing)
        {
            
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
