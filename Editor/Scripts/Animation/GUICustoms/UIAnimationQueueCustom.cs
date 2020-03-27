using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using TinaX.UIKit.Animation;
using UnityEngine;
using UnityEditorInternal;

namespace TinaXEditor.UIKit.Animation
{
    [CustomEditor(typeof(UIAnimationQueue),true)]
    public class UIAnimationQueueCustom : UIAnimationBaseCustom
    {
        SerializedProperty _queues;

        SerializedProperty _playOnAwake;
        SerializedProperty _title;
        SerializedProperty _onFinish;

        ReorderableList _list_queues;
        private bool _refresh_data = false;


        public override void OnInspectorGUI()
        {
            if (!_refresh_data || _queues == null)
                _refreshData();
            EditorGUILayout.Space();
            _list_queues.DoLayoutList();
            if(GUILayout.Button("Open Edit Window"))
            {
                UIAnimationQueueEditWindow.target = (UIAnimationQueue)target;
                UIAnimationQueueEditWindow.OpenUI();
            }

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_title);
            EditorGUILayout.PropertyField(_playOnAwake);

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_onFinish);

            this.serializedObject.ApplyModifiedProperties();
        }

        private void _refreshData()
        {
            _queues = this.serializedObject.FindProperty("Queues");

            _playOnAwake = this.serializedObject.FindProperty("playOnAwake");
            _title = this.serializedObject.FindProperty("title");
            _onFinish = base.serializedObject.FindProperty("onFinish");

            _list_queues = new ReorderableList(this.serializedObject, _queues, true, true, true, true);
            _list_queues.drawElementCallback = (rect, index, isActive, isFocus) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 2;
                SerializedProperty itemData = _queues.GetArrayElementAtIndex(index);
                SerializedProperty itemData_Anis = itemData.FindPropertyRelative("UI_Anis");
                SerializedProperty itemData_Delay = itemData.FindPropertyRelative("DelayAfter");
                if (itemData_Anis.arraySize > 0)
                {
                    for (var i = 0; i < itemData_Anis.arraySize; i++)
                    {
                        var rect_item = rect;
                        rect_item.y += i * (EditorGUIUtility.singleLineHeight + 2);
                        var ani_item = itemData_Anis.GetArrayElementAtIndex(i);
                        var ani_obj = ani_item.FindPropertyRelative("UI_Ani");
                        var ready = ani_item.FindPropertyRelative("ReadyOnQueueStart");

                        var rect_obj = rect_item;
                        rect_obj.width -= 80;
                        ani_obj.objectReferenceValue = EditorGUI.ObjectField(rect_obj, ani_obj.objectReferenceValue, typeof(UIAnimationBase), true);

                        var rect_ready = rect_item;
                        rect_ready.x += rect_obj.width + 4;
                        rect_ready.width = 55;
                        ready.boolValue = EditorGUI.ToggleLeft(rect_ready, new GUIContent("Ready", "If ready, The target of this animation will be set to the origin value at the beginning of the queue"), ready.boolValue);

                        var rect_del = rect_item;
                        rect_del.x += rect_obj.width + 4 + rect_ready.width + 2;
                        rect_del.width = 19;
                        if (GUI.Button(rect_del, new GUIContent("×", "Delete")))
                        {
                            itemData_Anis.DeleteArrayElementAtIndex(i);
                        }
                    }
                }
                //add btn
                var rect_btn_add = rect;
                rect_btn_add.y += (itemData_Anis.arraySize) * (EditorGUIUtility.singleLineHeight + 2);
                rect_btn_add.width = 80;
                if (GUI.Button(rect_btn_add, "Add"))
                {
                    var __index = itemData_Anis.arraySize;
                    itemData_Anis.InsertArrayElementAtIndex(__index);
                    var this_data = itemData_Anis.GetArrayElementAtIndex(__index);
                    this_data.FindPropertyRelative("UI_Ani").objectReferenceValue = null;
                    this_data.FindPropertyRelative("ReadyOnQueueStart").boolValue = false;
                }
                var rect_delay = rect;
                rect_delay.y += (itemData_Anis.arraySize + 1) * (EditorGUIUtility.singleLineHeight + 2);
                itemData_Delay.floatValue = EditorGUI.FloatField(rect_delay, new GUIContent("Delay After:"), itemData_Delay.floatValue);

            };
            _list_queues.elementHeightCallback = (index) =>
            {
                var count = _queues.GetArrayElementAtIndex(index).FindPropertyRelative("UI_Anis").arraySize;
                return (EditorGUIUtility.singleLineHeight + 2) * (count + 2) + 5;
            };
            _list_queues.onAddCallback = (list) =>
            {
                if (list.serializedProperty != null)
                {
                    list.serializedProperty.arraySize++;
                    list.index = list.serializedProperty.arraySize - 1;

                    SerializedProperty itemData = list.serializedProperty.GetArrayElementAtIndex(list.index);
                    SerializedProperty item_anis = itemData.FindPropertyRelative("UI_Anis");
                    SerializedProperty item_delay = itemData.FindPropertyRelative("DelayAfter");
                    item_anis.ClearArray();
                    item_delay.floatValue = 0;
                }
                else
                {
                    ReorderableList.defaultBehaviours.DoAddButton(list);
                }
            };
            _list_queues.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "UI Animation Queue");
            };
            _refresh_data = true;
        }

    }


    public class UIAnimationQueueEditWindow: EditorWindow
    {
        public static UIAnimationQueue target;

        private static UIAnimationQueueEditWindow wnd;

        public static void OpenUI()
        {
            if(wnd == null)
            {
                wnd = GetWindow<UIAnimationQueueEditWindow>();
                wnd.titleContent = new GUIContent("Queue Editor");
            }
            else
            {
                wnd.Show();
                wnd.Focus();
            }
        }


        private bool _refresh_data = false;

        ReorderableList _list_queues;
        SerializedProperty _queues;
        SerializedObject _ser_obj;

        private void _refreshData()
        {
            if (target == null) return;
            _ser_obj = new SerializedObject(target);
            _queues = _ser_obj.FindProperty("Queues");


            _list_queues = new ReorderableList(_ser_obj, _queues, true, true, true, true);
            _list_queues.drawElementCallback = (rect, index, isActive, isFocus) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 2;
                SerializedProperty itemData = _queues.GetArrayElementAtIndex(index);
                SerializedProperty itemData_Anis = itemData.FindPropertyRelative("UI_Anis");
                SerializedProperty itemData_Delay = itemData.FindPropertyRelative("DelayAfter");
                if (itemData_Anis.arraySize > 0)
                {
                    for (var i = 0; i < itemData_Anis.arraySize; i++)
                    {
                        var rect_item = rect;
                        rect_item.y += i * (EditorGUIUtility.singleLineHeight + 2);
                        var ani_item = itemData_Anis.GetArrayElementAtIndex(i);
                        var ani_obj = ani_item.FindPropertyRelative("UI_Ani");
                        var ready = ani_item.FindPropertyRelative("ReadyOnQueueStart");

                        var rect_obj = rect_item;
                        rect_obj.width -= 80;
                        ani_obj.objectReferenceValue = EditorGUI.ObjectField(rect_obj, ani_obj.objectReferenceValue, typeof(UIAnimationBase), true);

                        var rect_ready = rect_item;
                        rect_ready.x += rect_obj.width + 4;
                        rect_ready.width = 55;
                        ready.boolValue = EditorGUI.ToggleLeft(rect_ready, new GUIContent("Ready", "If ready, The target of this animation will be set to the origin value at the beginning of the queue"), ready.boolValue);

                        var rect_del = rect_item;
                        rect_del.x += rect_obj.width + 4 + rect_ready.width + 2;
                        rect_del.width = 19;
                        if (GUI.Button(rect_del, new GUIContent("×", "Delete")))
                        {
                            itemData_Anis.DeleteArrayElementAtIndex(i);
                        }
                    }
                }
                //add btn
                var rect_btn_add = rect;
                rect_btn_add.y += (itemData_Anis.arraySize) * (EditorGUIUtility.singleLineHeight + 2);
                rect_btn_add.width = 80;
                if (GUI.Button(rect_btn_add, "Add"))
                {
                    var __index = itemData_Anis.arraySize;
                    itemData_Anis.InsertArrayElementAtIndex(__index);
                    var this_data = itemData_Anis.GetArrayElementAtIndex(__index);
                    this_data.FindPropertyRelative("UI_Ani").objectReferenceValue = null;
                    this_data.FindPropertyRelative("ReadyOnQueueStart").boolValue = false;
                }
                var rect_delay = rect;
                rect_delay.y += (itemData_Anis.arraySize + 1) * (EditorGUIUtility.singleLineHeight + 2);
                itemData_Delay.floatValue = EditorGUI.FloatField(rect_delay, new GUIContent("Delay After:"), itemData_Delay.floatValue);

            };
            _list_queues.elementHeightCallback = (index) =>
            {
                var count = _queues.GetArrayElementAtIndex(index).FindPropertyRelative("UI_Anis").arraySize;
                return (EditorGUIUtility.singleLineHeight + 2) * (count + 2) + 5;
            };
            _list_queues.onAddCallback = (list) =>
            {
                if (list.serializedProperty != null)
                {
                    list.serializedProperty.arraySize++;
                    list.index = list.serializedProperty.arraySize - 1;

                    SerializedProperty itemData = list.serializedProperty.GetArrayElementAtIndex(list.index);
                    SerializedProperty item_anis = itemData.FindPropertyRelative("UI_Anis");
                    SerializedProperty item_delay = itemData.FindPropertyRelative("DelayAfter");
                    item_anis.ClearArray();
                    item_delay.floatValue = 0;
                }
                else
                {
                    ReorderableList.defaultBehaviours.DoAddButton(list);
                }
            };
            _list_queues.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "UI Animation Queue");
            };
            _refresh_data = true;
        }

        private void OnGUI()
        {
            if(target == null)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Editor target lost, please reopen it.");
                GUILayout.FlexibleSpace();
            }
            else
            {
                if(!_refresh_data || _queues == null || _ser_obj == null || _list_queues == null)
                {
                    _refreshData();
                }

                _list_queues.DoLayoutList();
                _ser_obj.ApplyModifiedProperties();
            }
        }

        private void OnDestroy()
        {
            wnd = null;
            target = null;
        }
    }
}
