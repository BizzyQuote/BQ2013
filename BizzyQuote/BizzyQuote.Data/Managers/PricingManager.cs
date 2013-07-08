using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BizzyQuote.Data.Entities;

namespace BizzyQuote.Data.Managers
{
    public class PricingManager : IDisposable
    {
        #region Variables
        BizzyQuoteDataContext db;
        #endregion

        #region Constructor & Dispose
        /// <summary>
        /// Pricing Manager 
        /// </summary>
        public PricingManager()
        {
            db = new BizzyQuoteDataContext(Properties.Settings.Default.BizzyQuoteConnectionString);
            var options = new DataLoadOptions();
            db.LoadOptions = options;
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
        public IEnumerable<Pricing> BySupplier(int supplierID)
        {
            return db.Pricings.Where(p => p.SupplierID == supplierID);
        }

        public IEnumerable<Pricing> ByCompany(int companyID)
        {
            return db.Pricings.Where(p => p.CompanyID == companyID);
        }

        public Pricing Create(Pricing pricing)
        {
            pricing.CreatedOn = DateTime.Now;
            pricing.ModifiedOn = DateTime.Now;

            db.Pricings.InsertOnSubmit(pricing);
            db.SubmitChanges();
            return pricing;
        }

        public Pricing Edit(Pricing pricing)
        {
            pricing.ModifiedOn = DateTime.Now;
            Pricing dbPricing = Single(pricing.ID);

            // get the database columns which need to be updated
            var databaseMembers =
                db.Mapping.MappingSource.GetModel(typeof(BizzyQuoteDataContext)).GetMetaType(typeof(Pricing)).DataMembers
                    .Where(d => d.IsAssociation == false && d.IsDbGenerated == false && d.IsPersistent == true);

            // reflect to get instances of the entity preoprties
            var editProperties =
                from p in dbPricing.GetType().GetProperties()
                join m in databaseMembers on p.Name equals m.Name
                select p;

            // copy the values
            PropertyInfo[] editProps = editProperties.ToArray();
            foreach (PropertyInfo propertyInfo in editProps)
            {
                propertyInfo.SetValue(dbPricing, propertyInfo.GetValue(pricing, null), null);
            }
            
            db.SubmitChanges();
            return dbPricing;
        }

        public Pricing Single(int id)
        {
            return db.Pricings.Single(p => p.ID == id);
        }

        public bool UpdatePrices(List<Pricing> prices)
        {
            try
            {
                foreach (var price in prices)
                {
                    if (price.ID > 0)
                    {
                        var dbPrice = Single(price.ID);
                        if (dbPrice.Price != price.Price)
                        {
                            var result = Edit(price);
                        }
                    }
                    else
                    {
                        var result = Create(price);
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
