using UnityEngine;

namespace Doji.Lively {

    /// <summary>
    /// A helper object that manages various Unity-specific tasks (coroutines, lifecycle events, etc.).
    /// </summary>
    public class UnityHelper : MonoBehaviour {

        public static event System.Action<bool> OnApplicationPauseEvent;
        public static event System.Action OnApplicationQuitEvent;

        public static UnityHelper Instance {
            get {
                if (_instance == null) {
                    GameObject coroutineRunner = new GameObject("com.doji.lively_CoroutineRunner");
                    _instance = coroutineRunner.AddComponent<UnityHelper>();
                    coroutineRunner.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                    DontDestroyOnLoad(coroutineRunner);
                }
                return _instance;
            }
        }
        private static UnityHelper _instance;

        private void OnApplicationPause(bool pauseStatus) {
            OnApplicationPauseEvent?.Invoke(pauseStatus);
        }

        private void OnApplicationQuit() {
            OnApplicationQuitEvent?.Invoke();
        }
    }
}