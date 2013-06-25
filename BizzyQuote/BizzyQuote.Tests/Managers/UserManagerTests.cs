using System;
using System.Text;
using System.Collections.Generic;
using BizzyQuote.Data.Entities;
using BizzyQuote.Data.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizzyQuote.Tests.Managers
{
    /// <summary>
    /// Summary description for UserManagerTests
    /// </summary>
    [TestClass]
    public class UserManagerTests
    {
        public UserManagerTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CreateUserTest()
        {
            User user = new User();
            user.CompanyID = 1;
            user.CreatedOn = DateTime.Now;
            user.Email = "john2@bizzyquote.com";
            user.FirstName = "John2";
            user.LastName = "Ocker2s";
            user.Username = "johnockers2";

            using (var um = new UserManager())
            {
                user = um.Create(user);
            }

            Assert.IsTrue(user.ID > 0);
        }
    }
}
