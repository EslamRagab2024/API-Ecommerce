using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTO
{
    public class RegisterDto
    {
        [Required]
        public string ?Username { get; set; }
        [Required]
        [EmailAddress]
        public string ?Email { get; set; }
        [Required]
        public string ?Password { get; set; }
        [Required]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}
