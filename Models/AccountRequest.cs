using System.ComponentModel.DataAnnotations;

namespace CourseWebsiteDotNet.Models
{
    public class AccountRequest
    {

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [MinLength(8, ErrorMessage = "Mật khẩu mới phải có ít nhất 8 ký tự.")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu.")]
        public string ConfirmNewPassword { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
