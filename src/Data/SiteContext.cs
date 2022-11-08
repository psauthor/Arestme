using DesigningApis.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesigningApis.Data
{
  public class SiteContext : DbContext
  {
    public SiteContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Site> Sites { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Region> Regions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder bldr)
    {
      base.OnConfiguring(bldr);

      bldr.UseInMemoryDatabase("HeritageSites");
    }
  }
}
