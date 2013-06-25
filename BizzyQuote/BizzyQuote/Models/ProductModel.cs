using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BizzyQuote.Data.Entities;

namespace BizzyQuote.Models
{
    public class ProductHouseModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<PartOfHouse> PartsOfHouse { get; set; }
        public IEnumerable<ProductToPartOfHouseModel> ProductPartHouse { get; set; }
    }

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