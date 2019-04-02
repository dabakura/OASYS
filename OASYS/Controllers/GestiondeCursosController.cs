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
    public class GestiondeCursosController : Controller
    {
        private BD_OASYS db = new BD_OASYS();

        // GET: GestiondeCursos
        public ActionResult Index()
        {
            var gestiondeCursos = db.GestiondeCursos.Include(g => g.Aula).Include(g => g.Curso).Include(g => g.Horario).Include(g => g.Persona);
            return View(gestiondeCursos.ToList());
        }

        // GET: GestiondeCursos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GestiondeCursos gestiondeCursos = db.GestiondeCursos.Find(id);
            if (gestiondeCursos == null)
            {
                return HttpNotFound();
            }
            return View(gestiondeCursos);
        }

        // GET: GestiondeCursos/Create
        public ActionResult Create()
        {
            ViewBag.IDaula = new SelectList(db.Aula, "IDaula", "nombre");
            ViewBag.IDcurso = new SelectList(db.Curso, "IDcurso", "nombre");
            ViewBag.IDhorario = new SelectList(db.Horario, "IDhorario", "dia");
            ViewBag.IDprofesor = new SelectList(db.Persona, "IDpersona", "nombre");
            return View();
        }

        // POST: GestiondeCursos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDGestiondeCursos,IDcurso,IDaula,IDhorario,IDprofesor,fechaRegistro,usuarioCrea,fechaModifica,usuarioModifica,Fechacrea,Indicador_Activo")] GestiondeCursos gestiondeCursos)
        {
            if (ModelState.IsValid)
            {
                gestiondeCursos.Indicador_Activo = true;
                gestiondeCursos.usuarioCrea= (int)Session["IdUsuario"];
                gestiondeCursos.fechaRegistro = DateTime.Now;
                db.GestiondeCursos.Add(gestiondeCursos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDaula = new SelectList(db.Aula, "IDaula", "nombre", gestiondeCursos.IDaula);
            ViewBag.IDcurso = new SelectList(db.Curso, "IDcurso", "nombre", gestiondeCursos.IDcurso);
            ViewBag.IDhorario = new SelectList(db.Horario, "IDhorario", "dia", gestiondeCursos.IDhorario);
            ViewBag.IDprofesor = new SelectList(db.Persona, "IDpersona", "nombre", gestiondeCursos.IDprofesor);
            return View(gestiondeCursos);
        }

        // GET: GestiondeCursos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GestiondeCursos gestiondeCursos = db.GestiondeCursos.Find(id);
            if (gestiondeCursos == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDaula = new SelectList(db.Aula, "IDaula", "nombre", gestiondeCursos.IDaula);
            ViewBag.IDcurso = new SelectList(db.Curso, "IDcurso", "nombre", gestiondeCursos.IDcurso);
            ViewBag.IDhorario = new SelectList(db.Horario, "IDhorario", "dia", gestiondeCursos.IDhorario);
            ViewBag.IDprofesor = new SelectList(db.Persona, "IDpersona", "nombre", gestiondeCursos.IDprofesor);
            return View(gestiondeCursos);
        }

        // POST: GestiondeCursos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDGestiondeCursos,IDcurso,IDaula,IDhorario,IDprofesor,fechaRegistro,usuarioCrea,fechaModifica,usuarioModifica,Fechacrea,Indicador_Activo")] GestiondeCursos gestiondeCursos)
        {
            gestiondeCursos.usuarioModifica= (int)Session["IdUsuario"];
            gestiondeCursos.fechaModifica = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(gestiondeCursos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDaula = new SelectList(db.Aula, "IDaula", "nombre", gestiondeCursos.IDaula);
            ViewBag.IDcurso = new SelectList(db.Curso, "IDcurso", "nombre", gestiondeCursos.IDcurso);
            ViewBag.IDhorario = new SelectList(db.Horario, "IDhorario", "dia", gestiondeCursos.IDhorario);
            ViewBag.IDprofesor = new SelectList(db.Persona, "IDpersona", "nombre", gestiondeCursos.IDprofesor);
            return View(gestiondeCursos);
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
