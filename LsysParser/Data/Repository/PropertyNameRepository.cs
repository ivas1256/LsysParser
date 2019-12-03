using LsysParser.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace LsysParser.Data.Repository
{
    class PropertyNameRepository : Repository<PropertyName>
    {
        public PropertyNameRepository(ProductsContext dbContext) : base(dbContext)
        {
        }

        public int GetIdByName(string name)
        {
            var check = dbContext.Set<PropertyName>().Where(x => x.Name.Equals(name)).FirstOrDefault();
            if (check != null)
                return check.Id;

            return 0;
        }
    }
}
