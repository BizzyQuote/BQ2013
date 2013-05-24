using System;
using System.Collections.Generic;
using System.Linq;
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
        /// QuoteManager 
        /// </summary>
        public QuoteManager()
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
        public Quote Create(Quote quote)
        {
            quote.CreatedOn = DateTime.Now;
            quote.ModifiedOn = DateTime.Now;

            db.Quotes.InsertOnSubmit(quote);
            db.SubmitChanges();

            return quote;
        }
        #endregion
    }
}
