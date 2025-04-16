using DataBaseModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility.Model
{
    internal class EmployeeModel
    {
        [ModelName("Подразделение")]
        public string Department { get; set; }
        [ModelName("ФИО")]
        public string FullName { get; set; }
        [ModelName("Логин")]
        public string Login { get; set; }
        [ModelName("Пароль")]
        public string Password {  get; set; }
        [ModelName("Должность")]
        public string JobTitle {  get; set; }

    }
}
