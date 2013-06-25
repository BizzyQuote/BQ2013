using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public Company Edit(Company company)
        {
            company.ModifiedOn = DateTime.Now;
            Company dbCompany = Single(company.ID);

            // get the database columns which need to be updated
            var databaseMembers =
                db.Mapping.MappingSource.GetModel(typeof(BizzyQuoteDataContext)).GetMetaType(typeof(Company)).DataMembers
                    .Where(d => d.IsAssociation == false && d.IsDbGenerated == false && d.IsPersistent == true);

            // reflect to get instances of the entity preoprties
            var editProperties =
                from p in dbCompany.GetType().GetProperties()
                join m in databaseMembers on p.Name equals m.Name
                select p;

            // copy the values
            PropertyInfo[] editProps = editProperties.ToArray();
            foreach (PropertyInfo propertyInfo in editProps)
            {
                propertyInfo.SetValue(dbCompany, propertyInfo.GetValue(company, null), null);
            }

            
            db.SubmitChanges();
            return dbCompany;
        }

        public Company Single(int id)
        {
            return db.Companies.Single(c => c.ID == id);
        }

        public IEnumerable<Company> All()
        {
            return db.Companies.OrderByDescending(c => c.CreatedOn);
        }

        
        #endregion
    }
}
