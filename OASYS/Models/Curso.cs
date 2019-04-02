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
    
    public partial class Curso
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Curso()
        {
            this.GestiondeCursos = new HashSet<GestiondeCursos>();
        }
    
        public int IDcurso { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int precio { get; set; }
        public int usuarioCrea { get; set; }
        public System.DateTime fechaModifica { get; set; }
        public int usuarioModifica { get; set; }
        public int Id_Nivel { get; set; }
        public Nullable<bool> Indicador_Activo { get; set; }
        public Nullable<System.DateTime> FechaRegistro { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GestiondeCursos> GestiondeCursos { get; set; }
        public virtual Nivel Nivel { get; set; }
    }
}