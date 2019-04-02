using OASYS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OASYS.Controllers
{
    public class ReporteController : Controller
    {
        // GET: Reporte
        public ActionResult Facturas()
        {
            var facturas = MantenimientoReport.Instance.TodasLasFacturas();
            return View(facturas);
        }

        [HttpPost]
        public ActionResult Facturas(DateTime desde, DateTime hasta)
        {
            var facturas = MantenimientoReport.Instance.FacturasEntre(desde,hasta);
            return View(facturas);
        }

        
        public ActionResult FacturasDescargar()
        {
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = MantenimientoReport.Instance.FacturasPDF();
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ReporteFactura.pdf");
        }

        public ActionResult Morosos()
        {
            var Morosos = MantenimientoReport.Instance.TodoMorososAdtuales();
            return View(Morosos);
        }

        public ActionResult MorososFechas()
        {
            var Morosos = MantenimientoReport.Instance.TodoMorososFacturas();
            return View(Morosos);
        }

        [HttpPost]
        public ActionResult MorososFechas(DateTime desde, DateTime hasta)
        {
            var Morosos = MantenimientoReport.Instance.FacturasMorososEntre(desde, hasta);
            return View(Morosos);
        }

        public ActionResult MorososDescargar()
        {
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = MantenimientoReport.Instance.MorososPDF();
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ReporteMoroso.pdf");
        }


    }
}