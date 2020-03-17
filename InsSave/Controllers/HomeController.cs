using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InsSave.Models;
using AngleSharp.Html.Parser;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace InsSave.Controllers
{
    public class HomeController : Controller
    {

        [HttpPost]
        public IActionResult GetMedia(string url)
        {
            var insMedia = new InsMedia();
            string scriptFlag = "window._sharedData";
            List<string> photoUrls = new List<string>();
            List<string> videoUrls = new List<string>();

            using (var client = new HttpClient())
            {
                var source = client.GetStringAsync(url).Result;
                var parser = new HtmlParser();
                var document = parser.ParseDocument(source);
                string JsonStr = string.Empty;
                var mathScript = document.Scripts.Where(t => t.Text.Contains(scriptFlag) && t.Text.Substring(0, 18) == scriptFlag).First();
                if (mathScript != null)
                {
                    JsonStr = mathScript.Text.Substring(21);
                    JsonStr = JsonStr.Remove(JsonStr.Length - 1);
                }
                var mediaJson = JObject.Parse(JsonStr);
                var shortcode_media = mediaJson["entry_data"]["PostPage"][0]["graphql"]["shortcode_media"];
                if (shortcode_media["edge_sidecar_to_children"] == null)
                {
                    // 单项
                    if ((bool)shortcode_media["is_video"] == false)
                    {
                        // 单张图片
                        photoUrls.Add((string)shortcode_media["display_url"]);
                    }
                    else
                    {
                        // 单个视频
                        videoUrls.Add((string)shortcode_media["video_url"]);
                    }
                }
                else
                {
                    // 多项
                    var edges = shortcode_media["edge_sidecar_to_children"]["edges"];
                    foreach (var edge in edges)
                    {
                        if ((bool)edge["node"]["is_video"] == false)
                        {
                            // 图片
                            photoUrls.Add((string)edge["node"]["display_url"]);
                        }
                        else
                        {
                            // 视频
                            videoUrls.Add((string)edge["node"]["video_url"]);
                        }
                    }
                }

                insMedia.PhotoUrls = photoUrls;
                insMedia.VideoUrls = videoUrls;

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
