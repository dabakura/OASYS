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
    public class NoticiasController : Controller
    {
        private BD_OASYS db = new BD_OASYS();

        // GET: Noticias
        public ActionResult Index()
        {
            return View(db.Noticias.ToList());
        }

        // GET: Noticias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Noticias noticias = db.Noticias.Find(id);
            if (noticias == null)
            {
                return HttpNotFound();
            }
            return View(noticias);
        }

        // GET: Noticias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Noticias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDNoticia,texto,usuarioCrea,fechaModifica,usuarioModifica,FechaRegistro,Indicador_Activo")] Noticias noticias)
        {
            noticias.FechaRegistro = DateTime.Now;
            noticias.Indicador_Activo = true;
            noticias.usuarioCrea= (int)Session["IdUsuario"];
            if (ModelState.IsValid)
            {
                db.Noticias.Add(noticias);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(noticias);
        }

        // GET: Noticias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Noticias noticias = db.Noticias.Find(id);
            if (noticias == null)
            {
                return HttpNotFound();
            }
            return View(noticias);
        }

        // POST: Noticias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDNoticia,texto,usuarioCrea,fechaModifica,usuarioModifica,FechaRegistro,Indicador_Activo")] Noticias noticias)
        {
            noticias.fechaModifica = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(noticias).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(noticias);
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
