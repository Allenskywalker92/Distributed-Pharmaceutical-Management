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
    
    public partial class City
    {
        public City()
        {
            this.Districts = new HashSet<District>();
        }
    
        public int CityID { get; set; }
        public string CityName { get; set; }
    
        public virtual ICollection<District> Districts { get; set; }
    }
}
