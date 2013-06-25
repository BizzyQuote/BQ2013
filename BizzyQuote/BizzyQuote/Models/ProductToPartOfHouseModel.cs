using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BizzyQuote.Models
{
    public class ProductToPartOfHouseModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int PartOfHouseID { get; set; }
        public string PartOfHouseName { get; set; }
        public int ProductPartOfHouseID { get; set; }
        public bool IsActive { get; set; }
    }
}