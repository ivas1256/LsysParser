using LsysParser.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.Data.Repository
{
    class ProductRepository : Repository<Product>
    {
        public ProductRepository(ProductsContext dbContext) : base(dbContext)
        {
        }

        public void SaveDirectly(Product product)
        {
            if (product.BrandId == 0 || product.BrandId == null)
            {
                dbContext.Set<Brand>().Add(product.Brand);
                dbContext.SaveChanges();
            }

            int productId = 0;
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var comm = new SqlCommand(@"INSERT INTO product (category_id,url,name,price,article,description,brand_id)
                        VALUES(@category_id, @url, @name, @price,
                        @article, @description, @brand_id) SELECT SCOPE_IDENTITY()", conn, trans))
                    {
                        comm.Parameters.Add(new SqlParameter("@category_id", product.Category.Id));
                        comm.Parameters.Add(new SqlParameter("@url", product.Url));
                        comm.Parameters.Add(new SqlParameter("@name", product.Name));
                        comm.Parameters.Add(new SqlParameter("@price", product.Price));
                        comm.Parameters.Add(new SqlParameter("@article", product.Article));
                        comm.Parameters.Add(new SqlParameter("@description", product.Description ?? ""));
                        if (product.Brand != null)
                            comm.Parameters.Add(new SqlParameter("@brand_id", product.Brand.Id));
                        else
                            comm.Parameters.Add(new SqlParameter("@brand_id", product.BrandId));

                        var obj = comm.ExecuteScalar();
                        productId = int.Parse(obj.ToString());
                    }

                    foreach (var file in product.Files)
                        using (var comm = new SqlCommand("INSERT INTO product_file (product_id, name, url) VALUES (@prodId, @name, @url)",
                                conn, trans))
                        {
                            comm.Parameters.Add(new SqlParameter("@prodId", productId));
                            comm.Parameters.Add(new SqlParameter("@name", file.Name));
                            comm.Parameters.Add(new SqlParameter("@url", file.Url));

                            comm.ExecuteNonQuery();
                        }

                    foreach (var prop in product.Propertyes)
                    {
                        if (prop.NameId == 0)
                            prop.NameId = AddPropertyName(prop.NameObj, conn, trans);
                        if (prop.ValueId == 0)
                            prop.ValueId = AddPropertyValue(prop.ValueObj, conn, trans);

                        using (var comm = new SqlCommand("INSERT INTO property (product_id,name_id,value_id) VALUES (@prodId, @nameId, @valueId)",
                            conn, trans))
                        {
                            comm.Parameters.Add(new SqlParameter("@prodId", productId));
                            comm.Parameters.Add(new SqlParameter("@nameId", prop.NameId));
                            comm.Parameters.Add(new SqlParameter("@valueId", prop.ValueId));

                            comm.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                }
            }
        }

        int AddPropertyName(PropertyName name, SqlConnection conn, SqlTransaction trans)
        {
            using (var comm = new SqlCommand("INSERT INTO property_name (name) VALUES (@name) SELECT SCOPE_IDENTITY()", conn, trans))
            {
                comm.Parameters.Add(new SqlParameter("@name", name.Name));

                var obj = comm.ExecuteScalar();
                return int.Parse(obj.ToString());
            }
        }

        int AddPropertyValue(PropertyValue value, SqlConnection conn, SqlTransaction trans)
        {
            using (var comm = new SqlCommand("INSERT INTO property_value (value) VALUES (@value) SELECT SCOPE_IDENTITY()", conn, trans))
            {
                comm.Parameters.Add(new SqlParameter("@value", value.Value));

                var obj = comm.ExecuteScalar();
                return int.Parse(obj.ToString());
            }
        }

        public bool ExistWithUrl(string url)
        {
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var comm = new SqlCommand("SELECT * FROM product where url = @url", conn))
            {
                comm.Parameters.Add(new SqlParameter("@url", url));
                if (comm.Connection.State != System.Data.ConnectionState.Open)
                    comm.Connection.Open();

                using (var reader = comm.ExecuteReader())
                {
                    reader.Read();
                    if (reader.HasRows)
                        return true;
                    return false;
                }
            }
        }

        public IEnumerable<Product> GetAllForTableView()
        {
            return dbContext.Set<Product>().Include("Category").Include("Brand").AsNoTracking().ToList();
        }

        public void RemoveAll()
        {
            dbContext.Database.ExecuteSqlCommand("DELETE FROM product");
        }

        public List<Product> GetAllForSave()
        {
            return dbContext.Set<Product>()
                .Include("Category")
                .Include("Brand")
                //.Include("Propertyes.NameObj")
                //.Include("Propertyes.ValueObj")
                //.Include("Files")
                .AsNoTracking()
                .ToList();
        }
    }
}
