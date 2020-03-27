using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using TinaX.UIKit.Animation;
using UnityEngine;

namespace TinaXEditor.UIKit.Animation
{
    [CustomEditor(typeof(CanvasAlphaAni),true)]
    public class CanvasAlphaAniCustom : UIAnimationBaseCustom
    {
        SerializedProperty _autoOrigin;
        SerializedProperty _fromAlpha;
        SerializedProperty _toAlpha;
        SerializedProperty _ease;

        private bool _refresh_data = false;

        private void _refreshData()
        {

            _autoOrigin = this.serializedObject.FindProperty("AutoOriginValue");
            _fromAlpha = this.serializedObject.FindProperty("FromAlpha");
            _toAlpha = this.serializedObject.FindProperty("ToAlpha");
            _ease = this.serializedObject.FindProperty("Ease");

            _refresh_data = true;
        }

        public override void OnInspectorGUI()
        {
            if (!_refresh_data || _autoOrigin == null)
                _refreshData();

            EditorGUILayout.PropertyField(_autoOrigin, new GUIContent("Auto Origin", "If true, When the animation starts, the current actual value is used as \"From Value\""));
            if (!_autoOrigin.boolValue)
                EditorGUILayout.PropertyField(_fromAlpha);
            EditorGUILayout.PropertyField(_toAlpha);
            EditorGUILayout.PropertyField(_ease);

            base.OnInspectorGUI();
        }
    }
}
