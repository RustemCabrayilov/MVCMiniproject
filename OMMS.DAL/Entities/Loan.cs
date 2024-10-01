using OMMS.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMMS.DAL.Entities
{
    public class Loan:BaseEntity
    {
        public string Title { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int Terms { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public Status Status { get; set; }
    }
}
