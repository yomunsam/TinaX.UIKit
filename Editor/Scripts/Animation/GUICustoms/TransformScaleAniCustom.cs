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
        SerializedProperty _fromValue;
        SerializedProperty _toValue;
        SerializedProperty _ease;
        protected override void OnEnable()
        {
            base.OnEnable();
            _fromValue = this.serializedObject.FindProperty("FromValue");
            _toValue = this.serializedObject.FindProperty("ToValue");
            _ease = this.serializedObject.FindProperty("Ease");

        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_fromValue);
            EditorGUILayout.PropertyField(_toValue);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Set current value as",GUILayout.MaxWidth(140));
            if (GUILayout.Button("Origin",GUILayout.MaxWidth(50)))
            {
                _fromValue.vector3Value = ((TransformScaleAni)target).transform.localScale;
            }
            if (GUILayout.Button("Target", GUILayout.MaxWidth(50)))
            {
                _toValue.vector3Value = ((TransformScaleAni)target).transform.localScale;
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_ease);

            base.OnInspectorGUI();
        }
    }
}
