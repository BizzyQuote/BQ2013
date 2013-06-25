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
            return db.Materials.Where(m => m.SupplierID == supplierID).OrderBy(m => m.Name);
        }

        public PartOfHouse CreatePartOfHouse(PartOfHouse partOfHouse)
        {
            db.PartOfHouses.InsertOnSubmit(partOfHouse);
            db.SubmitChanges();
            return partOfHouse;
        }

        public IEnumerable<PartOfHouse> ActivePartsOfHouse()
        {
            return db.PartOfHouses.Where(ph => ph.IsActive);
        }

        public IEnumerable<MaterialToProduct> ByMaterialID(int materialID)
        {
            return db.MaterialToProducts.Where(mp => mp.MaterialID == materialID);
        }

        public IEnumerable<ProductToPartOfHouse> AllProductToPartOfHouse()
        {
            return db.ProductToPartOfHouses;
        }

        public bool DeleteProductToPartOfHouse()
        {
            try
            {
                db.ProductToPartOfHouses.DeleteAllOnSubmit(AllProductToPartOfHouse());
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CreateProductToPartOfHouse(List<ProductToPartOfHouse> parts)
        {
            db.ProductToPartOfHouses.InsertAllOnSubmit(parts);
            db.SubmitChanges();
            return true;
        }
        #endregion
    }
}
