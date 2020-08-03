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

namespace TinaXEditor.UIKit.Components
{
    [CustomEditor(typeof(XHover))]
    public class HoverCustom : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Edit In Window"))
            {
                InspectorInWindow.ShowInspector(target);
            }
        }
    }
}
