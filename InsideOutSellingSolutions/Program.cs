using InsideOutSellingSolutions.Repository;
using InsideOutSellingSolutions.Repository.IRepository;
using InsideOutSellingSolutions.Repository.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add authentication services -------------------------SK
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // redirect here if not authenticated
        //options.AccessDeniedPath = "/Home/AccessDenied"; // ✅ Redirect here if not authorized (403) (Newly Added)
    });
//hello world .....

builder.Services.AddAuthorization();


builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<ICommonRepository, CommonRepository>();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
