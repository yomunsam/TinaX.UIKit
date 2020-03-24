using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public Color DefaultUIMaskColor = Color.black;

        public List<UIFolderItem> UI_Image_Folders;

        [System.Serializable]
        public struct UIFolderItem
        {
            public string Path;
            public bool Atlas;
        }

    }
}
