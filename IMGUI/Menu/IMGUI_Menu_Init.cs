#define INCLUDE_IN_RELEASE
#if UNITY_EDITOR || DEVELOPMENT_BUILD || INCLUDE_IN_RELEASE

using UnityEngine;

namespace IMGUI
{
    public static class IMGUI_Menu_Init
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            new GameObject("IMGUI_Menu", typeof(IMGUI_Menu));
        }
    }
}

#endif