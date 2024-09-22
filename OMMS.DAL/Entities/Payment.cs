using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMMS.DAL.Entities
{
    public class Payment:BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaymentType { get; set; }
        public int LoanId { get; set; }
        public Loan Loan { get; set; }
    }
}
