using AutoMapper;
using DesigningApis.Data;
using DesigningApis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesigningApis.Controllers
{
  [Route("api/regions/{key}/sites")]
  [ApiController]
  public class RegionSitesController : ControllerBase
    {
    private readonly ISiteRepository _repository;
    private readonly LinkGenerator _linkGenerator;
    private readonly IMapper _mapper;

    public RegionSitesController(ISiteRepository repository,
      LinkGenerator linkGenerator,
      IMapper mapper)
    {
      _repository = repository;
      _linkGenerator = linkGenerator;
      _mapper = mapper;
    }

    public async Task<SiteModel[]> Get(string key)
    {
      var results = await _repository.GetSitesByRegionAsync(key);
      return _mapper.Map<SiteModel[]>(results);
    }

  }
}
