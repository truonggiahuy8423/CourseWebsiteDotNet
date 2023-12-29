using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text;
using System.Text.Json;

namespace CourseWebsiteDotNet.Controllers
{
    public class StudentController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            ViewData["chosenItem"] = 3;
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

                ViewData["students"] = SQLExecutor.ExecuteQuery(
                        "SELECT hoc_vien.id_hoc_vien,  hoc_vien.ho_ten,  DATE_FORMAT(hoc_vien.ngay_sinh, '%d/%m/%Y') as ngay_sinh, hoc_vien.gioi_tinh, hoc_vien.email FROM hoc_vien order by hoc_vien.id_hoc_vien asc"
                    );

                return View("AdministratorStudentsList");
            }
            ViewData["ExceptionMessage"] = "Đã có lỗi xảy ra";
            return View("~/Views/Shared/ExeptionPage");
        }

        [HttpPost]
        public IActionResult insertStudent([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            var studentModel = new HocVienModel();
            studentModel.ho_ten = Convert.ToString(obj.ho_ten);
            studentModel.ngay_sinh = DateTime.Parse(obj.ngay_sinh);
            studentModel.gioi_tinh = Convert.ToInt32(obj.gioi_tinh);
            studentModel.email = Convert.ToString(obj.email);
            var studentRepo = new HocVienRepository();
            var processResult = studentRepo.InsertHocVien(studentModel);
            dynamic response = new
            {
                state = processResult.state,
                message = processResult.message,
                auto_increment_id = processResult.insertedId
            };
            return Ok(JsonConvert.SerializeObject(response));
        }

        [HttpPost]
        public IActionResult deleteStudent([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            HocVienRepository hocVienRepository = new HocVienRepository();
            Response response  = hocVienRepository.DeleteHocVien(Convert.ToInt32(obj.id_hoc_vien));
            return Ok(JsonConvert.SerializeObject(response));
        }

        [HttpGet]
        public IActionResult getStudentInfo(int id_hoc_vien)
        {
            HocVienRepository studentRepo = new HocVienRepository();
            HocVienModel student = studentRepo.GetHocVienById(Convert.ToInt32(id_hoc_vien));
            if (student != null)
            {
                // Return the student data as JSON
                return Json(new
                {
                    ho_ten = student.ho_ten,
                    ngay_sinh = student.ngay_sinh,
                    gioi_tinh = student.gioi_tinh,
                    email = student.email
                });
            }
            else
            {
                // Return a response indicating that the student is not found
                return Json(new
                {
                    error = $"Student {id_hoc_vien} not found "
                });
            }
        }

        [HttpPost]
        public IActionResult updateStudent([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            
            HocVienRepository studentRepo = new HocVienRepository();
            HocVienModel studentModel = studentRepo.GetHocVienById(Convert.ToInt32(obj.id_hoc_vien));
            studentModel.ho_ten = Convert.ToString(obj.ho_ten);
            studentModel.ngay_sinh = DateTime.Parse(obj.ngay_sinh);
            studentModel.gioi_tinh = Convert.ToInt32(obj.gioi_tinh);
            studentModel.email = Convert.ToString(obj.email);
            var processResult = studentRepo.UpdateHocVien(studentModel);
            dynamic response = new
            {
                state = processResult.state,
                message = processResult.message,
                auto_increment_id = processResult.insertedId
            };
            return Ok(JsonConvert.SerializeObject(response));
        }
    }
}
