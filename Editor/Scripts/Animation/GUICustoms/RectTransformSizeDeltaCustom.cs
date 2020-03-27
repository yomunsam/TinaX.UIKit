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
    [CustomEditor(typeof(RectTransformSizeDeltaAni),true)]
    public class RectTransformSizeDeltaCustom : UIAnimationBaseCustom
    {
        SerializedProperty _aniTarget;
        SerializedProperty _autoOrigin;
        SerializedProperty _autoTarget;

        SerializedProperty _fromValue;
        SerializedProperty _toValue;
        SerializedProperty _ease;

        private bool _refresh_data = false;
        private void _refreshData()
        {
            _aniTarget = this.serializedObject.FindProperty("AniTarget");
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
            EditorGUILayout.PropertyField(_aniTarget, new GUIContent("Animation Target", "The object that this animation acts on, if not specified, it defaults to the current Transform"));

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
                    if (_aniTarget.objectReferenceValue == null)
                        _aniTarget.objectReferenceValue = ((RectTransformSizeDeltaAni)target).GetComponent<RectTransform>();
                    if (_aniTarget.objectReferenceValue != null)
                        _fromValue.vector2Value = ((RectTransform)_aniTarget.objectReferenceValue).sizeDelta;
                }
            }
                
            if (!_autoTarget.boolValue)
            {
                if (GUILayout.Button("Target", GUILayout.MaxWidth(50)))
                {
                    if (_aniTarget.objectReferenceValue == null)
                        _aniTarget.objectReferenceValue = ((RectTransformSizeDeltaAni)target).GetComponent<RectTransform>();
                    if (_aniTarget.objectReferenceValue != null)
                        _toValue.vector2Value = ((RectTransform)_aniTarget.objectReferenceValue).sizeDelta;
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_ease);

            base.OnInspectorGUI();
        }
    }
}
