using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    public class MatriculaEstudianteController : Controller
    {
        private BD_OASYS db = new BD_OASYS();
        /// <summary>
        /// Crea la Lista con los datos
        /// Mauricio V.1
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int id_matricula)
        {
            var cursoporMatricula = db.CursoporMatricula.Include(c => c.GestiondeCursos1).Include(c => c.Matricula).Where(c => c.IdMatricula == id_matricula);
            return View(cursoporMatricula.ToList());
        }


        /// <summary>
        /// Retorna el nombre del curso
        /// Mauricio V.1
        /// </summary>
        /// <returns></returns>
        public String Curso(int Id)
        {
            var consulta = (from gestion in db.GestiondeCursos
                            where gestion.IDGestiondeCursos == Id
                            select gestion.IDcurso).Single();
            int id_curso = Convert.ToInt32(consulta);
            var nombre = (from curso in db.Curso
                          where curso.IDcurso == id_curso
                          select curso.nombre).Single();
            return nombre;
        }
        /// <summary>
        /// Retorna el Precio del Curso
        /// Mauricio V.1
        /// </summary>
        /// <returns></returns>
        public int Precio(int Id)
        {

            var consulta = (from gestion in db.GestiondeCursos
                            where gestion.IDGestiondeCursos == Id
                            select gestion.IDcurso).Single();
            int id_curso = Convert.ToInt32(consulta);
            var precio = (from curso in db.Curso
                          where curso.IDcurso == id_curso
                          select curso.precio).Single();
            return precio;
        }
        public ActionResult Confirmacion(int Usuario)
        {

            Confirmar_Prematricula(Usuario);
            return View();
        }
        public void Confirmar_Prematricula(int Usuario)
        {
            String Mensaje = "\b Matricula Realizada  Correctamente \b0 \n Puede proceder a realizar su pago fisicamente en la Institucion \n Gracias.";
            correo(Mensaje, Usuario);
        }
        public void correo(String mensajes, int ID_Estudiante)
        {
            String correo = ObtenerCorreo(ID_Estudiante);

            Enviar_correo(correo);

            //using (SmtpClient cliente = new SmtpClient("smtp.live.com", 587))
            //{
            //    cliente.EnableSsl = true;
            //    cliente.Credentials = new NetworkCredential("oasysconfirmacion@hotmail.com", "analisis123");
            //    System.Net.Mail.MailMessage mensaje = new System.Net.Mail.MailMessage("oasyscontrasena@hotmail.com", correo, "Confirmacion Matriucla", mensajes);
            //    mensaje.IsBodyHtml = true;
            //    try
            //    {
            //        cliente.Send(mensaje);
            //    }
            //    catch (Exception ex)
            //    {
            //        ex.ToString();
            //    }
            //}
        }

        public static void Enviar_correo( String correo)
        {
            string htmlMessage = @"<html>
                         <body> <center>
                         <img src='cid:EmbeddedContent_1' />
                         <h2 style='color: orange'>Confirmacion Matricula</h2>
                         <br>
                         <h4>Hola, por medio de este correo le confirmamos que su pre matricula ha sido confirmada.</h4>
                         <br>
                         <b>Recuerde que debe cancelar la matrícula de forma presencial a más tardar 12:00 am</b>" +
                         "</center></body>" +
                         "</html>";
            SmtpClient client = new SmtpClient("smtp.live.com", 587);

            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage("oasyscontrasena@hotmail.com",
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
            var ruta = HostingEnvironment.ApplicationPhysicalPath + "/Assets/img/brand/Logo - MATRICULA.png";
            LinkedResource img = new LinkedResource(ruta, mediaType);
            // Make sure you set all these values!!!
            img.ContentId = "EmbeddedContent_1";
            img.ContentType.MediaType = mediaType;
            img.TransferEncoding = TransferEncoding.Base64;
            img.ContentType.Name = img.ContentId;
            img.ContentLink = new Uri("cid:" + img.ContentId);
            htmlView.LinkedResources.Add(img);
            ////////////////////////////////////////////////////////////

            msg.AlternateViews.Add(plainView);
            msg.AlternateViews.Add(htmlView);
            msg.IsBodyHtml = true;
            msg.Subject = "OASYS - Matricula";
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("oasyscontrasena@hotmail.com", "analisis123");

            client.Send(msg);
        }


        public String ObtenerCorreo(int Id)
        {


            //var consulta = (from Persona in db.Matricula
            //                where Persona.IDmatricula == Id
            //                select Persona.IdEstudiante).Single();

            int id_estudiante = Id;
            var nombre = (from Persona in db.Persona
                          where Persona.idUsuario == id_estudiante
                          select Persona.correo).Single();
            return nombre;
        }
    }
}
