using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace TwitchStreaming {

    /// <summary>
    /// Initiates a streaming session for a given twitch channel.
    /// </summary>
    public class BasicSample : MonoBehaviour {

        public bool AutoStartStream = false;

        public string StreamKey;

        /// <summary>
        /// The Camera to stream video from.
        /// </summary>
        public Camera Camera;

        public int Width = 1280;
        public int Height = 720;

        private StreamingSession _session;

        private void Start() {
            _session = StreamingSession.Create(StreamKey, Camera, new CameraSettings(Width, Height));

            if (AutoStartStream) {
                _ = _session.StartStreaming();
            }
        }

        private void OnDestroy() {
            _session.Dispose();
        }
    }
}