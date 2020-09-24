using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Dto
{
    public class CustomerName
    {
        public string custName { get; set; }
    }
    public class CustomerOrdersDto
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int WieghtId { get; set; }
        public int? WieghtName { get; set; }
        public int MeasureId { get; set; }
        public string MeasureName { get; set; }
        public string quantity { get; set; }

    }
    public class ProductName
    {
        public int ProductId { get; set; }
        public string productName{ get; set; }
    }
    public class StoreName
    {
        public int StoreId { get; set; }
        public string storeName { get; set; }
    }
    public class WeightName
    {
        public int WeightId { get; set; }
        public int? weightName { get; set; }
    }
    public class MeasureName
    {
        public int MeasureId { get; set; }
        public string measureName { get; set; }
    }

    public class VehiclesData
    {
        public int VehicleId { get; set; }
        public string DriverName { get; set; }
    }
}