



using ProniaWebApp.Helper;
using ProniaWebApp.Models;

namespace ProniaWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController:Controller
    {
        AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _environment = env;
        }

        public IActionResult Index()
        {
            List<Slider>sliderList = _context.Sliders.ToList();
            return View(sliderList);
        }
        
        public IActionResult Create()
        {   
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slider slider)
        {
            if (!slider.ImageFile.ContentType.Contains("images"))
            {
                ModelState.AddModelError("ImageFile", "You can upload only image format!");
                return View();
            }
			if (slider.ImageFile.Length > 2097152)
			{
				ModelState.AddModelError("ImageFile", "Maximum size 2MB!");
				return View();
			}

            slider.ImgUrl = slider.ImageFile.Upload(_environment.WebRootPath, @"\Upload\SliderImage\");

            if(!ModelState.IsValid)
            {
                return View();
            }

            _context.Sliders.Add(slider);
            _context.SaveChanges();

			return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            Slider slider = _context.Sliders.Find(id);
            return View(slider);
        }

        [HttpPost]
        public IActionResult Update(Slider newSlider)
        {
            Slider oldSlider = _context.Sliders.Find(newSlider.Id);

			if (!newSlider.ImageFile.ContentType.Contains("images"))
			{
				ModelState.AddModelError("ImageFile", "You can upload only image format!");
				return View();
			}
			if (newSlider.ImageFile.Length > 2097152)
			{
				ModelState.AddModelError("ImageFile", "Maximum size 2MB!");
				return View();
			}

			FileManager.Delete(oldSlider.ImgUrl, _environment.WebRootPath, @"\Upload\SliderImage\");
			newSlider.ImgUrl = newSlider.ImageFile.Upload(_environment.WebRootPath, @"\Upload\SliderImage\");

			if (!ModelState.IsValid)
			{
				return View();
			}

            oldSlider.Title = newSlider.Title;
            oldSlider.SubTitle = newSlider.SubTitle;
            oldSlider.Description = newSlider.Description;
            oldSlider.ImgUrl = newSlider.ImgUrl;

            _context.SaveChanges();
			return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            Slider slider = _context.Sliders.Find(id);
            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            FileManager.Delete(slider.ImgUrl, _environment.WebRootPath, @"\Upload\SliderImage\");
            return RedirectToAction("Index");
        }

    }
}
