using Newtonsoft.Json;

namespace TwitchStreaming {

    internal partial class IngestEndpoints {
        [JsonProperty("ingests")]
        public Ingest[] Ingests { get; set; }
    }

    internal partial class Ingest {
        [JsonProperty("_id")]
        public long Id { get; set; }

        [JsonProperty("availability")]
        public long Availability { get; set; }

        [JsonProperty("default")]
        public bool Default { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url_template")]
        public string UrlTemplate { get; set; }

        [JsonProperty("url_template_secure")]
        public string UrlTemplateSecure { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }
    }
}