var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<ApiAuthMiddleware>();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
// Note To Readers - I know there's no debug messages througout. I could've 100% spent the time implementing,
// But since this was my first actual C# project, first MVC project, and my first non-monolithic project. I felt the need to finish the project asap (Mainly because I want to work on cooler things than a todo app)