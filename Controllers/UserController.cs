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

                return View("AdministratorUsersList");
            }
            ViewData["ExceptionMessage"] = "Đã có lỗi xảy ra";
            return View("~/Views/Shared/ExeptionPage");
        }
    }
}

