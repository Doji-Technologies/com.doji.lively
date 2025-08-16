using Newtonsoft.Json;

namespace Doji.Lively {

    // TODO: Code stripping issues with Android/IL2CPP builds.
    // Need to set Managed Stripping Level to Minimal for now
    // t.b.d. either add UnityEngine.Scripting.Preserve attribute to classes & constructors
    // or use link.xml in package
    internal partial class IngestEndpoints {
        public IngestEndpoints() { }

        [JsonProperty("ingests")]
        public Ingest[] Ingests { get; set; }
    }

    internal partial class Ingest {
        public Ingest() { }

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