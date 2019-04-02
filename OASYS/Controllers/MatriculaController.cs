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
    public class MatriculaController : Controller
    {
        private BD_OASYS db = new BD_OASYS();
        /// <summary>
        /// Crea la Lista con los datos
        /// Mauricio V.1
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.TipoUsuario = "ESTUD";
            int current = (int)Session["IdUsuario"];
            current = current;
            ViewData["id"] = current;
            var gestiondeCursos = db.GestiondeCursos.Include(g => g.Aula).Include(g => g.Curso).Include(g => g.Horario).Include(g => g.Persona);
            return View(gestiondeCursos.ToList());
        }



        /// <summary>
        /// Verifica si el usuario se encuentra matriculado en ese curso
        /// Mauricio V.1
        /// </summary>
        /// <returns></returns>
        public bool Matriculado(int matricula, int curso)
        {

            ViewBag.TipoUsuario = "ESTUD";

            matricula = matricula;

            int estado = db.CursoporMatricula.Where(d => d.IdMatricula == matricula).Where(d => d.GestiondeCursos == curso).Count();


            estado = estado;

            if (estado >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }


        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Realiza un Insert en la Base de Datos
        /// Mauricio V.1
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(int id_curso, int id_matricula)
        {
            ViewBag.TipoUsuario = "ESTUD";
            CursoporMatricula matricula = new CursoporMatricula();
            matricula.IdMatricula = id_matricula;
            matricula.GestiondeCursos = id_curso;



            db.CursoporMatricula.Add(matricula);

            db.SaveChanges();

            return RedirectToAction("Index");
        }



        /// <summary>
        ///Elimina de la base de datos la matricula
        /// Mauricio V.1
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(int id_curso, int id_matricula)
        {
            ViewBag.TipoUsuario = "ESTUD";

            CursoporMatricula matricula = new CursoporMatricula();

            matricula = db.CursoporMatricula.Where(d => d.IdMatricula == id_matricula)
                .Where(d => d.GestiondeCursos == id_curso).First();
            db.CursoporMatricula.Remove(matricula);

            db.SaveChanges();
            return RedirectToAction("Index");

        }


        public int Control_Id(int ID_USER)
        {
            ID_USER = ID_USER;
            ///Guarda el Id de la Ultima matricula
            int ultima = 0;
            ///Guarda el Id de la Matricula
            int Id_Matricula = 0;
            int ID_Persona = 0;



            ///Query para la Base de Datos
            ///Mauricio V.7
            

            var query2 = from b in db.Persona
                         where b.idUsuario == ID_USER
                         orderby b.IDpersona descending
                         select b.IDpersona;

           

            ID_Persona = query2.Take(1).SingleOrDefault();

            var query1 = from a in db.Matricula
                         where a.IdEstudiante == ID_Persona
                         orderby a.IDmatricula descending
                         select a.IDmatricula;

            ///Llama la ultima matricula que exista del usuario

            try
            {
              ultima = query1.Take(1).SingleOrDefault();

              
            }
            catch (Exception)
            {

                ultima = 0;
            }


            if (ultima == 0)
            {
                ///Significa que no posee niguna matricula, entonces hay que crear una nueva
                Matricula matricula = new Matricula();
               
                matricula.IdEstudiante = ID_Persona;

                db.Matricula.Add(matricula);

                db.SaveChanges();

                var ultimo = from a in db.Matricula
                             where a.IdEstudiante == ID_Persona
                             orderby a.IDmatricula descending
                             select a.IDmatricula;
                Id_Matricula = ultimo.Take(1).SingleOrDefault();


            }
            else
            {
                bool pagado = Pagado(ultima);
              

                if (pagado)
                {
                    ///Si se encuentra la factura pagada se crea un nuevo Id de matricula
                    Matricula matricula = new Matricula();
                    matricula.IdEstudiante = ID_Persona;
                    db.Matricula.Add(matricula);
                    db.SaveChanges();

                    //Se llama el ultimo ID de matricula del Usuario
                    var ultimo = from a in db.Matricula
                                 where a.IdEstudiante == ID_Persona
                                 orderby a.IDmatricula descending
                                 select a.IDmatricula;
                    Id_Matricula = ultimo.Take(1).SingleOrDefault();


                }
                else
                {

                    /// Si no se encuentra la factura se llama la ultima

                    var ultimo = from a in db.Matricula
                                 where a.IdEstudiante == ID_Persona
                                 orderby a.IDmatricula descending
                                 select a.IDmatricula;
                    Id_Matricula = ultimo.Take(1).SingleOrDefault();
                }

            }

            

            return Id_Matricula;
        }

        /// <summary>
        /// Retorna si la matricula se encuntra pagada o no 
        /// </summary>
        /// <param name="matricula"></param>
        /// <returns></returns>
        public bool Pagado(int matricula)
        {
            //guarda si la factura se encuentra pagada o no
            int b = 0;
            var query = from a in db.Facturacion
                        where a.IdMatricula == matricula
                        select a.IDfactura;

            try
            {
                b = query.Count();
            }
            catch (Exception)
            {

                b = 0;
            }

            if (b > 0)
            {
                ///Retorna si la factura se encuentra pagada
                return true;
            }
            else
            {
                ///Retorna si la factura no se encuentra
                return false;
            }
        }

       


    }
}