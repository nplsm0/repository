using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility.Model
{
    internal class JobTitleModel
    {
        [ModelName("Название")]
        public string Name { get; set; }
    }
}
