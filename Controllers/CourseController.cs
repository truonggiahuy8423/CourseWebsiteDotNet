using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseWebsiteDotNet.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            ViewData["chosenItem"] = 1;
            int? userId = HttpContext.Session.GetInt32("user_id");
            int? role = HttpContext.Session.GetInt32("role");
            int? roleId = HttpContext.Session.GetInt32("role_id");

            if (role == 1)
            {
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

                    return View("AdministratorCoursesList");
                }

                ViewData["ExceptionMessage"] = "Đã có lỗi xảy ra";
                return View("~/Views/Shared/ExeptionPage");

            }
            else if (role == 2)
            {
                return View("StudentCoursesList");

            }
            else if (role == 3)
            {
                return View("LecturerCoursesList");

            }

            ViewData["ExceptionMessage"] = "Đã có lỗi xảy ra";
            return View("~/Views/Shared/ExeptionPage");

        }
    }
}
