using LsysParser.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace LsysParser.Data.Repository
{
    class ErrorRepository : Repository<Error>
    {
        public ErrorRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
