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
    
    public partial class DrugCompany
    {
        public DrugCompany()
        {
            this.Drugs = new HashSet<Drug>();
        }
    
        public int DrugCompanyID { get; set; }
        public string DrugCompanyName { get; set; }
        public Nullable<bool> IsActive { get; set; }
    
        public virtual ICollection<Drug> Drugs { get; set; }
    }
}
