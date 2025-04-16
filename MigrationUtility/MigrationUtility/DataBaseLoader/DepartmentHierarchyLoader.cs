using DataBaseModel.Context;
using DataBaseModel.Model;
using Microsoft.EntityFrameworkCore;
using MigrationUtility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MigrationUtility.DataBaseLoader
{
    /// <summary>
    /// Представляет собой загрузчик иерархий подразделений из БД
    /// </summary>
    internal class DepartmentHierarchyLoader
    {
        private PostgresContext _context;
        private ErrorMessenger? _errorMessenger;
        public DepartmentHierarchyLoader(PostgresContext context, ErrorMessenger? errorMessenger = null)
        {
            _context = context;
            _errorMessenger = errorMessenger;
        }
        public IEnumerable<string> Get()
        {
            IEnumerable<Tuple<int, Department>> ordered = OrderDepartments(_context.Departments.ToList());
            IEnumerable<string> result = ordered.SelectMany(s => BuildDepartmentBranch(s.Item1, s.Item2));
            return result;
        }

        public IEnumerable<string>? Get(int departmentId)
        {
            Department department = _context.Departments.FirstOrDefault(s => s.Id == departmentId);
            if (department == null)
            {
                _errorMessenger?.Send($"Не удалось найти подразделение (ID = {departmentId})");
                return null;
            } 
            List<string> result = new List<string>();
            List<Department> departments = _context.Departments.ToList();
            Stack<Department> depsParent = GetParentsRecursive(department, departments);
            int level = depsParent.Count();
            for (int i = 0; i < level; i++)
            {
                result.Add(BuildDepartmentLine(i, depsParent.Pop()));
            }
            result.AddRange(BuildDepartmentBranch(level,department));
            return result;
        }

        private Stack<Department> GetParentsRecursive(Department department, IEnumerable<Department> departments)
        {
            Stack<Department> result = new Stack<Department>();
            Department parent = departments.FirstOrDefault(x => x.Id == department.ParentId);
            if (parent != null)
            {
                result.Push(parent);
                result.PushRange(GetParentsRecursive(parent, departments));
            }
            return result;
        }

        private IEnumerable<string> BuildDepartmentBranch(int level, Department d)
        {
            List<string> result = new List<string>();
            result.Add(BuildDepartmentLine(level, d));
            IEnumerable<Employee> employees = _context.Employees.Where(w => w.Department == d.Id).ToList();
            Employee? manager = _context.Employees.FirstOrDefault(w => w.Id == d.ManagerId);
            result.AddRange(employees.Select(s => BuildEmployeeLine(level, s, s == manager)));
            return result;
        }

        private string BuildDepartmentLine(int level, Department d) => $"{new string('=', level + 1)} {d.Name} ID = {d.Id}";

        private string BuildEmployeeLine(int level, Employee e, bool isManager)
        {
            string employeeLine = $"{new string(' ', level)}{(isManager ? '*' : '-')} {e.FullName} ID = {e.Id}";
            JobTitle? jobTitle = _context.JobTitles.FirstOrDefault(w => w.Id == (e.JobTitle ?? -1));
            if (jobTitle != null)
                employeeLine += $"({jobTitle.Name} ID = {e.JobTitle})";
            return employeeLine;
        }

        private IEnumerable<Tuple<int, Department>> OrderDepartments(IEnumerable<Department> departments)
        {
            IEnumerable<Department> root = departments.Where(w => w.Parent == null);
            List<Tuple<int, Department>> ordered = root.Select(s => new Tuple<int, Department>(0, s)).ToList();
            ordered.AddRange(OrderHierarchyRecursive(0, root, departments));
            return ordered;
        }

        private IEnumerable<Tuple<int, Department>> OrderHierarchyRecursive(int level, IEnumerable<Department> roots, IEnumerable<Department> departments)
        {
            level++;
            List<Tuple<int, Department>> ordered = new List<Tuple<int, Department>>();
            foreach (var root in roots)
            {
                var currentLevelItems = departments.Where(p => p.ParentId == root.Id);
                if (currentLevelItems.Any())
                {
                    foreach(var item in currentLevelItems)
                    {
                        ordered.Add(new Tuple<int, Department>(level, item));
                        ordered.AddRange(OrderHierarchyRecursive(level, [item], departments));
                    }
                }
            }
            return ordered;
        }
    }
}
