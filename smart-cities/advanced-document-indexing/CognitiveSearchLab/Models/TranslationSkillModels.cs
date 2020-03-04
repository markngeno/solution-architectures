using System;
using System.Collections.Generic;
using System.Text;

namespace CustomSkills.Models
{
    public class WebApiResponseError
    {
        public string Message { get; set; }
    }

    public class WebApiResponseWarning
    {
        public string Message { get; set; }
    }

    public class WebApiResponseRecord
    {
        public string RecordId { get; set; }

        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        public List<WebApiResponseError> Errors { get; set; } = new List<WebApiResponseError>();

        public List<WebApiResponseWarning> Warnings { get; set; } = new List<WebApiResponseWarning>();
    }

    public class WebApiEnricherResponse
    {
        public List<WebApiResponseRecord> Values { get; set; } = new List<WebApiResponseRecord>();
    }
}
