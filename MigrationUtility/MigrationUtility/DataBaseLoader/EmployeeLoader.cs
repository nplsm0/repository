using DataBaseModel.Context;
using DataBaseModel.Model;
using MigrationUtility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility.DataBaseLoader
{
    /// <summary>
    /// Представляет собой загрузчик сотрудников в БД
    /// </summary>
    internal class EmployeeLoader : IDataBaseLoader<EmployeeModel>
    {
        private PostgresContext _context;
        public EmployeeLoader(PostgresContext context)
        {
            _context = context;
        }

        public void Put(IEnumerable<EmployeeModel> employeeModels)
        {
            employeeModels.ForEach(v => InsertOrUpdate(v));
            _context.SaveChanges();
        }

        private void InsertOrUpdate(EmployeeModel employeeModel)
        {
            Employee? employee = LoadEmployee(employeeModel);
            if (employee == null)
            {
                Employee newEmployee = CreateEmployee(employeeModel);
                _context.Employees.Add(newEmployee);
            }
            else
            {
                employee.DepartmentField = _context.Departments.FirstOrDefault(w => w.Name == employeeModel.Department);
                employee.Login = employeeModel.Login;
                employee.Password = employeeModel.Password;
                employee.JobTitleField = _context.JobTitles.FirstOrDefault(w => w.Name == employeeModel.JobTitle);
            }
        }

        private Employee? LoadEmployee(EmployeeModel employeeModel)
        {
            Employee? employee = _context.Employees.FirstOrDefault(x => x.FullName == employeeModel.FullName);
            return employee;
        }

        private Employee CreateEmployee(EmployeeModel employeeModel)
        {
            Department department = _context.Departments.FirstOrDefault(w => w.Name == employeeModel.Department);
            JobTitle jobTitle = _context.JobTitles.FirstOrDefault(w => w.Name == employeeModel.JobTitle);
            Employee e = new Employee()
            {
                DepartmentField = department,
                FullName = employeeModel.FullName,
                Login = employeeModel.Login,
                Password = employeeModel.Password,
                JobTitleField = jobTitle
            };
            return e;
        }
    }
}
