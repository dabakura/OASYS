using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OASYS.Models;

namespace OASYS.Controllers
{
    public class CambioContraseñaController : Controller
    {
        BD_OASYS db = new BD_OASYS();
        int Id_user = 0;
        // GET: CambioContraseña
        public ActionResult Index(String Mensaje, int ID)
        {

            ViewBag.Mensaje = Mensaje;
            Id_user = ID;
            HttpContext.Session["ID_USER"] = ID;
            return View(db.Usuario.ToList());

        }

        [HttpPost]
        public ActionResult IniciarSesion(String contraseña1, String contraseña2)
        {
            int IdPer= (int)HttpContext.Session["ID_USER"];

            try
            {

                if (contraseña1 == contraseña2)
                {
                    Usuario usuario = new Usuario();
                    Persona persona = new Persona();
                    int IdUs = (from per in db.Persona
                                 where per.IDpersona == IdPer
                                 select per.idUsuario).Single();
                    usuario = (from usu in db.Usuario
                               where usu.IDusuario == IdUs
                               select usu).Single();
                    persona = (from per in db.Persona
                               where per.IDpersona==IdPer
                               select per).Single();

                    if (ModelState.IsValid)
                    {
                        ActualizaContra(usuario,contraseña1);
                        ActualizaEstado(persona);
                        db.SaveChanges();
                        
                    }
                    return RedirectToAction("Index", "Login", new { mensaje = "" });
                }
                else
                {
                    return RedirectToAction("Index", "CambioContraseña", new { mensaje = "Sus Contraseñas no coinciden", id = IdPer });

                }
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "CambioContraseña", new { mensaje = "Ocurrio un error.", id = IdPer });

            }


        }

        public void ActualizaContra(Usuario usuario ,string contraseña )
        {
           
            usuario.contraseña = MD5(contraseña);
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
            }

        }
        public void ActualizaEstado(Persona persona)
        {
 
            persona.estado = true;
            if (ModelState.IsValid)
            {
                db.Entry(persona).State = EntityState.Modified;
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