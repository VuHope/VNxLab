using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entity;
using WebMVC.Utility;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> DisplayUserProfile(string userId)
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
        public async Task<IActionResult> DisplayUserProfile(UserProfileViewModel model, IFormFile? file)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }
            string profilePictureUrl = user.ProfilePictureUrl;

            if (file != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string newsPath = Path.Combine(wwwRootPath, @"images\profile");

                if (!string.IsNullOrEmpty(profilePictureUrl))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, profilePictureUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(newsPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                profilePictureUrl = @"\images\profile\" + fileName;
            }

            user.FullName = model.FullName;
            user.Gender = model.Gender;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.City = model.City;
            user.Country = model.Country;
            user.ProfilePictureUrl = profilePictureUrl;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData[SD.Error] = "Error while updating profile";
                return View(model);
            }
            model.ProfilePictureUrl = profilePictureUrl;
            TempData[SD.Success] = "Profile updated successfully";
            return View(model);
        }
    }
}
