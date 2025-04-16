using MigrationUtility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility.DataBaseLoader
{
    /// <summary>
    /// Определяет операцию взаимодействия с БД для модели
    /// </summary>
    /// <typeparam name="T">Модель данных TSV-фалйа</typeparam>
    internal interface IDataBaseLoader<T>
    {
        void Put(IEnumerable<T> models);
    }
}
