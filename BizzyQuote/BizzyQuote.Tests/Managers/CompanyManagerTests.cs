using System;
using System.Text;
using System.Collections.Generic;
using BizzyQuote.Data.Entities;
using BizzyQuote.Data.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizzyQuote.Tests.Managers
{
    /// <summary>
    /// Summary description for CompanyManagerTests
    /// </summary>
    [TestClass]
    public class CompanyManagerTests
    {
        public CompanyManagerTests()
        {
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
        public void CreateCompanyTest()
        {
            var company = new Company
                {
                    Address = "460 South Marion pkwy apt 1402",
                    City = "Denver",
                    Email = "some.dude3@gmail.com",
                    Name = "Third Company",
                    Phone = "815-919-5371",
                    State = "CO",
                    Zip = "80209"
                };
            using (var cm = new CompanyManager())
            {
                company = cm.Create(company);
            }

            Assert.IsTrue(company.ID > 0);
        }

        [TestMethod]
        public void EditCompanyTest()
        {
            Company company;

            using (var cm = new CompanyManager())
            {
                company = cm.Single(1);
            }

            var oldTime = company.ModifiedOn;
            company.ModifiedOn = DateTime.Now;

            using (var cm = new CompanyManager())
            {
                company = cm.Edit(company);
            }

            using(var cm = new CompanyManager())
            {
                var company2 = cm.Single(1);
                Assert.IsTrue(company2.ModifiedOn > oldTime);
            }
        }
    }
}
