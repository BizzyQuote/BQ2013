using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizzyQuote.Data.Entities;

namespace BizzyQuote.Data.Managers
{
    public class CompanyManager : IDisposable
    {
        #region Variables
        BizzyQuoteDataContext db;
        #endregion

        #region Constructor & Dispose
        /// <summary>
        /// Company Manager 
        /// </summary>
        public CompanyManager()
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
        public Company Create(Company company)
        {
            company.CreatedOn = DateTime.Now;
            company.ModifiedOn = DateTime.Now;

            db.Companies.InsertOnSubmit(company);
            db.SubmitChanges();
            return company;
        }
        #endregion
    }
}
