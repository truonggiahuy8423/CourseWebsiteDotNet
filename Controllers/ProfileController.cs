using CourseWebsiteDotNet.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using System.Data;

namespace CourseWebsiteDotNet.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index(int? id, bool isLecturer)
        {
            int? userId = HttpContext.Session.GetInt32("user_id");
            int? role = HttpContext.Session.GetInt32("role");
            int? roleId = HttpContext.Session.GetInt32("role_id");
            var model = new UserModel();
            if (role == 1)
            { // Admin
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
                };
            }
            else if (role == 2)
            {
                var dataTable = SQLExecutor.ExecuteQuery(
                    "SELECT giang_vien.ho_ten, giang_vien.id_giang_vien, users.anh_dai_dien " +
                    "FROM users " +
                    "INNER JOIN giang_vien ON users.id_giang_vien = giang_vien.id_giang_vien " +
                    $"WHERE users.id_user = {userId}"
                );

                if (dataTable.Rows.Count > 0)
                {
                    // Retrieve data from DataTable
                    string? username = dataTable.Rows[0]["ho_ten"].ToString();
                    byte[]? avatarData = (byte[])dataTable.Rows[0]["anh_dai_dien"];

                    // Set data to ViewData
                    // Navbar
                    ViewData["username"] = username;
                    ViewData["role"] = "Giảng viên";
                    ViewData["avatar_data"] = avatarData;
                };
            }
            else if (role == 3)
            {
                var dataTable = SQLExecutor.ExecuteQuery(
                    "SELECT hoc_vien.ho_ten, hoc_vien.id_hoc_vien, users.anh_dai_dien " +
                    "FROM users " +
                    "INNER JOIN hoc_vien ON users.id_hoc_vien = hoc_vien.id_hoc_vien " +
                    $"WHERE users.id_user = {userId}"
                );

                if (dataTable.Rows.Count > 0)
                {
                    // Retrieve data from DataTable
                    string? username = dataTable.Rows[0]["ho_ten"].ToString();
                    byte[]? avatarData = (byte[])dataTable.Rows[0]["anh_dai_dien"];

                    // Set data to ViewData
                    // Navbar
                    ViewData["username"] = username;
                    ViewData["role"] = "Học viên";
                    ViewData["avatar_data"] = avatarData;
                };
            }


            if (isLecturer) {
            // giang vien
                var teacherID = Convert.ToInt32(id);
                var lecturersRepo = new GiangVienRepository();
                var userModel = new UserModel();
                var userData = SQLExecutor.ExecuteQuery(
                    "SELECT * " +
                    "FROM users " +
                    $"WHERE id_giang_vien = {teacherID}");

                ViewData["id"] = teacherID;
                ViewData["user"] = SQLExecutor.ExecuteQuery(
                    "SELECT * " +
                    "FROM giang_vien " +
                    $"WHERE id_giang_vien = {teacherID}");
                byte[]? avatarProfile = userData.Rows[0]["anh_dai_dien"] != DBNull.Value ? (byte[])userData.Rows[0]["anh_dai_dien"] : (byte[])null;
                ViewData["avatar_profile"] = avatarProfile;
                ViewData["attend"] = SQLExecutor.ExecuteQuery(
                    "SELECT lh.id_lop_hoc, mh.id_mon_hoc, mh.ten_mon_hoc, lh.ngay_bat_dau, lh.ngay_ket_thuc " +
                    "FROM phan_cong_giang_vien pc, lop_hoc lh, mon_hoc mh " +
                    $"WHERE pc.id_giang_vien = {teacherID} " +
                    "AND pc.id_lop_hoc = lh.id_lop_hoc " +
                    "AND mh.id_mon_hoc = lh.id_mon_hoc");

                ViewData["isLecturer"] = true;
                return View("ProfilePage");
            } else
            {
            // hoc vien
                var studentID = Convert.ToInt32(id);
                var studentsRepo = new HocVienRepository();
                var userData = SQLExecutor.ExecuteQuery(
                    "SELECT * " +
                    "FROM users " +
                    $"WHERE id_hoc_vien = {studentID}");

                ViewData["id"] = studentID;
                ViewData["user"] = SQLExecutor.ExecuteQuery(
                    "SELECT * " +
                    "FROM hoc_vien " +
                    $"WHERE id_hoc_vien = {studentID}");

                byte[]? avatarProfile = userData.Rows[0]["anh_dai_dien"] != DBNull.Value ? (byte[])userData.Rows[0]["anh_dai_dien"] : (byte[])null;
                ViewData["avatar_profile"] = avatarProfile;

                ViewData["attend"] = SQLExecutor.ExecuteQuery(
                    "SELECT lh.id_lop_hoc, mh.id_mon_hoc, mh.ten_mon_hoc, lh.ngay_bat_dau, lh.ngay_ket_thuc " +
                    "FROM hoc_vien_tham_gia tg, lop_hoc lh, mon_hoc mh " +
                    $"WHERE tg.id_hoc_vien = {studentID} " +
                    "AND tg.id_lop_hoc = lh.id_lop_hoc " +
                    "AND mh.id_mon_hoc = lh.id_mon_hoc");
                ViewData["isLecturer"] = false;
                return View("ProfilePage");
            }

        }
    }
}
