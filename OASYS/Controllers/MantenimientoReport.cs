using CrystalDecisions.CrystalReports.Engine;
using OASYS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OASYS.Controllers
{
    public class MantenimientoReport
    {
        private static MantenimientoReport _instance;

        public BD_OASYS db = new BD_OASYS();

        public static MantenimientoReport Instance => _instance ?? (_instance = new MantenimientoReport());

        private List<View_Facturas> ListaFacturas { get; set; }

        private List<View_Morosos> ListaMorosos { get; set; }

        public Borrado GetBorrado { get; set; }

        private DateTime? Desde { get; set; }

        private DateTime? Hasta { get; set; }

        private Boolean UltimoMes { get; set; }

        public List<View_Facturas> TodasLasFacturas()
        {
            Desde = null;
            Hasta = null;
            UltimoMes = false;
            ListaFacturas = db.View_Facturas.ToList();
            return ListaFacturas;
        }

        public List<View_Facturas> FacturasEntre( DateTime desde, DateTime hasta)
        {
            Desde = desde;
            Hasta = Fecha(hasta);
            var facturas = from f in db.View_Facturas
                           where (DateTime.Compare(f.fechaEmision, Desde.Value) >= 0 && DateTime.Compare(Hasta.Value, f.fechaEmision) >= 0)
                           select f;
            ListaFacturas = facturas.ToList();
            return ListaFacturas;
        }

        public List<View_Morosos> TodoMorososFacturas()
        {
            Desde = null;
            Hasta = null;
            UltimoMes = false;
            DateTime FechaActual = DateTime.Now;
            int dias = FechaActual.Day;
            ListaMorosos = (from m in db.View_Morosos.ToList()
                            where (((FechaActual - m.Ultima_Fecha).Days) > dias)
                            select m).ToList();
            return ListaMorosos;
        }

        public List<View_Morosos> TodoMorososAdtuales()
        {
            Desde = null;
            Hasta = null;
            UltimoMes = true;
            DateTime FechaActual = DateTime.Now;
            int dias = FechaActual.Day;
            ListaMorosos = (from m in db.View_Morosos.ToList()
                            where (m.Ultima_Fecha.Month != FechaActual.Month) && (((FechaActual - m.Ultima_Fecha).Days) <= (dias + 30))
                            select m).ToList();
            return ListaMorosos;
        }

        public List<View_Morosos> FacturasMorososEntre(DateTime desde, DateTime hasta)
        {
            Desde = desde;
            Hasta = Fecha(hasta);
            DateTime FechaActual = DateTime.Now;
            int dias = FechaActual.Day;
            ListaMorosos = (from m in db.View_Morosos.ToList()
                            where (((FechaActual - m.Ultima_Fecha).Days) > dias)
                            select m).ToList();
            var facturas = from f in ListaMorosos
                           where (DateTime.Compare(f.Ultima_Fecha, Desde.Value) >= 0 && DateTime.Compare(Hasta.Value, f.Ultima_Fecha) >= 0)
                           select f;
            ListaMorosos = facturas.ToList();
            return ListaMorosos;
        }

        private DateTime Fecha(DateTime fecha)
        {
            DateTime myfecha = new DateTime(fecha.Year, fecha.Month, fecha.Day, 23, 59,59);
            return myfecha;
        }
        public Stream FacturasPDF()
        {
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(HttpContext.Current.Server.MapPath("~/Report/ReportFacturas.rpt")));
            report.SetDataSource(ListaFacturas);
            if (Desde == null)
            {
                report.SetParameterValue("desde", "Siempre");
                report.SetParameterValue("hasta", DateTime.Now.ToShortDateString());
            } else
            {
                report.SetParameterValue("desde", Desde.Value.ToShortDateString());
                report.SetParameterValue("hasta", Hasta.Value.ToShortDateString());
            }
            return report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        }

        internal class Parametros
        {
            public string idfactura { get; set; }
            public string cliente { get; set; }
            public string fecha { get; set; }
            public string cedula { get; set; }
            public string telefono { get; set; }
            public string correo { get; set; }
            public string clavenumerica { get; set; }
        }

        public Stream FacturaPDF(int matricula)
        {
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(HttpContext.Current.Server.MapPath("~/Report/ReportFactura.rpt")));
            var detalles = (from d in db.View_FacturaDescripcion.ToList()
                            where (d.IDmatricula == matricula)
                            select d).ToList();
            var factura = (from f in db.Facturacion
                           where (f.IdMatricula == matricula)
                           select new Parametros {
                               idfactura = f.IDfactura+"",
                               cliente = f.Persona.nombre + " " + f.Persona.apellido1 + " " +f.Persona.apellido2,
                               fecha = f.fechaEmision.ToString(),
                               cedula = f.Persona.cedula,
                               telefono = f.telefono+"",
                               correo = f.Persona.correo,
                               clavenumerica = f.claveNumerica
                           }).First();
            report.SetDataSource(detalles);
            report.SetParameterValue("idfactura", factura.idfactura);
            report.SetParameterValue("cliente", factura.cliente);
            report.SetParameterValue("fecha", factura.fecha);
            report.SetParameterValue("cedula", factura.cedula);
            report.SetParameterValue("telefono", factura.telefono);
            report.SetParameterValue("correo", factura.correo);
            report.SetParameterValue("clavenumerica", factura.clavenumerica);
            return report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        }

        public Stream FacturaEliminadaPDF(int matricula)
        {
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(HttpContext.Current.Server.MapPath("~/Report/ReportFacturaEliminada.rpt")));
            var detalles = (from d in db.View_FacturaDescripcion.ToList()
                            where (d.IDmatricula == matricula)
                            select d).ToList();
            var factura = (from f in db.Facturacion
                           where (f.IdMatricula == matricula)
                           select new Parametros
                           {
                               idfactura = f.IDfactura + "",
                               cliente = f.Persona.nombre + " " + f.Persona.apellido1 + " " + f.Persona.apellido2,
                               fecha = f.fechaEmision.ToString(),
                               cedula = f.Persona.cedula,
                               telefono = f.telefono + "",
                               correo = f.Persona.correo,
                               clavenumerica = f.claveNumerica
                           }).First();
            report.SetDataSource(detalles);
            report.SetParameterValue("idfactura", factura.idfactura);
            report.SetParameterValue("cliente", factura.cliente);
            report.SetParameterValue("fecha", factura.fecha);
            report.SetParameterValue("cedula", factura.cedula);
            report.SetParameterValue("telefono", factura.telefono);
            report.SetParameterValue("correo", factura.correo);
            report.SetParameterValue("clavenumerica", factura.clavenumerica);
            return report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        }
        public Stream MorososPDF()
        {
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(HttpContext.Current.Server.MapPath("~/Report/ReportMorosos.rpt")));
            report.SetDataSource(ListaMorosos);
            if (UltimoMes)
            {
                report.SetParameterValue("desde", "Ultimo Mes");
                report.SetParameterValue("hasta", DateTime.Now.ToShortDateString());
            }
            else
            {
                if (Desde == null)
                {
                    DateTime fecha = DateTime.Now.AddMonths(-1);
                    DateTime Fecha = new DateTime(fecha.Year, fecha.Month, fecha.Day);
                    report.SetParameterValue("desde", "Siempre");
                    report.SetParameterValue("hasta", Fecha.ToShortDateString());
                }
                else
                {
                    report.SetParameterValue("desde", Desde.Value.ToShortDateString());
                    report.SetParameterValue("hasta", Hasta.Value.ToShortDateString());
                }
            }
            return report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        }
    }
}