using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseWebsiteDotNet.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            ViewData["chosenItem"] = 4;
            int? userId = HttpContext.Session.GetInt32("user_id");
            int? role = HttpContext.Session.GetInt32("role");
            int? roleId = HttpContext.Session.GetInt32("role_id");

            var dataTable = SQLExecutor.ExecuteQuery(
                    "SELECT ad.ho_ten, users.anh_dai_dien FROM users INNER JOIN ad ON users.id_ad = ad.id_ad WHERE users.id_user = " + userId
                );

            if (dataTable.Rows.Count > 0)
            {
                // Retrieve data from DataTable
                string? username = dataTable.Rows[0]["ho_ten"].ToString();
                byte[]? avatarData = (byte[])dataTable.Rows[0]["anh_dai_dien"];

                // Set data to ViewData
                ViewData["username"] = username;
                ViewData["role"] = "Administrator";
                ViewData["avatar_data"] = avatarData;

                ViewData["users"] = SQLExecutor.ExecuteQuery(
                        "SELECT users.*, COALESCE(ad.ho_ten, giang_vien.ho_ten, hoc_vien.ho_ten) AS ho_ten  " +
                        "FROM users LEFT JOIN ad ON users.id_ad = ad.id_ad LEFT JOIN giang_vien ON users.id_giang_vien = giang_vien.id_giang_vien LEFT JOIN hoc_vien ON users.id_hoc_vien = hoc_vien.id_hoc_vien;"
                    );

                return View("AdministratorUsersList");
            }
            ViewData["ExceptionMessage"] = "Đã có lỗi xảy ra";
            return View("~/Views/Shared/ExeptionPage");
        }

        [HttpGet]
        public IActionResult getInsertUserForm()
        {
            var userRepo = new UserRepository();
            ViewData["users"] = userRepo.GetAllUsers();

            var lecturerRepo = new GiangVienRepository();
            ViewData["lecturers"] = lecturerRepo.GetAllGiangVien();

            var studentRepo = new HocVienRepository();
            ViewData["students"] = studentRepo.GetAllHocVien();

            return View("InsertUserForm");
        }
    }
}

