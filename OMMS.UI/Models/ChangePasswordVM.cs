using System.ComponentModel.DataAnnotations;

namespace OMMS.UI.Models
{
    public class ChangePasswordVM
    {
        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }
        [Display(Name = "Password  Again")]
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Enter same password")]
        public string ConfirmPassword { get; set; }
    }
}
