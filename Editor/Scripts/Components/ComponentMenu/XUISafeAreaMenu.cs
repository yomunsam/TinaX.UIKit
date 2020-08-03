using TinaX;
using TinaX.UIKit.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TinaXEditor.UIKit.Components
{
    static class XUISafeAreaMenu
    {
        [MenuItem("GameObject/UI/TinaX/xUISafeArea", false, 3)]
        [MenuItem("GameObject/TinaX/UIKit/xUISafeArea", false, 3)]
        static void AddUISafeArea()
        {
            if (Selection.activeTransform == null) 
            {
                Debug.LogError("[TinaX.UIKit]" + 
                    (TinaXEditor.Utils.EditorGUIUtil.IsCmnHans ?
                    "添加UI对象“xUISafeArea”失败：您必须选中目标GameObject之后添加UI对象。" :
                    "Failed to add UI object \"xUISafeArea\": you must add UI object after selecting target GameObject."));
                return;
            }

            if (!Selection.activeTransform.GetComponent<Canvas>())
            {
                Debug.LogError("[TinaX.UIKit]" +
                    (TinaXEditor.Utils.EditorGUIUtil.IsCmnHans ?
                    "添加UI对象“xUISafeArea”失败：该UI元素必须添加到UGUI Canvas的子项中。" :
                    "Failed to add UI object \"xUISafeArea\": The UI element must be added to a child of UGUI Canvas."));
                return;
            }

            var go_name = "xUISafeArea";
            if (Selection.activeTransform.Find(go_name) != null)
            {
                var index = 1;

                while (Selection.activeTransform.Find("xUISafeArea" + index.ToString()) != null)
                {
                    index++;
                }
                go_name = "xUISafeArea" + index.ToString();
            }

            var go = Selection.activeGameObject
                        .CreateGameObject(go_name, typeof(XUISafeArea));

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
