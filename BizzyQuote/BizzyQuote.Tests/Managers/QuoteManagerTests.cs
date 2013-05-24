using System;
using System.Text;
using System.Collections.Generic;
using BizzyQuote.Data.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizzyQuote.Tests.Managers
{
    /// <summary>
    /// Summary description for QuoteManagerTests
    /// </summary>
    [TestClass]
    public class QuoteManagerTests
    {
        public QuoteManagerTests()
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
        public void CreateQuoteTest()
        {
            Quote quote = new Quote();
            quote.Address = "460 South Marion pkwy apt 1402";
            quote.Amount = 47M;
            
        }
    }
}
