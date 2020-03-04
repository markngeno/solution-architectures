using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiChannelBot.Dialogs
{
    public class LampPostReportState
    {
        public string Name { get; set; }

        public string Barcode { get; set; }

        public string Location { get; set; }

        public DateTime IssueDate { get; set; }
    }
}
