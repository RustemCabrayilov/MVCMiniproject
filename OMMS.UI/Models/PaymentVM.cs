using OMMS.DAL.Entities;

namespace OMMS.UI.Models
{
	public class PaymentVM
	{
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Date { get; set; }
		public string PaymentType { get; set; }
		public List<string> PaymentTypes { get; set; }
		public int LoanId { get; set; }
		public Loan Loan { get; set; }
		public Customer Customer { get; set; }
		public int CustomerId { get; set; }
	}
}
