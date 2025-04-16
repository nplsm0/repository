using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseModel.Model
{
    public class Employee
    {
        [Column("ID")]
        public int Id { get; set; }

        public int? Department { get; set; }
        [ForeignKey("Department")]
        public Department? DepartmentField{ get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password {  get; set; }
        public int? JobTitle { get; set; }
        [ForeignKey("JobTitle")]
        public JobTitle? JobTitleField{ get; set; }
    }
}
