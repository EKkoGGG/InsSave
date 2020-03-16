using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InsSave.Models;
using AngleSharp.Html.Parser;
using System.Net.Http;
using AngleSharp.Html.Dom;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;//IsoDateTimeConverter
using System.Linq;

namespace InsSave.Controllers
{
    public class HomeController : Controller
    {

        [HttpPost]
        public IActionResult GetPhoto(string url)
        {
            var insMedia = new InsMedia();
            string scriptFlag = "window._sharedData";
            using (var client = new HttpClient())
            {
                var source = client.GetStringAsync(url).Result;
                var parser = new HtmlParser();
                var document = parser.ParseDocument(source);
                string photoJsonStr = string.Empty;
                var mathScript = document.Scripts.Where(t => t.Text.Contains(scriptFlag) && t.Text.Substring(0, 18) == scriptFlag).First();
                if (mathScript != null)
                {
                    photoJsonStr = mathScript.Text.Substring(21);
                    photoJsonStr = photoJsonStr.Remove(photoJsonStr.Length - 1);
                }
                var photoJson = JObject.Parse(photoJsonStr);
                insMedia.PhotoUrls =
                (from p in photoJson["entry_data"]["PostPage"][0]["graphql"]["shortcode_media"]["edge_sidecar_to_children"]["edges"]
                select (string)p["node"]["display_url"]).ToList();
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
