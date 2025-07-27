using System.ComponentModel.DataAnnotations;

namespace ComiBerry.ViewModels
{
    public class VIEWMyAccountPasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Password confirmation is required.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
