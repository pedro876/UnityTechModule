#define INCLUDE_IN_RELEASE
#if UNITY_EDITOR || DEVELOPMENT_BUILD || INCLUDE_IN_RELEASE

using UnityEngine;

namespace IMGUI
{
    public static class IMGUI_Framerate_Init
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            new GameObject("IMGUI_Framerate", typeof(IMGUI_Framerate));
        }
    }
}

#endif