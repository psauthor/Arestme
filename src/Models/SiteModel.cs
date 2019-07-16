using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesigningApis.Models
{
  public class SiteModel
  {
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public int YearInscribed { get; set; }
    public string Url { get; set; }
    public string ImageUrl { get; set; }
    public string DescriptionMarkup { get; set; }
    public string States { get; set; }
    [Required]
    public LocationModel Location { get; set; }
    public string CategoryName { get; set; }
    public string RegionName { get; set; }

  }
}
