using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Relational;
using System.Data;

namespace CourseWebsiteDotNet.Controllers
{
    public class SubjectController : Controller
    {
        public IActionResult Index()
        {
            ViewData["chosenItem"] = 5;
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

                return View("AdministratorSubjectsList");
            }
            ViewData["ExceptionMessage"] = "Đã có lỗi xảy ra";
            return View("~/Views/Shared/ExeptionPage");
        }
        [HttpGet]
        public IActionResult GetListSubject(string? name)
        {
            string query = name == null ? "SELECT * FROM mon_hoc" : $"SELECT * FROM mon_hoc WHERE ten_mon_hoc LIKE '%{name}%'";
            var dataTable = SQLExecutor.ExecuteQuery(query);

            List<MonHocModel> danhSachMonHoc = new List<MonHocModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                MonHocModel monHoc = new MonHocModel
                {
                    id_mon_hoc = Convert.ToInt32(row["id_mon_hoc"]),
                    ten_mon_hoc = row["ten_mon_hoc"].ToString()
                };
                danhSachMonHoc.Add(monHoc);
            }

            return Json(danhSachMonHoc, new System.Text.Json.JsonSerializerOptions());

        }
        [HttpPost]
        public IActionResult ThemMonHoc(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var monHoc = new MonHocModel { ten_mon_hoc = name };
                MonHocRepository subjectRepo = new MonHocRepository();
                var checkTontai = subjectRepo.GetAllMonHoc().FirstOrDefault(c => c.ten_mon_hoc == name);
                if (checkTontai != null)
                {
                    return Json(new { status = 4, mess = "Tên môn học đã tồn tại" });
                }

                Response response = subjectRepo.InsertMonHoc(monHoc);

                if (response.state == true)
                {
                    return Json(new { status = 1, mess = response.message });
                }
                else
                {
                    return Json(new { status = 2, mess = response.message });
                }
            }

            return Json(new { status = 3, mess = "Tên môn học không thể để trống" });
        }
        [HttpPost]
        public IActionResult SuaMonHoc(int id, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {

                MonHocRepository subjectRepo = new MonHocRepository();
                MonHocModel? monHoc = subjectRepo.GetMonHocById(id);

                if (monHoc == null)
                    return Json(new { status = 2, mess = "Sửa thất bại" });

                var checkMonhoc = subjectRepo.GetAllMonHoc();
                if (checkMonhoc.Where(c => c.ten_mon_hoc == name).Count() > 0)
                    return Json(new { status = 4, mess = "Tên môn học đã tồn tại" });


                monHoc.ten_mon_hoc = name;
                Response response = subjectRepo.UpdateMonHoc(monHoc);

                if (response.state == true)
                {
                    return Json(new { status = 1, mess = response.message });
                }
                else
                {
                    return Json(new { status = 2, mess = response.message });
                }
            }

            return Json(new { status = 3, mess = "Tên môn học không thể để trống" });
        }
        [HttpPost]
        public IActionResult DeleteSelected(int id)
        {
            try
            {

                MonHocRepository subjectRepo = new MonHocRepository();
                LopHocRepository lopHocRepo = new LopHocRepository();

                IEnumerable<LopHocModel> checkMonHoc = lopHocRepo.GetAllLopHoc().Where(c => c.id_mon_hoc == id);
                if (checkMonHoc.Count() > 0)
                {
                    var monHoc = subjectRepo.GetMonHocById(id);
                    return Json(new { status = 3, mess = $"Không thể xóa môn học: {monHoc.ten_mon_hoc} vì nó đang được sử dụng" });
                }


                Response response = subjectRepo.DeleteMonHoc(id);
                if (response.state == true)
                {
                    return Json(new { status = 1, mess = response.message });
                }
                else
                {
                    return Json(new { status = 2, mess = response.message });
                }

            }
            catch (Exception ex)
            {
                return Json(new { status = 4, mess = "Lỗi, Xóa thất bại" });

            }
        }

    }
}
