using TinaX.UIKit.Animation;
using UnityEditor;

namespace TinaXEditor.UIKit.Animation
{
    //[CustomEditor(typeof(UIAnimationBase),true)]
    public class UIAnimationBaseCustom : Editor
    {
        //private bool refresh_data = false;
        //SerializedProperty _duration;
        //SerializedProperty _playOnAwake;
        //SerializedProperty _pingPong;
        //SerializedProperty _delay_before;
        //SerializedProperty _title;

        //SerializedProperty _onFinish;


        //public override void OnInspectorGUI()
        //{
        //    if (!refresh_data || _title == null)
        //        refreshData();
        //    EditorGUILayout.Space();
        //    EditorGUILayout.PropertyField(_title, true);
        //    EditorGUILayout.PropertyField(_duration, true);
        //    EditorGUILayout.PropertyField(_playOnAwake, true);
        //    EditorGUILayout.PropertyField(_delay_before, true);
        //    EditorGUILayout.PropertyField(_pingPong, true);
        //    if (_pingPong.boolValue)
        //    {
        //        EditorGUILayout.LabelField("If PingPong, onFinish event will never be called");
        //    }

        //    EditorGUILayout.Space();
        //    EditorGUILayout.PropertyField(_onFinish, true);

        //    this.serializedObject.ApplyModifiedProperties();
        //}

        //private void refreshData()
        //{
        //    _duration = this.serializedObject.FindProperty("Duration");
        //    _playOnAwake = this.serializedObject.FindProperty("playOnAwake");
        //    _pingPong = this.serializedObject.FindProperty("pingPong");
        //    _delay_before = this.serializedObject.FindProperty("DelayBefore");
        //    _title = this.serializedObject.FindProperty("title");
        //    _onFinish = this.serializedObject.FindProperty("onFinish");
        //    refresh_data = true;
        //}
    }
}
