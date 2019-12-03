using LsysParser.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;

namespace LsysParser.Data.Repository
{
    class FileRepository : Repository<ProductFile>
    {
        public FileRepository(ProductsContext dbContext) : base(dbContext)
        {
        }

        public void RemoveAll()
        {
            dbContext.Database.ExecuteSqlCommand("DELETE FROM product_file");
        }

        public void UpdateName(ProductFile prodFile)
        {
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var comm = new SqlCommand(@"UPDATE product_file SET name = @name WHERE id = @id", conn, trans))
                    {
                        comm.Parameters.Add(new SqlParameter("@name", prodFile.Name));
                        comm.Parameters.Add(new SqlParameter("@id", prodFile.Id));

                        comm.ExecuteNonQuery();
                        trans.Commit();
                    }
                }
            }
        }
    }
}
