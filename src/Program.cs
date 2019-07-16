using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DesigningApis.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DesigningApis
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = CreateWebHostBuilder(args).Build();
      SeedDb(host);
      host.Run();
    }

    private static void SeedDb(IWebHost host)
    {
      using (var scope = host.Services.CreateScope())
      {
        var repo = scope.ServiceProvider.GetRequiredService<ISiteRepository>();

        repo.BuildDatabaseAsync().Wait();
        repo.SaveAllAsync().Wait();
      }
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
  }
}
