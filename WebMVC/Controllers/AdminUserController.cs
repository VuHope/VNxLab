using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using WebMVC.Data;
using WebMVC.Utility;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    [Authorize(Roles = SD.Admin)]
    public class AdminUserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminUserController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var userList = _dbContext.ApplicationUsers.ToList();
            var userRole = _dbContext.UserRoles.ToList();
            var roles = _dbContext.Roles.ToList();
            foreach (var user in userList)
            {
                var user_role = userRole.FirstOrDefault(u => u.UserId == user.Id);
                if (user_role == null)
                {
                    user.Role = "Không có quyền";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(r => r.Id == user_role.RoleId).Name;
                }
            }

            return View(userList);
        }

        public async Task<IActionResult> ManageRole(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            // Kiểm tra nếu đây là tài khoản admin mặc định
            if (user.Email == "admin@gmail.com")
            {
                TempData[SD.Error] = "Không thể thay đổi role của tài khoản admin mặc định.";
                return RedirectToAction(nameof(Index)); // Hoặc trả về view phù hợp
            }
            List<string> exstingUserRoles = await _userManager.GetRolesAsync(user) as List<string>;
            var model = new RolesViewModel
            {
                User = user,
            };

            foreach (var role in _roleManager.Roles)
            {
                RoleSelection roleSelection = new RoleSelection
                {
                    RoleName = role.Name
                };
                if (exstingUserRoles.Any(r => r == role.Name))
                {
                    roleSelection.IsSelected = true;
                }
                model.RoleList.Add(roleSelection);

            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRole(RolesViewModel rolesViewModel)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(rolesViewModel.User.Id);
            if (user == null)
            {
                return NotFound();
            }

            var oldUserRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, oldUserRoles);


            if (!result.Succeeded)
            {
                TempData[SD.Error] = "Không thể xóa quyền hiện tại của người dùng";
                return View(rolesViewModel);
            }
            result = await _userManager.AddToRolesAsync(user,
                rolesViewModel.RoleList.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                TempData[SD.Error] = "Error while adding roles";
                return View(rolesViewModel);
            }

            TempData[SD.Success] = "Roles assigned successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUnlock(string userId)
        {
            ApplicationUser user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            // Kiểm tra nếu người dùng là admin
            if (user.Email == "admin@gmail.com")
            {
                TempData[SD.Error] = "Bạn không thể khóa tài khoản admin.";
                return RedirectToAction(nameof(Index));
            }
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                //user is locked and will remain locked untill lockoutend time
                //clicking on this action will unlock them
                user.LockoutEnd = DateTime.Now;
                TempData[SD.Success] = "User unlocked successfully";
            }
            else
            {
                //user is not locked, and we want to lock the user
                user.LockoutEnd = DateTime.Now.AddYears(1000);
                TempData[SD.Success] = "User locked successfully";
            }
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UserProfile(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var model = new UserProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Gender = user.Gender,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(UserProfileViewModel model, IFormFile? file)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string newsPath = Path.Combine(wwwRootPath, @"images\profile");
                if (!string.IsNullOrEmpty(model.ProfilePictureUrl))
                {
                    var oldImagePath =
                        Path.Combine(wwwRootPath, model.ProfilePictureUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                using (var fileStream = new FileStream(Path.Combine(newsPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                model.ProfilePictureUrl = @"\images\profile\" + fileName;
            }

            user.FullName = model.FullName;
            user.Gender = model.Gender;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.City = model.City;
            user.Country = model.Country;
            user.ProfilePictureUrl = model.ProfilePictureUrl;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData[SD.Error] = "Error while updating profile";
                return View(model);
            }

            TempData[SD.Success] = "Profile updated successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
