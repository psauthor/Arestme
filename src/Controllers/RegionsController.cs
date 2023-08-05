using AutoMapper;
using DesigningApis.Data;
using DesigningApis.Models;
using Microsoft.AspNetCore.Cors;
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
  [ApiController]
  [EnableCors("Prevent")]
  public class RegionsController : ControllerBase
  {
    private readonly ISiteRepository _repository;
    private readonly LinkGenerator _linkGenerator;
    private readonly IMapper _mapper;

    public RegionsController(ISiteRepository repository, 
      LinkGenerator linkGenerator,
      IMapper mapper)
    {
      _repository = repository;
      _linkGenerator = linkGenerator; 
      _mapper = mapper;
    }

    public async Task<ActionResult<RegionModel[]>> Get()
    {
      var results = await _repository.GetAllRegionsAsync();
      return _mapper.Map<RegionModel[]>(results);
    }

    [HttpGet("{key}")]
    public async Task<ActionResult<RegionModel>> Get(string key)
    {
      var result = await _repository.GetRegionByKeyAsync(key);
      if (result == null) return NotFound();

      return _mapper.Map<RegionModel>(result);
    }
  }
}
