using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BizzyQuote.Data.Entities;

namespace BizzyQuote.Data.Managers
{
    public class WasteFactorManager : IDisposable
    {
        #region Variables
        BizzyQuoteDataContext db;
        #endregion

        #region Constructor & Dispose
        /// <summary>
        /// Waste Factor Manager 
        /// </summary>
        public WasteFactorManager()
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
        public IEnumerable<WasteFactor> ByCompany(int companyID)
        {
            return db.WasteFactors.Where(wf => wf.CompanyID == companyID);
        }

        public WasteFactor Create(WasteFactor factor)
        {
            factor.CreatedOn = DateTime.Now;
            factor.ModifiedOn = DateTime.Now;

            db.WasteFactors.InsertOnSubmit(factor);
            db.SubmitChanges();
            return factor;
        }

        public WasteFactor Edit(WasteFactor factor)
        {
            factor.ModifiedOn = DateTime.Now;
            WasteFactor dbfactor = Single(factor.ID);

            // get the database columns which need to be updated
            var databaseMembers =
                db.Mapping.MappingSource.GetModel(typeof(BizzyQuoteDataContext)).GetMetaType(typeof(WasteFactor)).DataMembers
                    .Where(d => d.IsAssociation == false && d.IsDbGenerated == false && d.IsPersistent == true);

            // reflect to get instances of the entity preoprties
            var editProperties =
                from p in dbfactor.GetType().GetProperties()
                join m in databaseMembers on p.Name equals m.Name
                select p;

            // copy the values
            PropertyInfo[] editProps = editProperties.ToArray();
            foreach (PropertyInfo propertyInfo in editProps)
            {
                propertyInfo.SetValue(dbfactor, propertyInfo.GetValue(factor, null), null);
            }


            db.SubmitChanges();
            return dbfactor;
        }

        public WasteFactor Single(int id)
        {
            return db.WasteFactors.Single(c => c.ID == id);
        }

        public bool UpdateWasteFactors(List<WasteFactor> factors)
        {
            try
            {
                foreach (var factor in factors)
                {
                    if (factor.ID > 0)
                    {
                        var dbFactor = Single(factor.ID);
                        if (dbFactor.WasteFactor1 != factor.WasteFactor1)
                        {
                            var result = Edit(factor);
                        }
                    }
                    else
                    {
                        var result = Create(factor);
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
