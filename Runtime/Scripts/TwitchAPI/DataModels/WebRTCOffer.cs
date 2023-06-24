using Newtonsoft.Json;

namespace TwitchStreaming {

    internal partial class WebRTCOffer {
        [JsonProperty("offer")]
        public string Offer { get; set; }

        [JsonProperty("streamKey")]
        public string StreamKey { get; set; }

        [JsonProperty("maxResolution")]
        public MaxResolution MaxResolution { get; set; }

        [JsonProperty("maxFramerate")]
        public long MaxFramerate { get; set; }

        [JsonProperty("maxBitrate")]
        public long MaxBitrate { get; set; }
    }

    internal partial class MaxResolution {
        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }
    }
}
