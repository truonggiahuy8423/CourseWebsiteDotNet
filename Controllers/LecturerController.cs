using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.DependencyResolver;

namespace CourseWebsiteDotNet.Controllers
{
    public class LecturerController : Controller
    {
        public IActionResult Index()
        {
            ViewData["chosenItem"] = 2;
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

                // Mainsection
                ViewData["teachers"] = SQLExecutor.ExecuteQuery(
                    "SELECT giang_vien.id_giang_vien, giang_vien.ho_ten, giang_vien.email " +
                    "FROM giang_vien"
                );

                return View("AdministratorLecturersList");
            }
            ViewData["ExceptionMessage"] = "Đã có lỗi xảy ra";
            return View("~/Views/Shared/ExeptionPage");
        }
        public IActionResult getInsertForm()
        {
            var lecturersModel = new GiangVienRepository();

            ViewData["lecturers"] = lecturersModel.GetAllGiangVien();

            return View("InsertTeacherForm");
        }

        public JsonResult insertTeacher()
        {
            var teacherData = json_decode(json_encode($this->request->getJSON()), true);
            var teacher = new GiangVienModel();
            var model = new GiangVienModel();

            teacher.ho_ten = teacherData["ho_ten"];
            teacher.ngay_sinh = teacherData["ngay_sinh"];
            teacher.gioi_tinh = teacherData["gioi_tinh"];
            teacher.email = teacherData["email"];

                return this->response->setJSON($model->insertGiangVien($teacher));
        }
    }
}
