using CourseWebsiteDotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Data;

namespace CourseWebsiteDotNet.Controllers
{
    public class AccountManagementController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
         UserRepository _userRepo = new UserRepository();   
        public AccountManagementController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
           int userid = HttpContext.Session.GetInt32("user_id").Value;
           var user = _userRepo.GetUserById(userid);
            if (user != null)
            {
                ViewData["avatar_data"] = user.anh_dai_dien;
            }

            
            return View();
        }
        [HttpPost]
        public IActionResult ChangeAvatarAndPassword(AccountRequest accountRequest)
        {
            byte[] imageBytes = null;
            int userId = HttpContext.Session.GetInt32("user_id").Value;
            var user = _userRepo.GetUserById(userId);
            if (accountRequest.Avatar != null && accountRequest.Avatar.Length > 0)
            {
                
                using (var memoryStream = new MemoryStream())
                {
                    accountRequest.Avatar.CopyTo(memoryStream);
                    imageBytes = memoryStream.ToArray();
                    user.anh_dai_dien = imageBytes;
                    _userRepo.UpdateUser(user);
                   
                }

            }
            if (ModelState.IsValid)
            {
                try
                {
                                 
                    if (user.mat_khau == accountRequest.CurrentPassword)
                    {
                        user.mat_khau = accountRequest.NewPassword;
                        _userRepo.UpdateUser(user);

                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật khẩu hiện tại của bạn nhập đang không chính xác");
                    }


                    TempData["SuccessMessage"] = "Lưu thay đổi thành công ";
                    
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lưu thay đổi thất bại");
                }
                ViewData["avatar_data"] = user.anh_dai_dien;
                return View("Index", accountRequest);

            }
            else
            {
                ViewData["avatar_data"] = user.anh_dai_dien;
                return View("Index",accountRequest );
            }
        }
    }
}
