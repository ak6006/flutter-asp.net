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
    
    public partial class SP_Sales_BarCode_Check_Result
    {
        public int store_store_id { get; set; }
        public Nullable<System.DateTime> store_has_productDate { get; set; }
        public string shiftName { get; set; }
        public string Shift_Name { get; set; }
        public string Customer_Name { get; set; }
        public string transVehcile_num { get; set; }
        public string transVehcile_driver_name { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public string productName { get; set; }
        public Nullable<int> weight_net { get; set; }
        public string barcode_serialNumber { get; set; }
    }
}
