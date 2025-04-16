using MigrationUtility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MigrationUtility.DataBaseLoader
{
    /// <summary>
    /// Представляет собой сортировщик подразделений по уровню вложенноси иерархии
    /// </summary>
    internal class DepartmentOrderer
    {
        public IEnumerable<DepartmentModel> OrderHierarchy(IEnumerable<DepartmentModel> departmentModels)
        {
            IEnumerable<DepartmentModel> root = departmentModels.Where(p => string.IsNullOrEmpty(p.Parent));
            List<DepartmentModel> orderedHierarchy = root.ToList();
            orderedHierarchy.AddRange(OrderHierarchyRecursive(root, departmentModels));

            IEnumerable<DepartmentModel> lostHierarchyItems = departmentModels.ExceptBy(orderedHierarchy.Select(s => (s.Name, s.Parent)), x => (x.Name, x.Parent));
            IEnumerable<DepartmentModel> lostHierarchyItemsRoot = lostHierarchyItems.ExceptBy(lostHierarchyItems.Select(s => s.Name), x => x.Parent);
            List<DepartmentModel> orderedLostHierarchyItems = lostHierarchyItemsRoot.ToList();
            orderedLostHierarchyItems.AddRange(OrderHierarchyRecursive(lostHierarchyItemsRoot, lostHierarchyItems));
            orderedHierarchy.AddRange(orderedLostHierarchyItems);
            return orderedHierarchy;
        }

        private IEnumerable<DepartmentModel> OrderHierarchyRecursive(IEnumerable<DepartmentModel> roots, IEnumerable<DepartmentModel> departmentModels)
        {
            List<DepartmentModel> ordered = new List<DepartmentModel>();
            foreach (var v in roots)
            {
                var items = departmentModels.Where(p => p.Parent == v.Name);
                if (items.Any())
                {
                    ordered.AddRange(items);
                    ordered.AddRange(OrderHierarchyRecursive(items, departmentModels));
                }
            }
            return ordered;
        }
    }
}
