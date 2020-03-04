using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiChannelBot.Models;

namespace MultiChannelBot.Services.Interfaces
{
  public interface IComputerVisionService
  {
    Task<ComputerVisionDetectResult> Detect(string imageUrl, string accesToken);
  }
}
