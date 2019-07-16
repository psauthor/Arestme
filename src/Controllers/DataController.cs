using DesigningApis.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesigningApis.Controllers
{
  [ApiController]
  public class DataController : ControllerBase
  {
    private readonly ISiteRepository _repository;

    public DataController(ISiteRepository repository)
    {
      _repository = repository;
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
  }
}
