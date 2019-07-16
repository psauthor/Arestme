using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesigningApis.Data.Entities
{
  public class Location
  {
    public int Id { get; internal set; }
    public string Name { get; internal set; }
    public double Longitude { get; internal set; }
    public double Latitude { get; internal set; }
  }
}
