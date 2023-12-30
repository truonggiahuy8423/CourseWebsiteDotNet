using CourseWebsiteDotNet.Models;
using Google.Protobuf;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Tls.Crypto;
using System;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CourseWebsiteDotNet.Controllers
{
    public class CourseController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CourseController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private bool checkPreviledge(int courseid)
        {
			int? userId = HttpContext.Session.GetInt32("user_id");
			int? role = HttpContext.Session.GetInt32("role");
			int? roleId = HttpContext.Session.GetInt32("role_id");

            if (role == 1)
            {
                return true;
            } else if (role == 2)
            {
                var rs = SQLExecutor.ExecuteQuery($@"SELECT * FROM phan_cong_giang_vien WHERE phan_cong_giang_vien.id_giang_vien = {roleId} AND phan_cong_giang_vien.id_lop_hoc = {courseid}");
                return rs.Rows.Count == 0 ? false : true;

            } else if (role == 3)
            {
				var rs = SQLExecutor.ExecuteQuery($@"SELECT * FROM hoc_vien_tham_gia WHERE hoc_vien_tham_gia.id_hoc_vien = {roleId} AND hoc_vien_tham_gia.id_lop_hoc = {courseid}"
);
				return rs.Rows.Count == 0 ? false : true;
			}
            return false;
		}
        public IActionResult Index()
        {
            ViewData["chosenItem"] = 1;
            LoadNavbar();
            int? userId = HttpContext.Session.GetInt32("user_id");
            int? role = HttpContext.Session.GetInt32("role");
            int? roleId = HttpContext.Session.GetInt32("role_id");
            if (role == 1)
            {
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
            else if (role == 2)
            {
                // Mainsection
                ViewData["courses"] = SQLExecutor.ExecuteQuery(
                        $@"SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc
                            FROM lop_hoc 
                            INNER JOIN mon_hoc on lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc 
                            where lop_hoc.id_lop_hoc in (select phan_cong_giang_vien.id_lop_hoc from phan_cong_giang_vien where phan_cong_giang_vien.id_giang_vien = {(int)roleId}) order by lop_hoc.ngay_bat_dau desc
            "
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

                return View("LecturerCoursesList");
            }
            else if (role == 3)
            {
                // Mainsection
                ViewData["courses"] = SQLExecutor.ExecuteQuery(
                    $@"SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc
                        FROM lop_hoc 
                        INNER JOIN mon_hoc on lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc 
                        where lop_hoc.id_lop_hoc in (select hoc_vien_tham_gia.id_lop_hoc from hoc_vien_tham_gia where hoc_vien_tham_gia.id_hoc_vien = {(int)roleId}) order by lop_hoc.ngay_bat_dau desc"
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

                return View("StudentCoursesList");
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
        [HttpGet]
        public IActionResult test4(string name, int id)
        {

            // Xử lý dữ liệu từ phần thân của yêu cầu (POST) và query string (GET)
            // Sử dụng id từ query string (GET)

            return Ok(JsonConvert.SerializeObject((new HocVienRepository()).GetHocVienById(id)));
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
        public IActionResult getListOfCoursesForTeacher()
        {
            int? roleId = HttpContext.Session.GetInt32("role_id");
            
            var courses = SQLExecutor.ExecuteQuery(
                            $@"SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc
                            FROM lop_hoc 
                            INNER JOIN mon_hoc on lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc 
                            where lop_hoc.id_lop_hoc in (select phan_cong_giang_vien.id_lop_hoc from phan_cong_giang_vien where phan_cong_giang_vien.id_giang_vien = {(int)roleId})
            ");
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

        public IActionResult getListOfCoursesForStudent()
        {
            int? roleId = HttpContext.Session.GetInt32("role_id");

            var courses = SQLExecutor.ExecuteQuery(
$@"SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc
                        FROM lop_hoc 
                        INNER JOIN mon_hoc on lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc 
                        where lop_hoc.id_lop_hoc in (select hoc_vien_tham_gia.id_lop_hoc from hoc_vien_tham_gia where hoc_vien_tham_gia.id_hoc_vien = {(int)roleId})");
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
        //public function getListOfCoursesForTeacher()
        //{
        //$id_giang_vien = session()->get('id_role');

        //$model = new LopModel();
        //$courses = $model->executeCustomQuery(
        //    "SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc
        //    FROM lop_hoc
        //    INNER JOIN mon_hoc on lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc where lop_hoc.id_lop_hoc in (select phan_cong_giang_vien.id_lop_hoc from phan_cong_giang_vien where phan_cong_giang_vien.id_giang_vien = $id_giang_vien)"
        //);
        //    for ($i = 0; $i < count($courses); $i++) {
        //    $courses[$i]['lecturers'] = $model->executeCustomQuery(
        //        "SELECT giang_vien.id_giang_vien, giang_vien.ho_ten
        //        FROM phan_cong_giang_vien INNER JOIN giang_vien ON phan_cong_giang_vien.id_giang_vien = giang_vien.id_giang_vien
        //        WHERE phan_cong_giang_vien.id_lop_hoc = {$courses[$i]['id_lop_hoc']}; "
        //        );
        //    }
        //    usort($courses, [$this, 'compareCoursesByBeginDate']);
        //    return $this->response->setJSON($courses);
        //}
        //public function getListOfCoursesForStudent()
        //{
        //$id_hoc_vien = session()->get('id_role');

        //$model = new LopModel();
        //$courses = $model->executeCustomQuery(
        //    "SELECT lop_hoc.id_lop_hoc,  DATE_FORMAT(lop_hoc.ngay_bat_dau, '%d/%m/%Y') as ngay_bat_dau,  DATE_FORMAT(lop_hoc.ngay_ket_thuc, '%d/%m/%Y') as ngay_ket_thuc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc
        //        FROM lop_hoc
        //        INNER JOIN mon_hoc on lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc where lop_hoc.id_lop_hoc in (select hoc_vien_tham_gia.id_lop_hoc from hoc_vien_tham_gia where hoc_vien_tham_gia.id_hoc_vien = $id_hoc_vien)"
        //    );
        //    for ($i = 0; $i < count($courses); $i++) {
        //    $courses[$i]['lecturers'] = $model->executeCustomQuery(
        //        "SELECT giang_vien.id_giang_vien, giang_vien.ho_ten
        //        FROM phan_cong_giang_vien INNER JOIN giang_vien ON phan_cong_giang_vien.id_giang_vien = giang_vien.id_giang_vien
        //        WHERE phan_cong_giang_vien.id_lop_hoc = {$courses[$i]['id_lop_hoc']}; "
        //        );
        //    }
        //    usort($courses, [$this, 'compareCoursesByBeginDate']);
        //    return $this->response->setJSON($courses);
        //}

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
        [HttpPost]
        public IActionResult insertStudentsIntoClass([FromBody] JsonDocument dataReceived)
        {
            //string json = HttpContext.Request.Form["json"];

            //dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(json);
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            int? courseid = HttpContext.Session.GetInt32("courseid");
            dynamic student_id_list = (ExpandoObject)obj.student_id_list;

            ExpandoObject response = new ExpandoObject();
            foreach (var property in (IDictionary<string, object>)student_id_list)
            {
                HocVienThamGiaModel pcModel = new HocVienThamGiaModel()
                {
                    id_hoc_vien = Convert.ToInt32(property.Key),
                    id_lop_hoc = Convert.ToInt32(courseid)
                };
                HocVienThamGiaRepository pcRepo = new HocVienThamGiaRepository();
                ((IDictionary<string, object>)response)[property.Key] = pcRepo.InsertHocVienThamGia(pcModel);

            }
            return Ok(JsonConvert.SerializeObject(response));
        }
        [HttpPost]
        public IActionResult insertLecturersIntoClass2([FromBody] JsonDocument dataReceived)
        {
            //string json = HttpContext.Request.Form["json"];

            //dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(json);
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            int? courseid = HttpContext.Session.GetInt32("courseid");
            dynamic lecturer_id_list = (ExpandoObject)obj.lecturer_id_list;

            ExpandoObject response = new ExpandoObject();
            foreach (var property in (IDictionary<string, object>)lecturer_id_list)
            {
                PhanCongGiangVienModel pcModel = new PhanCongGiangVienModel()
                {
                    id_giang_vien = Convert.ToInt32(property.Key),
                    id_lop_hoc = Convert.ToInt32(courseid)
                };
                PhanCongGiangVienRepository pcRepo = new PhanCongGiangVienRepository();
                ((IDictionary<string, object>)response)[property.Key] = pcRepo.InsertPhanCongGiangVien(pcModel);

            }
            return Ok(JsonConvert.SerializeObject(response));

        }
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
        [HttpGet]
        public IActionResult Information(int? courseid)
        {
            if (courseid == null || courseid == 0)
            {
                return Index();
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
				ViewData["ExceptionMessage"] = "Bạn không có quyền truy cập lớp học này";
				return View("ExceptionPage");
			}

			int? userId = HttpContext.Session.GetInt32("user_id");
            int? role = HttpContext.Session.GetInt32("role");
            int? roleId = HttpContext.Session.GetInt32("role_id");
            if (role == 1)
            {
                LoadNavbar();
                HttpContext.Session.SetInt32("courseid", (int)courseid);
                // MainSection
                courses.Columns.Add("so_luong_giang_vien");
                courses.Columns.Add("so_luong_buoi_hoc");
                courses.Columns.Add("so_luong_buoi_hoc_da_hoc");

                courses.Rows[0]["so_luong_giang_vien"] = Convert.ToInt32(SQLExecutor.ExecuteQuery(
                    $@"
                            SELECT COUNT(phan_cong_giang_vien.id_giang_vien) as slgv FROM lop_hoc LEFT JOIN phan_cong_giang_vien ON lop_hoc.id_lop_hoc = phan_cong_giang_vien.id_lop_hoc
                            WHERE lop_hoc.id_lop_hoc = {courseid}
                            GROUP BY lop_hoc.id_lop_hoc
                        "
                 ).Rows[0]["slgv"]);

                courses.Rows[0]["so_luong_buoi_hoc"] = Convert.ToInt32(SQLExecutor.ExecuteQuery(
                   $@"
                            SELECT COUNT(buoi_hoc.id_buoi_hoc) as slbh FROM lop_hoc LEFT JOIN buoi_hoc ON lop_hoc.id_lop_hoc = buoi_hoc.id_lop_hoc
                            WHERE lop_hoc.id_lop_hoc = {courseid}
                            GROUP BY lop_hoc.id_lop_hoc
                        "
                ).Rows[0]["slbh"]);

                courses.Rows[0]["so_luong_buoi_hoc_da_hoc"] = Convert.ToInt32(SQLExecutor.ExecuteQuery(
                   $@"
                            SELECT COUNT(buoi_hoc.id_buoi_hoc) as slbh FROM lop_hoc LEFT JOIN buoi_hoc ON lop_hoc.id_lop_hoc = buoi_hoc.id_lop_hoc and buoi_hoc.trang_thai = 2
                            WHERE lop_hoc.id_lop_hoc = {courseid}
                            GROUP BY lop_hoc.id_lop_hoc
                        "
                ).Rows[0]["slbh"]);
                ViewData["subject_name"] = courses.Rows[0]["ten_mon_hoc"];
                ViewData["subjectid"] = Convert.ToInt32(courses.Rows[0]["id_mon_hoc"]);
                ViewData["slbh"] = Convert.ToInt32(courses.Rows[0]["so_luong_buoi_hoc"]);
                ViewData["slbhdh"] = Convert.ToInt32(courses.Rows[0]["so_luong_buoi_hoc_da_hoc"]);
                ViewData["slgv"] = Convert.ToInt32(courses.Rows[0]["so_luong_giang_vien"]);
                ViewData["ngbd"] = courses.Rows[0]["ngay_bat_dau"];
                ViewData["ngkt"] = courses.Rows[0]["ngay_ket_thuc"];


                // Layout data
                ViewData["class_name"] = $"{courses.Rows[0]["ten_mon_hoc"]} {courses.Rows[0]["id_mon_hoc"].ToString().PadLeft(3, '0')}.{courses.Rows[0]["id_lop_hoc"].ToString().PadLeft(6, '0')}";
                ViewData["state"] = KiemTraTinhTrang((string)courses.Rows[0]["ngay_bat_dau"], (string)courses.Rows[0]["ngay_ket_thuc"]);
                ViewData["courseid"] = Convert.ToInt32(courses.Rows[0]["id_lop_hoc"]);
                ViewData["student_quantity"] = Convert.ToInt32(courses.Rows[0]["so_luong_hoc_vien"]);

                // Mainsection
                return View("AdministratorCourseInformation");

            }
            else if (role == 2)
            {
                LoadNavbar();
                HttpContext.Session.SetInt32("courseid", (int)courseid);
                // MainSection
                courses.Columns.Add("so_luong_giang_vien");
                courses.Columns.Add("so_luong_buoi_hoc");
                courses.Columns.Add("so_luong_buoi_hoc_da_hoc");

                courses.Rows[0]["so_luong_giang_vien"] = Convert.ToInt32(SQLExecutor.ExecuteQuery(
                    $@"
                            SELECT COUNT(phan_cong_giang_vien.id_giang_vien) as slgv FROM lop_hoc LEFT JOIN phan_cong_giang_vien ON lop_hoc.id_lop_hoc = phan_cong_giang_vien.id_lop_hoc
                            WHERE lop_hoc.id_lop_hoc = {courseid}
                            GROUP BY lop_hoc.id_lop_hoc
                        "
                 ).Rows[0]["slgv"]);

                courses.Rows[0]["so_luong_buoi_hoc"] = Convert.ToInt32(SQLExecutor.ExecuteQuery(
                   $@"
                            SELECT COUNT(buoi_hoc.id_buoi_hoc) as slbh FROM lop_hoc LEFT JOIN buoi_hoc ON lop_hoc.id_lop_hoc = buoi_hoc.id_lop_hoc
                            WHERE lop_hoc.id_lop_hoc = {courseid}
                            GROUP BY lop_hoc.id_lop_hoc
                        "
                ).Rows[0]["slbh"]);

                courses.Rows[0]["so_luong_buoi_hoc_da_hoc"] = Convert.ToInt32(SQLExecutor.ExecuteQuery(
                   $@"
                            SELECT COUNT(buoi_hoc.id_buoi_hoc) as slbh FROM lop_hoc LEFT JOIN buoi_hoc ON lop_hoc.id_lop_hoc = buoi_hoc.id_lop_hoc and buoi_hoc.trang_thai = 2
                            WHERE lop_hoc.id_lop_hoc = {courseid}
                            GROUP BY lop_hoc.id_lop_hoc
                        "
                ).Rows[0]["slbh"]);
                ViewData["subject_name"] = courses.Rows[0]["ten_mon_hoc"];
                ViewData["subjectid"] = Convert.ToInt32(courses.Rows[0]["id_mon_hoc"]);
                ViewData["slbh"] = Convert.ToInt32(courses.Rows[0]["so_luong_buoi_hoc"]);
                ViewData["slbhdh"] = Convert.ToInt32(courses.Rows[0]["so_luong_buoi_hoc_da_hoc"]);
                ViewData["slgv"] = Convert.ToInt32(courses.Rows[0]["so_luong_giang_vien"]);
                ViewData["ngbd"] = courses.Rows[0]["ngay_bat_dau"];
                ViewData["ngkt"] = courses.Rows[0]["ngay_ket_thuc"];


                // Layout data
                ViewData["class_name"] = $"{courses.Rows[0]["ten_mon_hoc"]} {courses.Rows[0]["id_mon_hoc"].ToString().PadLeft(3, '0')}.{courses.Rows[0]["id_lop_hoc"].ToString().PadLeft(6, '0')}";
                ViewData["state"] = KiemTraTinhTrang((string)courses.Rows[0]["ngay_bat_dau"], (string)courses.Rows[0]["ngay_ket_thuc"]);
                ViewData["courseid"] = Convert.ToInt32(courses.Rows[0]["id_lop_hoc"]);
                ViewData["student_quantity"] = Convert.ToInt32(courses.Rows[0]["so_luong_hoc_vien"]);

                // Mainsection
                return View("LecturerCourseInformation");

            }
            else if (role == 3)
            {
				LoadNavbar();
				HttpContext.Session.SetInt32("courseid", (int)courseid);
				// MainSection
				courses.Columns.Add("so_luong_giang_vien");
				courses.Columns.Add("so_luong_buoi_hoc");
				courses.Columns.Add("so_luong_buoi_hoc_da_hoc");

				courses.Rows[0]["so_luong_giang_vien"] = Convert.ToInt32(SQLExecutor.ExecuteQuery(
					$@"
                            SELECT COUNT(phan_cong_giang_vien.id_giang_vien) as slgv FROM lop_hoc LEFT JOIN phan_cong_giang_vien ON lop_hoc.id_lop_hoc = phan_cong_giang_vien.id_lop_hoc
                            WHERE lop_hoc.id_lop_hoc = {courseid}
                            GROUP BY lop_hoc.id_lop_hoc
                        "
				 ).Rows[0]["slgv"]);

				courses.Rows[0]["so_luong_buoi_hoc"] = Convert.ToInt32(SQLExecutor.ExecuteQuery(
				   $@"
                            SELECT COUNT(buoi_hoc.id_buoi_hoc) as slbh FROM lop_hoc LEFT JOIN buoi_hoc ON lop_hoc.id_lop_hoc = buoi_hoc.id_lop_hoc
                            WHERE lop_hoc.id_lop_hoc = {courseid}
                            GROUP BY lop_hoc.id_lop_hoc
                        "
				).Rows[0]["slbh"]);

				courses.Rows[0]["so_luong_buoi_hoc_da_hoc"] = Convert.ToInt32(SQLExecutor.ExecuteQuery(
				   $@"
                            SELECT COUNT(buoi_hoc.id_buoi_hoc) as slbh FROM lop_hoc LEFT JOIN buoi_hoc ON lop_hoc.id_lop_hoc = buoi_hoc.id_lop_hoc and buoi_hoc.trang_thai = 2
                            WHERE lop_hoc.id_lop_hoc = {courseid}
                            GROUP BY lop_hoc.id_lop_hoc
                        "
				).Rows[0]["slbh"]);
				ViewData["subject_name"] = courses.Rows[0]["ten_mon_hoc"];
				ViewData["subjectid"] = Convert.ToInt32(courses.Rows[0]["id_mon_hoc"]);
				ViewData["slbh"] = Convert.ToInt32(courses.Rows[0]["so_luong_buoi_hoc"]);
				ViewData["slbhdh"] = Convert.ToInt32(courses.Rows[0]["so_luong_buoi_hoc_da_hoc"]);
				ViewData["slgv"] = Convert.ToInt32(courses.Rows[0]["so_luong_giang_vien"]);
				ViewData["ngbd"] = courses.Rows[0]["ngay_bat_dau"];
				ViewData["ngkt"] = courses.Rows[0]["ngay_ket_thuc"];


				// Layout data
				ViewData["class_name"] = $"{courses.Rows[0]["ten_mon_hoc"]} {courses.Rows[0]["id_mon_hoc"].ToString().PadLeft(3, '0')}.{courses.Rows[0]["id_lop_hoc"].ToString().PadLeft(6, '0')}";
				ViewData["state"] = KiemTraTinhTrang((string)courses.Rows[0]["ngay_bat_dau"], (string)courses.Rows[0]["ngay_ket_thuc"]);
				ViewData["courseid"] = Convert.ToInt32(courses.Rows[0]["id_lop_hoc"]);
				ViewData["student_quantity"] = Convert.ToInt32(courses.Rows[0]["so_luong_hoc_vien"]);
				return View("StudentCourseInformation");

            }
            return Index();
        }
        public IActionResult getListOfSubjects()
        {
            return Ok(JsonConvert.SerializeObject(SQLExecutor.ExecuteQuery("SELECT * FROM mon_hoc")));
        }
       

            public string KiemTraTinhTrang(string ngayBatDau, string ngayKetThuc)
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





        public IActionResult getScheduleListByCourseId()
        {
            int? courseid = HttpContext.Session.GetInt32("courseid");
            var rs = SQLExecutor.ExecuteQuery(
               $@"
                    SELECT buoi_hoc.id_buoi_hoc, buoi_hoc.trang_thai, buoi_hoc.id_phong, DATE_FORMAT(buoi_hoc.ngay, '%d/%m/%Y') AS ngay, DAYOFWEEK(buoi_hoc.ngay) AS thu,
                    ca.id_ca, ca.thoi_gian_bat_dau, ca.thoi_gian_ket_thuc, a.id_lop_hoc, a.id_mon_hoc, a.ten_mon_hoc
                    FROM buoi_hoc INNER JOIN ca ON buoi_hoc.id_ca = ca.id_ca
                    LEFT JOIN (
                        SELECT lop_hoc.id_lop_hoc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc
                        FROM lop_hoc INNER JOIN mon_hoc ON lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc
                    ) AS a ON a.id_lop_hoc = buoi_hoc.id_lop_hoc
                    WHERE buoi_hoc.id_lop_hoc = {courseid}
                    ORDER BY buoi_hoc.ngay ASC"
            );
            return Ok(JsonConvert.SerializeObject(rs));
        }

        public IActionResult getListOfStudentsByCourseId()
        {
            int? courseid = HttpContext.Session.GetInt32("courseid");
            DataTable students = SQLExecutor.ExecuteQuery(
                $@"
                    SELECT hoc_vien.*, DATE_FORMAT(hoc_vien.ngay_sinh, '%d/%m/%Y') as ngay_sinh_hv FROM hoc_vien_tham_gia INNER JOIN hoc_vien on hoc_vien.id_hoc_vien = hoc_vien_tham_gia.id_hoc_vien
                    WHERE hoc_vien_tham_gia.id_lop_hoc = {courseid};
                "
            );

            students.Columns.Add("so_buoi_vang", typeof(int));

            for (int i = 0; i < students.Rows.Count; i++)
            {
                students.Rows[i]["so_buoi_vang"] = Convert.ToInt32(SQLExecutor.ExecuteQuery(
                    $@"SELECT COUNT(buoi_hoc.id_buoi_hoc) as so_buoi_vang FROM buoi_hoc INNER JOIN diem_danh ON buoi_hoc.id_buoi_hoc = diem_danh.id_buoi_hoc
                WHERE buoi_hoc.id_lop_hoc = {courseid}
                AND diem_danh.id_hoc_vien = {students.Rows[i]["id_hoc_vien"]}
                AND buoi_hoc.trang_thai = 2 AND diem_danh.co_mat = 0; "
                ).Rows[0]["so_buoi_vang"]);
            }
            return Ok(JsonConvert.SerializeObject(students));

        }

        public IActionResult getListOfLecturersByCourseId()
        {
            int? courseid = HttpContext.Session.GetInt32("courseid");
            DataTable lecturers = SQLExecutor.ExecuteQuery(
                $@"
                    SELECT giang_vien.id_giang_vien, giang_vien.ho_ten, giang_vien.email
                    FROM phan_cong_giang_vien INNER JOIN giang_vien ON phan_cong_giang_vien.id_giang_vien = giang_vien.id_giang_vien
                    WHERE phan_cong_giang_vien.id_lop_hoc = {courseid}; 
                "
            );
            return Ok(JsonConvert.SerializeObject(lecturers));

        }
        [HttpPost]
        //public function updateCourse()
        //{
        //$courseData = json_decode(json_encode($this->request->getJSON()), true);
        //$course = new LopModel();
        //$course->id_lop_hoc = $courseData['id_lop_hoc'];
        //$course->ngay_bat_dau = $courseData['ngay_bat_dau'];
        //$course->ngay_ket_thuc = $courseData['ngay_ket_thuc'];
        //$course->id_mon_hoc = $courseData['id_mon_hoc'];
        //    // return $this->response->setJSON($course);
        //    return $this->response->setJSON($course->updateLop($course));
        //}
        public IActionResult updateCourse([FromBody] JsonDocument dataReceived)
        {
            dynamic course = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            int? courseid = HttpContext.Session.GetInt32("courseid");
            LopHocModel model = new LopHocModel()
            {
                id_lop_hoc = (int)courseid,
                id_mon_hoc = (int)course.id_mon_hoc,
                ngay_bat_dau = DateTime.ParseExact(course.ngay_bat_dau, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                ngay_ket_thuc = DateTime.ParseExact(course.ngay_ket_thuc, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            };

            return Ok(JsonConvert.SerializeObject((new LopHocRepository()).UpdateLopHoc(model)));

        }

        //public function getListOfLecturersByCourseId()
        //{
        //// $id_lop_hoc = 110;
        //$id_lop_hoc = $this->request->getVar("id");
        //$model = new GiangVienModel();
        //$lecturers = $model->executeCustomQuery(
        //    "SELECT giang_vien.id_giang_vien, giang_vien.ho_ten, giang_vien.email
        //        FROM phan_cong_giang_vien INNER JOIN giang_vien ON phan_cong_giang_vien.id_giang_vien = giang_vien.id_giang_vien
        //        WHERE phan_cong_giang_vien.id_lop_hoc = {$id_lop_hoc}; "
        //);
        //    return $this->response->setJSON($lecturers);
        //}
        [HttpPost]
        public IActionResult deleteLecturerFromCourse(int id_giang_vien)
        {
            int? courseid = HttpContext.Session.GetInt32("courseid");
            return Ok(JsonConvert.SerializeObject(
                (new PhanCongGiangVienRepository()).DeletePhanCongGiangVien((new PhanCongGiangVienModel() { id_lop_hoc = courseid, id_giang_vien = id_giang_vien }))
                ));
        }
        public IActionResult getInsertClassForm()
        {
            var subjectRepo = new MonHocRepository();
            ViewData["subjects"] = subjectRepo.GetAllMonHoc();

            var lecturerRepo = new GiangVienRepository();
            ViewData["lecturers"] = lecturerRepo.GetAllGiangVien();


            return View("InsertClassForm");
        }
        public IActionResult getInsertLecturerForm()
        {
            var lecturerRepo = new GiangVienRepository();
            ViewData["lecturers"] = lecturerRepo.GetAllGiangVien();
            return View("InsertLecturerIntoClassForm");

        }
        public IActionResult getInsertStudentForm()
        {
            var studentRepo = new HocVienRepository();
            ViewData["students"] = studentRepo.GetAllHocVien();
            return View("InsertStudentIntoClassForm");
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
        [HttpPost]
        public IActionResult deleteStudentFromCourse([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            int? courseid = HttpContext.Session.GetInt32("courseid");
            ExpandoObject response = new ExpandoObject();
            foreach (long id_hoc_vien in obj.danh_sach_id_hoc_vien)
            {
                HocVienThamGiaRepository repo = new HocVienThamGiaRepository();
                HocVienThamGiaModel m = new HocVienThamGiaModel()
                {
                    id_lop_hoc = (int)courseid,
                    id_hoc_vien = Convert.ToInt32(id_hoc_vien)
                };
                ((IDictionary<string, object>)response)[Convert.ToString(id_hoc_vien)] = repo.DeleteHocVienThamGia(m);
            }
            return Ok(JsonConvert.SerializeObject(response));
        }
        public IActionResult getInsertScheduleForm()
        {
            ViewData["rooms"] = new PhongRepository().GetAllPhong();
            ViewData["shifts"] = new CaRepository().GetAllCa();
            return View("InsertScheduleIntoClassForm");
        }
        [HttpPost]
        public IActionResult getScheduleList([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            string ca = (Convert.ToInt32(obj.id_ca) == -1 ? "" : $"AND buoi_hoc.id_ca = {obj.id_ca}");
            string phong = (Convert.ToInt32(obj.id_phong) == -1 ? "" : $"AND buoi_hoc.id_phong = {obj.id_phong}");
            string thu = (Convert.ToInt32(obj.thu_trong_tuan) == -1 ? "" : $"AND DAYOFWEEK(buoi_hoc.ngay) = {obj.thu_trong_tuan}");
            DataTable dt = SQLExecutor.ExecuteQuery(
                $@"
                    SELECT buoi_hoc.id_buoi_hoc, buoi_hoc.trang_thai, buoi_hoc.id_phong, DATE_FORMAT(buoi_hoc.ngay, '%d/%m/%Y') AS ngay, DAYOFWEEK(buoi_hoc.ngay) AS thu, 
            ca.id_ca, ca.thoi_gian_bat_dau, ca.thoi_gian_ket_thuc, a.id_lop_hoc, a.id_mon_hoc, a.ten_mon_hoc
            FROM buoi_hoc INNER JOIN ca ON buoi_hoc.id_ca = ca.id_ca
            LEFT JOIN(
                SELECT lop_hoc.id_lop_hoc, mon_hoc.id_mon_hoc, mon_hoc.ten_mon_hoc
                FROM lop_hoc INNER JOIN mon_hoc ON lop_hoc.id_mon_hoc = mon_hoc.id_mon_hoc
            ) AS a ON a.id_lop_hoc = buoi_hoc.id_lop_hoc
            WHERE buoi_hoc.ngay >= '{obj.ngay_bat_dau}' AND buoi_hoc.ngay <= '{obj.ngay_ket_thuc}' {ca}
            {phong}
            {thu}
                ORDER BY buoi_hoc.ngay ASC, buoi_hoc.id_phong ASC, buoi_hoc.id_ca ASC
                "
                );

            return Ok(JsonConvert.SerializeObject(dt));
        }
        [HttpPost]
        public IActionResult insertScheduleIntoClass([FromBody] JsonDocument dataReceived)
        {
            int? courseid = HttpContext.Session.GetInt32("courseid");
            List<int> obj = JsonConvert.DeserializeObject<List<int>>(dataReceived.RootElement.ToString());

            ExpandoObject response = new ExpandoObject();

            for (int i = 0; i < obj.Count; i++)
            {

                ((IDictionary<string, object>)response)[Convert.ToString(obj[i])] = SQLExecutor.ExecuteDML(
                    $@"UPDATE buoi_hoc SET id_lop_hoc = {courseid} where id_buoi_hoc = {obj[i]}"
                    );
            }
            return Ok(JsonConvert.SerializeObject(response));

        }


        [HttpPost]
        public IActionResult deleteScheduleFromCourse([FromBody] JsonDocument dataReceived)
        {
            int? courseid = HttpContext.Session.GetInt32("courseid");
            List<int> obj = JsonConvert.DeserializeObject<List<int>>(dataReceived.RootElement.ToString());

            ExpandoObject response = new ExpandoObject();

            for (int i = 0; i < obj.Count; i++)
            {

                ((IDictionary<string, object>)response)[Convert.ToString(obj[i])] = SQLExecutor.ExecuteDML(
                    $@"UPDATE buoi_hoc SET id_lop_hoc = null where id_buoi_hoc = {obj[i]}"
                    );
            }
            return Ok(JsonConvert.SerializeObject(response));

        }

    }
    //public function insertScheduleIntoClass()
    //{
    //$data = json_decode($this->request->getVar("json"), true);
    //$id_lop_hoc = json_decode($this->request->getVar("id"), true);

    //$result = array();
    //$model = new BuoiHocModel();
    //    foreach ($data as $id_buoi_hoc) {
    //    $result["{$id_buoi_hoc}"] = $model->updateIdLopHoc($id_lop_hoc, $id_buoi_hoc);
    //    }
    //    return $this->response->setJSON($result);
    //}

}
