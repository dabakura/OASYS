using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OASYS.Models;
using System.Web.Security;
using System.Net.Mail;

using System.Text;
using System.Web.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace OASYS.Controllers
{
    public class LoginController : Controller
    {
        BD_OASYS db = new BD_OASYS();
        public ActionResult Index(string mensaje)
        {
            ViewBag.Mensaje = mensaje;
            ViewBag.TipoUsuario = "Inicio";
            return View(db.Usuario.ToList());
        }

        public ActionResult Cambiar_Contraseña()
        {
            ViewBag.TipoUsuario = "Inicio";
            return View();
        }

        public ActionResult Confirmacion_Cambio(string correo)
        {
           
            var result = from per in db.Persona
                         where per.correo == correo
                         select per.IDpersona;
            var res = result.FirstOrDefault();
            int Id = Convert.ToInt32(res);

            if (result != null)
            {

                // Specifies the attachment as an embedded image
                // contentid can be any string.

                Usuario user = new Usuario();
                Persona per = new Persona();
                string contrasena = GenerateRandomPassword();
                string contraMd5 = MD5(contrasena);
                per = db.Persona.Find(Id);
                user = db.Usuario.Find(Id);
                user.contraseña = contraMd5;
                db.Entry(user).State = EntityState.Modified;
                per.estado = false;
                db.Entry(per).State = EntityState.Modified;
                db.SaveChanges();
                string mensaje = contrasena;
                //correoenviar(mensaje, correo);
                Enviar_correo(mensaje, correo);
            }
            return View();
        }


        public void Correoenviar(String mensajes, String correo)
        {
            using (SmtpClient cliente = new SmtpClient("smtp.live.com", 587))
            {
                cliente.EnableSsl = true;
                cliente.Credentials = new NetworkCredential("oasyscontrasena@hotmail.com", "analisis123");
                System.Net.Mail.MailMessage mensaje = new System.Net.Mail.MailMessage("oasyscontrasena@hotmail.com", correo, "Confirmacion Cambio - Contraseña", mensajes);
                mensaje.IsBodyHtml = true;
                try
                {
                    cliente.Send(mensaje);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
        }

       
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

        public bool Confirmado(string correo)
        {
            var result =(from per in db.Persona
                         where per.correo == correo
                         select per.estado).Single();

            //var res = result.FirstOrDefault();

            if (result == true)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public ActionResult IniciarSesion(String correo, String contraseña)
        {
            ViewBag.TipoUsuario = "Inicio";

            bool respuesta = Iniciar(correo, MD5(contraseña));
            bool confirmado = Confirmado(correo);

            if (confirmado)
            {
                if (respuesta)
                {
                    int tipo = Tipo_Usuario(correo);
                    int prueba = IdUsuario(correo);

                    Session["IdUsuario"] = db.Persona.Where(x => x.correo == correo).Select(x => x.idUsuario).FirstOrDefault();
                    Session["Rol"] = db.Persona.Where(x => x.correo == correo).Select(x => x.idRol).FirstOrDefault();
                    int rol = (int)Session["Rol"];
                    
                    int us = (int)Session["IdUsuario"];
                   
                    

                    Session["NombreUsuario"] = from user in db.Persona
                                               where (user.correo == correo)
                                               select user.nombre + " " + user.apellido1;

                    if (tipo == 1)
                    {
                        return RedirectToAction("Index", "InicioAdmin");
                    }

                    else if (tipo == 2)
                    {
                        return RedirectToAction("Index", "InicioColab");
                    }

                    else if (tipo == 3)
                    {
                        return RedirectToAction("Index", "InicioProfesor");

                    }

                    else if (tipo == 4)
                    {
                        return RedirectToAction("Index", "InicioEstudiante");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }

                }
                else
                {
                    return RedirectToAction("Index", "Login", new { mensaje = "Error en el Usuario/Contraseña, Favor verifique su Informacion" });
                }

            }
            else
            {
                int ID = IdUsuario(correo);

                if (respuesta)
                {
                    return RedirectToAction("Index", "CambioContraseña", new { mensaje = "", id = ID });

                }
                else
                {
                    return RedirectToAction("Index", "Login", new { mensaje = "Error en el Usuario/Contraseña, Favor verifique su Informacion" });

                }
            }




        }

        public ActionResult CerrarSesion()
        {
            Session.Abandon();

            Session["Rol"] = 0;

            return RedirectToAction("Index","Home");
        }


        public ActionResult Close()
        {
           

            return RedirectToAction("Index", "Home");
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
            LinkedResource img = new LinkedResource(@"C:\Users\maj97\OneDrive\Documents\Oasys - Ultima Version\OASYS_FINAL_NUEVO\O.A.S.Y.S\OASYS\OASYS\Assets\img\brand\LOGO_CONTRASENA.jpg", mediaType);
            // Make sure you set all these values!!!
            img.ContentId = "EmbeddedContent_1";
            img.ContentType.MediaType = mediaType;
            img.TransferEncoding = TransferEncoding.Base64;
            img.ContentType.Name = img.ContentId;
            img.ContentLink = new Uri("cid:" + img.ContentId);
            htmlView.LinkedResources.Add(img);
            //////////////////////////////////////////////////////////////

            msg.AlternateViews.Add(plainView);
            msg.AlternateViews.Add(htmlView);
            msg.IsBodyHtml = true;
            msg.Subject = "OASYS - Contraseña";
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("oasyscontrasena@hotmail.com", "analisis123");

            client.Send(msg);
        }
        public int Tipo_Usuario(string correo)
        {
            var User = from a in db.Persona
                       where (a.correo == correo)
                       select a.idUsuario;


            //var query = from a in db.Usuario
            //            where (a.usuario1 == User)
            //            select a.IDusuario;

            var Id_User = User.First();



            var rol = from a in db.Permiso
                      where (a.idUsuario == Id_User)
                      select a.idRol;


            int Rol = rol.First();

            return Rol;

        }

        public bool Iniciar(String correo, String password)
        {

                var id = from a in db.Persona
                         where (a.correo == correo)
                         select a.idUsuario;

                int User = id.First();


                var contra = from a in db.Usuario
                             where (a.IDusuario == User)
                             select a.contraseña;

                var contrasena = contra.First();


                if (password == contrasena)
                {
                    return true;

                }
                else
                {
                    return false;
                }

        }


        public int IdUsuario(string correo)
        {
            var id = from a in db.Persona
                     where (a.correo == correo)
                     select a.IDpersona;

            int User = id.First();

            return User;
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
