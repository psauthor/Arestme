using AutoMapper;
using DesigningApis.Data.Entities;
using DesigningApis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesigningApis.Data
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      this.CreateMap<Site, SiteModel>()
        .ForMember(s => s.CategoryName, m => m.MapFrom(c => c.Category.Name))
        .ForMember(s => s.RegionName, m => m.MapFrom(c => c.Region.Name))
        .ReverseMap();

      this.CreateMap<Location, LocationModel>()
        .ReverseMap();

      this.CreateMap<Region, RegionModel>();
    }
  }
}
