using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using System.Security.Claims;
using WebMVC.Data;
using WebMVC.Utility;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        public ContactController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> Index()
        {
            var lstContact = new List<Contact>();
            if (User.IsInRole(SD.Admin))
            {
                var newListContact = await _context.Contacts.OrderByDescending(x => x.Id).ToListAsync();
                var lstAdminId = newListContact.Select(x => x.AdminId).ToList();
                var aspNetUsers = _context.Users.Where(x => lstAdminId.Contains(x.Id)).ToList();
                lstContact = newListContact.Select(x => new Contact()
                {
                    Id = x.Id,
                    AdminId = aspNetUsers.FirstOrDefault(a => a.Id == x.AdminId)?.UserName,
                    Answer = x.Answer,
                    CreatedAt = x.CreatedAt,
                    Email = x.Email,
                    Message = x.Message,
                    Status = x.Status,
                    Name = x.Name,
                    UserId = x.UserId,
                    ApplicationUser = x.ApplicationUser
                }).ToList();
            }
            ViewData["lstContact"] = lstContact;
            return View();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> DetailContact(int id)
        {
            var contact = new Contact();
            contact = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == id);

            return View(contact);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitDetailContact(Contact? contactNew)
        {
            if (contactNew.Id != 0)
            {

                var contactFind = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == contactNew.Id && x.Status == 0);
                if (contactFind != null)
                {
                    contactFind.Answer = contactNew.Answer;
                    contactFind.AdminId = User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).First().Value;
                    contactFind.Status = 1;

                    // Lưu thông tin vào csdl
                    _context.SaveChanges();
                    // gửi đến gmail đó

                    await _emailSender.SendEmailAsync(
                        contactFind.Email,
                        "Answer the question",
                        $"Question: {contactFind.Message}. \nAnswer: {contactFind.Answer}.");
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            if (id != 0)
            {
                var contact = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == id);
                if (contact != null)
                {
                    _context.Contacts.Remove(contact);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitContact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Contacts.Add(new Contact
                {
                    Email = model.Email.Trim(),
                    Name = model.Name.Trim(),
                    Message = model.Message.Trim()
                });
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home", null);
            }

            return View("Index"); // Return the view with error messages
        }
    }
}
