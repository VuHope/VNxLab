using Microsoft.AspNetCore.Authorization;
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

        public AdminUserController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
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
    }
}
