using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using DesigningApis.Data;
using DesigningApis.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DesigningApis
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<SiteContext>()
        .AddEntityFrameworkInMemoryDatabase();

      services.AddScoped<ISiteRepository, SiteRepository>();

      services.AddAutoMapper(typeof(Startup));
      services.AddHttpCacheHeaders(opt => opt.MaxAge = 600);
      services.AddResponseCaching();

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

      services.AddMvc(cfg =>
      {
        cfg.RespectBrowserAcceptHeader = true;
      })
        .AddXmlSerializerFormatters()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
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
      app.UseMvc();
    }
  }
}
