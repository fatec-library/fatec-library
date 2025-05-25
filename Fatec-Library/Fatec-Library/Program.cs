using Fatec_Library.Models;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Seriv�o Cookies Navegador - Configurar autentica��o com cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuario/Login"; // p�gina de login
        options.LogoutPath = "/Usuario/Logout"; // p�gina de logout
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

//Conex�o com o MongoDB
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


// Ative autentica��o e autoriza��o**
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
