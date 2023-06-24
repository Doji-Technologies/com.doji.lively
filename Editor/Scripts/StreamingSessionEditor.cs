using UnityEditor;
using UnityEngine;

namespace TwitchStreaming.Editor {

    [CustomEditor(typeof(StreamingSession))]
    public class StreamingSessionEditor : UnityEditor.Editor {

        private SerializedProperty _widthProp;
        private SerializedProperty _heightProp;

        void OnEnable() {
            _widthProp = serializedObject.FindProperty("Width");
            _heightProp = serializedObject.FindProperty("Height");
        }

        public override void OnInspectorGUI() {
            base.DrawDefaultInspector();
            int width = _widthProp.intValue;
            int height = _heightProp.intValue;

            if (width % 16 != 0 || height % 16 != 0) {
                EditorGUILayout.HelpBox("Width/Height must be 16-pixel aligned", MessageType.Warning);
            }
        }
    }
}