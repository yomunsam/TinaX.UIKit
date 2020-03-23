using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using TinaX;
using TinaX.UIKit;

namespace TinaXEditor.UIKit
{
    public static class UIKitUtility
    {
        public static void CreateUIPage(string path, string ui_name = null)
        {
            string prefab_path = null;

            if(ui_name != null)
            {
                prefab_path = Path.Combine(path, ui_name + ".prefab");
            }
            else
            {
                int t = 1;
                string fileName = "UIPage1";
                while (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), path, fileName)))
                {
                    t++;
                    fileName = "UIPage" + t;
                }
                prefab_path = Path.Combine(path, fileName + ".prefab");
                ui_name = fileName;
            }

            var go = new GameObject("UIPage");
            MakeUIPage(ref go);
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefab_path);
            UnityEngine.Object.DestroyImmediate(go);

        }


        public static void MakeUIPage(ref GameObject go)
        {
            go.SetLayerRecursive(5);

            

            var canvas = go.GetComponentOrAdd<Canvas>();
            canvas.overrideSorting = true;
            


            var rect_trans = go.GetComponent<RectTransform>();
            rect_trans.anchorMin = Vector2.zero;
            rect_trans.anchorMax = Vector2.one;
            rect_trans.sizeDelta = Vector2.zero;

            var graphc_raycaster = go.GetComponentOrAdd<UnityEngine.UI.GraphicRaycaster>();
            graphc_raycaster.ignoreReversedGraphics = true;
            graphc_raycaster.blockingObjects = UnityEngine.UI.GraphicRaycaster.BlockingObjects.None;


            var uipage = go.GetComponentOrAdd<UIPage>();
        }

    }
}

