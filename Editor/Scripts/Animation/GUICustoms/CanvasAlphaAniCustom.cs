using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using TinaX.UIKit.Animation;

namespace TinaXEditor.UIKit.Animation
{
    [CustomEditor(typeof(CanvasAlphaAni),true)]
    public class CanvasAlphaAniCustom : UIAnimationBaseCustom
    {
        SerializedProperty _fromAlpha;
        SerializedProperty _toAlpha;
        SerializedProperty _ease;
        protected override void OnEnable()
        {
            base.OnEnable();
            _fromAlpha = this.serializedObject.FindProperty("FromAlpha");
            _toAlpha = this.serializedObject.FindProperty("ToAlpha");
            _ease = this.serializedObject.FindProperty("Ease");

        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_fromAlpha);
            EditorGUILayout.PropertyField(_toAlpha);
            EditorGUILayout.PropertyField(_ease);

            base.OnInspectorGUI();
        }
    }
}
