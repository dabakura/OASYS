using OASYS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace OASYS.Controllers
{
    public class ConfirmaMatriculaController : Controller
    {
        private BD_OASYS db = new BD_OASYS();
        public ActionResult Index(int id_matricula, int id_user)
        {
            ViewData["iduser"] = id_user;
            ViewData["id_matricula"] = id_matricula;
            var cursoporMatricula = db.CursoporMatricula.Include(c => c.GestiondeCursos1).Include(c => c.Matricula).Where(c => c.IdMatricula == id_matricula);
            return View(cursoporMatricula.ToList());
        }

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
    }
}