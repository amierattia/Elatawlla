using Microsoft.AspNetCore.Mvc;
using NetWork.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using NetWork.viewmodels;
using NetWork.viewmodels;

namespace NetWork.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Instructor,Super_Instructor")]
        public IActionResult Index()
        {
            var userEmail = User.Identity.Name;
            var userRole = User.IsInRole("Admin") || User.IsInRole("Instructor") || User.IsInRole("Super_Instructor");

            var chats = userRole 
                ? _context.Chats.ToList()
                : _context.Chats.Where(c => c.SenderEmail == userEmail).ToList();  

            return View(chats);
        }


        [Authorize(Roles = "Admin,Instructor,Super_Instructor")]
        public IActionResult Details(int id)
        {
            var chat = _context.Chats.FirstOrDefault(c => c.Id == id);
            if (chat == null)
            {
                return NotFound();
            }

            var userEmail = User.Identity.Name;
            var userRole = User.IsInRole("Admin") || User.IsInRole("Instructor") || User.IsInRole("Super_Instructor");

            if (!userRole && chat.SenderEmail != userEmail)
            {
                return Forbid();  
            }

            return View(chat);
        }


        [HttpGet]
        public IActionResult Create()
        {
            var userEmail = User.Identity.Name; 
            var model = new ChatViewModel
            {
                SenderEmail = userEmail
            };
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ChatViewModel model)
        {
            if (ModelState.IsValid)
            {
                var chat = new Chat
                {
                    SenderEmail = model.SenderEmail,
                    Message = model.Message,
                    MessageType = model.MessageType,
                    SentDate = DateTime.Now,
                    Status = "مفتوحة"
                };

                _context.Chats.Add(chat);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "تم إرسال المشكلة وسيتم حلها في أقرب وقت.";

                return RedirectToAction("Create");  
            }

            return View(model);
        }



        [HttpPost]
        [Authorize(Roles = "Admin,Instructor,Super_Instructor")]
        public IActionResult Respond(int id, string response, string role)
        {
            var chat = _context.Chats.FirstOrDefault(c => c.Id == id);
            if (chat == null)
            {
                return NotFound();
            }

            if (role == "Admin")
            {
                chat.AdminResponse = response;
            }
            else if (role == "Instructor")
            {
                chat.InstructorResponse = response;
            }

            chat.Status = "مغلقة"; 
            return RedirectToAction("Details", new { id = chat.Id });
        }
    }
}
