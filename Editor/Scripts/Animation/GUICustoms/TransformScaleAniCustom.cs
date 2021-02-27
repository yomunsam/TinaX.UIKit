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
    [CustomEditor(typeof(TransformScaleAni),true)]
    public class TransformScaleAniCustom : UIAnimationBaseCustom
    {
        //SerializedProperty _aniTarget;
        //SerializedProperty _autoOrigin;
        //SerializedProperty _autoTarget;

        //SerializedProperty _fromValue;
        //SerializedProperty _toValue;
        //SerializedProperty _ease;

        //private bool _refresh_data = false;
        //private void _refreshData()
        //{
        //    _aniTarget = this.serializedObject.FindProperty("AniTarget");
        //    _autoOrigin = this.serializedObject.FindProperty("AutoOriginValue");
        //    _autoTarget = this.serializedObject.FindProperty("AutoTargetValue");

        //    _fromValue = this.serializedObject.FindProperty("FromValue");
        //    _toValue = this.serializedObject.FindProperty("ToValue");
        //    _ease = this.serializedObject.FindProperty("Ease");
        //    _refresh_data = true;
        //}

        //private TransformScaleAni _target;
        
        //public override void OnInspectorGUI()
        //{
        //    if (!_refresh_data || _fromValue == null)
        //        _refreshData();
        //    if (_target == null) _target = (TransformScaleAni)target;

        //    if (!_refresh_data || _fromValue == null)
        //        _refreshData();
        //    EditorGUILayout.PropertyField(_aniTarget, new GUIContent("Animation Target", "The object that this animation acts on, if not specified, it defaults to the current Transform"));

        //    EditorGUILayout.PropertyField(_autoOrigin, new GUIContent("Auto Origin", "If true, When the animation start, the current actual value is used as \"From Value\""));
        //    EditorGUILayout.PropertyField(_autoTarget, new GUIContent("Auto Target", "If true, When the animation start, the current actual value is used as \"To Value\""));
        //    if (!_target.AutoOriginValue)
        //        EditorGUILayout.PropertyField(_fromValue);
        //    if (!_target.AutoTargetValue)
        //        EditorGUILayout.PropertyField(_toValue);

        //    if (_autoOrigin.boolValue && _autoTarget.boolValue)
        //    {
        //        EditorUtility.DisplayDialog("Error", "You cannot enable both \"Auto Origin\" and \"Auto Target\"", "Okey");
        //        _autoTarget.boolValue = false;
        //    }

        //    EditorGUILayout.BeginHorizontal();
        //    EditorGUILayout.LabelField("Set current value as",GUILayout.MaxWidth(140));
        //    if (GUILayout.Button("Origin",GUILayout.MaxWidth(50)))
        //    {
        //        if (_target.AniTarget == null)
        //        {
        //            _target.AniTarget = _target.transform;
        //            this.serializedObject.Update();
        //        }
        //        _fromValue.vector3Value = _target.AniTarget.localScale;
        //    }
        //    if (GUILayout.Button("Target", GUILayout.MaxWidth(50)))
        //    {
        //        if (_target.AniTarget == null)
        //        {
        //            _target.AniTarget = _target.transform;
        //            this.serializedObject.Update();
        //        }
        //        _toValue.vector3Value = _target.AniTarget.localScale;
        //    }
        //    EditorGUILayout.EndHorizontal();
        //    GUILayout.Space(5);
        //    EditorGUILayout.PropertyField(_ease);

        //    base.OnInspectorGUI();
        //}
    }
}
