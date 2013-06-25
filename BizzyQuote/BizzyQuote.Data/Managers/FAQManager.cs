using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BizzyQuote.Data.Entities;

namespace BizzyQuote.Data.Managers
{
    public class FAQManager : IDisposable
    {
        #region Variables
        BizzyQuoteDataContext db;
        #endregion

        #region Constructor & Dispose
        /// <summary>
        /// FAQ Manager 
        /// </summary>
        public FAQManager()
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
        public FAQ Create(FAQ faq)
        {
            db.FAQs.InsertOnSubmit(faq);
            db.SubmitChanges();

            return faq;
        }

        public FAQ Edit(FAQ faq)
        {
            FAQ dbFAQ = Single(faq.ID);

            // get the database columns which need to be updated
            var databaseMembers =
                db.Mapping.MappingSource.GetModel(typeof(BizzyQuoteDataContext)).GetMetaType(typeof(FAQ)).DataMembers
                    .Where(d => d.IsAssociation == false && d.IsDbGenerated == false && d.IsPersistent == true);

            // reflect to get instances of the entity preoprties
            var editProperties =
                from p in dbFAQ.GetType().GetProperties()
                join m in databaseMembers on p.Name equals m.Name
                select p;

            // copy the values
            PropertyInfo[] editProps = editProperties.ToArray();
            foreach (PropertyInfo propertyInfo in editProps)
            {
                propertyInfo.SetValue(dbFAQ, propertyInfo.GetValue(faq, null), null);
            }


            db.SubmitChanges();
            return dbFAQ;
        }

        public FAQ Single(int id)
        {
            return db.FAQs.Single(q => q.ID == id);
        }

        public IQueryable<FAQ> All()
        {
            return db.FAQs;
        }
        #endregion
    }
}
