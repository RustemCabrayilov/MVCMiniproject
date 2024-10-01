using OMMS.DAL.Entities;

namespace OMMS.UI.Models
{
	public class EmployeeVM
	{
        public int Id { get; set; }
        public string Name { get; set; }
		public string Surname { get; set; }
		public string Position { get; set; }
		public int BranchId { get; set; }
		public string BranchName { get; set; }
        public List<Branch> Branchs { get; set; }
        public string AppUserId { get; set; }
        public string UserName { get; set; }
        public List<AppUser> Users { get; set; }
	}
}
