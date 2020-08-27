using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Dto
{
    public class CustomerDto
    {
        //[JsonProperty(PropertyName = "CustomerId")]
        //public int رقم_الوكيل { get; set; }


        //[JsonProperty(PropertyName = "AddId")]
        //public int معرف { get; set; }


        //[JsonProperty(PropertyName = "Name")]
        //public string الاسم { get; set; }


        //[JsonProperty(PropertyName = "Country")]
        //public string دولة { get; set; }



        //[JsonProperty(PropertyName = "Government")]
        //public string المحافظة { get; set; }



        //[JsonProperty(PropertyName = "City")]
        //public string المدينة { get; set; }


        //[JsonProperty(PropertyName = "Email")]
        //public string بريد_الكتروني { get; set; }


        //[JsonProperty(PropertyName = "Address")]
        //public string عنوان { get; set; }



        //[JsonProperty(PropertyName = "Phone")]
        //public string تلفون { get; set; }


        //[JsonProperty(PropertyName = "Mobile")]
        //public string هاتف { get; set; }


        //[JsonProperty(PropertyName = "Fax")]
        //public string فاكس { get; set; }



        public int CustomerId { get; set; }
        public int AddId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Government { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
    }
}