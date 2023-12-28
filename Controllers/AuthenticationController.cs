using Microsoft.AspNetCore.Mvc;
using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Http;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;

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


        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            UserRepository _userRepo = new UserRepository();
            var user =  _userRepo.GetUserByEmail(email);
            if (user == null ||  (user.Email == null && user.id_user == null)) return Json(new { result = 0 });

            var codeToken = GenerateRandomCode(6);
          
            #region Subject and body
            var subject = "Reset your password";
     
            var body = $@"
                            <html>
                            <head>
                                <style>
                                    body {{
                                        font-family: Arial, sans-serif;
                                        margin: 0;
                                        padding: 0;
                                    }}
                                    .container {{
                                        max-width: 600px;
                                        margin: 0 auto;
                                        padding: 20px;
                                        background-color: #f7f7f7;
                                    }}
                                    .header {{
                                        background-color: #3498db;
                                        color: #fff;
                                        text-align: center;
                                        padding: 10px 0;
                                    }}
                                    .content {{
                                        background-color: #fff;
                                        padding: 20px;
                                        border-radius: 5px;
                                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                    }}
                                    .footer {{
                                        text-align: center;
                                        margin-top: 20px;
                                        color: #888;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <div class='header'>
                                        <h1>Đặt lại mật khẩu của bạn!</h1>
                                    </div>
                                    <div class='content'>
                                        <p>Xin chào !</p>
                                        <p> Chúng tôi đã nhận được thông tin của bạn về việc quên mật khẩu..</p>
                                        <p>Chúng tôi đã hỗ trợ đổi mật khẩu:</p>
                                        <ul>   
                                            <li><strong>UserName:</strong> {user.tai_khoan}</li>
                                            <li><strong>New password:</strong> {codeToken}</li>
                                        </ul>
                                        <p>Vui lòng liên hệ với chúng tôi nếu việc đổi mật khẩu không thành công..</p>
                                        <p>Xin cảm ơn và chúc một ngày tốt lành!</p>
                                    </div>
                                    <div class='footer'>
                                        <p>© 2023 Your Company. All rights reserved.</p>
                                    </div>
                                </div>
                            </body>
                            </html>";
            #endregion

            if (SendMail(email, subject, body, user.id_user, codeToken))
            {


                return Json(new { result = 1 });//Gửi mail thành công
            }
            else
            {
                return Json(new { result = 2 });//Gửi không thành công
            }
        }
        private string GenerateRandomCode(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool SendMail(string to, string subject, string body,int? userid, string passnew)
        {
            try
            {
                string fromEmail = "cateringmanagement2023@gmail.com";
                string password = "hqlo uqlv qjmb wzzm";
                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromEmail);
                message.Subject = subject;
                message.To.Add(new MailAddress(to));
                message.Body = body;
                message.IsBodyHtml = true;
                var smtpClient = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                };
                smtpClient.Send(message);
                UserRepository _userRepo = new UserRepository();

                var  user =  _userRepo.GetUserById(userid.Value);

                if (user != null)
                {
                    user.mat_khau = passnew;

                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
