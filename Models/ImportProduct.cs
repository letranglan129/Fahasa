//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fahasa.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ImportProduct
    {
        public long ImportId { get; set; }
        public int ProductId { get; set; }
        public long Price { get; set; }
        public int Amount { get; set; }
    
        public virtual Import Import { get; set; }
        public virtual Product Product { get; set; }
    }
}
