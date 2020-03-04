using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MultiChannelBot.Utils
{
  public static class ImageUtils
  {
    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "Avoiding Improper Instantiation antipattern : https://docs.microsoft.com/en-us/azure/architecture/antipatterns/improper-instantiation/")]
    private static readonly HttpClient Client = new HttpClient(new HttpClientHandler { UseCookies = true });

    public static async Task<byte[]> GetImageAsync(string imageUrl, string accessToken)
    {
      var uri = new Uri(imageUrl);
      var request = new HttpRequestMessage(HttpMethod.Get, uri);

      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
      request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));

      var responseMessage = await Client.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);

      return await responseMessage.Content.ReadAsByteArrayAsync();
    }
  }
}
