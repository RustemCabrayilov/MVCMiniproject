using System.ComponentModel.DataAnnotations;

namespace OMMS.UI.Models
{
    public class SignUpVM
    {
        [Display(Name = "User Name")]
        [Required]
        public string UserName { get; set; }
        [Display(Name = "Email")]
        [Required]
        public string Email { get; set; }
        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }
        [Display(Name = "Password  Again")]
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Enter same password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string PhoneNumber {  get; set; }
    }
}
