using System.Collections.Generic;
using System.Threading.Tasks;
using DesigningApis.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DesigningApis.Data
{
  public interface ISiteRepository
  {
    Task<bool> BuildDatabaseAsync();
    Task<bool> ClearDatabaseAsync();

    Task<bool> SaveAllAsync();
    void Add(Site model);
    void Add(Region model);
    void Add(Category model);
    void Delete<T>(T model) where T : class;

    Task<Site[]> GetAllSitesAsync();
    Task<Site> GetSiteAsync(int id);
    Task<Site[]> GetSitesByStateAsync(string state);
    Task<Site[]> GetAllSitesWithPaging(int page, int pagesize);
    Task<int> GetSiteCountAsync();
    Task<Site[]> GetSitesByRegionAsync(string key);

    Task<Category> GetCategoryByNameAsync(string name);

    Task<Region> GetRegionByNameAsync(string regionName);
    Task<Region> GetRegionByKeyAsync(string key);
    Task<Region[]> GetAllRegionsAsync();
  }
}