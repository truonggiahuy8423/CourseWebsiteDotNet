using Microsoft.AspNetCore.Mvc;
using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Http;

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
