using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX;
using TinaXEditor.Utils;
using TinaX.UIKit.Components;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace TinaXEditor.UIKit.Components
{
    [CustomEditor(typeof(XUIBackground))]
    public class UIBackgroundCustom : Editor
    {
        private SerializedProperty m_SP_BgAspectRationDefine;
        public override void OnInspectorGUI()
        {
            if (m_SP_BgAspectRationDefine == null)
                m_SP_BgAspectRationDefine = this.serializedObject.FindProperty("BgAspectRationDefine");

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(I18Ns.BgAspectRation);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_SP_BgAspectRationDefine, GUIContent.none);
            if (GUILayout.Button(I18Ns.Refresh, GUILayout.MaxWidth(60)))
            {
                var bg = (XUIBackground)target;
                var rectTrans = bg.GetComponent<RectTransform>();
                var rect = rectTrans.rect;
                Vector2Int aspectRation = new Vector2Int((int)rect.width, (int)rect.height);
                if(aspectRation.x != aspectRation.y && aspectRation.x != 0 && aspectRation.y != 0)
                {
                    int divisor = GetMaxCommonDivisor(aspectRation.x, aspectRation.y);
                    aspectRation.x /= divisor;
                    aspectRation.y /= divisor;
                }


                m_SP_BgAspectRationDefine.vector2IntValue = aspectRation;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            //base.OnInspectorGUI();
            //if(GUILayout.Button("Edit In Window"))
            //{
            //    InspectorInWindow.ShowInspector(target);
            //}

            this.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 取最大公约数
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        private int GetMaxCommonDivisor(int value1, int value2)
        {
            int getMax(int v1, int v2) => v1 > v2 ? v1 : v2;
            int getMin(int v1, int v2) => v1 < v2 ? v1 : v2;
            if (value1 == value2)
                return value1;
            int max = getMax(value1, value1);
            int min = getMin(value1, value2);

            int a = value1, b = value2;

            while(max % min != 0)
            {
                a = min;
                b = max % min;
                max = getMax(a, b);
                min = getMin(a, b);
            }
            return min;
        }

        static class I18Ns
        {
            public static string BgAspectRation
            {
                get
                {
                    if (EditorGUIUtil.IsCmnHans)
                        return "背景图横纵比";
                    if (EditorGUIUtil.IsJapanese)
                        return "背景図の横縦比";
                    return "Background Aspect Ration";
                }
            }

            public static string Refresh
            {
                get
                {
                    if (EditorGUIUtil.IsCmnHans)
                        return "刷新";
                    if (EditorGUIUtil.IsJapanese)
                        return "更新";
                    return "Refresh";
                }
            }
        }
    }
}
