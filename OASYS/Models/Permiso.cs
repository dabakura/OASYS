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
    
    public partial class Permiso
    {
        public int idPermiso { get; set; }
        public Nullable<int> idUsuario { get; set; }
        public int idRol { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public int usuarioCrea { get; set; }
        public Nullable<System.DateTime> fechaModifica { get; set; }
        public Nullable<int> usuarioModifica { get; set; }
    
        public virtual Rol Rol { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
