using System.ComponentModel.DataAnnotations;

namespace OMMS.UI.Models
{
    public class SignUpVM
    {

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
  
        public string ConfirmPassword { get; set; }
        public string PhoneNumber {  get; set; }
    }
}
