using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using MultiChannelBot.Models;
using MultiChannelBot.Models.Options;
using MultiChannelBot.Services.Interfaces;
using MultiChannelBot.Utils;

namespace MultiChannelBot.Services
{
  public class ComputerVisionService : IComputerVisionService
  {
    private readonly ComputerVisionClient _computerVisionApi;
    private readonly List<string> _lampPostNames;

    public ComputerVisionService(ComputerVisionOptions computerVisionOptions)
    {
      _computerVisionApi = new ComputerVisionClient(
              new ApiKeyServiceClientCredentials(computerVisionOptions.ApiKey),
              new System.Net.Http.DelegatingHandler[] { });
      _computerVisionApi.Endpoint = computerVisionOptions.ApiEndpoint;
      _lampPostNames = computerVisionOptions.LampPostNames.ToLowerInvariant().Split(',').ToList();
    }

    public async Task<ComputerVisionDetectResult> Detect(string imageUrl, string accessToken)
    {
      var image = await ImageUtils.GetImageAsync(imageUrl, accessToken);
      var results = await _computerVisionApi.AnalyzeImageInStreamAsync(
          new MemoryStream(image),
          new[] { VisualFeatureTypes.Tags, VisualFeatureTypes.Description, VisualFeatureTypes.Categories });

      return new ComputerVisionDetectResult
      {
        ThereIsALampPost = ContainsObjectOfLampPostReferences(results),
        Description = results.Description.Captions.FirstOrDefault()?.Text,
      };
    }

    private bool ContainsObjectOfLampPostReferences(ImageAnalysis results)
    {
      return _lampPostNames.Any(name => results.Tags.Any(tag => string.Equals(tag.Name, name, StringComparison.InvariantCultureIgnoreCase)))
              || _lampPostNames.Any(name => results.Categories.Any(category => string.Equals(category.Name, name, StringComparison.InvariantCultureIgnoreCase))); ;
    }
  }
}
