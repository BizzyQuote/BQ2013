using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BizzyQuote.Data.Entities;
using BizzyQuote.Data.Enums;

namespace BizzyQuote.Models
{
    public class MaterialModel
    {
        public MaterialModel(Material material)
        {
            Name = material.Name;
            SubName = material.SubName;
            UnitCost = material.UnitCost.GetValueOrDefault();
            Measurement = material.Measurement;
            Height = material.Height.GetValueOrDefault();
            Width = material.Width.GetValueOrDefault();
            Thickness = material.Thickness;
            Texture = material.Texture;
            Finish = material.Finish;
            Overlap = material.Overlap.GetValueOrDefault();
            SKU = material.SKU;
            IsActive = material.IsActive;
            ID = material.ID;
            CreatedOn = material.CreatedOn;
            ModifiedOn = material.ModifiedOn;
            ManufacturerID = material.ManufacturerID;

            var values = from Measurement e in Enum.GetValues(typeof(Measurement))
                         select new { Id = e, Name = e.ToString() };
            MeasurementList = new SelectList(values, "Id", "Name");
        }

        public MaterialModel()
        {
            var values = from Measurement e in Enum.GetValues(typeof(Measurement))
                         select new { Id = e, Name = e.ToString() };
            MeasurementList = new SelectList(values, "Id", "Name");
        }

        [Display(Name = "Material Name")]
        [Required]
        [StringLength(100, ErrorMessage = "The maximum length is 100 characters")]
        public string Name { get; set; }

        [Display(Name = "Material SubName")]
        public string SubName { get; set; }

        [Display(Name = "Unit Cost")]
        [Required]
        public decimal UnitCost { get; set; }

        [Display(Name = "Measurement")]
        public Measurement Measurement { get; set; }

        [Display(Name = "Height")]
        [Required]
        public decimal Height { get; set; }

        [Display(Name = "Width")]
        [Required]
        public decimal Width { get; set; }

        [Display(Name = "Thickness")]
        [StringLength(20, ErrorMessage = "The maximum length is 20 characters")]
        public string Thickness { get; set; }

        [Display(Name = "Texture")]
        [StringLength(50, ErrorMessage = "The maximum length is 50 characters")]
        public string Texture { get; set; }

        [Display(Name = "Finish")]
        public string Finish { get; set; }

        [Display(Name = "Overlap")]
        public decimal Overlap { get; set; }

        [Display(Name = "SKU")]
        public string SKU { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public SelectList MeasurementList { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ManufacturerID { get; set; }
        public int ID { get; set; }
    }
}