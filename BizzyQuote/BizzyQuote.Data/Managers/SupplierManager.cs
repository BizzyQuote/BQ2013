using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BizzyQuote.Data.Entities;

namespace BizzyQuote.Data.Managers
{
    public class SupplierManager : IDisposable
    {
         #region Variables
        BizzyQuoteDataContext db;
        #endregion

         #region Constructor & Dispose
        /// <summary>
        /// Supplier Manager 
        /// </summary>
        public SupplierManager()
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
        public Supplier Create(Supplier supplier)
        {
            supplier.CreatedOn = DateTime.Now;
            supplier.ModifiedOn = DateTime.Now;

            db.Suppliers.InsertOnSubmit(supplier);
            db.SubmitChanges();
            return supplier;
        }

        public Supplier Edit(Supplier supplier)
        {
            supplier.ModifiedOn = DateTime.Now;
            Supplier dbSupplier = Single(supplier.ID);

            // get the database columns which need to be updated
            var databaseMembers =
                db.Mapping.MappingSource.GetModel(typeof(BizzyQuoteDataContext)).GetMetaType(typeof(Supplier)).DataMembers
                    .Where(d => d.IsAssociation == false && d.IsDbGenerated == false && d.IsPersistent == true);

            // reflect to get instances of the entity preoprties
            var editProperties =
                from p in dbSupplier.GetType().GetProperties()
                join m in databaseMembers on p.Name equals m.Name
                select p;

            // copy the values
            PropertyInfo[] editProps = editProperties.ToArray();
            foreach (PropertyInfo propertyInfo in editProps)
            {
                propertyInfo.SetValue(dbSupplier, propertyInfo.GetValue(supplier, null), null);
            }


            db.SubmitChanges();
            return dbSupplier;
        }

        public Supplier Single(int id)
        {
            return db.Suppliers.Single(c => c.ID == id);
        }

        public IEnumerable<Supplier> All()
        {
            return db.Suppliers.OrderBy(c => c.Name);
        }

        public IEnumerable<Supplier> Active()
        {
            return db.Suppliers.Where(s => s.IsActive == true).OrderBy(s => s.Name);
        }

        public IEnumerable<CompanyToSupplier> ByCompanyID(int companyID)
        {
            return db.CompanyToSuppliers.Where(cs => cs.CompanyID == companyID);
        }
        #endregion
    }
}
