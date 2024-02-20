using Images.Data;
using Images.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            int id = im.AddAndGetId(imagePath, password);

            return RedirectToAction(`thankyou?{id}`);
        }
        public IActionResult ThankYou(int id)
        {
            var im = new ImageManager(_connectionString);
            return View(im.GetImageById(id));
        }
        public IActionResult ViewImage(int id)
        {
            return View();
        }

    }
}