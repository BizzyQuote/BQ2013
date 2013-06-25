using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizzyQuote.Data.Entities
{
    public partial class Product
    {
        public List<Material> AvailableMaterials { get; set; }
    }
}
