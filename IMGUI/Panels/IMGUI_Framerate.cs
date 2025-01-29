using Architecture;
using UnityEngine;

namespace IMGUI
{
    public class IMGUI_Framerate : MonoBehaviour
    {

        public static IMGUI_Framerate Instance { get; private set; }
        public static PersistentBool ShouldDisplay = new PersistentBool("IMGUI_Framerate_ShouldDisplay", false);


        const float deltaTimeLerp = 0.1f;
        private float avgDeltaTime = 0f;

        private void OnEnable()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDisable()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            avgDeltaTime = Mathf.Lerp(avgDeltaTime, Time.deltaTime, deltaTimeLerp);
        }

        private void OnGUI()
        {
            if (!ShouldDisplay.Value) return;

            IMGUI_Layout.InsideBox(0.2f, 0.1f, IMGUI_Layout.Anchor.BottomLeft, (rect) =>
            {
                float avgFramerate = 1f / avgDeltaTime;
                float framerate = 1f / Time.deltaTime;
                IMGUI_Layout.Label(ref rect, $"Fps (avg): {avgFramerate}");
                IMGUI_Layout.Label(ref rect, $"Fps: {framerate}");
            });
        }
    }
}
