using MigrationUtility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MigrationUtility
{
    /// <summary>
    /// Представляет собой валидатор данных TSV-файлов
    /// </summary>
    /// <typeparam name="T">Тип модели TSV-файла</typeparam>
    internal class DataValidator<T>
    {
        private Func<T, bool> _filter;

        public void SetFilter(Func<T, bool> predicate)
        {
            _filter = predicate;
        }

        public bool Validate(T model)
        {
            return _filter.Invoke(model);
        }
    }
}
