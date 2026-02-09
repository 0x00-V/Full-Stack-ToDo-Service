using api.dbactions;
using api.Models;

DbInitialiser.InitialiseDB();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.MapControllers();
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
app.UseSwaggerUI(options =>
{
   options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"); 
});


app.Run();