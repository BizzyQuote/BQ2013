using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using BizzyQuote.Data.Entities;

namespace BizzyQuote.Data.Managers
{
    public class UserManager : IDisposable
    {
        #region Variables
        BizzyQuoteDataContext db;
        #endregion

        #region Constructor & Dispose
        /// <summary>
        /// User Manager 
        /// </summary>
        public UserManager()
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
        public User Create(User user, string password)
        {
            var results = Membership.GetAllUsers();


            MembershipUser membershipUser = Membership.CreateUser(user.Username, password, user.Email);
            
            user.CreatedOn = DateTime.Now;
            user.ModifiedOn = DateTime.Now;

            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();

            return user;
        }
        #endregion
        
    }
}
