using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RennovationsBot.Options
{
    public class ComputerVisionOptions
    {
        public string ApiEndpoint { get; set; }

        public string ApiKey { get; set; }

        public string DisposableCategories { get; set; }

        public string NotDisposableCategories { get; set; }

        public string ChemicalNames { get; set; }

        public string LampPostNames { get; set; }
    }
}
