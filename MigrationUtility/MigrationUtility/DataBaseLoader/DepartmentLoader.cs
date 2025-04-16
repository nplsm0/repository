using DataBaseModel.Context;
using DataBaseModel.Model;
using MigrationUtility.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility.DataBaseLoader
{
    /// <summary>
    /// Представляет собой загрузчик подразделений в БД
    /// </summary>
    internal class DepartmentLoader : IDataBaseLoader<DepartmentModel>
    {
        private PostgresContext _context;
        private ErrorMessenger _errorMessager;

        public DepartmentLoader(PostgresContext context, ErrorMessenger errorMessager)
        {
            _context = context;
            _errorMessager = errorMessager;
        }

        public void Put(IEnumerable<DepartmentModel> departmentModels)
        {
            DepartmentOrderer departmentOrderer = new DepartmentOrderer();
            IEnumerable<DepartmentModel> ordered = departmentOrderer.OrderHierarchy(departmentModels);
            ordered.ForEach(v => InsertOrUpdate(v));
        }

        private void InsertOrUpdate(DepartmentModel departmentModel)
        {
            Department? department = LoadDepartment(departmentModel);
            if (department == null)
            {
                Department parent = _context.Departments.FirstOrDefault(w => departmentModel.Parent == w.Name);
                if (parent == null && !string.IsNullOrEmpty(departmentModel.Parent))
                {
                    _errorMessager.Send($"Подразделение «{departmentModel.Name}» не было загружено, поскольку родительского подразделения «{departmentModel.Parent}» нет в БД");
                    return;
                }
                Department newDepartment = CreateDepartment(departmentModel, parent);
                _context.Departments.Add(newDepartment);
            }
            else
            {
                department.Manager = _context.Employees.FirstOrDefault(w => departmentModel.Manager == w.FullName);
                department.Phone = department.Phone;
            }
            _context.SaveChanges();
        }

        private Department? LoadDepartment(DepartmentModel departmentModel)
        {
            Department? department = null;
            if (string.IsNullOrEmpty(departmentModel.Parent))
                department = _context.Departments.FirstOrDefault(w => w.Name == departmentModel.Name && w.Parent == null);
            else
                department = _context.Departments.FirstOrDefault(w => w.Name == departmentModel.Name && w.Parent != null && w.Parent.Name == departmentModel.Parent);
            return department;
        }

        private Department CreateDepartment(DepartmentModel department, Department parent)
        {
            Employee employee = _context.Employees.FirstOrDefault(w => department.Manager == w.FullName);
            Department d = new Department()
            {
                Name = department.Name,
                Parent = parent,
                Manager = employee,
                Phone = department.Phone
            };
            return d;
        }
    }
}
