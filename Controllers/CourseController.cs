using CourseWebsiteDotNet.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CourseWebsiteDotNet.Controllers
{
    public class CourseController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CourseController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
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
        public class ModelForTest2
        {
            public string data { get; set; }
            public int data2 { get; set; }
        }
        [HttpPost]
        [HttpGet]
        public IActionResult test3([FromBody] dynamic dataRe, [FromQuery] int id)
        {
            var data = dataRe.GetProperty("data").GetString();
            //var data = dataRe.data;
            //var data2 = dataRe.data2;

            // Xử lý dữ liệu từ phần thân của yêu cầu (POST) và query string (GET)
            // Sử dụng id từ query string (GET)

            return Ok($"Received data - id: {id}, data from body: {data}");
        }
        public class FormData
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        //public IActionResult test4([FromBody] string name, int id)
        //{
        //    using (var reader = new StreamReader(HttpContext.Request.Body))
        //    {
        //        var body = reader.ReadAsync();
        //        var formData = JsonConvert.DeserializeObject<FormData>(body);

        //        // Xử lý dữ liệu từ phần thân của yêu cầu (POST)
        //        // Sử dụng formData.id và formData.name

        //        return Ok($"Received data - id: {formData.id}, name: {formData.name}");
        //    }
        //}
        [HttpPost]
        public IActionResult test4(string name, int id)
        {

            // Xử lý dữ liệu từ phần thân của yêu cầu (POST) và query string (GET)
            // Sử dụng id từ query string (GET)

            return Ok($"Received data - id: {id}, name: {name}");
        }
        [HttpPost]
        public IActionResult test2([FromBody] ModelForTest2 dataRe)
        {
            //var data = _httpContextAccessor.HttpContext.Request.Query["data"];
            //var idGiangVien = _httpContextAccessor.HttpContext.Request.Query["data2"];
            var data = dataRe.data;
            return Ok(data);
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
        [HttpPost]
        public IActionResult insertCourse([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            var courseModel = new LopHocModel();
            courseModel.id_mon_hoc = Convert.ToInt32(obj.id_mon_hoc);
            courseModel.ngay_bat_dau = DateTime.Parse(obj.ngay_bat_dau);
            courseModel.ngay_ket_thuc = DateTime.Parse(obj.ngay_ket_thuc);
            var courseRepo = new LopHocRepository();
            var processResult = courseRepo.InsertLopHoc(courseModel);
            dynamic response = new
            {
                state = processResult.state,
                message = processResult.message,
                auto_increment_id = processResult.insertedId
            };
            return Ok(JsonConvert.SerializeObject(response));
        }
        public IActionResult insertLecturersIntoClass([FromBody] JsonDocument dataReceived)
        {
            //string json = HttpContext.Request.Form["json"];

            //dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(json);
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            var id_lop_hoc = Convert.ToInt32(obj.id_lop_hoc);
            dynamic lecturer_id_list = (ExpandoObject)obj.lecturer_id_list;

            ExpandoObject response = new ExpandoObject();
            foreach (var property in (IDictionary<string, object>)lecturer_id_list)
            {
                PhanCongGiangVienModel pcModel = new PhanCongGiangVienModel()
                {
                    id_giang_vien = Convert.ToInt32(property.Key),
                    id_lop_hoc = Convert.ToInt32(id_lop_hoc)
                };
                PhanCongGiangVienRepository pcRepo = new PhanCongGiangVienRepository();
                ((IDictionary<string, object>)response)[property.Key] = pcRepo.InsertPhanCongGiangVien(pcModel);

            }


            return Ok(JsonConvert.SerializeObject(response));

        }
        public IActionResult getInsertClassForm()
        {
            var subjectRepo = new MonHocRepository();
            ViewData["subjects"] = subjectRepo.GetAllMonHoc();

            var lecturerRepo = new GiangVienRepository();
            ViewData["lecturers"] = lecturerRepo.GetAllGiangVien();


            return View("InsertClassForm");
        }
        [HttpGet]
        public IActionResult Information(int? courseid)
        {
            if (courseid == null)
            {
                return Index();
            }
            //if (!int.TryParse(courseid.ToString(), out int cid))
            //{
            //    ViewData["ExceptionMessage"] = "Hành động không xác định";
            //    return View("ExceptionPage");
            //}
            DataTable courses = SQLExecutor.ExecuteQuery(
                " SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc, COUNT(hoc_vien_tham_gia.id_hoc_vien) as so_luong_hoc_vien "
              + " FROM lop_hoc"
            + " INNER JOIN mon_hoc ON lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc " +
            " LEFT JOIN hoc_vien_tham_gia ON lop_hoc.id_lop_hoc = hoc_vien_tham_gia.id_lop_hoc " +
            $" WHERE lop_hoc.id_lop_hoc = {(int)courseid} " +
            " GROUP BY lop_hoc.id_lop_hoc, lop_hoc.ngay_bat_dau, lop_hoc.ngay_ket_thuc, lop_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc; ");
            if (courses.Rows.Count <= 0)
            {
                ViewData["ExceptionMessage"] = "Lớp học không tồn tại";
                return View("ExceptionPage");
            }

            HttpContext.Session.SetInt32("course", (int)courseid);

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
                    // Navbar data
                    ViewData["username"] = username;
                    ViewData["role"] = "Administrator";
                    ViewData["avatar_data"] = avatarData;

                    // MainSection


                    // Layout data
                    ViewData["class_name"] = "";
                    // Mainsection
                    return View("AdministratorCourseInformation");
                }
            }
            else if (role == 2)
            {
                return View("LecturerCourseInformation");

            }
            else if (role == 3)
            {
                return View("AdministratorCourseInformation");

            }
            return View("AdministratorCourseInformation");

        }
        public IActionResult deleteCourse([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            ExpandoObject response = new ExpandoObject();
            foreach (string id_lop_hoc in obj.courses)
            {
                LopHocRepository lopHocRepository = new LopHocRepository();
                ((IDictionary<string, object>)response)[id_lop_hoc] = lopHocRepository.DeleteLopHoc(Convert.ToInt32(id_lop_hoc));
            }

            return Ok(JsonConvert.SerializeObject(response));
        }
    }
}
