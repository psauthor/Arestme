using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesigningApis.Data.Entities
{
  public class Site
  {
    public int Id { get;  set; }
    public string Name { get; set; }
    public int YearInscribed { get; set; }
    public string Url { get; set; }
    public string ImageUrl { get; set; }
    public string DescriptionMarkup { get; set; }
    public string States { get; set; }
    public Location Location { get; set; }
    public Category Category { get; set; }
    public Region Region { get; set; }
  }
}
