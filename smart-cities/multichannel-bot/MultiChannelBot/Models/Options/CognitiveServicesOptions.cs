using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiChannelBot.Models.Options
{
  public class CognitiveServicesOptions
  {
    public ComputerVisionOptions ComputerVision { get; set; }

    public BingVisualSearchApiOptions BingVisualSearch { get; set; }
  }
}
