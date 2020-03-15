using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InsSave.Models;
using AngleSharp.Html.Parser;
using System.Net.Http;
using AngleSharp.Html.Dom;
using InsSave.Models;

namespace InsSave.Controllers
{
    public class HomeController : Controller
    {

        [HttpPost]
        public IActionResult GetPhoto(string url)
        {
            var insMedia = new InsMedia();
            using (var client = new HttpClient())
            {
                //string Url = "https://www.instagram.com/p/B7PzQc_HYtO/?utm_source=ig_web_copy_link";
                var source = client.GetStringAsync(url).Result;
                var parser = new HtmlParser();
                var document = parser.ParseDocument(source);
                var photoUrl = document.All[37] as IHtmlMetaElement;
                insMedia.PhotoUrl = photoUrl.Content;
                return View(insMedia);
            }
        }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
