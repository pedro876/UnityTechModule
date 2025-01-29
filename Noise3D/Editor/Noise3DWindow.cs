using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HLSL
{
    public class Noise3DWindow : ScriptableObjectWindow<Noise3DObject>
    {
        [MenuItem("TechModule/Noise3D", priority = 0)]
        public static void ShowWindow() => GetWindow<Noise3DWindow>("Noise3D");


    }
}


