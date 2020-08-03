using TinaX;
using TinaX.UIKit.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TinaXEditor.UIKit.Components
{
    static class XUIBackgroundMenu
    {
        [MenuItem("GameObject/UI/TinaX/xUIBackground", false, 2)]
        [MenuItem("GameObject/TinaX/UIKit/xUIBackground", false, 2)]
        static void AddUIBackground()
        {
            if (Selection.activeTransform == null) 
            {
                Debug.LogError("[TinaX.UIKit]" + 
                    (TinaXEditor.Utils.EditorGUIUtil.IsCmnHans ? 
                    "添加UI对象“xUIBackground”失败：您必须选中目标GameObject之后添加UI对象。" :
                    "Failed to add UI object \"xUIBackground\": you must add UI object after selecting target GameObject."));
                return;
            }

            if (!Selection.activeTransform.GetComponent<Canvas>())
            {
                Debug.LogError("[TinaX.UIKit]" +
                    (TinaXEditor.Utils.EditorGUIUtil.IsCmnHans ?
                    "添加UI对象“xUIBackground”失败：该UI元素必须添加到UGUI Canvas的子项中。" :
                    "Failed to add UI object \"xUIBackground\": The UI element must be added to a child of UGUI Canvas."));
                return;
            }

            var go_name = "xUIBackground";
            if (Selection.activeTransform.Find(go_name) != null)
            {
                var index = 1;

                while (Selection.activeTransform.Find("xUIBackground" + index.ToString()) != null)
                {
                    index++;
                }
                go_name = "xUIBackground" + index.ToString();
            }

            var go = Selection.activeGameObject
                        .CreateGameObject(go_name, typeof(Image), typeof(XUIBackground));

            var rectTrans = go.GetComponent<RectTransform>();
            rectTrans.anchoredPosition = Vector2.zero;
            rectTrans.localScale = Vector3.one;

            #region 全屏设置
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.sizeDelta = Vector2.zero;
            #endregion

            Selection.activeTransform = go.transform;
        }
    }
}
