using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.WebRTC;
using UnityEngine;

namespace TwitchStreaming {

    /// <summary>
    /// Initiates a streaming session for a given twitch channel.
    /// </summary>
    public class StreamingSession : MonoBehaviour {

        public bool AutoStartStream = false;

        public string StreamKey;

        /// <summary>
        /// The Camera to stream video from.
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// The (optional) AudioListener to stream audio from.
        /// </summary>
        public AudioListener AudioListener;

        public int Width = 1280;
        public int Height = 720;

        public RTCPeerConnection PeerConnection { get; set; }
        public VideoStreamTrack VideoTrack { get; set; }
        public AudioStreamTrack AudioTrack { get; set; }


        private RTCConfiguration _configuration = new RTCConfiguration() {
            iceServers = new RTCIceServer[] { new RTCIceServer { urls = new string[] { "stun:stun.l.google.com:19302" } } }
        };

        private async void Start() {
            if (AutoStartStream) {
                await StartStreaming();
            }
        }

        public async Task StartStreaming() {
            if (Camera == null) {
                throw new InvalidOperationException("No valid camera found to start streaming from");
            }

            ValidateStreamKey();

            await CreateOfferAsync();
        }

        private async Task CreateOfferAsync() {
            PeerConnection = CreateRTCPeerConnection();

            var offerCreation = PeerConnection.CreateOffer();
            while (!offerCreation.IsDone) {
                await Task.Yield();
            }

            if (offerCreation.IsError) {
                OnRTCError(offerCreation.Error);
                return;
            }
            RTCSessionDescription offer = offerCreation.Desc;

            Debug.Log("Created WebRTC " + offer.type);

            var setOfferOp = PeerConnection.SetLocalDescription(ref offer);
            while (!setOfferOp.IsDone) {
                await Task.Yield();
            }

            if (setOfferOp.IsError) {
                OnRTCError(setOfferOp.Error);
                return;
            }

            RTCSessionDescription answer = await TwitchIngestAPI.SendWebRTCOfferAsync(offer, StreamKey);

            var setAnswerOp = PeerConnection.SetRemoteDescription(ref answer);
            while (!setAnswerOp.IsDone) {
                await Task.Yield();
            }

            if (setAnswerOp.IsError) {
                OnRTCError(setAnswerOp.Error);
                return;
            }

            Debug.Log("WeBRTC handshake successful.");
        }

        private RTCPeerConnection CreateRTCPeerConnection() {
            RTCPeerConnection peerConnection = new RTCPeerConnection(ref _configuration) {
                OnIceCandidate = OnIceCandidate,
                OnIceConnectionChange = OnIceConnectionChange
            };

            MediaStream mediaStream = Camera.CaptureStream(Width, Height);

            VideoTrack = mediaStream.GetVideoTracks().First();
            var v = peerConnection.AddTrack(VideoTrack);

            if (AudioListener != null) {
                AudioTrack = new AudioStreamTrack(AudioListener);
                peerConnection.AddTrack(AudioTrack);
            }

            StartCoroutine(WebRTC.Update());

            return peerConnection;
        }

        private void OnRTCError(RTCError error) {
            Debug.LogError(error.errorType + ": " + error.message.ToString());
        }

        private void OnIceCandidate(RTCIceCandidate candidate) { }

        private void OnIceConnectionChange(RTCIceConnectionState state) { }

        private void OnApplicationPause(bool pause) {
            if (VideoTrack != null) {
                VideoTrack.Enabled = !pause;
            }
            if (AudioTrack != null) {
                AudioTrack.Enabled = !pause;
            }
        }

        private void ValidateStreamKey() {
            if (string.IsNullOrEmpty(StreamKey)) {
                throw new System.ArgumentException($"stream key can not be null or empty", nameof(StreamKey));
            }

            if (!StreamKey.StartsWith("live_")) {
                throw new System.ArgumentException($"Invalid stream key: {StreamKey}", nameof(StreamKey));
            }
        }
    }
}