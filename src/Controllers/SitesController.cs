using AutoMapper;
using DesigningApis.Constraints;
using DesigningApis.Data;
using DesigningApis.Data.Entities;
using DesigningApis.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesigningApis.Controllers
{
  [Route("api/[controller]")]
  [ApiVersion("1.0", Deprecated = true)]
  [ApiVersion("2.0")]
  [ApiController]
  public class SitesController : ControllerBase
  {
    private readonly ISiteRepository _repository;
    private readonly LinkGenerator _linkGenerator;
    private readonly IMapper _mapper;

    public SitesController(ISiteRepository repository, LinkGenerator linkGenerator,
      IMapper mapper)
    {
      _repository = repository;
      _linkGenerator = linkGenerator;
      _mapper = mapper;
    }

    // GET api/values
    [HttpGet]
    public async Task<ActionResult<SiteModel[]>> GetAsync([FromQuery]bool useWrapper = false)
    {
      var results = await _repository.GetAllSitesAsync();
      var count = results.Count();

      if (useWrapper)
      {
        return Ok(new
        {
          count,
          results
        });
      }
      else
      {
        Response.Headers.Add("X-Total-Count", count.ToString());
        return _mapper.Map<SiteModel[]>(results);
      }
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<SiteModel[]>> Get10Async()
    {
      var results = await _repository.GetAllSitesAsync();
      var count = results.Count();

      return Ok(new
      {
        count,
        results
      });
    }

    [HttpGet("{id:int}", Name = "SiteById")]
    public async Task<ActionResult<SiteModel>> GetAsync(int id)
    {
      var site = await _repository.GetSiteAsync(id);
      if (site == null) return NotFound();

      return _mapper.Map<SiteModel>(site);
    }

    [HttpGet]
    public async Task<ActionResult<SiteModel[]>> GetByStateAsync([RequiredFromQuery]string state)
    {
      return _mapper.Map<SiteModel[]>(await _repository.GetSitesByStateAsync(state));
    }

    [HttpGet(Name = "SitePaging")]
    public async Task<ActionResult> GetByStateAsync([RequiredFromQuery]int page, [FromQuery] int pagesize = 10, [FromQuery] bool useHeaders = true)
    {
      var pagedSites = await _repository.GetAllSitesWithPaging(page, pagesize);

      var totalCount = await _repository.GetSiteCountAsync();

      string nextPage;
      if (pagesize != 10) nextPage = _linkGenerator.GetPathByRouteValues("SitePaging", new { page = page + 1, pagesize });
      else nextPage = _linkGenerator.GetPathByRouteValues("SitePaging", new { page = page + 1 });

      string prevPage;
      if (pagesize != 10) prevPage = _linkGenerator.GetPathByRouteValues("SitePaging", new { page = page - 1, pagesize });
      else prevPage = _linkGenerator.GetPathByRouteValues("SitePaging", new { page = page - 1 });

      var results = _mapper.Map<SiteModel[]>(pagedSites);

      if (useHeaders)
      {
        HttpContext.Response.Headers.Add("X-NextPage", nextPage);
        if (page > 1) HttpContext.Response.Headers.Add("X-PrevPage", prevPage);
        HttpContext.Response.Headers.Add("X-TotalCount", totalCount.ToString());

        return Ok(results);
      }
      else
      {
        if (page > 1) return Ok(new { totalCount, nextPage, prevPage, results });
        else return Ok(new { totalCount, nextPage, results });

      }
    }

    [HttpPost]
    public async Task<ActionResult<SiteModel>> PostAsync([FromBody]SiteModel model)
    {

      try
      {
        var newSite = _mapper.Map<Site>(model);

        if (!string.IsNullOrWhiteSpace(model.CategoryName))
        {
          newSite.Category = await _repository.GetCategoryByNameAsync(model.CategoryName);
        }

        if (!string.IsNullOrWhiteSpace(model.RegionName))
        {
          newSite.Region = await _repository.GetRegionByNameAsync(model.RegionName);
        }

        _repository.Add(newSite);
        if (await _repository.SaveAllAsync())
        {
          return CreatedAtRoute("SiteById", new { id = newSite.Id }, _mapper.Map<SiteModel>(newSite));
        }
        else
        {
          return BadRequest();
        }

      }
      catch (Exception ex)
      {
        return BadRequest($"Failed to create new site: {ex}");
      }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SiteModel>> PutAsync([FromBody]SiteModel model, int id)
    {
      var site = await _repository.GetSiteAsync(id);
      if (site == null) return NotFound();

      bool updateCategory = model.CategoryName != site.Category?.Name && !string.IsNullOrWhiteSpace(model.CategoryName);
      bool updateRegion = model.CategoryName != site.Category?.Name && !string.IsNullOrWhiteSpace(model.CategoryName);

      _mapper.Map(model, site);

      if (updateCategory)
      {
        site.Category = await _repository.GetCategoryByNameAsync(model.CategoryName);
      }

      if (updateRegion)
      {
        site.Region = await _repository.GetRegionByNameAsync(model.RegionName);
      }

      if (await _repository.SaveAllAsync())
      {
        return Ok(_mapper.Map<SiteModel>(site));
      }
      else
      {
        return BadRequest("No changes detected");
      }

    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Site>> DeleteAsync(int id)
    {
      var site = await _repository.GetSiteAsync(id);
      if (site == null) return NotFound();

      _repository.Delete(site);

      if (await _repository.SaveAllAsync())
      {
        return Ok();
      }

      return BadRequest();

    }

  }
}
