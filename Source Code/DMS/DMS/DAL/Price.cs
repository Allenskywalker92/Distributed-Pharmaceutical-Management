//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DMS.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Price
    {
        public int DrugID { get; set; }
        public int UnitID { get; set; }
        public double UnitPrice { get; set; }
    
        public virtual Drug Drug { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
