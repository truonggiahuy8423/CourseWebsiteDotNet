using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Text.Json;

namespace CourseWebsiteDotNet.Controllers
{
    public class AttendanceController : Controller
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
        private bool checkPreviledge(int courseid)
        {
            int? userId = HttpContext.Session.GetInt32("user_id");
            int? role = HttpContext.Session.GetInt32("role");
            int? roleId = HttpContext.Session.GetInt32("role_id");

            if (role == 1)
            {
                return true;
            }
            else if (role == 2)
            {
                var rs = SQLExecutor.ExecuteQuery($@"SELECT * FROM phan_cong_giang_vien WHERE phan_cong_giang_vien.id_giang_vien = {roleId} AND phan_cong_giang_vien.id_lop_hoc = {courseid}");
                return rs.Rows.Count == 0 ? false : true;

            }
            else if (role == 3)
            {
                return false;
            }
            return false;
        }
        public IActionResult Index()
        {
            return View();
        }
        private string KiemTraTinhTrang(string ngayBatDau, string ngayKetThuc)
        {
            DateTime datetimeBatDau = DateTime.ParseExact(ngayBatDau, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime datetimeKetThuc = DateTime.ParseExact(ngayKetThuc, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime datetimeHienTai = DateTime.Now;

            datetimeBatDau = datetimeBatDau.Date;
            datetimeKetThuc = datetimeKetThuc.Date.Add(new TimeSpan(23, 59, 59)); // Set time to end of the day

            if (datetimeBatDau <= datetimeHienTai && datetimeKetThuc >= datetimeHienTai)
            {
                return "<span class=\"class__item--inprocess\">Đang diễn ra</span>";
            }
            else if (datetimeKetThuc < datetimeHienTai)
            {
                return "<span class=\"class__item--over\">Đã kết thúc</span>";
            }
            else
            {
                return "<span class=\"class__item--upcoming\">Sắp diễn ra</span>";
            }
        }
        [HttpGet]
        public IActionResult getSchedule()
        {
            int? courseid = HttpContext.Session.GetInt32("courseid");

            return Ok(JsonConvert.SerializeObject(SQLExecutor.ExecuteQuery($@"
                   SELECT buoi_hoc.id_buoi_hoc, buoi_hoc.trang_thai, buoi_hoc.id_phong, DATE_FORMAT(buoi_hoc.ngay, '%d/%m/%Y') AS ngay, DAYOFWEEK(buoi_hoc.ngay) AS thu,
                    ca.thoi_gian_bat_dau, ca.thoi_gian_ket_thuc
                    FROM buoi_hoc INNER JOIN ca ON buoi_hoc.id_ca = ca.id_ca
                    WHERE buoi_hoc.id_lop_hoc = {courseid} ORDER BY buoi_hoc.ngay ASC
            ")));
        }
        [HttpPost] 
        public IActionResult submitAttendanceRecords([FromBody] JsonDocument dataReceived)
        {
            List<ExpandoObject> attends = JsonConvert.DeserializeObject<List<ExpandoObject>>(dataReceived.RootElement.ToString());
			int? courseid = HttpContext.Session.GetInt32("courseid");
			int? scheduleid = HttpContext.Session.GetInt32("scheduleid");

            BuoiHocModel schedule = new BuoiHocRepository().GetBuoiHocById((int)scheduleid);
            if (schedule.trang_thai != 2)
            {
                schedule.trang_thai = 2;
				var rs = new BuoiHocRepository().UpdateBuoiHoc(schedule);
			}
            string ms = "";
			for (int i = 0; i < attends.Count; i++)
            {
                dynamic attendance = attends[i];
                DiemDanhModel attendanceModel = new DiemDanhRepository().GetDiemDanhById((int)scheduleid, Convert.ToInt32(attendance.studentId));
                if (attendanceModel != null)
                {
                    attendanceModel.co_mat = Convert.ToInt32(attendance.isAttend);
                    attendanceModel.ghi_chu = attendance.note;
                    new DiemDanhRepository().UpdateDiemDanh(attendanceModel);
                }
            }
            var rs2 = new
            {
                state = true,
                message = "Cập nhật thông tin thành công" + ms
            };
            return Ok(JsonConvert.SerializeObject(rs2));
		}
		[HttpGet]
        public IActionResult getAttendanceRecords(int? scheduleId)
        {
            if (scheduleId == null || scheduleId == 0) {
                return BadRequest();
            }
            List<BuoiHocModel> model = new BuoiHocRepository().GetAllBuoiHocByCourseId((int)scheduleId);
			


			if (!model.Contains(new BuoiHocModel() { id_buoi_hoc = scheduleId }))
            {
                var notFoundResponse = new
                {
                    state = false,
                    message = "Buổi học không tồn tại."
                };
                return Json(notFoundResponse);
            }
            HttpContext.Session.SetInt32("scheduleid", (int)scheduleId);
            int? courseid = HttpContext.Session.GetInt32("courseid");

            return Ok(JsonConvert.SerializeObject(SQLExecutor.ExecuteQuery($@"
                   SELECT diem_danh.id_buoi_hoc, diem_danh.co_mat, hoc_vien.id_hoc_vien, hoc_vien.ho_ten, diem_danh.ghi_chu FROM buoi_hoc INNER JOIN diem_danh on buoi_hoc.id_buoi_hoc = diem_danh.id_buoi_hoc 
                    inner join hoc_vien on diem_danh.id_hoc_vien = hoc_vien.id_hoc_vien
                    where buoi_hoc.id_buoi_hoc = {scheduleId}
            ")));
        }
        [HttpGet]
        public IActionResult Attendance(int? courseid)
        {
            if (courseid == null || courseid == 0)
            {
                LoadNavbar();
                ViewData["ExceptionMessage"] = "Lớp học không tồn tại";
                return View("ExceptionPage");
            }

            DataTable courses = SQLExecutor.ExecuteQuery(
                " SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc, COUNT(hoc_vien_tham_gia.id_hoc_vien) as so_luong_hoc_vien "
              + " FROM lop_hoc"
            + " INNER JOIN mon_hoc ON lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc " +
            " LEFT JOIN hoc_vien_tham_gia ON lop_hoc.id_lop_hoc = hoc_vien_tham_gia.id_lop_hoc " +
            $" WHERE lop_hoc.id_lop_hoc = {(int)courseid} " +
            " GROUP BY lop_hoc.id_lop_hoc, lop_hoc.ngay_bat_dau, lop_hoc.ngay_ket_thuc, lop_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc; ");
            if (courses.Rows.Count <= 0)
            {
                LoadNavbar();
                ViewData["ExceptionMessage"] = "Lớp học không tồn tại";
                return View("ExceptionPage");
            }
            if (!checkPreviledge((int)courseid))
            {
                LoadNavbar();
                ViewData["ExceptionMessage"] = "Quyền truy cập hạn chế";
                return View("ExceptionPage");
            }
            int? role = HttpContext.Session.GetInt32("role");

            LoadNavbar();
            if (role == 1)
            {
                HttpContext.Session.SetInt32("courseid", (int)courseid);



                // Layout data
                ViewData["class_name"] = $"{courses.Rows[0]["ten_mon_hoc"]} {courses.Rows[0]["id_mon_hoc"].ToString().PadLeft(3, '0')}.{courses.Rows[0]["id_lop_hoc"].ToString().PadLeft(6, '0')}";
                ViewData["state"] = KiemTraTinhTrang((string)courses.Rows[0]["ngay_bat_dau"], (string)courses.Rows[0]["ngay_ket_thuc"]);
                ViewData["student_quantity"] = Convert.ToInt32(courses.Rows[0]["so_luong_hoc_vien"]);
                ViewData["courseid"] = Convert.ToInt32(courseid);

                return View("AttendancePage");
            }
            else if (role == 2)
            {
                HttpContext.Session.SetInt32("courseid", (int)courseid);


                // Layout data
                ViewData["class_name"] = $"{courses.Rows[0]["ten_mon_hoc"]} {courses.Rows[0]["id_mon_hoc"].ToString().PadLeft(3, '0')}.{courses.Rows[0]["id_lop_hoc"].ToString().PadLeft(6, '0')}";
                ViewData["state"] = KiemTraTinhTrang((string)courses.Rows[0]["ngay_bat_dau"], (string)courses.Rows[0]["ngay_ket_thuc"]);
                ViewData["student_quantity"] = Convert.ToInt32(courses.Rows[0]["so_luong_hoc_vien"]);
                ViewData["courseid"] = Convert.ToInt32(courseid);

                return View("AttendancePage");

            }
            ViewData["ExceptionMessage"] = "Đã có lỗi xảy ra, vui lòng đăng nhập lại";
            return View("ExceptionPage");
        }
    }
}
