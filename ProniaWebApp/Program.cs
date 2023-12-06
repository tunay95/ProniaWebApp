using Microsoft.EntityFrameworkCore;
using ProniaWebApp.DAL;
using ProniaWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromSeconds(5);
});



builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddScoped<LayoutServices>();
var app = builder.Build();
app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
  );
app.MapControllerRoute(
    name: "default",
    pattern: "{Controller=Home}/{Action=index}/{id?}"
    );


app.UseSession();
app.UseStaticFiles();

app.Run();
