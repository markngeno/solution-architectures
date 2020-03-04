using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MultiChannelBot.Models
{
    public class FacebookCustomModel
    {
        [JsonProperty("attachment")]
        public FacebookAttachment Attachment { get; set; }
    }

    public class FacebookAttachment
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("payload")]
        public FacebookPayload Payload { get; set; }
    }

    public class FacebookPayload
    {
        [JsonProperty("template_type")]
        public string TemplateType { get; set; }

        [JsonProperty("elements")]
        public List<FacebookElements> Elements { get; set; }
    }

    public class FacebookElements
    {
        [JsonProperty("media_type")]
        public string MediaType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("buttons")]
        public List<FacebookButton> Buttons { get; set; }
    }

    public class FacebookButton
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
