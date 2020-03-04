using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiChannelBot.Services.Interfaces
{
  public interface IBingVisualSearchService
  {
    Task<string> RecognizeBarCodeAsync(string imageUrl, string accessToken = null);
  }
}
