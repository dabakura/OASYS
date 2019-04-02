using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using OASYS.Models;

namespace OASYS.Controllers
{
    public class ContabilidadController : Controller
    {
        private BD_OASYS db = new BD_OASYS();

        // GET: Contabilidad
        public ActionResult Index()
        {
            var facturacion = db.Facturacion.Include(f => f.Persona).Include(f => f.Matricula).Include(f => f.TIPOPAGO);
            return View(facturacion.ToList());
        }

        // GET: Contabilidad/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Facturacion facturacion = db.Facturacion.Find(id);
            if (facturacion == null)
            {
                return HttpNotFound();
            }
            return View(facturacion);
        }

        // GET: Contabilidad/Create
        public ActionResult Create()
        {
            ViewBag.idEstudiante = new SelectList(db.Persona, "IDpersona", "cedula");
            ViewBag.IdMatricula = new SelectList(db.Matricula, "IDmatricula", "IDmatricula");
            ViewBag.TipodePago = new SelectList(db.TIPOPAGO, "ID", "TIPODEPAGO");
            return View();
        }

        // POST: Contabilidad/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDfactura,claveNumerica,idEstudiante,IdMatricula,fechaEmision,telefono,subtotal,total,cantidadCursos,usuarioCrea,FechaRegistro,Nombre,TipodePago,Maestro,Detalle,Indicador_Activo")] Facturacion facturacion)
        {
            if (ModelState.IsValid)
            {
                db.Facturacion.Add(facturacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idEstudiante = new SelectList(db.Persona, "IDpersona", "cedula", facturacion.idEstudiante);
            ViewBag.IdMatricula = new SelectList(db.Matricula, "IDmatricula", "IDmatricula", facturacion.IdMatricula);
            ViewBag.TipodePago = new SelectList(db.TIPOPAGO, "ID", "TIPODEPAGO", facturacion.TipodePago);
            return View(facturacion);
        }

        // GET: Contabilidad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Facturacion facturacion = db.Facturacion.Find(id);
            if (facturacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.idEstudiante = new SelectList(db.Persona, "IDpersona", "cedula", facturacion.idEstudiante);
            ViewBag.IdMatricula = new SelectList(db.Matricula, "IDmatricula", "IDmatricula", facturacion.IdMatricula);
            ViewBag.TipodePago = new SelectList(db.TIPOPAGO, "ID", "TIPODEPAGO", facturacion.TipodePago);
            return View(facturacion);
        }

        // POST: Contabilidad/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDfactura,claveNumerica,idEstudiante,IdMatricula,fechaEmision,telefono,subtotal,total,cantidadCursos,usuarioCrea,FechaRegistro,Nombre,TipodePago,Maestro,Detalle,Indicador_Activo")] Facturacion facturacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facturacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idEstudiante = new SelectList(db.Persona, "IDpersona", "cedula", facturacion.idEstudiante);
            ViewBag.IdMatricula = new SelectList(db.Matricula, "IDmatricula", "IDmatricula", facturacion.IdMatricula);
            ViewBag.TipodePago = new SelectList(db.TIPOPAGO, "ID", "TIPODEPAGO", facturacion.TipodePago);
            return View(facturacion);
        }

        // GET: Contabilidad/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Facturacion facturacion = db.Facturacion.Find(id);
            if (facturacion == null)
            {
                return HttpNotFound();
            }
            return View(facturacion);
        }

        [HttpPost]
        public ActionResult ObtenerFactura(int IdMatrucula, int IdEstudiante, string Mensaje)
        {
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = MantenimientoReport.Instance.FacturaEliminadaPDF(IdMatrucula);
            stream.Seek(0, SeekOrigin.Begin);
            correo(Mensaje, IdEstudiante, stream);
            stream = MantenimientoReport.Instance.FacturaEliminadaPDF(IdMatrucula);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "FacturaAbortada.pdf");
        }

        public void correo(String mensajes, int ID_Estudiante, Stream stream)
        {

            using (SmtpClient cliente = new SmtpClient("smtp.live.com", 587))
            {
                String correo = ObtenerCorreo(ID_Estudiante);
                cliente.EnableSsl = true;
                cliente.Credentials = new NetworkCredential("oasysfactura@hotmail.com", "analisis123");
                MailMessage mensaje = new MailMessage("oasysfactura@hotmail.com", correo, "Factura Electronica - OASYS", mensajes);
                if (stream != null)
                    mensaje.Attachments.Add(new Attachment(stream, "FacturaAbortada.pdf"));
                try
                {
                    cliente.Send(mensaje);
                }
                catch (Exception ex)
                {
                    Convert.ToString(ex);
                }
            }
        }


        public String ObtenerCorreo(int Id)
        {
            BD_OASYS db = new BD_OASYS();
            int id_estudiante = Id;
            var nombre = (from Persona in db.Persona
                          where Persona.IDpersona == id_estudiante
                          select Persona.correo).Single();
            return nombre;
        }

        // POST: Contabilidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Facturacion facturacion = db.Facturacion.Find(id);
            facturacion.Indicador_Activo = false;
            db.SaveChanges();
            NotadeCredito nota = new NotadeCredito();
            nota.IDfactura = facturacion.IDfactura;
            nota.claveNumerica = facturacion.claveNumerica;
            nota.idEstudiante = facturacion.idEstudiante;
            nota.IdMatricula = facturacion.IdMatricula;
            nota.fechaEmision = DateTime.Now;
            nota.telefono = 50511155;
            nota.subtotal = facturacion.total;
            nota.total = facturacion.total;
            nota.FechaRegistro = facturacion.FechaRegistro;
            nota.Nombre = facturacion.Nombre;
            nota.Maestro = facturacion.Maestro;
            nota.Detalle = facturacion.Detalle;
            db.NotadeCredito.Add(nota);
            db.SaveChanges();
            MantenimientoReport.Instance.GetBorrado = new Borrado { IdEstudiante = nota.idEstudiante, IdMatrucula = nota.IdMatricula };
            return RedirectToAction("Elimina","Contabilidad",null);
        }

        public ActionResult Elimina()
        {
            ViewBag.IdEstudiante = MantenimientoReport.Instance.GetBorrado.IdEstudiante;
            ViewBag.Mensaje = MantenimientoReport.Instance.GetBorrado.Mensaje;
            ViewBag.IdMatrucula = MantenimientoReport.Instance.GetBorrado.IdMatrucula;
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public bool ESTADO(int id_fac)
        {
            bool student;

            using (var context = new BD_OASYS())
            {
                var query = from st in context.Facturacion
                            where st.IDfactura == id_fac
                            select st.Indicador_Activo;

                student = Convert.ToBoolean(query.FirstOrDefault());
            }

            return student;

        }
    }
}

