using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.WebRTC;
using UnityEngine;

namespace Doji.Lively {

    public struct CameraSettings {

        public int Width;
        public int Height;

        public CameraSettings(int width = 1280, int height = 720) {
            if (width % 16 != 0 || height % 16 != 0) {
                throw new ArgumentException($"Width/Height must be 16-pixel aligned. Was {width}x{height}.");
            }
            Width = width;
            Height = height;
        }
    }

    /// <summary>
    /// Streams the output of a <see cref="UnityEngine.Camera"/> to a twitch channel.
    /// </summary>
    public class CameraStreamingSession : StreamingSession {

        /// <summary>
        /// The Camera to stream video from.
        /// </summary>
        public Camera Camera { get; private set; }

        public CameraSettings Settings { get; private set; }

        public CameraStreamingSession(string streamKey, Camera camera, CameraSettings settings) : base(streamKey) {
            Camera = camera;
            Settings = settings;
            MediaStream mediaStream = Camera.CaptureStream(Settings.Width, Settings.Height);
            VideoTrack = mediaStream.GetVideoTracks().First();
            if (Camera.TryGetComponent(out AudioListener audioListener)) {
                AudioTrack = new AudioStreamTrack(audioListener);
            }
        }
    }

    /// <summary>
    /// Streams the contents of a <see cref="Texture"/> to a twitch channel.
    /// </summary>
    public class TextureStreamingSession : StreamingSession {

        public TextureStreamingSession(string streamKey, Texture texture) : base(streamKey) {
            var track = new VideoStreamTrack(texture, Graphics.Blit);
        }
    }

    public abstract class StreamingSession : IDisposable {

        private static Coroutine _updateCoroutine;

        public StreamingSession(string streamKey) {
            _updateCoroutine ??= UnityHelper.Instance.StartCoroutine(WebRTC.Update());
            UnityHelper.OnApplicationPauseEvent += OnApplicationPause;
            StreamKey = streamKey;
            ValidateStreamKey();
        }

        public static StreamingSession Create(string streamKey, Camera camera, CameraSettings settings) {
            if (camera == null) {
                throw new ArgumentNullException("Camera for streaming can not be null", nameof(camera));
            }
            return new CameraStreamingSession(streamKey, camera, settings);
        }

        public static StreamingSession Create(string streamKey, Texture texture) {
            if (texture == null) {
                throw new ArgumentNullException("Texture for streaming can not be null", nameof(texture));
            }
            return new TextureStreamingSession(streamKey, texture);
        }

        public string StreamKey { get; private set; }

        /// <summary>
        /// Holds the WebRTC connection to a Twitch Ingestion server.
        /// </summary>
        public RTCPeerConnection PeerConnection { get; private set; }

        /// <summary>
        /// The VideoTrack to send.
        /// </summary>
        public VideoStreamTrack VideoTrack { get; protected set; }

        /// <summary>
        /// Optional AudioTrack
        /// </summary>
        public AudioStreamTrack AudioTrack { get; protected set; }

        private RTCConfiguration _configuration = new RTCConfiguration() {
            iceServers = new RTCIceServer[] { new RTCIceServer { urls = new string[] { "stun:stun.l.google.com:19302" } } }
        };

        public async Task StartStreaming() {
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

            var v = peerConnection.AddTrack(VideoTrack);

            if (AudioTrack != null) {
                peerConnection.AddTrack(AudioTrack);
            }

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
                throw new ArgumentException($"stream key can not be null or empty", nameof(StreamKey));
            }

            if (!StreamKey.StartsWith("live_")) {
                throw new ArgumentException($"Invalid stream key: {StreamKey}", nameof(StreamKey));
            }
        }

        public void Dispose() {
            UnityHelper.OnApplicationPauseEvent -= OnApplicationPause;
            PeerConnection.Dispose();
        }
    }
}