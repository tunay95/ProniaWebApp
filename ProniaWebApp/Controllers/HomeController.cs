using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaWebApp.DAL;
using ProniaWebApp.Models;
using ProniaWebApp.ViewModels;

namespace ProniaWebApp.Controllers
{
    public class HomeController:Controller
    {
        AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> sliders = _context.Sliders.ToList();
            HomeVM homeVM = new HomeVM()
            {
                Sliders = sliders,
                Products =await _context.Products.Include(p => p.ProductImages).ToListAsync()
            };

            return View(homeVM);
        }
    }
}
