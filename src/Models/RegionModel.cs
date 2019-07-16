using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesigningApis.Models
{
  public class RegionModel
  {
    public string Name { get; set; }
    public string Key
    {
      get => string.Join("-", Name.ToLower().Split(" "));
    }

  }
}
