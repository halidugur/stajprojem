using Microsoft.AspNetCore.Mvc;
using stajprojem.Models;
using stajprojem.Data; // DbContext için

public class ProductsController : Controller
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // Filtreleme sayfası - GET
    [HttpGet]
    public IActionResult Filter(ProductFilterViewModel filter)
    {
        // Tüm ürünleri getir
        var products = _context.Products.AsQueryable();

        // Filtreler
        if (filter.CategoryId.HasValue)
            products = products.Where(p => p.category_id == filter.CategoryId);

        if (!string.IsNullOrEmpty(filter.ProductName))
            products = products.Where(p => p.product_name.Contains(filter.ProductName));

        if (filter.MinPrice.HasValue)
            products = products.Where(p => p.unit_price >= filter.MinPrice);

        if (filter.MaxPrice.HasValue)
            products = products.Where(p => p.unit_price <= filter.MaxPrice);

        if (filter.MinStock.HasValue)
            products = products.Where(p => p.stock_quantity >= filter.MinStock);

        if (filter.MaxStock.HasValue)
            products = products.Where(p => p.stock_quantity <= filter.MaxStock);

        // View'e hem filtreyi hem sonucu gönder
        ViewBag.Filter = filter;
        return View(products.ToList());
    }
}
