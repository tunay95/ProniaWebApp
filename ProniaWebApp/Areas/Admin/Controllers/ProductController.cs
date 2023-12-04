using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using ProniaWebApp.Areas.Admin.ViewModels.Product;
using ProniaWebApp.Helper;

namespace ProniaWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        AppDbContext _context { get; set; }
        IWebHostEnvironment _env { get; set; }
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {

            List<Product> products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductsTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p=>p.ProductImages)
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

            bool resultCategory = await _context.Categories.AnyAsync(c => c.Id == createProductVM.CategoryId);
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
                CategoryId = createProductVM.CategoryId,
                ProductsTags = new List<ProductTag>(),
                ProductImages=new List<ProductImg>()
            };

            if (createProductVM.TagIds != null)
            {
                foreach (int tagId in createProductVM.TagIds)
                {
                    bool resultTag = await _context.Tags.AnyAsync(c => c.Id == tagId);
                    if (!resultTag)
                    {
                        ModelState.AddModelError("TagIds", $"{tagId}--> no such a tag was found");
                        return View();
                    }

                    ProductTag productTag = new ProductTag()
                    {
                        Product = product,
                        TagId = tagId,
                    };

                    _context.ProductTags.Add(productTag);
                }
            }

            if (!createProductVM.MainPhoto.CheckType("image/"))
            {
                ModelState.AddModelError("MainPhoto","Duzgun formatda sekil yerlesdirin");
                return View();           
            }

            if (!createProductVM.MainPhoto.CheckLength(3000))
            {
                ModelState.AddModelError("MainPhoto", "max 3mb sekil yukleye bilersiz");
                return View();
            }

            if (!createProductVM.MainPhoto.CheckType("image/"))
            {
                ModelState.AddModelError("HoverPhoto", "Duzgun formatda sekil yerlesdirin");
                return View();
            }

            if (!createProductVM.MainPhoto.CheckLength(3000))
            {
                ModelState.AddModelError("HoverPhoto", "max 3mb sekil yukleye bilersiz");
                return View();
            }

            ProductImg mainImg = new ProductImg()
            {
                IsPrime = true,
                ImgUrl = createProductVM.MainPhoto.Upload(_env.WebRootPath,@"\Upload\Product\"),
                Product = product,
            };

            ProductImg hoverImg = new ProductImg()
            {
                IsPrime = false,
                ImgUrl = createProductVM.HoverPhoto.Upload(_env.WebRootPath, @"\Upload\Product\"),
                Product = product,
            };

            TempData["Error"] = "";

            product.ProductImages.Add(mainImg);
            product.ProductImages.Add(hoverImg);

            if (createProductVM.Photos != null)
            {
                foreach (var item in createProductVM.Photos)
                {
					if (!item.CheckType("image/"))
					{
                        TempData["Error"] += $"{item.FileName} --> type is wrong \t";
                        continue;
					}

					if (!item.CheckLength(3000))
					{
						TempData["Error"] += $"{item.FileName} 's quality is more than 3 mb \t";
						continue;
					}

					ProductImg newPhoto = new ProductImg()
					{
						IsPrime = false,
						ImgUrl = item.Upload(_env.WebRootPath, @"\Upload\Product\"),
						Product = product,
					};
                    product.ProductImages.Add(newPhoto);
				}
            }


            await _context.ProductImgs.AddAsync(mainImg);
            await _context.ProductImgs.AddAsync(hoverImg);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();



            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return View("Error");
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            Product product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductsTags)
                .ThenInclude(p => p.Tag)
                .Include(p=>p.ProductImages)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            if (product == null)
            {
                return View("Error");
            }
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();

            UpdateProductVM updateProductVM = new UpdateProductVM()
            {
                Id = id,
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                Price = product.Price,
                TagIds = new List<int>(),
                productImg=new List<ProductImagesVM>()
            };

            foreach (var item in product.ProductsTags)
            {
                updateProductVM.TagIds.Add(item.TagId);
            }
            
            foreach (var item in product.ProductImages)
            {
                ProductImagesVM productImg = new ProductImagesVM()
                {
                    IsPrime = item.IsPrime,
                    ImgUrl=item.ImgUrl,
                    Id = item.Id,
                };

                updateProductVM.productImg.Add(productImg);
            }


            return View(updateProductVM);

        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductVM updateProductVM)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }

            Product existProduct = await _context.Products.Include(p=>p.ProductImages).Where(p => p.Id == updateProductVM.Id).FirstOrDefaultAsync();
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

            existProduct.Name = updateProductVM.Name;
            existProduct.Description = updateProductVM.Description;
            existProduct.SKU = updateProductVM.SKU;
            existProduct.CategoryId = updateProductVM.CategoryId;
            existProduct.Price = updateProductVM.Price;


            if (updateProductVM.TagIds != null)
            {

                foreach (int tagId in updateProductVM.TagIds)
                {
                    bool resultTag = await _context.Tags.AnyAsync(c => c.Id == tagId);
                    if (!resultTag)
                    {
                        ModelState.AddModelError("TagIds", $"{tagId}--> no such a tag was found");
                        return View();
                    }


                }

                List<int> createTags;
                if (existProduct.ProductsTags != null)
                {
                 createTags= updateProductVM.TagIds.Where(ti => !existProduct.ProductsTags.Exists(pt => pt.TagId == ti)).ToList();
                }

                else
                {
                    createTags = updateProductVM.TagIds.ToList();
                }

                foreach (var tagId in createTags)
                {
                    ProductTag productTag = new ProductTag()
                    {
                        TagId = tagId,
                        ProductId = existProduct.Id

                    };
                    //existProduct.ProductsTags.Add(productTag);
                    _context.ProductTags.Add(productTag);
                }

                List<ProductTag> removeTags = existProduct.ProductsTags.Where(pt =>!updateProductVM.TagIds.Contains(pt.TagId)).ToList();

                _context.ProductTags.RemoveRange(removeTags);

            }

            else
            {
                var productTagList=_context.ProductTags.Where(pt=>pt.ProductId==existProduct.Id).ToList();
                _context.ProductTags.RemoveRange(productTagList);
            }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            if (updateProductVM.MainPhoto != null)
            {
	            if (!updateProductVM.MainPhoto.CheckType("image/"))
				{
					ModelState.AddModelError("MainPhoto", "Duzgun formatda sekil yerlesdirin");
					return View();
				}

				if (!updateProductVM.MainPhoto.CheckLength(3000))
				{
					ModelState.AddModelError("MainPhoto", "max 3mb sekil yukleye bilersiz");
					return View();
				}

                ProductImg newMainImages = new ProductImg()
                {
                    IsPrime = true,
                    ProductId=existProduct.Id,
                    ImgUrl = updateProductVM.MainPhoto.Upload(_env.WebRootPath,@"\Upload\Product\")
                };
                var oldMainPhoto = existProduct.ProductImages?.FirstOrDefault(p => p.IsPrime == true);
                existProduct.ProductImages.Remove(oldMainPhoto);
                existProduct.ProductImages.Add(newMainImages);
            }

			if (updateProductVM.HoverPhoto != null)
			{
				if (!updateProductVM.HoverPhoto.CheckType("image/"))
				{
					ModelState.AddModelError("HoverPhoto", "Duzgun formatda sekil yerlesdirin");
					return View();
				}

				if (!updateProductVM.HoverPhoto.CheckLength(3000))
				{
					ModelState.AddModelError("HoverPhoto", "max 3mb sekil yukleye bilersiz");
					return View();
				}

				ProductImg newHoverImages = new ProductImg()
				{
					IsPrime = false,
					ProductId = existProduct.Id,
					ImgUrl = updateProductVM.HoverPhoto.Upload(_env.WebRootPath, @"\Upload\Product\")
				};
				var oldHoverPhoto = existProduct.ProductImages?.FirstOrDefault(p => p.IsPrime == false);
				existProduct.ProductImages.Remove(oldHoverPhoto);
				existProduct.ProductImages.Add(newHoverImages);
			}

            if (updateProductVM.Photos != null)
            {

            }
		}

    }
}
