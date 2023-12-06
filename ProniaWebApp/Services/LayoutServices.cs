namespace ProniaWebApp.Services
{
    public class LayoutServices
    {
        AppDbContext _context;

        public LayoutServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string,string>> GetSetting()
        {
            Dictionary<string, string> setting = _context.Settings.ToDictionary(s=>s.Key,s=>s.Value);
            return setting;
        
        }

    }
}
