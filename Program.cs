using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using web_bh.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<web_bhContext>(options => {
    string conn = builder.Configuration.GetConnectionString("web_bhContext");
    options.UseSqlServer(conn);
});

builder.Services.AddSingleton<CartContext>();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});


builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// app.UseEndpoints(endpoints =>
//         {
//           endpoints.MapControllerRoute(
//             name : "areas",
//             pattern : "{area:exists}/{controller=Home}/{action=Index}/{id?}" 
//           );
//         });

app.MapAreaControllerRoute( 
    name: "areas",
    pattern : "Admin/{controller=MyAdmin}/{action=Index}/{id?}",
    areaName : "Admin"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
