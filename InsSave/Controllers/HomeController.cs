using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InsSave.Models;
using AngleSharp.Html.Parser;
using System.Net.Http;
using AngleSharp.Html.Dom;

namespace InsSave.Controllers
{
    public class HomeController : Controller
    {

        [HttpPost]
        public IActionResult Test(string url)
        {
            using (var client = new HttpClient())
            {
                //string Url = "https://www.instagram.com/p/B7PzQc_HYtO/?utm_source=ig_web_copy_link";
                var source = client.GetStringAsync(url).Result;
                var parser = new HtmlParser();
                var document = parser.ParseDocument(source);
                var s1 = document.All[37] as IHtmlMetaElement;
                ViewBag.str = s1.Content;

                return View();
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
