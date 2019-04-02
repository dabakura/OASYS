using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OASYS.Models
{
    public class Borrado
    {
        public string Mensaje { get { return "Se a anulado su Factura"; } set => Mensaje = value; }
        public int IdMatrucula { get; set; }
        public int IdEstudiante { get; set; }
    }
}