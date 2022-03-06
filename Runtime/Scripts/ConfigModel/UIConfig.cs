using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TinaX.UIKit.Internal
{
    public class UIConfig : ScriptableObject
    {
        public bool EnableUIKit = true;
        public UINameMode UINameMode;

        #region If UIGroup
        public UIGroup DefaultUIGroup;
        #endregion

        #region If UIName by Relative Directory
        public string UIRootDirectoryLoadPath;
        #endregion

        public bool UseUICamera;
        public UICameraConfig UICameraConfig;

        public bool AutoCreateEventSystem = true;

        public bool DontDestroyEventSystem = true;

        public Color DefaultUIMaskColor = Color.black;

        #region Canvas Scaler

        public CanvasScaler.ScaleMode UICanvasScalerMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        public float UIScaleFactor = 1;
        public float ReferencePixelsPerUnit = 100;

        public Vector2 ReferenceResolution = new Vector2(1600, 1200);
        public CanvasScaler.ScreenMatchMode ScreenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        public float CanvasScalerMatchWidthOrHeight = 0;

        public CanvasScaler.Unit PhySicalUnit = CanvasScaler.Unit.Points;
        public float FallbackScreenDPI = 96;
        public float DefaultSpriteDPI = 96;
        #endregion

        public List<UIFolderItem> UI_Image_Folders;
        public bool UseLegacySpritePacker = false; //使用旧版图集

        [System.Serializable]
        public struct UIFolderItem
        {
            public string Path;
            public bool Atlas;
        }

    }
}
