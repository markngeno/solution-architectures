using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RennovationsBot.Models
{
    public class ComputerVisionDetectResult
    {
        public bool ThereIsAFridge { get; set; }

        public bool ThereIsADog { get; set; }

        public bool ThereIsAChemical { get; set; }

        public bool ThereIsALampPost { get; set; }

        public string Description { get; set; }
    }
}
