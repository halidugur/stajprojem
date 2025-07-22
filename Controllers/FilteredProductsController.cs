using Microsoft.AspNetCore.Mvc;
using stajprojem.Data;
using stajprojem.Models;
using System.Linq;

namespace stajprojem.Controllers
{
    public class FilteredProductsController : Controller
    {
        private readonly AppDbContext _context;

        public FilteredProductsController(AppDbContext context)
        {
            _context = context;
        }

        // Hem kategori, hem arama metni ile filtreleme!
        public IActionResult Index(int? CategoryId, string search)
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;

            var products = _context.Products.AsQueryable();

            if (CategoryId.HasValue && CategoryId.Value > 0)
                products = products.Where(p => p.category_id == CategoryId.Value);

            // Büyük/küçük harfe duyarsız arama
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p =>
                    p.product_name != null &&
                    p.product_name.Contains(search, System.StringComparison.OrdinalIgnoreCase));
            }

            return View(products.ToList());
        }
    }
}
