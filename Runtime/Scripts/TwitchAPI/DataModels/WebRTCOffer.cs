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

        public override string ToString() {
            byte[] data = System.Convert.FromBase64String(Offer);
            string offerJson = System.Text.Encoding.UTF8.GetString(data);

            var definition = new { type = "", sdp = "" };
            var offer = JsonConvert.DeserializeAnonymousType(offerJson, definition);
            return $"Type: {offer.type}\nSdp: {offer.sdp}";
        }
    }

    internal partial class MaxResolution {
        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }
    }
}
