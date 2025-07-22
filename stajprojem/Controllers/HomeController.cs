using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using stajprojem.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace stajprojem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // --- Sorgula GET ---
        [HttpGet]
        public IActionResult Sorgula()
        {
            return View();
        }

        // --- Sorgula POST ---
        [HttpPost]
        public async Task<IActionResult> Sorgula(string userQuery)
        {
            var client = _httpClientFactory.CreateClient("NlpSqlApi");

            var requestBody = new { text = userQuery };
            SqlResponseViewModel apiResponse = null;

            try
            {
                var response = await client.PostAsJsonAsync("generate-sql", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    apiResponse = await response.Content.ReadFromJsonAsync<SqlResponseViewModel>();
                }
                else
                {
                    apiResponse = new SqlResponseViewModel
                    {
                        success = false,
                        error = "API ile iletiþimde hata oluþtu!"
                    };
                }
            }
            catch (Exception ex)
            {
                apiResponse = new SqlResponseViewModel
                {
                    success = false,
                    error = "Baðlantý hatasý: " + ex.Message
                };
            }

            return View(apiResponse);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
