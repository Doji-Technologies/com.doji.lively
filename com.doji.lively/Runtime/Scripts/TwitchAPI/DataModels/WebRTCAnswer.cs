using System;
using Newtonsoft.Json;

namespace Doji.Lively {

    internal partial class WebRTCAnswer {
        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("ingestSessionId")]
        public Guid IngestSessionId { get; set; }
    }
}
