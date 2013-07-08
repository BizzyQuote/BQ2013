using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BizzyQuote.Models
{
    public class PricingModel
    {
        public int ID { get; set; }
        public int MaterialID { get; set; }
        public int? CompanyID { get; set; }
        public int? SupplierID { get; set; }
        public string MaterialName { get; set; }
        public string CompanyName { get; set; }
        public string SupplierName { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}