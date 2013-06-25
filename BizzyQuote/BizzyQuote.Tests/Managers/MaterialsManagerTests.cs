using System;
using System.Text;
using System.Collections.Generic;
using BizzyQuote.Data.Entities;
using BizzyQuote.Data.Enums;
using BizzyQuote.Data.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizzyQuote.Tests.Managers
{
    /// <summary>
    /// Summary description for MaterialsManagerTests
    /// </summary>
    [TestClass]
    public class MaterialsManagerTests
    {
        public MaterialsManagerTests()
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
        public void CreateMaterialTest()
        {
            Material material = new Material();

            material.CreatedOn = DateTime.Now;
            material.Measurement = Measurement.SquareFeet;
            material.ModifiedOn = DateTime.Now;
            material.Name = "Wood Siding";
            material.SubName = "Light Blue";

            using (var mm = new MaterialsManager())
            {
                material = mm.CreateMaterial(material);
                Assert.IsTrue(material.ID > 0);
            }
        }

        [TestMethod]
        public void CreateProduct()
        {
            Product product = new Product();

            product.CreatedOn = DateTime.Now;
            product.Measurement = Measurement.SquareFeet;
            product.ModifiedOn = DateTime.Now;
            product.Name = "Siding";

            using (var mm = new MaterialsManager())
            {
                product = mm.CreateProduct(product);
                Assert.IsTrue(product.ID > 0);
            }
        }
    }
}
