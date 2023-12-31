using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text.Json;

namespace CourseWebsiteDotNet.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            ViewData["chosenItem"] = 4;
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

                ViewData["users"] = SQLExecutor.ExecuteQuery(
                        "SELECT users.*, COALESCE(ad.ho_ten, giang_vien.ho_ten, hoc_vien.ho_ten) AS ho_ten  " +
                        "FROM users LEFT JOIN ad ON users.id_ad = ad.id_ad LEFT JOIN giang_vien ON users.id_giang_vien = giang_vien.id_giang_vien LEFT JOIN hoc_vien ON users.id_hoc_vien = hoc_vien.id_hoc_vien;"
                    );

                return View("AdministratorUsersList");
            }
            ViewData["ExceptionMessage"] = "Đã có lỗi xảy ra";
            return View("~/Views/Shared/ExeptionPage");
        }

        [HttpGet]
        public IActionResult getInsertUserForm()
        {
            var userRepo = new UserRepository();
            ViewData["users"] = userRepo.GetAllUsers();

            var lecturerRepo = new GiangVienRepository();
            ViewData["lecturers"] = lecturerRepo.GetAllGiangVien();

            var studentRepo = new HocVienRepository();
            ViewData["students"] = studentRepo.GetAllHocVien();

            return View("InsertUserForm");
        }

        [HttpGet]
        public IActionResult getUpdateUserForm()
        {
            var userRepo = new UserRepository();
            ViewData["users"] = userRepo.GetAllUsers();

            var lecturerRepo = new GiangVienRepository();
            ViewData["lecturers"] = lecturerRepo.GetAllGiangVien();

            var studentRepo = new HocVienRepository();
            ViewData["students"] = studentRepo.GetAllHocVien();

            return View("UpdateUserForm");
        }

        //[HttpPost]
        //public IActionResult insertUser([FromBody] JsonDocument dataReceived)
        //{
        //    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
        //    var userModel = new UserModel();

        //    byte[] imageBytes = Convert.FromBase64String(Convert.ToString(obj.file));
        //    userModel.anh_dai_dien = imageBytes;
        //    //userModel.anh_dai_dien = Convert.ToBase64String(obj.anh_dai_dien);
        //    userModel.tai_khoan = Convert.ToString(obj.account);
        //    userModel.mat_khau = Convert.ToString(obj.password);
        //    var role = Convert.ToInt32(obj.role);
        //    if (role == 0)
        //    {
        //        userModel.id_ad = Convert.ToInt32(obj.id_role);
        //        userModel.id_giang_vien = null;
        //        userModel.id_hoc_vien = null;
        //    }
        //    else if (role == 1) 
        //    {
        //        userModel.id_ad = null;
        //        userModel.id_giang_vien = Convert.ToInt32(obj.id_role);
        //        userModel.id_hoc_vien = null;
        //    }
        //    else if (role == 2)
        //    {
        //        userModel.id_ad = null;
        //        userModel.id_giang_vien = null;
        //        userModel.id_hoc_vien = Convert.ToInt32(obj.id_role);
        //    }
        //    var userRepo = new UserRepository();
        //    userModel.thoi_gian_dang_nhap_gan_nhat = null;
        //    var processResult = userRepo.InsertUser(userModel);
        //    dynamic response = new
        //    {
        //        state = processResult.state,
        //        message = processResult.message,
        //        auto_increment_id = processResult.insertedId
        //    };
        //    return Ok(JsonConvert.SerializeObject(response));
        //}

        [HttpPost]
        public IActionResult InsertUser(IFormFile file, string account, string password, int role, int id_role)
        {
            var userModel = new UserModel();
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                userModel.anh_dai_dien = ms.ToArray();
            }

            userModel.tai_khoan = account;
            userModel.mat_khau = password;

            if (role == 0)
            {
                userModel.id_ad = Convert.ToInt32(id_role);
                userModel.id_giang_vien = null;
                userModel.id_hoc_vien = null;
            }
            else if (role == 1)
            {
                userModel.id_ad = null;
                userModel.id_giang_vien = Convert.ToInt32(id_role);
                userModel.id_hoc_vien = null;
            }
            else if (role == 2)
            {
                userModel.id_ad = null;
                userModel.id_giang_vien = null;
                userModel.id_hoc_vien = Convert.ToInt32(id_role);
            }

            var userRepo = new UserRepository();
            userModel.thoi_gian_dang_nhap_gan_nhat = null;

            var processResult = userRepo.InsertUser(userModel);

            var response = new
            {
                state = processResult.state,
                message = processResult.message,
                auto_increment_id = processResult.insertedId
            };

            return Ok(response);
        }

        [HttpPost]
        public IActionResult getListOfAdmins()
        {
            var adRepo = new AdRepository();
            var adList = adRepo.GetAllAd();
            if (adList != null && adList.Any())
            {
                return Json(new
                {
                    admins = adList
                });
            }
            else
            {
                return Json(new
                {
                    error = $"Không tìm thấy admin"
                });
            }
        }

        [HttpPost]
        public IActionResult getListOfStudents()
        {
            var studentRepo = new HocVienRepository();
            var studentList = studentRepo.GetAllHocVien();
            if (studentList != null && studentList.Any())
            {
                return Json(new
                {
                    students = studentList
                });
            }
            else
            {
                return Json(new
                {
                    error = $"Không tìm thấy học viên"
                });
            }
        }

        [HttpPost]
        public IActionResult getListOfLecturers()
        {
            var lecturerRepo = new GiangVienRepository();
            var lecturerList = lecturerRepo.GetAllGiangVien();
            if (lecturerList != null && lecturerList.Any())
            {
                return Json(new
                {
                    lecturers = lecturerList
                });
            }
            else
            {
                return Json(new
                {
                    error = $"Không tìm thấy giảng viên"
                });
            }
        }

        //[HttpGet]
        //public IActionResult getUsersList() 
        //{
        //    var userRepo = new UserRepository();
        //    var processResult = userRepo.GetAllUsers();
        //    if (processResult != null)
        //    {
        //        // Return the student data as JSON
        //        return Json(new
        //        {
        //            processResult
        //        });
        //    }
        //    else
        //    {
        //        // Return a response indicating that the student is not found
        //        return Json(new
        //        {
        //            error = $"Không tìm thấy user"
        //        });
        //    }
        //}

        [HttpPost]
        public IActionResult deleteUser([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());
            UserRepository userRepository = new UserRepository();
            Response response = userRepository.DeleteUser(Convert.ToInt32(obj.id_user));
            return Ok(JsonConvert.SerializeObject(response));
        }

        [HttpPost]
        public IActionResult updatUser([FromBody] JsonDocument dataReceived)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(dataReceived.RootElement.ToString());

            UserRepository userRepo = new UserRepository();
            UserModel userModel = userRepo.GetUserById(Convert.ToInt32(obj.id_user));

            byte[] imageBytes = Convert.FromBase64String(Convert.ToString(obj.anh_dai_dien));
            userModel.anh_dai_dien = imageBytes;
            userModel.tai_khoan = Convert.ToString(obj.tai_khoan);
            userModel.mat_khau = Convert.ToString(obj.mat_khau);
            var processResult = userRepo.UpdateUser(userModel);
            dynamic response = new
            {
                state = processResult.state,
                message = processResult.message,
                auto_increment_id = processResult.insertedId
            };
            return Ok(JsonConvert.SerializeObject(response));
        }

        [HttpGet]
        public IActionResult getUserInfo(int id_user)
        {
            UserRepository userRepo = new UserRepository();
            UserModel user = userRepo.GetUserById(Convert.ToInt32(id_user));
            if (user != null)
            {
                return Json(new
                {
                    anh_dai_dien = user.anh_dai_dien,
                    tai_khoan = user.tai_khoan,
                });
            }
            else
            {
                return Json(new
                {
                    error = $"Không tìm thấy user {id_user}"
                });
            }
        }
    }
}

