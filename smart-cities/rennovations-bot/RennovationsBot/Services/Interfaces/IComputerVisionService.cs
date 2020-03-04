using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RennovationsBot.Models;

namespace RennovationsBot.Services.Interfaces
{
    public interface IComputerVisionService
    {
        Task<ComputerVisionDetectResult> Detect(string imageUrl);

        Task<bool> TextHasAnyChemicalName(string imageUrl);
    }
}
