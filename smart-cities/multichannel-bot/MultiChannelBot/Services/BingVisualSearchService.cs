using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MultiChannelBot.Models;
using MultiChannelBot.Models.Options;
using MultiChannelBot.Services.Interfaces;
using MultiChannelBot.Utils;
using Newtonsoft.Json;

namespace MultiChannelBot.Services
{
  public class BingVisualSearchService : IBingVisualSearchService
  {
    private static readonly HttpClient Client = new HttpClient();
    private readonly string _subscriptionKey;
    private readonly string _serviceUri;


    public BingVisualSearchService(BingVisualSearchApiOptions options)
    {
      _subscriptionKey = options.ApiKey;
      _serviceUri = options.ApiEndpoint;
    }

    public async Task<string> RecognizeBarCodeAsync(string imageUrl, string accessToken = null)
    {
      BingVisualSearchResponse visualSearchResult = null;

      string result = string.Empty;
      try
      {
        var content = new MultipartFormDataContent("--------------------------498758971529224930840173");

        var image = string.IsNullOrWhiteSpace(accessToken) ? await Client.GetByteArrayAsync(imageUrl) : await ImageUtils.GetImageAsync(imageUrl, accessToken);

        content.Add(new ByteArrayContent(image)
        {
          Headers =
          {
            ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
              Name = "image",
              FileName = "image",
            },
            ContentType = new MediaTypeHeaderValue("image/jpeg"),
          },
        });
        content.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

        using (var response = await Client.PostAsync($"{_serviceUri}/visualsearch?mkt=en-us&safeSearch=Strict", content))
        {
          response.EnsureSuccessStatusCode();
          var json = await response.Content.ReadAsStringAsync();
          visualSearchResult = JsonConvert.DeserializeObject<BingVisualSearchResponse>(json);

          var textLines = visualSearchResult.Tags.SelectMany(tag => tag.Actions).Where(action => action.ActionType.Equals("TextRecognition", StringComparison.OrdinalIgnoreCase)).SelectMany(action => action.Data.Regions).SelectMany(region => region.Lines).ToList();
          var sb = new StringBuilder();

          foreach (var line in textLines)
          {
            sb.AppendLine(line.Text);
          }
          result = sb.ToString();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Encountered exception. " + ex.Message);
      }
      
      return result;
    }
  }
}
