using LsysParser.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.Data.Repository
{
    class CategoryRepository : Repository<Category>
    {
        public CategoryRepository(ProductsContext dbContext) : base(dbContext)
        {
        }

        public Category GetByUrl(string url)
        {
            return dbContext.Set<Category>().Where(x => x.Url.Equals(url)).FirstOrDefault();
        }
    }
}
