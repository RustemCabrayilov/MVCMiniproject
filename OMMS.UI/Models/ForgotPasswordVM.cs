using System.ComponentModel.DataAnnotations;

namespace OMMS.UI.Models
{
    public class ForgotPasswordVM
    {
        [Required]
        public string Email { get; set; }
    }
}
