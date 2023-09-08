using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TVCartoons.Web.Models;

namespace TVCartoons.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUrl()
        {
            return Json(await GenerateUrl());
        }

        public async Task<string> GenerateUrl()
        {
            var random = new Random();
            var success = false;
            var urlSucces = string.Empty;
            do
            {
                try
                {
                    var url = $"https://lacartoons.com/serie/capitulo/{random.Next(1, 31001)}";
                    using (var httpClient = new HttpClient())
                    {
                        var html = await httpClient.GetStringAsync(url);
                        var doc = new HtmlDocument();
                        doc.LoadHtml(html);
                        var iframeNode = doc.DocumentNode.SelectSingleNode("//iframe");
                        if (iframeNode != null)
                        {
                            var iframeSrc = iframeNode.GetAttributeValue("src", "");
                            var videoHtml = await httpClient.GetStringAsync(iframeSrc);
                            success = true;
                            urlSucces = iframeSrc;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error: {ex.InnerException?.Message ?? ex.Message}");
                }
            } while (!success);
            return urlSucces;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}