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
    public class QuoteManager : IDisposable
    {
        #region Variables
        BizzyQuoteDataContext db;
        #endregion

        #region Constructor & Dispose
        /// <summary>
        /// Quote Manager 
        /// </summary>
        public QuoteManager()
        {
            db = new BizzyQuoteDataContext(Properties.Settings.Default.BizzyQuoteConnectionString);
            var options = new DataLoadOptions();
            options.LoadWith<Quote>(q => q.QuoteItems);
            options.LoadWith<QuoteItem>(qi => qi.Material);
            options.LoadWith<QuoteItem>(qi => qi.Product);
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
        public Quote Create(Quote quote)
        {
            quote.CreatedOn = DateTime.Now;
            quote.ModifiedOn = DateTime.Now;

            db.Quotes.InsertOnSubmit(quote);
            db.SubmitChanges();

            return quote;
        }

        public Quote Edit(Quote quote)
        {
            quote.ModifiedOn = DateTime.Now;
            Quote dbQuote = Single(quote.ID);

            // get the database columns which need to be updated
            var databaseMembers =
                db.Mapping.MappingSource.GetModel(typeof(BizzyQuoteDataContext)).GetMetaType(typeof(Quote)).DataMembers
                    .Where(d => d.IsAssociation == false && d.IsDbGenerated == false && d.IsPersistent == true);

            // reflect to get instances of the entity preoprties
            var editProperties =
                from p in dbQuote.GetType().GetProperties()
                join m in databaseMembers on p.Name equals m.Name
                select p;

            // copy the values
            PropertyInfo[] editProps = editProperties.ToArray();
            foreach (PropertyInfo propertyInfo in editProps)
            {
                propertyInfo.SetValue(dbQuote, propertyInfo.GetValue(quote, null), null);
            }


            db.SubmitChanges();
            return dbQuote;
        }

        public Quote Single(int id)
        {
            return db.Quotes.Single(q => q.ID == id);
        }

        public IEnumerable<Quote> All()
        {
            return db.Quotes.OrderByDescending(q => q.CreatedOn);
        }

        public IEnumerable<Quote> ActiveByCompany(int companyID)
        {
            return db.Quotes.Where(q => q.IsActive == true && q.CompanyID == companyID).OrderByDescending(q => q.CreatedOn);
        }

        public IEnumerable<Quote> ActiveByEmployee(int employeeID)
        {
            return db.Quotes.Where(q => q.IsActive == true && q.EmployeeID == employeeID).OrderByDescending(q => q.CreatedOn);
        }

        public QuoteItem CreateItem(QuoteItem quoteItem)
        {
            db.QuoteItems.InsertOnSubmit(quoteItem);
            db.SubmitChanges();

            return quoteItem;
        }

        public QuoteOption CreateOption(QuoteOption option)
        {
            db.QuoteOptions.InsertOnSubmit(option);
            db.SubmitChanges();
            return option;
        }

        public IEnumerable<QuoteOption> QuoteOptions(int id)
        {
            return db.QuoteOptions.Where(o => o.QuoteID == id);
        }
        #endregion
    }
}
