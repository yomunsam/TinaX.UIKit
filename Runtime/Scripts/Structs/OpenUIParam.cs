using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TinaX;

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
