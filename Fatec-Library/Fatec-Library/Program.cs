using Fatec_Library.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
