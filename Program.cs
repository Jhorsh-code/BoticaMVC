using BoticaMVC.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BoticaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cn")));

var app = builder.Build();
QuestPDF.Settings.License = LicenseType.Community;


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BoticaDbContext>();
    SeedData.Inicializar(db);
}

app.Run();
