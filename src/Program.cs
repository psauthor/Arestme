using DesigningApis.Data;
using DesigningApis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Versioning;
using DesigningApis.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddDbContext<SiteContext>();

services.AddScoped<ISiteRepository, SiteRepository>();

services.AddAutoMapper(typeof(Program));
services.AddHttpCacheHeaders(opt => opt.MaxAge = 600);
services.AddResponseCaching();

services.AddCors(setup =>
{
  setup.AddDefaultPolicy(cfg =>
  {
    cfg.AllowAnyOrigin();
    cfg.AllowAnyMethod();
    cfg.AllowAnyHeader();
  });

  setup.AddPolicy("Prevent", cfg =>
  {
    cfg.WithOrigins("https://localhost:5002");
  });
});

services.AddApiVersioning(cfg =>
{
  cfg.ReportApiVersions = true;
  cfg.DefaultApiVersion = new ApiVersion(2, 0);
  cfg.AssumeDefaultVersionWhenUnspecified = true;
  cfg.ApiVersionReader = ApiVersionReader.Combine(
    new AcceptHeaderApiVersionReader(),
    new QueryStringApiVersionReader("v"),
    new HeaderApiVersionReader("X-Version"));
});

services.AddControllers(cfg =>
{
  cfg.RespectBrowserAcceptHeader = true;
})
  .AddXmlSerializerFormatters();

services.AddRazorPages();

var app = builder.Build();

SeedDb(app);

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseCors(cfg =>
{
  cfg.AllowAnyHeader();
  cfg.AllowAnyMethod();
  cfg.AllowAnyOrigin();
});

app.UseStaticFiles();
app.UseResponseCaching();
app.UseHttpCacheHeaders();

app.MapControllers();
app.MapRazorPages();
app.Run();


void SeedDb(WebApplication app)
{
  using (var scope = app.Services.CreateScope())
  {
    var repo = scope.ServiceProvider.GetRequiredService<ISiteRepository>();

    repo.BuildDatabaseAsync().Wait();
    repo.SaveAllAsync().Wait();
  }
}

partial class Program { }
