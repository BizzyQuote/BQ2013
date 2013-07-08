using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BizzyQuote.Data.Entities;
using Newtonsoft.Json;

namespace BizzyQuote.Models
{
    public class CompanyModel
    {
        [Required]
        [Display(Name = "Company name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [Required]
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class WasteFactorModel
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int ProductLineID { get; set; }
        public string ProductName { get; set; }
        public string ProductLineName { get; set; }
        [Display(Name = "Waste Factor")]
        public decimal Factor { get; set; }
        public int CompanyID { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}