using DSP.Service;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5000");

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ApiService>();
builder.Services.AddHttpClient<FileTransferService>();
builder.Services.AddScoped<FileTransferService>();
builder.Services.AddScoped<ParticipantAuthService>();

builder.Services.AddScoped<DatabaseService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<DatabaseService>>();
    var configuration = builder.Configuration;
    var host = configuration["Database:Host"];
    var database = configuration["Database:Database"];
    var username = configuration["Database:Username"];
    var password = configuration["Database:Password"];

    return new DatabaseService(host, database, username, password, logger);
});



var app = builder.Build();


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
