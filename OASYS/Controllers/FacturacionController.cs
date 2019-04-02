using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using OASYS.Models;

namespace OASYS.Controllers
{
    public class FacturacionController : Controller
    {
        public BD_OASYS db = new BD_OASYS();

        // GET: Facturacions

        /// <summary>
        /// Dirige a la pantalla, Recibe el precio, el id de Matricula y el Id del Estudiante
        /// Mauricio  V.8
        /// </summary>
        /// <param name="ID_ESTUDIANTE"></param>
        /// <param name="ID_MATRICULA"></param>
        /// <param name="PRECIO"></param>
        /// <returns></returns>
        public ActionResult Index(int ID_ESTUDIANTE, int ID_MATRICULA, int PRECIO, int Total_cursos)
        {
            ViewBag.TipoUsuario = "ADMIN";
            ViewData["id_est"] = ID_ESTUDIANTE;
            ViewData["id_matricula"] = ID_MATRICULA;
            ViewData["precio"] = PRECIO;
            ViewData["Total_Cursos"] = Total_cursos;
            var facturacion = db.Facturacion.Include(f => f.Persona).Include(f => f.Matricula);

            var gestiondeCursos = from a in db.GestiondeCursos
                                  join c in db.CursoporMatricula on a.IDGestiondeCursos equals c.GestiondeCursos
                                  where a.IDGestiondeCursos == c.GestiondeCursos && c.IdMatricula == ID_MATRICULA
                                  select a;

            gestiondeCursos.ToList();


            return View(facturacion.ToList());
        }

        [HttpPost]
        public ActionResult ObtenerFactura(int matricula, int idEstudiante)
        {
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = MantenimientoReport.Instance.FacturaPDF(matricula);
            stream.Seek(0, SeekOrigin.Begin);
            correo(MantenimientoReport.Instance.Mensaje, idEstudiante, stream);
            stream = MantenimientoReport.Instance.FacturaPDF(matricula);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "Factura.pdf");
        }

        public int agregarFactura(int idMatricula, int idEstudiante, String nombre, String apellido1, String apellido2, int Total, int TotalCursos)
        {
            ViewBag.TipoUsuario = "ADMIN";

            var consulta = (from Facturacion in db.Facturacion
                            select Facturacion.IDfactura).Count();
            int fac = 1;
            String mensaje = "Facturacion Electronica OASYS \n";

            if (consulta == 0)
            {
                Facturacion facturacion = new Facturacion();
                String Clave = ClaveNumerica(fac);
                facturacion.IDfactura = 1;
                facturacion.idEstudiante = idEstudiante;
                facturacion.IdMatricula = idMatricula;
                facturacion.Nombre = nombre + " " + apellido1 + " " + apellido2;
                facturacion.fechaEmision = DateTime.Now;
                facturacion.cantidadCursos = TotalCursos;
                facturacion.Indicador_Activo = true;
                facturacion.claveNumerica = Clave;
                facturacion.usuarioCrea = (int)Session["IdUsuario"];
                facturacion.Indicador_Activo = true;
                facturacion.Maestro = nombre + " " + apellido1 + " " + apellido2;
                facturacion.Detalle = "Cantidad de Cursos" + TotalCursos;
                facturacion.telefono = 51511152;
                facturacion.total = Total;
                facturacion.subtotal = Total;
                facturacion.FechaRegistro = DateTime.Now;

                db.Facturacion.Add(facturacion);
                MantenimientoReport.Instance.Mensaje = mensaje +  "Clave Numerica: " + Clave + "<br/>Nombre:" + nombre + " " + apellido1 + " " + apellido2 + "<br/>Fecha: " + DateTime.Now + "<br/>Cantidad de cursos:" + 1 + "<br/>Total:" + Total;
            }
            else
            {
                var consulta1 = (from Facturacion in db.Facturacion
                                 select Facturacion.IDfactura).Max();
                fac = Convert.ToInt32(consulta1) + 10;


                Facturacion facturacion = new Facturacion();
                String Clave = ClaveNumerica(fac);
                facturacion.IDfactura = fac;
                facturacion.idEstudiante = idEstudiante;
                facturacion.fechaEmision = DateTime.Now;
                facturacion.IdMatricula = idMatricula;
                facturacion.Nombre = nombre + " " + apellido1 + " " + apellido2;
                facturacion.cantidadCursos = 1;
                facturacion.usuarioCrea = 1; 
                facturacion.cantidadCursos = TotalCursos;
                facturacion.Indicador_Activo = true;
                facturacion.Maestro = nombre + " " + apellido1 + " " + apellido2;
                facturacion.Detalle = "Cantidad de Cursos" + TotalCursos;
                facturacion.FechaRegistro = DateTime.Now;
                facturacion.claveNumerica = Clave;
                facturacion.telefono = 51511152;
                facturacion.total = Total;
                facturacion.subtotal = Total;
                db.Facturacion.Add(facturacion);
                MantenimientoReport.Instance.Mensaje = "Numero de factura: " + fac + "<br/>Clave Numerica: " + Clave + "<br/>Nombre:" + nombre + " " + apellido1 + " " + apellido2 + "<br/>Fecha: " + DateTime.Now + "<br/>Cantidad de cursos:" + 1 + "<br/>Total:" + Total;
            }
            db.SaveChanges();
            //correo(mensaje, idEstudiante, null);
            return fac;
        }


        public String Nombre(int Id)
        {
            ViewBag.TipoUsuario = "ADMIN";

            BD_OASYS db = new BD_OASYS();
            var nombre = (from Persona in db.Persona
                          where Persona.IDpersona == Id
                          select Persona.nombre).Single();
            return nombre;
        }// string nombre

        public String Apellido1(int Id)
        {

            BD_OASYS db = new BD_OASYS();
            var nombre = (from Persona in db.Persona
                          where Persona.IDpersona == Id
                          select Persona.apellido1).Single();
            return nombre;
        }
        public String Apellido2(int Id)
        {
            BD_OASYS db = new BD_OASYS();
            var nombre = (from Persona in db.Persona
                          where Persona.IDpersona == Id
                          select Persona.apellido2).Single();
            return nombre;
        }

        public int id_Estudiante(int Id)
        {
            return Id;
        }


        public void correo(String mensajes, int ID_Estudiante,Stream stream)
        {

            using (SmtpClient cliente = new SmtpClient("smtp.live.com", 587))
            {
                string htmlMessage = @"<html>
                         <body> <center>
                         <img src='cid:EmbeddedContent_1' />
                         <h2 style='color: orange'>Facturación Electronica</h2>
                         <br>
                         <h4>OASYS le envia la factura correspondiente a su matricula realisada</h4>
                         <br>
                         <b>" + mensajes + "</b>" +
                         "</center></body>" +
                         "</html>";
                String correo = ObtenerCorreo(ID_Estudiante);
                MailMessage mensaje = new MailMessage("oasyscontrasena@hotmail.com",
                                                  correo);
                // Create the HTML view
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                                                             htmlMessage,
                                                             Encoding.UTF8,
                                                             MediaTypeNames.Text.Html);
                // Create a plain text message for client that don't support HTML
                AlternateView plainView = AlternateView.CreateAlternateViewFromString(
                                                            Regex.Replace(htmlMessage,
                                                                          "<[^>]+?>",
                                                                          string.Empty),
                                                            Encoding.UTF8,
                                                            MediaTypeNames.Text.Plain);
                string mediaType = MediaTypeNames.Image.Jpeg;
                string ruta = HostingEnvironment.ApplicationPhysicalPath + "/Assets/img/logo.png";
                LinkedResource img = new LinkedResource(ruta, mediaType);
                //Make sure you set all these values!!!
                img.ContentId = "EmbeddedContent_1";
                img.ContentType.MediaType = mediaType;
                img.TransferEncoding = TransferEncoding.Base64;
                img.ContentType.Name = img.ContentId;
                img.ContentLink = new Uri("cid:" + img.ContentId);
                htmlView.LinkedResources.Add(img);
                mensaje.AlternateViews.Add(plainView);
                mensaje.AlternateViews.Add(htmlView);
                mensaje.IsBodyHtml = true;
                mensaje.Subject = "OASYS - Facturación";
                cliente.EnableSsl = true;
                cliente.Credentials = new NetworkCredential("oasyscontrasena@hotmail.com", "analisis123");

                if(stream!=null)
                    mensaje.Attachments.Add(new Attachment(stream, "Factura.pdf"));
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


        public void pdf(String mensaje)
        {

        }

        public String ObtenerCorreo(int Id)
        {
            BD_OASYS db = new BD_OASYS();


            //var consulta = (from Persona in db.Matricula
            //                where Persona.IDmatricula == Id
            //                select Persona.IdEstudiante).Single();


            int id_estudiante = Id;



            var nombre = (from Persona in db.Persona
                          where Persona.IDpersona == id_estudiante
                          select Persona.correo).Single();



            return nombre;
        }

        public String ClaveNumerica(int ID_FAC)
        {
            int codigo_pais = 506;
            int dia = DateTime.Now.Day;
            int mes = DateTime.Now.Month;
            int year = 19;
            long cedula = 196325789654;
            String numero = Numero(ID_FAC);
            int SEGURIDAD = 189452896;



            String Clave = "" + codigo_pais + dia + mes + year + cedula + numero + SEGURIDAD;


            return Clave;
        }

        public string Clave(int ID_FAC)
        {
            var clave = (from a in db.Facturacion
                         where a.IdMatricula == ID_FAC
                         select a.claveNumerica).Single();
            return clave.ToString();
        }

        public String Numero(int ID_FAC)
        {
            if (ID_FAC < 10)
            {
                return "0000000000000000000" + ID_FAC;
            }

            if (ID_FAC < 100)
            {
                return "000000000000000000" + ID_FAC;
            }
            else

            if (ID_FAC < 1000)
            {
                return "00000000000000000" + ID_FAC;
            }

            if (ID_FAC < 10000)
            {
                return "00000000000000000" + ID_FAC;
            }

            if (ID_FAC < 100000)
            {
                return "0000000000000000" + ID_FAC;
            }


            if (ID_FAC < 1000000)
            {
                return "000000000000000" + ID_FAC;
            }


            if (ID_FAC < 10000000)
            {
                return "00000000000000" + ID_FAC;
            }

            if (ID_FAC < 1000000)
            {
                return "0000000000000" + ID_FAC;
            }

            if (ID_FAC < 10000000)
            {
                return "000000000000" + ID_FAC;
            }
            else
            {
                return "0" + ID_FAC;
            }
        }
    }
}