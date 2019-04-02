using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using OASYS.Models;

namespace OASYS.Controllers
{
    public class CrearUsuarioController : Controller
    {
        public static string nombre;
        public static string NombreUsuario;
        public static int IdUsuario;
        public static int IdUsuarioPermiso;
        public static int IdRolPermiso;
        public static int IdRol;
        public static string contraseña = GenerateRandomPassword();


        private BD_OASYS db = new BD_OASYS();

        public static string MD5(string word)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(word));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDusuario,usuario1,contraseña,FechaRegistro,usuarioCrea,fechaModifica,usuarioModifica,Indicador_Activo")] Usuario usuario)
        {
            int IdUsu = (from us in db.Usuario
                         orderby us.IDusuario descending
                         select us.IDusuario).Take(1).Single();
            usuario.IDusuario = IdUsu + 1;
            NombreUsuario = usuario.usuario1;
            nombre = usuario.usuario1;
            usuario.contraseña = MD5(contraseña);
            usuario.FechaRegistro = DateTime.Now;
            usuario.Indicador_Activo = true;
            usuario.usuarioCrea = 1;
            Session["Permitir"] = "Permitido";

            if (ModelState.IsValid)
            {
                db.Usuario.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("CreatePersona");
            }

            return View(usuario);
        }

        public ActionResult CreatePersona()
        {
            ViewData["ID_USUARIOCREA"] = ViewData["ID_USUARIOCREA"];
            ViewBag.idUsuario = new SelectList(db.Usuario, "IDusuario", "usuario1");
            ViewBag.tipo_identificacion = new SelectList(db.TipodeIdentificacion, "ID_Identificacion", "TIPO_IDENTIFICACION");
            return View();
        }

        // POST: Personas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePersona([Bind(Include = "IDpersona,cedula,tipo_identificacion,nombre,apellido1,apellido2,direccion,correo,fechaNacimiento,foto,estado,idUsuario,idRol,fechaRegistro,usuarioCrea,fechaModifica,usuarioModifica,Indicador_Activo")] Persona persona)
        {
            persona.idRol = 4;
            Permiso permiso = new Permiso();
            if (persona.idRol == 1)
            {
                IdRolPermiso = persona.idRol;
                IdRol = persona.idRol;
            }
            else
            {
                if (persona.idRol == 2)
                {
                    IdRolPermiso = persona.idRol;
                    IdRol = persona.idRol;
                }
                else
                {
                    if (persona.idRol == 3)
                    {
                        IdRolPermiso = persona.idRol;
                        IdRol = persona.idRol;
                    }
                    else
                    {
                        IdRolPermiso = persona.idRol;
                        IdRol = persona.idRol;
                    }
                }
            }

            IdUsuario = (from usuario in db.Usuario
                         where usuario.usuario1 == nombre
                         select usuario.IDusuario).Single();

            IdUsuarioPermiso = (from usuario in db.Usuario
                                where usuario.usuario1 == nombre
                                select usuario.IDusuario).Single();
            persona.idUsuario = IdUsuario;
            persona.fechaRegistro = DateTime.Now;
            persona.Indicador_Activo = true;
            persona.estado = false;
            permiso.idUsuario = IdUsuarioPermiso;
            permiso.idRol = IdRolPermiso;
            permiso.FechaRegistro = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Persona.Add(persona);
                db.Permiso.Add(permiso);
                db.SaveChanges();
                Enviar_correo(contraseña, persona.correo);
                return RedirectToAction("Index", "Login");
            }
           
            ViewBag.idUsuario = new SelectList(db.Usuario, "IDusuario", "usuario1", persona.idUsuario);
            ViewBag.tipo_identificacion = new SelectList(db.TipodeIdentificacion, "ID_Identificacion", "TIPO_IDENTIFICACION", persona.tipo_identificacion);
            return View(persona);
        }

        public static void Enviar_correo(String mensajes, String correo)
        {
            string htmlMessage = @"<html>
                         <body> <center>
                         <img src='cid:EmbeddedContent_1' />
                         <h2 style='color: orange'>Cambio de Contraseña</h2>
                         <br>
                         <h4>Hola, le enviamos la nueva contraseña de su cuenta, esta es:</h4>
                         <br>
                         <b>" + mensajes + "</b>" +
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
            //LinkedResource img = new LinkedResource(@"C:\Users\Andrés Zamora\Desktop\O.A.S.Y.S\OASYS\OASYS\Assets\img\logo.png", mediaType);
            // Make sure you set all these values!!!
            //img.ContentId = "EmbeddedContent_1";
            //img.ContentType.MediaType = mediaType;
            //img.TransferEncoding = TransferEncoding.Base64;
            //img.ContentType.Name = img.ContentId;
            //img.ContentLink = new Uri("cid:" + img.ContentId);
            //htmlView.LinkedResources.Add(img);
            //////////////////////////////////////////////////////////////

            msg.AlternateViews.Add(plainView);
            msg.AlternateViews.Add(htmlView);
            msg.IsBodyHtml = true;
            msg.Subject = "OASYS - Contraseña";
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("oasyscontrasena@hotmail.com", "analisis123");

            client.Send(msg);
        }

        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        "abcdefghijkmnopqrstuvwxyz",    // lowercase
        "0123456789",                   // digits
        "!@$?_-"                        // non-alphanumeric
             };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
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

    public class PasswordOptions
    {

        int _RequiredLength;

        public int RequiredLength
        {
            get
            {
                return this._RequiredLength;
            }
            set
            {
                this._RequiredLength = value;
            }
        }

        int _RequiredUniqueChars;
        public int RequiredUniqueChars
        {
            get
            {
                return this._RequiredUniqueChars;
            }
            set
            {
                this._RequiredUniqueChars = value;
            }
        }

        bool _RequireDigit;
        public bool RequireDigit
        {
            get
            {
                return this._RequireDigit;
            }
            set
            {
                this._RequireDigit = value;
            }
        }

        bool _RequireLowercase;
        public bool RequireLowercase
        {
            get
            {
                return this._RequireLowercase;
            }
            set
            {
                this._RequireLowercase = value;
            }
        }

        bool _RequireNonAlphanumeric;
        public bool RequireNonAlphanumeric
        {
            get
            {
                return this._RequireNonAlphanumeric;
            }
            set
            {
                this._RequireNonAlphanumeric = value;
            }
        }


        bool _RequireUppercase;

        public bool RequireUppercase
        {
            get
            {
                return this._RequireUppercase;
            }
            set
            {
                this._RequireUppercase = value;
            }
        }


    }
}