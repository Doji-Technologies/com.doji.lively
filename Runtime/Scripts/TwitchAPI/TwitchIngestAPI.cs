using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Unity.WebRTC;
using UnityEngine;
using static TwitchStreaming.WebRequestAsyncUtility;

namespace TwitchStreaming {

    internal static class TwitchIngestAPI {

        public delegate void OnResult<T>(T result);

        private static readonly string TWITCH_INGEST_ENDPOINTS_URL = "https://ingest.twitch.tv/ingests";

        public static async Task<RTCSessionDescription> SendWebRTCOfferAsync(RTCSessionDescription webRTCOffer, string streamKey) {
            IngestEndpoints ingestEndpoints = await GetEndpointsAsync();

            if (ingestEndpoints == null) {
                return default;
            }
            if (ingestEndpoints.Ingests == null) {
                return default;
            }
            if (ingestEndpoints.Ingests.Length == 0) {
                return default;
            }

            string offerUrl = ingestEndpoints.Ingests[0].UrlTemplate
                .Replace("rtmp://", "https://")
                .Replace("contribute", "webrtc")
                .Replace("/app/", ":4443/offer")
                .Replace("{stream_key}", "");

            string offerJson = JsonConvert.SerializeObject(webRTCOffer, new Newtonsoft.Json.Converters.StringEnumConverter() { NamingStrategy = new LowerCaseNamingStrategy() });
            string base64Offer = Convert.ToBase64String(Encoding.UTF8.GetBytes(offerJson));

            var twitchOffer = new WebRTCOffer() {
                Offer = base64Offer,
                StreamKey = streamKey,
                MaxResolution = new MaxResolution() { Width = 1920, Height = 1080 },
                MaxFramerate = 30,
                MaxBitrate = 8500
            };

            string postData = JsonConvert.SerializeObject(twitchOffer);
            Debug.Log(postData);
            WebRTCAnswer answer = await SendOfferAsync(offerUrl, postData);
            string answerJson = Encoding.UTF8.GetString(Convert.FromBase64String(answer.Answer));
            RTCSessionDescription result = JsonConvert.DeserializeObject<RTCSessionDescription>(answerJson);

            Debug.Log("Answer");
            Debug.Log(result.sdp);
            return result;
        }

        private static async Task<IngestEndpoints> GetEndpointsAsync() {
            return await HttpRequestAsync<IngestEndpoints>(TWITCH_INGEST_ENDPOINTS_URL, HTTPVerb.GET);
        }

        private static async Task<WebRTCAnswer> SendOfferAsync(string url, string postData) {
            return await HttpRequestAsync<WebRTCAnswer>(url, HTTPVerb.POST, postData);
        }

        /*private async Task<UnityWebRequest> GetEndpointsAsync() {
            UnityWebRequest wr = null;
            try {
                wr = UnityWebRequest.Get(TWITCH_INGEST_ENDPOINTS_URL);

                var asyncOp = wr.SendWebRequest();
                while (!asyncOp.webRequest.isDone) {
                    await Task.Yield();
                }
                return asyncOp.webRequest;

            } catch (Exception e) {
                Debug.LogError(e);
                wr.Dispose();
            }
            return null;
        }*/

        private static async Task<T> HttpRequestAsync<T>(string url, HTTPVerb verb, string postData = null, params Tuple<string, string>[] requestHeaders) {
            T result = await WebRequestAsync<T>.SendWebRequestAsync(url, verb, postData, requestHeaders);
            return result;
        }
    }
}