using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility
{
    /// <summary>
    /// Атрибут имени поля модели данных TSV-файла
    /// </summary>
    internal class ModelNameAttribute : Attribute
    {
        public string Name { get; private set; }
        public ModelNameAttribute(string name) => Name = name;
    }
}
