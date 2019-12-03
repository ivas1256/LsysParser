using LsysParser.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.Data.Repository
{
    class BrandRepository : Repository<Brand>
    {
        public BrandRepository(ProductsContext dbContext) : base(dbContext)
        {
        }

        public int GetIdByName(string name)
        {
            var check = dbContext.Set<Brand>().Where(x => x.Name.Equals(name)).FirstOrDefault();
            if (check != null)
                return check.Id;

            return 0;
        }

        public void RemoveAll()
        {
            dbContext.Database.ExecuteSqlCommand("DELETE FROM brand");
        }
    }
}
