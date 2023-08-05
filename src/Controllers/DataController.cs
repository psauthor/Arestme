using DesigningApis.Data;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesigningApis.Controllers
{
  [HttpCacheExpiration(NoStore = true, MaxAge = 1)]
  [ApiController]
  public class DataController : ControllerBase
  {
    private readonly ISiteRepository _repository;
    private readonly ILogger<DataController> _logger;

    public DataController(ISiteRepository repository, ILogger<DataController> logger)
    {
      _repository = repository;
      _logger = logger;
    }

    [HttpOptions("api/data/dumpchanges")]
    public async Task<ActionResult> DumpChanges()
    {
      if (await _repository.ClearDatabaseAsync())
      {
        return Ok(new { success = true });
      }
      else 
      {
        return Ok(new { success = false });
      }
    }

    [HttpGet("api/data/flaky")]
    public ActionResult Flaky()
    {
      var rando = Random.Shared.Next(2);
      _logger.LogInformation($"Flaky Result: {rando}");
      if (rando == 1)
      {
        return Ok("Worked This Time");
      }
      else
      {
        return this.Problem("Failed to run");
      }
    }
  }
}
