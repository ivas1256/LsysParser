using LsysParser.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace LsysParser.Data.Repository
{
    class PropertyValueRepository : Repository<PropertyValue>
    {
        public PropertyValueRepository(ProductsContext dbContext) : base(dbContext)
        {
        }


        public int GetIdByValue(string Value)
        {
            var check = dbContext.Set<PropertyValue>().Where(x => x.Value.Equals(Value)).FirstOrDefault();
            if (check != null)
                return check.Id;

            return 0;
        }
    }
}
