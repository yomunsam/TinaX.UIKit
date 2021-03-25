using UnityEngine;

namespace TinaX.UIKit
{
    public class OpenUIParam
    {
        public string UIName;
        public bool UseMask = false;
        public bool CloseByMask = false;
        public Color? MaskColor = null;
        public Transform UIRoot;
        public XComponent.XBehaviour xBehaviour = null;
        public bool DependencyInjection = true;
    }
}
