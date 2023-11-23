


namespace ProniaWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController:Controller
    {
        AppDbContext _context;

        public SliderController(AppDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Create(Slider slider)
        {
            string filname=slider.ImageFile.Name;
            string path = @"C:\Users\TUNAY\source\repos\ProniaWebApp\ProniaWebApp\wwwroot\Upload\SliderImage\" + filname;
            using( FileStream stream = new FileStream(path, FileMode.Create))
            {
                slider.ImageFile.CopyTo(stream);
            }

            slider.ImgUrl = filname;

            if(!ModelState.IsValid)
            {

                return View();
            }

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
