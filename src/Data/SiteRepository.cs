using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using DesigningApis.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DesigningApis.Data
{
  public class SiteRepository : ISiteRepository
  {
    private readonly SiteContext _context;
    private readonly IHostingEnvironment _environment;
    private readonly ILogger<SiteRepository> _logger;

    public SiteRepository(SiteContext context,
      IHostingEnvironment environment,
      ILogger<SiteRepository> logger)
    {
      _context = context;
      _environment = environment;
      _logger = logger;
    }

    public async Task<Site[]> GetAllSitesAsync()
    {
      return await _context.Sites
        .Include(s => s.Category)
        .Include(s => s.Location)
        .Include(s => s.Region)
        .OrderBy(s => s.Name)
        .ToArrayAsync();
    }

    public async Task<Site> GetSiteAsync(int id)
    {
      return await _context.Sites
        .Include(s => s.Category)
        .Include(s => s.Location)
        .Include(s => s.Region)
        .Where(s => s.Id == id)
        .FirstOrDefaultAsync();
    }

    public async Task<Site[]> GetSitesByStateAsync(string state)
    {
      return await _context.Sites
        .Include(s => s.Category)
        .Include(s => s.Location)
        .Include(s => s.Region)
        .Where(s => s.States.ToLower().Contains(state.ToLower()))
        .ToArrayAsync();
    }

    public async Task<Site[]> GetAllSitesWithPaging(int page, int pagesize)
    {
      return await _context.Sites
        .Include(s => s.Category)
        .Include(s => s.Location)
        .Include(s => s.Region)
        .OrderBy(s => s.Name)
        .Skip((page - 1) * pagesize)
        .Take(pagesize)
        .ToArrayAsync();
    }

    public async Task<int> GetSiteCountAsync()
    {
      return await _context.Sites
        .CountAsync();
    }

    public async Task<bool> ClearDatabaseAsync()
    {
      try
      {
        if (await _context.Database.EnsureDeletedAsync() && await _context.Database.EnsureCreatedAsync())
        {
          return await BuildDatabaseAsync() && await SaveAllAsync();
        }
        else
        {
          _logger.LogError($"Failed to drop and create the database");
        }
      }
      catch (Exception ex)
      {
        _logger.LogError($"Threw exception while clearing database: {ex}");

      }

      return false;
    }


    public async Task<bool> BuildDatabaseAsync()
    {
      try
      {
        var results = BuildSites();

        await _context.Sites.AddRangeAsync(results);

        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Threw exception while building database: {ex}");

      }

      return false;
    }

    public async Task<Site[]> GetSitesByRegionAsync(string key)
    {
      return await _context.Sites
        .Include(s => s.Category)
        .Include(s => s.Location)
        .Include(s => s.Region)
        .OrderBy(s => s.Name)
        .Where(s => EF.Functions.Like(s.Region.Name, key.ToLower().Replace("-", "%")))
        .ToArrayAsync();
    }

    public async Task<Category> GetCategoryByNameAsync(string name)
    {
      return await _context.Categories.Where(c => c.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
    }

    public async Task<Region[]> GetAllRegionsAsync()
    {
      return await _context.Regions.OrderBy(r => r.Name).ToArrayAsync();
    }

    public async Task<Region> GetRegionByNameAsync(string name)
    {
      return await _context.Regions.Where(c => c.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
    }

    public async Task<Region> GetRegionByKeyAsync(string key)
    {
      return await _context.Regions.Where(c =>  EF.Functions.Like(c.Name.ToLower(), key.Replace("-", "%"))).FirstOrDefaultAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
      try
      {
        return await _context.SaveChangesAsync() > 0;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Failed to save changes: {ex}");
        return false;
      }
    }

    private IEnumerable<Site> BuildSites()
    {
      var doc = XDocument.Load(Path.Combine(_environment.ContentRootPath, "Data/whc-en.xml"));

      var sites = new List<Site>();
      var categories = new List<Category>();
      var regions = new List<Region>();

      foreach (var s in doc.Descendants("row"))
      {
        var site = new Site()
        {
          Name = FromElement<string>(s, "site"),
          YearInscribed = FromElement<int>(s, "date_inscribed"),
          Url = FromElement<string>(s, "http_url"),
          Id = FromElement<int>(s, "id_number"),
          ImageUrl = FromElement<string>(s, "image_url"),
          DescriptionMarkup = FromElement<string>(s, "short_description"),
          States = FromElement<string>(s, "states"),
          Location = new Location()
          {
            Name = FromElement<string>(s, "location"),
            Longitude = FromElement<double>(s, "longitude"),
            Latitude = FromElement<double>(s, "longitude"),
          }
        };

        var catName = FromElement<string>(s, "category");
        var category = categories.FirstOrDefault(c => c.Name == catName);
        if (category == null)
        {
          category = new Category()
          {
            Id = categories.Count(),
            Name = catName
          };
          categories.Add(category);
        }

        site.Category = category;

        var regionName = FromElement<string>(s, "region");
        var region = regions.FirstOrDefault(r => r.Name == regionName);
        if (region == null)
        {
          region = new Region()
          {
            Id = regions.Count(),
            Name = regionName
          };
          regions.Add(region);
        }

        site.Region = region;

        sites.Add(site);

      };

      return sites;
    }

    T FromElement<T>(XElement element, XName name)
    {
      var child = element.Element(name);
      if (child != null)
      {
        return (T)Convert.ChangeType(element.Element(name).Value, typeof(T));
      }

      return default(T);
    }

    public void Add(Site model)
    {
      var newId = _context.Sites.Max(s => s.Id) + 1;
      var newLocationId = _context.Sites.Select(s => s.Location).Max(s => s.Id) + 1;
      model.Id = newId;
      model.Location.Id = newLocationId;

      _context.Add(model);
    }

    public void Add(Region model)
    {
      var newId = _context.Regions.Max(r => r.Id) + 1;
      model.Id = newId;

      _context.Add(model);
    }

    public void Add(Category model)
    {
      var newId = _context.Categories.Max(c => c.Id) + 1;
      model.Id = newId;

      _context.Add(model);
    }

    public void Delete<T>(T model) where T : class
    {
      _context.Remove(model);
    }
  }
}

