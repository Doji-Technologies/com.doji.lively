using System;
using UnityEngine;

namespace Doji.Lively.Samples {

    /// <summary>
    /// Initiates a streaming session for a given twitch channel using a Texture.
    /// </summary>
    public class TextureSample : MonoBehaviour {

        public bool AutoStartStream = false;

        public string StreamKey;

        /// <summary>
        /// The Camera to stream video from.
        /// </summary>
        public RenderTexture RT;

        public int Width = 1280;
        public int Height = 720;

        private StreamingSession _session;

        private async void Start() {
            _session = StreamingSession.Create(StreamKey, RT);

            if (AutoStartStream) {
                try {
                    await _session.StartStreaming();
                } catch (Exception ex) {
                    Debug.LogException(ex);
                }
            }
        }

        private void OnDestroy() {
            _session?.Dispose();
        }
    }
}