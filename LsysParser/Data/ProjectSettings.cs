using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.Data
{
    public class ProjectSettings
    {
        /// <summary>
        /// Парсить ли одинаковые товары (обычно они идут в разных категориях)
        /// </summary>
        public bool IsIncludeDuplicatesProducts { get; set; } = false;

        public bool IsShowLog { get; set; } = true;
    }
}
