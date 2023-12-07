using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaWebApp.ViewModels;

namespace ProniaWebApp.Services
{
    public class LayoutServices
    {
        AppDbContext _context;
        IHttpContextAccessor _http;

        public LayoutServices(IHttpContextAccessor http)
        {
            _http = http;
        }

        public LayoutServices(AppDbContext context)
        {
            _context = context;
        }



        public async Task<Dictionary<string,string>> GetSetting()
        {
            Dictionary<string, string> setting = _context.Settings.ToDictionary(s=>s.Key,s=>s.Value);
            return setting;
        
        }

        public async Task<List<BasketItemVM>> GetBasket()
        {
            var jsonCookie = _http.HttpContext.Request.Cookies["Basket"];
            List<BasketItemVM> basketItems = new List<BasketItemVM>();
            if (jsonCookie != null)
            {
                var cookieItems = JsonConvert.DeserializeObject<List<CookieItemVm>>(jsonCookie);

                bool countCheck = false;
                List<CookieItemVm> deletedCookie = new List<CookieItemVm>();
                foreach (var item in cookieItems)
                {
                    Product product = await _context.Products.Where(p => p.IsDeleted == false).Include(p => p.ProductImages.Where(p => p.IsPrime == true)).FirstOrDefaultAsync(p => p.Id == item.Id);
                    if (product == null)
                    {
                        deletedCookie.Add(item);
                        continue;
                    }

                    basketItems.Add(new BasketItemVM()
                    {
                        Id = item.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Count = item.Count,
                        ImgUrl = product.ProductImages.FirstOrDefault().ImgUrl
                    });
                }
                if (deletedCookie.Count > 0)
                {
                    foreach (var delete in deletedCookie)
                    {
                        cookieItems.Remove(delete);
                    }
                    _http.HttpContext.Response.Cookies.Append("Basket", JsonConvert.SerializeObject(cookieItems));
                }



            }
            return basketItems;
        }
    }
}
