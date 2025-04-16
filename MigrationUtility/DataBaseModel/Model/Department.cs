using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseModel.Model
{
    public class Department
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("ParentID")]
        public int? ParentId { get; set; }
        public Department? Parent { get; set; }
        [Column("ManagerID")]
        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
