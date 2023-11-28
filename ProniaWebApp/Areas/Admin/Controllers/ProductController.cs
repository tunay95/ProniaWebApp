using Microsoft.EntityFrameworkCore;
using ProniaWebApp.Areas.Admin.ViewModels.Product;

namespace ProniaWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController:Controller
    {
        AppDbContext _context {  get; set; }
        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            List<Product> products= await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductsTags)
                .ThenInclude(pt=>pt.Tag)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM createProductVM)
        {
			ViewBag.Categories = await _context.Categories.ToListAsync();
			ViewBag.Tags = await _context.Tags.ToListAsync();
			if (!ModelState.IsValid)
            {
				return View("Error");
			}
            
            bool resultCategory = await _context.Categories.AnyAsync(c=>c.Id==createProductVM.CategoryId);
            if (!resultCategory)
            {
                ModelState.AddModelError("Category", "Category not found");
                return View();
            }

            Product product = new Product()
            {
                Name = createProductVM.Name,
                Price = createProductVM.Price,
                Description = createProductVM.Description,
                SKU = createProductVM.SKU,
                CategoryId = createProductVM.CategoryId

            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();



           return RedirectToAction("Index");
		}

        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id==id);  
            if(product == null)
            {
                return View("Error");
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Update(int id)
        {
			Product product = await _context.Products.Where(p=>p.Id==id).FirstOrDefaultAsync();
            if(product == null)
            {
                return View("Error");
            }
			ViewBag.Tags = await _context.Tags.ToListAsync();
			ViewBag.Categories = await _context.Categories.ToListAsync();

            UpdateProductVM updateProductVM = new UpdateProductVM()
            {
                Id = id,
                Name=product.Name,
                Description=product.Description,
                SKU=product.SKU,
                CategoryId = product.CategoryId,
                Price=product.Price,

            };
            
            return View(updateProductVM);

        }

		[HttpPost]
        public async Task<IActionResult> Update(UpdateProductVM updateProductVM)
        {
			ViewBag.Categories = await _context.Categories.ToListAsync();
			ViewBag.Tags = await _context.Tags.ToListAsync();

            if(!ModelState.IsValid)
            {
                return View();
            }

			Product existProduct = await _context.Products.Where(p => p.Id == updateProductVM.Id).FirstOrDefaultAsync();
			if (existProduct == null)
			{
				return View("Error");
			}

			bool resultCategory = await _context.Categories.AnyAsync(c => c.Id == updateProductVM.CategoryId);
			if (!resultCategory)
			{
				ModelState.AddModelError("Category", "Category not found");
				return View();
			}

		    existProduct.Name=updateProductVM.Name;
            existProduct.Description=updateProductVM.Description;
            existProduct.SKU=updateProductVM.SKU;
            existProduct.CategoryId = updateProductVM.CategoryId;
            existProduct.Price = updateProductVM.Price;


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
		}

	}


}
