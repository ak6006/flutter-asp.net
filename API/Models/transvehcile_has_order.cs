//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class transvehcile_has_order
    {
        public int transVehcile_has_order_Id { get; set; }
        public int transVehcile_v_id { get; set; }
        public int order_order_id { get; set; }
        public Nullable<int> transVehcile_has_order_state { get; set; }
        public Nullable<System.DateTime> trans_in_date { get; set; }
        public Nullable<System.DateTime> trans_out_date { get; set; }
    
        public virtual order order { get; set; }
    }
}
