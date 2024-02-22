using Images.Data;
using Images.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Images.Web.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog = UploadedImages; Integrated Security=true;";
        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddImage(IFormFile image, string password)
        {
            var fileName = $"{Guid.NewGuid()}-{image.FileName}";
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            var fs = new FileStream(imagePath, FileMode.CreateNew);
            image.CopyTo(fs);

            var im = new ImageManager(_connectionString);
            int id = im.AddAndGetId(imagePath, password, fileName);

            return Redirect($"/home/thankyou?id={id}");
        }
        public IActionResult ThankYou(int id)
        {
            var im = new ImageManager(_connectionString);
            return View(im.GetImageById(id));
        }
        public IActionResult ViewImage(int id)
        {
            var im = new ImageManager(_connectionString);
            var image = im.GetImageById(id);

            var existingInSession = HttpContext.Session.Get<List<int>>("Session");

            if (existingInSession == null && TempData["InvalidPassword"] == null)
            {
                var ivm = new ImageViewModel
                {
                    Image = image,
                    NeedPassword = true
                };
                return View(ivm);
            }
            else if (existingInSession == null && TempData["InvalidPassword"] != null)
            {
                ViewBag.InvalidPassword = TempData["InvalidPassword"];
                var ivm = new ImageViewModel
                {
                    Image = image,
                    NeedPassword = true
                };
                return View(ivm);
            }
            else if (existingInSession.Contains(id))
            {
                var ivm = new ImageViewModel
                {
                    Image = image,
                    NeedPassword = false
                };
                im.IncreaseImageViews(id);
                return View(ivm);
            }
            else
            {
                if (TempData["InvalidPassword"] != null)
                {
                    ViewBag.InvalidPassword = TempData["InvalidPassword"];
                }
                var ivm = new ImageViewModel
                {
                    Image = image,
                    NeedPassword = true
                };
                return View(ivm);
            }

        }
        public IActionResult Password(int id, string password)
        {
            var im = new ImageManager(_connectionString);
            var image = im.GetImageById(id);
            if (image.Password == password)
            {
                TempData["InvalidPassword"] = false;
                var ids = HttpContext.Session.Get<List<int>>("Session");
                if (ids == null)
                {
                    ids = new List<int>();
                }
                ids.Add(image.Id);
                HttpContext.Session.Set("Session", ids);
            }
            else
            {
                TempData["InvalidPassword"] = true;
            }
            return Redirect($"/home/viewimage?id={id}");
        }

    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}