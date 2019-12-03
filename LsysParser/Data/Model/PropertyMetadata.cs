using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.Data.Model
{
    public partial class Property
    {
        public override string ToString()
        {
            return $"{NameObj.Name} = {ValueObj.Value}";
        }
    }
}
