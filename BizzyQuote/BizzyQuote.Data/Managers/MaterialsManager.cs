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
    public class MaterialsManager : IDisposable
    {
        #region Variables
        BizzyQuoteDataContext db;
        #endregion

        #region Constructor & Dispose
        /// <summary>
        /// Materials Manager 
        /// </summary>
        public MaterialsManager()
        {
            db = new BizzyQuoteDataContext(Properties.Settings.Default.BizzyQuoteConnectionString);
            var options = new DataLoadOptions();
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
        public Material CreateMaterial(Material material)
        {
            material.CreatedOn = DateTime.Now;
            material.ModifiedOn = DateTime.Now;

            db.Materials.InsertOnSubmit(material);
            db.SubmitChanges();
            return material;
        }

        public Product CreateProduct(Product product)
        {
            product.CreatedOn = DateTime.Now;
            product.ModifiedOn = DateTime.Now;

            db.Products.InsertOnSubmit(product);
            db.SubmitChanges();
            return product;
        }

        public Product EditProduct(Product product)
        {
            product.ModifiedOn = DateTime.Now;
            Product dbProduct = SingleProduct(product.ID);

            // get the database columns which need to be updated
            var databaseMembers =
                db.Mapping.MappingSource.GetModel(typeof(BizzyQuoteDataContext)).GetMetaType(typeof(Product)).DataMembers
                    .Where(d => d.IsAssociation == false && d.IsDbGenerated == false && d.IsPersistent == true);

            // reflect to get instances of the entity preoprties
            var editProperties =
                from p in dbProduct.GetType().GetProperties()
                join m in databaseMembers on p.Name equals m.Name
                select p;

            // copy the values
            PropertyInfo[] editProps = editProperties.ToArray();
            foreach (PropertyInfo propertyInfo in editProps)
            {
                propertyInfo.SetValue(dbProduct, propertyInfo.GetValue(product, null), null);
            }


            db.SubmitChanges();
            return dbProduct;
        }

        public IEnumerable<Product> AllProducts()
        {
            return db.Products.OrderBy(mp => mp.CreatedOn);
        }

        public IEnumerable<Product> ActiveProducts()
        {
            return db.Products.Where(mp => mp.IsActive).OrderBy(mp => mp.CreatedOn);
        }

        public IEnumerable<Product> ActiveProducts(int companyID)
        {
            return db.Products.Where(mp => mp.IsActive).OrderBy(mp => mp.CreatedOn);
        }

        public Product SingleProduct(int id)
        {
            return db.Products.SingleOrDefault(m => m.ID == id);
        }

        public Material EditMaterial(Material material)
        {
            material.ModifiedOn = DateTime.Now;
            Material dbMaterial = SingleMaterial(material.ID);

            // get the database columns which need to be updated
            var databaseMembers =
                db.Mapping.MappingSource.GetModel(typeof(BizzyQuoteDataContext)).GetMetaType(typeof(Material)).DataMembers
                    .Where(d => d.IsAssociation == false && d.IsDbGenerated == false && d.IsPersistent == true);

            // reflect to get instances of the entity preoprties
            var editProperties =
                from p in dbMaterial.GetType().GetProperties()
                join m in databaseMembers on p.Name equals m.Name
                select p;

            // copy the values
            PropertyInfo[] editProps = editProperties.ToArray();
            foreach (PropertyInfo propertyInfo in editProps)
            {
                propertyInfo.SetValue(dbMaterial, propertyInfo.GetValue(material, null), null);
            }


            db.SubmitChanges();
            return dbMaterial;
        }

        public IEnumerable<Material> AllMaterials()
        {
            return db.Materials.OrderBy(m => m.CreatedOn);
        }

        public IEnumerable<Material> ActiveMaterials()
        {
            return db.Materials.Where(m => m.IsActive).OrderBy(m => m.CreatedOn);
        }

        public IEnumerable<Material> ActiveMaterials(int companyID)
        {
            return db.Materials.Where(m => m.IsActive).OrderBy(m => m.CreatedOn);
        }

        public Material SingleMaterial(int id)
        {
            return db.Materials.SingleOrDefault(m => m.ID == id);
        }

        public IEnumerable<Material> BySupplier(int supplierID)
        {
            return db.Materials.Where(m => m.ManufacturerID == supplierID).OrderBy(m => m.Name);
        }

        public ProductLine CreateProductLine(ProductLine partOfHouse)
        {
            db.ProductLines.InsertOnSubmit(partOfHouse);
            db.SubmitChanges();
            return partOfHouse;
        }

        public IEnumerable<ProductLine> ActivePartsOfHouse()
        {
            return db.ProductLines.Where(ph => ph.IsActive);
        }

        public IEnumerable<MaterialToProduct> ByMaterialID(int materialID)
        {
            return db.MaterialToProducts.Where(mp => mp.MaterialID == materialID);
        }

        public IEnumerable<MaterialToProduct> ByManufacturerID(int manufacturerID)
        {
            var list = from mp in db.MaterialToProducts
                       join m in db.Materials on mp.MaterialID equals m.ID
                       where m.ManufacturerID == manufacturerID
                       select mp;
            return list;
        }

        public bool DeleteMaterialToProducts(List<int> mpDelete)
        {
            try
            {
                foreach (var i in mpDelete)
                {
                    var mp = SingleMaterialToProduct(i);
                    db.MaterialToProducts.DeleteOnSubmit(mp);
                    db.SubmitChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool InsertMaterialToProducts(List<MaterialToProduct> mpUpdate)
        {
            db.MaterialToProducts.InsertAllOnSubmit(mpUpdate);
            db.SubmitChanges();
            return true;
        }

        public MaterialToProduct Create(MaterialToProduct mp)
        {
            db.MaterialToProducts.InsertOnSubmit(mp);
            db.SubmitChanges();
            return mp;
        }

        public MaterialToProduct Edit(MaterialToProduct mp)
        {
            db.MaterialToProducts.InsertOnSubmit(mp);
            db.SubmitChanges();
            return mp;
        }

        public IEnumerable<ProductToLine> AllProductToProductLine()
        {
            return db.ProductToLines;
        }

        public bool DeleteProductToProductLine()
        {
            try
            {
                db.ProductToLines.DeleteAllOnSubmit(AllProductToProductLine());
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CreateProductToProductLine(List<ProductToLine> parts)
        {
            db.ProductToLines.InsertAllOnSubmit(parts);
            db.SubmitChanges();
            return true;
        }

        public IEnumerable<MaterialToProduct> AllMaterialToProducts()
        {
            return db.MaterialToProducts;
        }

        public MaterialToProduct SingleMaterialToProduct(int id)
        {
            return db.MaterialToProducts.Single(mp => mp.ID == id);
        }
        #endregion
    }
}
