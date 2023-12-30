using CourseWebsiteDotNet.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace CourseWebsiteDotNet.Controllers
{
    public class CourseResourceController : Controller
    {
        private readonly IConfiguration _configuration;

        public CourseResourceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
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
                var rs = SQLExecutor.ExecuteQuery($@"SELECT * FROM hoc_vien_tham_gia WHERE hoc_vien_tham_gia.id_hoc_vien = {roleId} AND hoc_vien_tham_gia.id_lop_hoc = {courseid}");
                return rs.Rows.Count == 0 ? false : true;
            }
            return false;
        }
        [HttpGet]
        public IActionResult Resource(int? courseid)
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
                ViewData["ExceptionMessage"] = "Bạn không có quyền truy cập lớp học này";
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

                return View("AdministratorCourseResource");
            }
            else if (role == 2)
            {
                HttpContext.Session.SetInt32("courseid", (int)courseid);


                // Layout data
                ViewData["class_name"] = $"{courses.Rows[0]["ten_mon_hoc"]} {courses.Rows[0]["id_mon_hoc"].ToString().PadLeft(3, '0')}.{courses.Rows[0]["id_lop_hoc"].ToString().PadLeft(6, '0')}";
                ViewData["state"] = KiemTraTinhTrang((string)courses.Rows[0]["ngay_bat_dau"], (string)courses.Rows[0]["ngay_ket_thuc"]);
                ViewData["student_quantity"] = Convert.ToInt32(courses.Rows[0]["so_luong_hoc_vien"]);
                ViewData["courseid"] = Convert.ToInt32(courseid);

                return View("LecturerCourseResource");

            }
            else if (role == 3)
            {

            }

            ViewData["ExceptionMessage"] = "Đã có lỗi xảy ra, vui lòng đăng nhập lại";
            return View("ExceptionPage");
        }
        public IActionResult getResources()
        {
            int? courseid = HttpContext.Session.GetInt32("courseid");
            if (courseid == null)
            {
                return BadRequest();
            }
            ExpandoObject result = new ExpandoObject();

            ((IDictionary<string, object>)result)["folders"] = new MucRepository().GetMucByCourseId((int)courseid);

            ((IDictionary<string, object>)result)["notis"] = new ThongBaoRepository().GetThongBaoByCourseId((int)courseid);

            ((IDictionary<string, object>)result)["links"] = new DuongLinkRepository().GetLinksByCourseId((int)courseid);

            ((IDictionary<string, object>)result)["files"] = SQLExecutor.ExecuteQuery(
                $@"
                                SELECT 
            v.*,
            m.id_muc,
            g.id_giang_vien,
            g.ho_ten,
            t.*
        FROM
            vi_tri_tep_tin v
        INNER JOIN
            muc m ON v.id_muc = m.id_muc
        INNER JOIN
            tep_tin_tai_len t ON t.id_tep_tin_tai_len = v.id_tep_tin_tai_len
        INNER JOIN
            users u ON u.id_user = t.id_user
        INNER JOIN
            giang_vien g ON g.id_giang_vien = u.id_giang_vien
        WHERE
            m.id_lop_hoc = {courseid}
        ORDER BY
            v.ngay_dang ASC;
            
                "

                );

            ((IDictionary<string, object>)result)["assignments"] = new BaiTapRepository().GetBaiTapByCourseId((int)courseid);

            return Ok(JsonConvert.SerializeObject(result));
        }
        [HttpPost]
        public IActionResult GetFile(int? fileId)
        {
            if (fileId == 0 || fileId == null)
            {
                var notFoundResponse = new
                {
                    state = false,
                    message = "Tệp tin không tồn tại."
                };
                return Json(notFoundResponse);
            }
            // Validate permission
            int? courseid = HttpContext.Session.GetInt32("courseid");

            DataTable file = SQLExecutor.ExecuteQuery($@"                
                SELECT vi_tri_tep_tin.id_tep_tin_tai_len FROM lop_hoc INNER JOIN muc on lop_hoc.id_lop_hoc = muc.id_lop_hoc 
                INNER JOIN vi_tri_tep_tin on muc.id_muc = vi_tri_tep_tin.id_muc WHERE lop_hoc.id_lop_hoc = {courseid} and vi_tri_tep_tin.id_tep_tin_tai_len = {fileId};
            ");

            if (file.Rows.Count == 0)
            {
                var notFoundResponse = new
                {
                    state = false,
                    message = "Bạn không có quyền truy cập tài nguyên này."
                };
                return Json(notFoundResponse);
            }


            TepTinTaiLenModel fileData = (new TepTinTaiLenRepository().GetTepTinTaiLenById((int)fileId));

            // Validate is file exist in DB
            if (fileData == null)
            {
                var notFoundResponse = new
                {
                    state = false,
                    message = "Tệp tin không tồn tại."
                };
                return Json(notFoundResponse);
            }

            string binPath = Directory.GetCurrentDirectory();
            var filePath = binPath + $"\\UserFiles\\{fileData.decoded}.{fileData.extension}";
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);

                    // Chuyển đổi dữ liệu tệp thành base64
                    var base64Data = Convert.ToBase64String(fileBytes);

                    // Tạo đối tượng JSON
                    var jsonResponse = new
                    {
                        state = true,
                        data = base64Data,
                        nameFile = $"{fileData.ten_tep}",
                        extension = $"{fileData.extension}"
                    };

                    // Trả về JSON
                    return Json(jsonResponse);
                }
                else
                {
                    // Trả về JSON nếu tệp không tồn tại
                    var notFoundResponse = new
                    {
                        state = false,
                        message = "Tệp tin không tồn tại."
                    };
                    return Json(notFoundResponse);
                }
            }
            catch (Exception ex)
            {
                // Trả về JSON nếu có lỗi
                var errorResponse = new
                {
                    state = false,
                    message = ex.Message
                };
                return Json(errorResponse);
            }
        }
        [HttpPost]
        public IActionResult removeResource([FromBody] JsonDocument dataReceived)
        {
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            int? courseid = HttpContext.Session.GetInt32("courseid");
            int id = Convert.ToInt32(data.id);
            int id_muc = Convert.ToInt32(data.id_muc);
            string type = data.type;

            MucModel muc = (new MucRepository().GetMucById(id_muc));
            if (muc != null && muc.id_lop_hoc == courseid)
            {
                switch (type)
                {
                    case "file":
                        return Ok(JsonConvert.SerializeObject(new ViTriTepTinRepository().DeleteViTriTepTin(id_muc, id)));
                        break;
                    case "noti":
                        return Ok(JsonConvert.SerializeObject(new ThongBaoRepository().DeleteThongBao(id)));
                        break;
                    case "link":
                        return Ok(JsonConvert.SerializeObject(new DuongLinkRepository().DeleteDuongLink(id)));
                        break;
                    case "asssignment":
                        return Ok(JsonConvert.SerializeObject(new BaiTapRepository().DeleteBaiTap(id)));
                        break;
                }
            }
            dynamic response = new
            {
                state = false,
                message = "Đã có lỗi xảy ra!"

            };
            return Ok(JsonConvert.SerializeObject(response));

        }

        public IActionResult getAddResourceIntoCourseForm()
        {
            return View("AddResourceIntoClassForm");
        }
        [HttpPost]
        public IActionResult postNewFolder([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            int id_muc = Convert.ToInt32(obj.id_muc);
            int? courseid = HttpContext.Session.GetInt32("courseid");
            string name = obj.ten_muc;
            MucModel muc = (new MucRepository().GetMucById(id_muc));
            if (muc != null && muc.id_lop_hoc == courseid)
            {
                MucModel mucModel = new MucModel()
                {
                    id_lop_hoc = (int)courseid,
                    id_muc_cha = id_muc,
                    ten_muc = name
                };
                return Ok(JsonConvert.SerializeObject(new MucRepository().InsertMuc(mucModel)));

            }
            dynamic response = new
            {
                state = false,
                message = "Đã có lỗi xảy ra!"

            };
            return Ok(JsonConvert.SerializeObject(response));
        }

        [HttpPost]
        public IActionResult postNewLinkOnClass([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            int id_muc = Convert.ToInt32(obj.id_muc);
            int? courseid = HttpContext.Session.GetInt32("courseid");
            string link = obj.link;
            string tieu_de = obj.tieu_de;

            MucModel muc = (new MucRepository().GetMucById(id_muc));
            if (muc != null && muc.id_lop_hoc == courseid)
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                DuongLinkModel dlink = new DuongLinkModel()
                {
                    link = link,
                    tieu_de = tieu_de,
                    id_muc = id_muc,
                    id_giang_vien = (int)HttpContext.Session.GetInt32("role_id"),
                    ngay_dang = vietnamNow
                };
                return Ok(JsonConvert.SerializeObject(new DuongLinkRepository().InsertDuongLink(dlink)));

            }
            dynamic response = new
            {
                state = false,
                message = "Đã có lỗi xảy ra!"

            };
            return Ok(JsonConvert.SerializeObject(response));
        }


        [HttpPost]
        public IActionResult postNewNotiOnClass([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            int id_muc = Convert.ToInt32(obj.id_muc);
            int? courseid = HttpContext.Session.GetInt32("courseid");
            string noi_dung = obj.noi_dung;
            string tieu_de = obj.tieu_de;

            MucModel muc = (new MucRepository().GetMucById(id_muc));
            if (muc != null && muc.id_lop_hoc == courseid)
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                ThongBaoModel tb = new ThongBaoModel()
                { 
                    noi_dung = noi_dung,
                    tieu_de = tieu_de,
                    id_muc = id_muc,
                    id_giang_vien = (int)HttpContext.Session.GetInt32("role_id"),
                    ngay_dang = vietnamNow
                };
                return Ok(JsonConvert.SerializeObject(new ThongBaoRepository().InsertThongBao(tb)));

            }
            dynamic response = new
            {
                state = false,
                message = "Đã có lỗi xảy ra!"

            };
            return Ok(JsonConvert.SerializeObject(response));
        }
        static DateTime CreateDateTimeFromString(string dateTimeString)
        {
            // Định dạng của chuỗi ngày giờ (ví dụ: "yyyy-MM-dd HH:mm")
            string format = "yyyy-MM-dd HH:mm";

            try
            {
                // Thử tạo đối tượng DateTime từ chuỗi với định dạng cụ thể
                DateTime result = DateTime.ParseExact(dateTimeString, format, System.Globalization.CultureInfo.InvariantCulture);
                return result;
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid date format");
                return DateTime.MinValue; // Trả về giá trị mặc định nếu có lỗi
            }
        }
        [HttpPost]
        public IActionResult postAssignmentOnClass([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            int id_muc = Convert.ToInt32(obj.id_muc);
            int? courseid = HttpContext.Session.GetInt32("courseid");
            string name = obj.ten_bai_tap;
            string content = obj.noi_dung;
            string th = obj.thoi_han;
            string thn = obj.thoi_han_nop;
            MucModel muc = (new MucRepository().GetMucById(id_muc));
            if (muc != null && muc.id_lop_hoc == courseid)
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                BaiTapModel tb = new BaiTapModel()
                {
                    noi_dung = content,
                    ten = name,
                    id_muc = id_muc,
                    id_giang_vien = (int)HttpContext.Session.GetInt32("role_id"),
                    thoi_han = CreateDateTimeFromString(th),
                    thoi_han_nop = CreateDateTimeFromString(thn),
                    ngay_dang = vietnamNow,
                    

                };
                return Ok(JsonConvert.SerializeObject(new BaiTapRepository().InsertBaiTap(tb)));

            }
            dynamic response = new
            {
                state = false,
                message = "Đã có lỗi xảy ra!"

            };
            return Ok(JsonConvert.SerializeObject(response));
        }

        [HttpPost]
        public IActionResult postFileOnClass([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            int id_muc = Convert.ToInt32(obj.id_muc);
            int? courseid = HttpContext.Session.GetInt32("courseid");
            int fileid = Convert.ToInt32(obj.file_id);


            // Validate file id
            TepTinTaiLenModel file = new TepTinTaiLenRepository().GetTepTinTaiLenById(fileid);
            if (file == null || file.id_user != HttpContext.Session.GetInt32("user_id"))
            {
                dynamic response2 = new
                {
                    state = false,
                    message = "Tệp tin không tồn tại!"

                };
                return Ok(JsonConvert.SerializeObject(response2));
            }


            MucModel muc = (new MucRepository().GetMucById(id_muc));
            if (muc != null && muc.id_lop_hoc == courseid)
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                ViTriTepTinModel tb = new ViTriTepTinModel()
                {
                    id_tep_tin_tai_len = (int)file.id_tep_tin_tai_len,
                    id_muc = id_muc,
                    ngay_dang = vietnamNow,


                };
                return Ok(JsonConvert.SerializeObject(new ViTriTepTinRepository().InsertViTriTepTin(tb)));

            }
            dynamic response = new
            {
                state = false,
                message = "Đã có lỗi xảy ra!"

            };
            return Ok(JsonConvert.SerializeObject(response));
        }

        public IActionResult getChooseUserFileForm()
        {
            ViewData["files"] = new TepTinTaiLenRepository().GetTepTinTaiLenByUserId((int)HttpContext.Session.GetInt32("user_id"));
            return View("ChooseFileForm");
        }


        public IActionResult UploadFile()
        {
            // Lấy file từ request
            var file = HttpContext.Request.Form.Files.GetFile("file");

            if (file != null && file.Length > 0)
            {
                var encoded = Guid.NewGuid().ToString();
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                var fileModel = new TepTinTaiLenModel 
                {
                    decoded = encoded,
                    id_user = (int)HttpContext.Session.GetInt32("user_id"),
                    ngay_tai_len = vietnamNow,
                    ten_tep = Path.GetFileNameWithoutExtension(file.FileName),
                    extension = Path.GetExtension(file.FileName).Substring(1, Path.GetExtension(file.FileName).Length - 1)
                };

                var rs = new TepTinTaiLenRepository().InsertTepTinTaiLen(fileModel);

                if (rs.state)
                {

                    string binPath = Directory.GetCurrentDirectory();
                    var folderPath = binPath + $"\\UserFiles\\";
                    // Di chuyển file đến thư mục lưu trữ
                    var newFileName = encoded + "." + fileModel.extension;
                    var filePath = folderPath + newFileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var response = new
                    {
                        state = true,
                        auto_increment_id = rs.insertedId,
                        message = "Đăng tải tệp thành công",
                        fileName = fileModel.ten_tep,
                        extension = fileModel.extension
                    };

                    return Ok(JsonConvert.SerializeObject(response));
                }
                else
                {
                    return Json(new { state = false, message = "Tệp tải lên không thành công" });
                }
            }
            else
            {
                return Json(new { state = false, message = "Tệp tải lên không thành công" });
            }
        }
    }
}
