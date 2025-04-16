using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseModel.Model
{
    [Table("JobTitle")]
    public class JobTitle
    {
        [Column("ID")]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Employee> Employees { get; } = new List<Employee>();
    }
}
