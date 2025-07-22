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

        public IActionResult Index(int? CategoryId)
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;

            var products = _context.Products.AsQueryable();
            if (CategoryId.HasValue && CategoryId.Value > 0)
                products = products.Where(p => p.category_id == CategoryId.Value);

            return View(products.ToList());
        }

    }
}

