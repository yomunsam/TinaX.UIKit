using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using TinaX.UIKit.Animation;

namespace TinaXEditor.UIKit.Animation
{
    [CustomEditor(typeof(UIAnimationBase),true)]
    public class UIAnimationBaseCustom : Editor
    {
        SerializedProperty _duration;
        SerializedProperty _playOnAwake;
        SerializedProperty _pingPong;
        SerializedProperty _title;

        SerializedProperty _onFinish;
        protected virtual void OnEnable()
        {
            _duration = this.serializedObject.FindProperty("Duration");
            _playOnAwake = this.serializedObject.FindProperty("playOnAwake");
            _pingPong = this.serializedObject.FindProperty("pingPong");
            _title = this.serializedObject.FindProperty("title");
            _onFinish = this.serializedObject.FindProperty("onFinish");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_title, true);
            EditorGUILayout.PropertyField(_duration, true);
            EditorGUILayout.PropertyField(_playOnAwake, true);
            EditorGUILayout.PropertyField(_pingPong, true);
            if (_pingPong.boolValue)
            {
                EditorGUILayout.LabelField("If PingPong, onFinish event will never be called");
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_onFinish, true);

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
