using Microsoft.AspNetCore.Mvc;
using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Http;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;

namespace CourseWebsiteDotNet.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Index(string? error, string? account = null, string? password = null)
        {
            if (error != null)
            {
                ViewData["Error"] = error;
            }
            if (account != null) {
                ViewData["Acc"] = account;
            }
            if (password != null) {
                ViewData["Pas"] = password;

            }
            return View("Index");
        }
        public IActionResult Logout()
        {
            ISession session = HttpContext.Session;

            // Xóa tất cả các biến session
            session.Clear();

            return RedirectToAction("Index");
        }
        public IActionResult Active() { 
            int? userid = HttpContext.Session.GetInt32("user_id");
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            // Format the current date and time to match the SQL format
            string currentDateTimeFormatted = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            var rs = SQLExecutor.ExecuteDML(
                $"UPDATE users SET thoi_gian_dang_nhap_gan_nhat = '{currentDateTimeFormatted}' where users.id_user = {userid}"
                );
            return Ok(JsonConvert.SerializeObject(rs));
        }
        //public function active()
        //{
        //    if (!session()->has('id_user'))
        //    {
        //        return redirect()->to('/');
        //    }
        //$id = session()->get('id_user');
        //$model = new UserModel();
        //    date_default_timezone_set('Asia/Ho_Chi_Minh');

        //// Lấy thời gian hiện tại
        //$current_time = date('Y-m-d H:i:s');
        //$result = $model->executeCustomDDL(
        //    "UPDATE users SET thoi_gian_dang_nhap_gan_nhat = '{$current_time}' where users.id_user = $id"
        //);
        //    return $this->response->setJSON($result);

        //}

        public IActionResult Login()
        {
            string userAccount = HttpContext.Request.Form["account"];
            string userPassword = HttpContext.Request.Form["password"];

            // Validate Format
            if (userAccount == "")
            {
                return Index("Vui lòng nhập tài khoản", userAccount, userPassword);
            }

            if (userAccount.Length < 8 || userAccount.Length > 20)
            {
                return Index("Tài khoản chứa từ 8 - 20 ký tự", userAccount, userPassword);
            }

            if (userPassword == "")
            {
                return Index("Vui lòng nhập tài khoản", userAccount, userPassword);
            }

            if (userPassword.Length < 8 || userPassword.Length > 20)
            {
                return Index("Mật khẩu chứa từ 8 - 20 ký tự", userAccount, userPassword);
            }

            // Authentication
            var userModel = new UserRepository();
            UserModel? user = userModel.GetUserByAuthentication(userAccount, userPassword);


            if (user != null)
            {
                if (user.id_ad != null)
                {
                    HttpContext.Session.SetInt32("user_id", user.id_user);
                    HttpContext.Session.SetInt32("role", 1);
                    HttpContext.Session.SetInt32("role_id", (int)user.id_ad );
                }
                else if (user.id_giang_vien != null)
                {
                    HttpContext.Session.SetInt32("user_id", user.id_user);
                    HttpContext.Session.SetInt32("role", 2);
                    HttpContext.Session.SetInt32("role_id", (int)user.id_giang_vien);
                }
                else if (user.id_hoc_vien != null)
                {
                    HttpContext.Session.SetInt32("user_id", user.id_user);
                    HttpContext.Session.SetInt32("role", 3);
                    HttpContext.Session.SetInt32("role_id", (int)user.id_hoc_vien);
                }
                return RedirectToRoute("courses");
            }
            else
            {
                return Index("Tài khoản hoặc mật khẩu không đúng", userAccount, userPassword);
            }


        }
    }
}
