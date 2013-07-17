using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizzyQuote.Data.Entities;

namespace BizzyQuote.Data.Managers
{
    public class ManufacturerManager : IDisposable
    {
        #region Variables
        BizzyQuoteDataContext db;
        #endregion

        #region Constructor & Dispose
        /// <summary>
        /// Manufacturer Manager 
        /// </summary>
        public ManufacturerManager()
        {
            db = new BizzyQuoteDataContext(Properties.Settings.Default.BizzyQuoteConnectionString);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            db.Dispose();
        }
        #endregion

        #region Methods
        public IEnumerable<Manufacturer> All()
        {
            return db.Manufacturers.OrderBy(m => m.Name);
        }
        #endregion
    }
}
