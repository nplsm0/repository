using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility.Model
{
    internal class DepartmentModel
    {
        [ModelName("Название")]
        public string Name { get; set; }
        [ModelName("Родительское подразделение")]
        public string Parent { get; set; }
        [ModelName("Руководитель")]
        public string Manager { get; set; }
        [ModelName("Телефон")]
        public string Phone { get; set; }
    }
}
