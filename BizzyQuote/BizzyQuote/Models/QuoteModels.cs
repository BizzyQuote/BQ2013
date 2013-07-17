using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BizzyQuote.Data.Entities;

namespace BizzyQuote.Models
{
    public class QuoteModels
    {
    }

    public class QuoteOptionsModel
    {
        public List<ProductLineSelectionModel> ProductLineSelectionModels { get; set; }
        public List<ManufacturerSelectionModel> ManufacturerSelectionModels { get; set; }
        public int QuoteID { get; set; }
    }

    public class ProductLineSelectionModel
    {
        public int ProductLineID { get; set; }
        public string ProductLineName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ManufacturerSelectionModel
    {
        public int ManufacturerID { get; set; }
        public string ManufacturerName { get; set; }
        public bool IsSelected { get; set; }
    }
}