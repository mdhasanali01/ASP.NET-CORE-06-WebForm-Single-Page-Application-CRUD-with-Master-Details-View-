using Microsoft.EntityFrameworkCore;
using SPA_BookRepository_CRUD_Mid_09.HostedService;
using SPA_BookRepository_CRUD_Mid_09.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BookDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("db")));
builder.Services.AddHostedService<DbSeederHostedService>();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
var app = builder.Build();

app.UseStaticFiles();
app.MapControllerRoute(
    name:"default",
    pattern:"{controller=SPA}/{action=Index}/{id?}");


app.Run();
