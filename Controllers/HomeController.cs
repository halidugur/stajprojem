using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using stajprojem.Models;
using stajprojem.Services;        // EKLEND�
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace stajprojem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AdoSqlExecutorService _sqlExec;   // EKLEND�

        public HomeController(
            ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory,
            AdoSqlExecutorService sqlExec)                 // EKLEND�
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _sqlExec = sqlExec;                            // EKLEND�
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Sorgula()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Sorgula(string userQuery)
        {
            var client = _httpClientFactory.CreateClient("NlpSqlApi");

            var requestBody = new { text = userQuery };
            SqlResponseViewModel apiResponse;

            try
            {
                var response = await client.PostAsJsonAsync("generate-sql", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    apiResponse = await response.Content.ReadFromJsonAsync<SqlResponseViewModel>()
                                 ?? new SqlResponseViewModel { success = false, error = "Bo� yan�t al�nd�." };
                }
                else
                {
                    apiResponse = new SqlResponseViewModel
                    {
                        success = false,
                        error = "API ile ileti�imde hata olu�tu!"
                    };
                }
            }
            catch (Exception ex)
            {
                apiResponse = new SqlResponseViewModel
                {
                    success = false,
                    error = "Ba�lant� hatas�: " + ex.Message
                };
            }

            // --- SQL'i veritaban�nda �al��t�r (sadece SELECT) ---
            if (apiResponse.success && !string.IsNullOrWhiteSpace(apiResponse.sql))
            {
                if (!apiResponse.sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    apiResponse.success = false;
                    apiResponse.error = "Sadece SELECT sorgular�na izin veriliyor.";
                    return View(apiResponse);
                }

                try
                {
                    var (cols, rows) = await _sqlExec.ExecuteAsync(apiResponse.sql);
                    apiResponse.Columns = cols;
                    apiResponse.Rows = rows;
                }
                catch (Exception ex)
                {
                    apiResponse.success = false;
                    apiResponse.error = "SQL �al��t�rma hatas�: " + ex.Message;
                }
            }
            // ----------------------------------------------------

            return View(apiResponse);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
