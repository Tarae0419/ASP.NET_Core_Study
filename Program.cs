using Microsoft.EntityFrameworkCore;
using ServerStudy.DataBase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserData>(options =>
    options.UseMySql(
        "Server=localhost;Database=UserData;User=root;Password=1234;",
        new MySqlServerVersion(new Version(8, 0, 32))
    ));

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.Run();