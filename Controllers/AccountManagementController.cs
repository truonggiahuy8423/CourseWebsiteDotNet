using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Data;

namespace CourseWebsiteDotNet.Controllers
{
    public class AccountManagementController : Controller
    {
        private void LoadNavbar()
        {
            int? userId = HttpContext.Session.GetInt32("user_id");
            int? role = HttpContext.Session.GetInt32("role");
            int? roleId = HttpContext.Session.GetInt32("role_id");

            if (role == 1)
            {
                var dataTable = SQLExecutor.ExecuteQuery(
                   "SELECT ad.ho_ten, users.anh_dai_dien FROM users INNER JOIN ad ON users.id_ad = ad.id_ad WHERE users.id_user = " + userId
                );
                string? username = dataTable.Rows[0]["ho_ten"].ToString();
                byte[]? avatarData = dataTable.Rows[0]["anh_dai_dien"] != DBNull.Value ? dataTable.Rows[0]["anh_dai_dien"] as byte[] : null;

                // Set data to ViewData
                // Navbar data
                ViewData["username"] = username;
                ViewData["role"] = "Administrator";
                ViewData["avatar_data"] = avatarData;
            }
            else if (role == 2)
            {
                var dataTable = SQLExecutor.ExecuteQuery(
                   $@"SELECT giang_vien.ho_ten, giang_vien.id_giang_vien, users.anh_dai_dien
                    FROM users
                    INNER JOIN giang_vien ON users.id_giang_vien = giang_vien.id_giang_vien
                    WHERE users.id_user = {userId}"
                );
                string? username = dataTable.Rows[0]["ho_ten"].ToString();
                byte[]? avatarData = dataTable.Rows[0]["anh_dai_dien"] != DBNull.Value ? dataTable.Rows[0]["anh_dai_dien"] as byte[] : null;

                // Set data to ViewData
                // Navbar data
                ViewData["username"] = username;
                ViewData["role"] = "Giảng viên";
                ViewData["avatar_data"] = avatarData;
            }
            else if (role == 3)
            {
                var dataTable = SQLExecutor.ExecuteQuery(
                   $@"SELECT hoc_vien.ho_ten, hoc_vien.id_hoc_vien, users.anh_dai_dien
                FROM users
                INNER JOIN hoc_vien ON users.id_hoc_vien = hoc_vien.id_hoc_vien
                WHERE users.id_user = {userId}"
                );
                string? username = dataTable.Rows[0]["ho_ten"].ToString();
                byte[]? avatarData = dataTable.Rows[0]["anh_dai_dien"] != DBNull.Value ? dataTable.Rows[0]["anh_dai_dien"] as byte[] : null;

                // Set data to ViewData
                // Navbar data
                ViewData["username"] = username;
                ViewData["role"] = "Học viên";
                ViewData["avatar_data"] = avatarData;
            }

        }

        private readonly IWebHostEnvironment _hostingEnvironment;
        UserRepository _userRepo = new UserRepository();
        public AccountManagementController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            int userid = HttpContext.Session.GetInt32("user_id").Value;
            var user = _userRepo.GetUserById(userid);
            if (user != null)
            {
                LoadNavbar();
            }


            return View();
        }
        [HttpPost]
        public IActionResult ChangeAvatarAndPassword(AccountRequest accountRequest)
        {
            byte[] imageBytes = null;
            int userId = HttpContext.Session.GetInt32("user_id").Value;
            var user = _userRepo.GetUserById(userId);

            if (accountRequest.Avatar != null && accountRequest.Avatar.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    accountRequest.Avatar.CopyTo(memoryStream);
                    imageBytes = memoryStream.ToArray();
                    user.anh_dai_dien = imageBytes;
                    _userRepo.UpdateUser(user);
                }
            }

            // Check if only changing the avatar (not modifying password)
            if (string.IsNullOrEmpty(accountRequest.CurrentPassword) &&
                string.IsNullOrEmpty(accountRequest.NewPassword) &&
                string.IsNullOrEmpty(accountRequest.ConfirmNewPassword))
            {
                TempData["SuccessMessage"] = "Lưu thay đổi thành công";

                // Clear ModelState errors for password-related fields
                ModelState.Remove("CurrentPassword");
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmNewPassword");

                LoadNavbar();
                return View("Index", accountRequest);
            }

            // Continue with password change logic if password-related fields are provided
            if (ModelState.IsValid)
            {
                try
                {
                    if (user.mat_khau == accountRequest.CurrentPassword)
                    {
                        user.mat_khau = accountRequest.NewPassword;
                        _userRepo.UpdateUser(user);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật khẩu hiện tại của bạn nhập đang không chính xác");
                    }

                    TempData["SuccessMessage"] = "Lưu thay đổi thành công";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lưu thay đổi thất bại");
                }

                LoadNavbar();
                return View("Index", accountRequest);
            }
            else
            {
                LoadNavbar();
                return View("Index", accountRequest);
            }
        }

    }
}
