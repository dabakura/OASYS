//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OASYS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Noticias
    {
        public int IDNoticia { get; set; }
        public string texto { get; set; }
        public int usuarioCrea { get; set; }
        public System.DateTime fechaModifica { get; set; }
        public Nullable<System.DateTime> usuarioModifica { get; set; }
        public Nullable<System.DateTime> FechaRegistro { get; set; }
        public Nullable<bool> Indicador_Activo { get; set; }
    }
}