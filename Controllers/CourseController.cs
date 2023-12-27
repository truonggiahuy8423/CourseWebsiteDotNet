using CourseWebsiteDotNet.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

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
                    // Navbar
                    ViewData["username"] = username;
                    ViewData["role"] = "Administrator";
                    ViewData["avatar_data"] = avatarData;

                    // Mainsection
                    ViewData["courses"] = SQLExecutor.ExecuteQuery(
                        "SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc FROM lop_hoc INNER JOIN mon_hoc on lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc order by lop_hoc.ngay_bat_dau desc"
                    );
                    (ViewData["courses"] as DataTable).Columns.Add("lecturers", typeof(DataTable));

                    foreach (DataRow row in (ViewData["courses"] as DataTable).Rows)
                    {
                        row["lecturers"] = SQLExecutor.ExecuteQuery(
                            "SELECT giang_vien.id_giang_vien, giang_vien.ho_ten " +
                            "FROM phan_cong_giang_vien INNER JOIN giang_vien ON phan_cong_giang_vien.id_giang_vien = giang_vien.id_giang_vien " +
                            $"WHERE phan_cong_giang_vien.id_lop_hoc = {row["id_lop_hoc"]};"
                            );
                        
                    }


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

        public IActionResult test()
        {
            var rs = SQLExecutor.ExecuteQuery("SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc FROM lop_hoc INNER JOIN mon_hoc on lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc order by lop_hoc.ngay_bat_dau desc");
            string json = JsonConvert.SerializeObject(rs);
            return Ok(json);
        }
        public IActionResult getListOfCourses()
        {
            var courses = SQLExecutor.ExecuteQuery(
                        "SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc FROM lop_hoc INNER JOIN mon_hoc on lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc order by lop_hoc.ngay_bat_dau desc"
                    );
            courses.Columns.Add("lecturers", typeof(DataTable));

            foreach (DataRow row in courses.Rows)
            {
                row["lecturers"] = SQLExecutor.ExecuteQuery(
                    "SELECT giang_vien.id_giang_vien, giang_vien.ho_ten " +
                    "FROM phan_cong_giang_vien INNER JOIN giang_vien ON phan_cong_giang_vien.id_giang_vien = giang_vien.id_giang_vien " +
                    $"WHERE phan_cong_giang_vien.id_lop_hoc = {row["id_lop_hoc"]};"
                    );

            }
            return Ok(JsonConvert.SerializeObject(courses));
        }
        public IActionResult getInsertClassForm()
        {
            var subjectRepo = new MonHocRepository();
            ViewData["subjects"] = subjectRepo.GetAllMonHoc();

            var lecturerRepo = new GiangVienRepository();
            ViewData["lecturers"] = lecturerRepo.GetAllGiangVien();


            return View("InsertClassForm");
        }
    }
}
