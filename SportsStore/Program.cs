using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(op => 
    op.UseNpgsql(connectionString));
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IProductRepository, EFProductRepository>();
builder.Services.AddTransient<IOrderRepository, EFOrderRepository>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<Cart>( sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Host.UseDefaultServiceProvider(op => op.ValidateScopes = false);
var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseStatusCodePages();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: null,
        pattern: "{category}/Page{productPage}",
        defaults: new { controller = "Product", action = "List"}
        );
    endpoints.MapControllerRoute(
        name: null,
        pattern: "Page{productPage}",
        defaults: new { controller = "Product", action = "List", productPage = 1}
        );
    endpoints.MapControllerRoute(
        name: null,
        pattern: "{category}",
        defaults: new { controller = "Product", action = "List", productPage = 1 }
        );
    endpoints.MapControllerRoute(
        name: null,
        pattern: "",
        defaults: new { controller = "Product", action = "List", productPage = 1 }
        );
    endpoints.MapControllerRoute(
        name: null,
        pattern: "{controller}/{action}/{id?}"
        );
});
SeedData.EnsurePopulated(app);
app.Run();
