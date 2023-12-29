using CourseWebsiteDotNet.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Reflection;

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
                byte[]? avatarData = (byte[])dataTable.Rows[0]["anh_dai_dien"];

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
                byte[]? avatarData = (byte[])dataTable.Rows[0]["anh_dai_dien"];

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
                byte[]? avatarData = (byte[])dataTable.Rows[0]["anh_dai_dien"];

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
            } else if (role == 2)
            {

            } else if (role == 3)
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
        public IActionResult GetFile(int fileId)
        {
            TepTinTaiLenModel fileData = (new TepTinTaiLenRepository().GetTepTinTaiLenById(fileId));
            string binPath = Directory.GetCurrentDirectory();
            //string projectPath = Path.GetFullPath(Path.Combine(binPath, "..", ".."));
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
                        message = "Tệp tin không tồn tại." + filePath
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
                    message = ex.Message + filePath
                };
                return Json(errorResponse);
            }
        }

        //public function getResources()
        //{
        //    if (!session()->has('id_user'))
        //    {
        //        return redirect()->to('/');
        //    }
        //$id_lop_hoc = $this->request->getPost('id_lop_hoc');
        //    if (!is_numeric($id_lop_hoc))
        //    {
        //        // Xử lý lỗi hoặc trả về kết quả không hợp lệ
        //        return $this->response->setJSON(['error' => 'Invalid id_lop_hoc']);
        //    }
        //$mucModel = new MucModel();
        //$rs = array();
        //$rs['folders'] = $mucModel->getMucByIdLopHoc($id_lop_hoc);

        //$thongBaoModel = new ThongBaoModel();
        //$rs['notis'] = $thongBaoModel->getThongBaoByCourseId($id_lop_hoc);

        //$linkModel = new LinkModel();
        //$rs['links'] = $linkModel->getAllLinksByCourseId($id_lop_hoc);

        //$viTriTepModel = new vi_tri_tep_tinModel();
        //$rs['files'] = $viTriTepModel->executeCustomQuery(
        //    "SELECT 
        //    v.*,
        //    m.id_muc,
        //    g.id_giang_vien,
        //    g.ho_ten,
        //    t.*
        //FROM
        //    vi_tri_tep_tin v
        //INNER JOIN
        //    muc m ON v.id_muc = m.id_muc
        //INNER JOIN
        //    tep_tin_tai_len t ON t.id_tep_tin_tai_len = v.id_tep_tin_tai_len
        //INNER JOIN
        //    users u ON u.id_user = t.id_user
        //INNER JOIN
        //    giang_vien g ON g.id_giang_vien = u.id_giang_vien
        //WHERE
        //    m.id_lop_hoc = {$id_lop_hoc}
        //ORDER BY
        //    v.ngay_dang ASC;
        //    "
        //);

        //$baiTapModel = new BaiTapModel();
        //$rs["assignments"] = $baiTapModel->getBaiTapByIdLopHoc($id_lop_hoc);
        //    return $this->response->setJSON($rs);
        //}
    }
}
