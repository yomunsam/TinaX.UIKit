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
    [CustomEditor(typeof(RectTransformAnchoredPositionAni),true)]
    public class RectTransformAnchoredPositionAniCustom : UIAnimationBaseCustom
    {
        SerializedProperty _autoOrigin;
        SerializedProperty _autoTarget;
        SerializedProperty _fromValue;
        SerializedProperty _toValue;
        SerializedProperty _ease;

        private bool _refresh_data = false;
        private void _refreshData()
        {
            _autoOrigin = this.serializedObject.FindProperty("AutoOriginValue");
            _autoTarget = this.serializedObject.FindProperty("AutoTargetValue");
            _fromValue = this.serializedObject.FindProperty("FromValue");
            _toValue = this.serializedObject.FindProperty("ToValue");
            _ease = this.serializedObject.FindProperty("Ease");
            _refresh_data = true;
        }

        
        public override void OnInspectorGUI()
        {
            if (!_refresh_data || _fromValue == null)
                _refreshData();

            EditorGUILayout.PropertyField(_autoOrigin, new GUIContent("Auto Origin", "If true, When the animation start, the current actual value is used as \"From Value\""));
            EditorGUILayout.PropertyField(_autoTarget, new GUIContent("Auto Target", "If true, When the animation start, the current actual value is used as \"To Value\""));

            if (!_autoOrigin.boolValue)
            {
                EditorGUILayout.PropertyField(_fromValue);
            }

            if (!_autoTarget.boolValue)
            {
                EditorGUILayout.PropertyField(_toValue);
            }

            if (_autoOrigin.boolValue && _autoTarget.boolValue)
            {
                EditorUtility.DisplayDialog("Error", "You cannot enable both \"Auto Origin\" and \"Auto Target\"", "Okey");
                _autoTarget.boolValue = false;
            }    

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Set current value as",GUILayout.MaxWidth(140));
            if (!_autoOrigin.boolValue)
            {
                if (GUILayout.Button("Origin", GUILayout.MaxWidth(50)))
                {
                    var rectTrans = ((RectTransformAnchoredPositionAni)target).GetComponent<RectTransform>();
                    if (rectTrans != null)
                        _fromValue.vector2Value = rectTrans.anchoredPosition;
                }
            }
                
            if (!_autoTarget.boolValue)
            {
                if (GUILayout.Button("Target", GUILayout.MaxWidth(50)))
                {
                    var rectTrans = ((RectTransformAnchoredPositionAni)target).GetComponent<RectTransform>();
                    if (rectTrans != null)
                        _toValue.vector2Value = rectTrans.anchoredPosition;
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_ease);

            base.OnInspectorGUI();
        }
    }
}
