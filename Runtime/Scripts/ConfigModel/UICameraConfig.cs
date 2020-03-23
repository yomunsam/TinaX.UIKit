using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TinaX.UIKit
{
    [CreateAssetMenu(fileName ="UICameraConfig", menuName = "TinaX/UIKit/UI Camera Config",order = 122)]
    public class UICameraConfig : ScriptableObject
    {

        public CameraClearFlags clearFlags = CameraClearFlags.Depth;
        public Color backgroundColor = new Color(49 / 255, 77 / 255, 121 / 255 , 1);
        public int cullingMask = 1 << 5;

        public bool orthographic = true;
        public float orthographicSize = 5;

        public float nearClipPlane = 0.3f;
        public float farClipPlane = 1000f;

        public float depth = 999;

        public RenderingPath renderingPath = RenderingPath.UsePlayerSettings;
        public RenderTexture targetTexture;
        public bool useOcclusionCulling = true;
        public bool allowHDR = false;
        public bool allowMSAA = false;

    }
}
