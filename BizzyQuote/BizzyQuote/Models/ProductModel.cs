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
        public IEnumerable<ProductLine> PartsOfHouse { get; set; }
        public IEnumerable<ProductToProductLineModel> ProductPartHouse { get; set; }
    }

    public class ProductToProductLineModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductLineID { get; set; }
        public string ProductLineName { get; set; }
        public int ProductProductLineID { get; set; }
        public bool IsActive { get; set; }
    }

    public class MaterialToProductModel
    {
        public int MaterialToProductID { get; set; }
        public int ProductID { get; set; }
        public int MaterialID { get; set; }
        public string ProductName { get; set; }
        public string MaterialName { get; set; }
        public bool IsActive { get; set; }
    }

    public class MaterialProductGridModel
    {
        public IEnumerable<MaterialToProductModel> MaterialProducts { get; set; }
        public IEnumerable<Material> Materials { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}