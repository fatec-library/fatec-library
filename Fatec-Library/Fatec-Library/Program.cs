using Fatec_Library.Models;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Serivço Cookies Navegador - Configurar autenticação com cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuario/Login"; // página de login
        options.LogoutPath = "/Usuario/Logout"; // página de logout
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

//Conexão com o MongoDB
ContextMongodb.ConnectionString = builder.Configuration.GetValue<string>("MongoConnection:ConnectionString");
ContextMongodb.Database = builder.Configuration.GetValue<string>("MongoConnection:Database");
ContextMongodb.IsSSL = builder.Configuration.GetValue<bool>("MongoConnection:IsSSL");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();


// Ative autenticação e autorização**
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
