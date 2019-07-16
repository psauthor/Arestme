using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DesigningApis.Services
{
  // Adapted from https://github.com/Microsoft/aspnet-api-versioning/issues/42
  public class AcceptHeaderApiVersionReader : IApiVersionReader
  {
    // looking for application/vnd.wilderminds.arest-v2+json
    private const string Pattern = @".wilderminds.arest-v(\d+(\.\d+)?)\+\S+$";

    public void AddParameters(IApiVersionParameterDescriptionContext context)
    {
    }

    public string Read(HttpRequest request)
    {
      var mediaType = request.Headers["Accept"].Single();
      if (Regex.IsMatch(mediaType, Pattern, RegexOptions.RightToLeft))
      {
        var match = Regex.Match(mediaType, Pattern, RegexOptions.RightToLeft);
        return match.Success ? match.Groups[1].Value : null;
      }

      return null;
    }
  }
}
