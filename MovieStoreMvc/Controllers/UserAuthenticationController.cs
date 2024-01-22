using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private IUserAuthenticationService authService;
        public UserAuthenticationController(IUserAuthenticationService authService, UserManager<ApplicationUser> _userManager)
        {
            this.userManager = _userManager;
            this.authService = authService;
        }

        /*  public async Task<IActionResult> Register11()
          {
              var model = new RegistrationModel{

                Email= "user@gmail.com",
                Username = "user",
                Name = "Jake",
                Password = "User@123",
                PasswordConfirm = "User@123",
                Role = "User"

              };

              var result = await authService.RegisterAsync(model);
              return Ok(result.Message);
          }*/
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {

                return View(model);
            }
            var result = await authService.LoginAsync(model);
            if (result.StatusCode == 1)
            {
                TempData["msg"] = "login success";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["msg"] = "Could not logged in ...";
                return RedirectToAction(nameof(Login));
            }

        }

        public async Task<IActionResult> Logout()
        {
            await authService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            RegistrationModel registration = new RegistrationModel();
            registration.AvailableRoles = await authService.GetAvailableRoles();

            return View(registration);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await authService.GetAvailableRoles();


                return View(model);
            }
            var result = await authService.RegisterAsync(model);
            if (result.StatusCode == 1)
            {
                TempData["msg"] = "register success";
                return RedirectToAction("Index", "Home");
            }
            else
            {

                model.AvailableRoles = await authService.GetAvailableRoles();

                TempData["msg"] = "Could not register ...";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangePass()
        {
            var user = await userManager.GetUserAsync(User);

            // Check if the user is signed in
            if (user == null)
            {
                // Handle the case where the user is not signed in
                return RedirectToAction("Login"); // Redirect to your login action
            }

            var userProfileModel = new UserProfileViewModel
            {
                // Transfer necessary information from ApplicationUser to UserProfileModel
                Name = user.UserName,
                Email = user.Email
                // Add other properties as needed
            };

            return View(userProfileModel);
        }


        [HttpPost]
        public async Task<IActionResult> ChangePass(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin user hiện tại
                var user = await userManager.GetUserAsync(User);

                if (user == null)
                {
                    // Xử lý trường hợp user không tồn tại
                    return RedirectToAction("Login");
                }

                // Thay đổi thông tin username và email
                user.UserName = model.Name;
                user.Email = model.Email;

                // Thay đổi mật khẩu nếu người dùng nhập mật khẩu hiện tại và mật khẩu mới
                if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
                {
                    // Kiểm tra mật khẩu hiện tại
                    var changePasswordResult = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                    if (!changePasswordResult.Succeeded)
                    {
                        // Xử lý trường hợp thay đổi mật khẩu không thành công
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        TempData["ErrorMessage"] = "Fail to update";
                        return View(model);
                    }
                }

                // Lưu thông tin thay đổi
                var updateResult = await userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    // Xử lý trường hợp cập nhật thành công
                
                    return RedirectToAction("Logout", "UserAuthentication"); // Chuyển hướng đến trang chủ hoặc trang cập nhật thành công
                }
                else
                {
                    // Xử lý trường hợp cập nhật không thành công
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    TempData["ErrorMessage"] = "Fail to update";
                }
            }

            // Trả về view với model nếu có lỗi xảy ra
            return View(model);
        }


    }
}
