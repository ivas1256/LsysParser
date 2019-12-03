using LsysParser.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;

namespace LsysParser.Data.Repository
{
    class PropertyRepository : Repository<Property>
    {
        public PropertyRepository(ProductsContext dbContext) : base(dbContext)
        {
        }

        public List<Property> FindWithData(Expression<Func<Property, bool>> predicte)
        {
            return dbContext.Set<Property>().Include("NameObj").Include("ValueObj")
                .Where(predicte).ToList();
        }

        public void RemoveAll()
        {
            dbContext.Database.ExecuteSqlCommand("DELETE FROM property");
            dbContext.Database.ExecuteSqlCommand("DELETE FROM property_name");
            dbContext.Database.ExecuteSqlCommand("DELETE FROM property_value");
        }
    }
}
