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
    
    public partial class GestionDePagos
    {
        public int IDGESTION { get; set; }
        public int idPersona { get; set; }
        public int IdFactura { get; set; }
    
        public virtual Facturacion Facturacion { get; set; }
        public virtual Persona Persona { get; set; }
    }
}
