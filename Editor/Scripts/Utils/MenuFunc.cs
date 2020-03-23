using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX;
using System.IO;
using TinaXEditor.UIKit;

namespace TinaXEditor.UIKit.Internal
{
    public class MenuFunc
    {
        [MenuItem("Assets/Create/TinaX/UIKit/UI Page",priority = 10)]
        static void CreateUI()
        {
            string path = "Assets/";
            foreach(var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if(!path.IsNullOrEmpty() && File.Exists(path) && AssetDatabase.IsValidFolder(path))
                {
                    break;
                }
            }
            UIKitUtility.CreateUIPage(path);
        }
    }
}

