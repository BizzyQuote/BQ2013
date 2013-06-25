using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
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
            var options = new DataLoadOptions();
            options.LoadWith<User>(u => u.Company);
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
        public User Create(User user)
        {
            user.CreatedOn = DateTime.Now;
            user.ModifiedOn = DateTime.Now;

            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();

            return user;
        }

        public User Edit(User user)
        {
            user.ModifiedOn = DateTime.Now;
            User dbUser = Single(user.ID);

            // get the database columns which need to be updated
            var databaseMembers =
                db.Mapping.MappingSource.GetModel(typeof(BizzyQuoteDataContext)).GetMetaType(typeof(User)).DataMembers
                    .Where(d => d.IsAssociation == false && d.IsDbGenerated == false && d.IsPersistent == true);

            // reflect to get instances of the entity preoprties
            var editProperties =
                from p in dbUser.GetType().GetProperties()
                join m in databaseMembers on p.Name equals m.Name
                select p;

            // copy the values
            PropertyInfo[] editProps = editProperties.ToArray();
            foreach (PropertyInfo propertyInfo in editProps)
            {
                propertyInfo.SetValue(dbUser, propertyInfo.GetValue(user, null), null);
            }


            db.SubmitChanges();
            return dbUser;
        }

        public User Single(int id)
        {
            return db.Users.Single(u => u.ID == id);
        }

        public User ByUsername(string username)
        {
            return db.Users.SingleOrDefault(u => u.Username == username);
        }

        public IEnumerable<User> All()
        {
            return db.Users;
        }

        public IEnumerable<User> ByCompany(int id)
        {
            return db.Users.Where(u => u.CompanyID == id);
        }

        public IEnumerable<User> BySupplier(int id)
        {
            return db.Users.Where(u => u.SupplierID == id);
        }
        #endregion
        
    }
}
