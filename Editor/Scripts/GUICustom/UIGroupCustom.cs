using System.IO;
using TinaX;
using TinaX.UIKit;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace TinaXEditor.UIKit.Internal
{
    [CustomEditor(typeof(UIGroup))]
    public class UIGroupCustom : Editor
    {
        private ReorderableList mReorderableList_Group;
        private Texture mImg_Folder;

        public override void OnInspectorGUI()
        {
            if(mImg_Folder == null)
            {
                mImg_Folder = AssetDatabase.LoadAssetAtPath<Texture>("Packages/io.nekonya.tinax.uikit/Editor/Res/icons/folder.png");
            }
            if(mReorderableList_Group == null)
            {
                mReorderableList_Group = new ReorderableList(this.serializedObject,
                    this.serializedObject.FindProperty("Items"),
                    true,
                    false,
                    true,
                    true);
                mReorderableList_Group.drawHeaderCallback = rect =>
                {
                    rect.height = EditorGUIUtility.singleLineHeight;

                    var rect_uiName = rect;
                    rect_uiName.width = (rect.width - 25) / 2 - 3;
                    rect_uiName.x += 15;

                    var rect_path = rect;
                    rect_path.width = (rect.width - 25) / 2 - 3;
                    rect_path.x += rect_uiName.width + 15;


                    EditorGUI.LabelField(rect_uiName, "UI Name", EditorStyles.boldLabel);
                    EditorGUI.LabelField(rect_path, "Load Path", EditorStyles.boldLabel);
                };
                mReorderableList_Group.drawElementCallback = (rect, index, selected, focused) =>
                {
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.y += 2;

                    SerializedProperty itemData = mReorderableList_Group.serializedProperty.GetArrayElementAtIndex(index);
                    SerializedProperty item_uiName = itemData.FindPropertyRelative("Name");
                    SerializedProperty item_Path = itemData.FindPropertyRelative("Path");

                    var rect_name = rect;
                    rect_name.width = (rect.width - 25) / 2 - 3;
                    var rect_path = rect;
                    rect_path.width = (rect.width - 25) / 2 - 3;
                    rect_path.x += rect_name.width + 5;
                    var rect_btn = rect;
                    rect_btn.width = 28;
                    rect_btn.x += rect_name.width + 5 + rect_path.width + 2;


                    item_uiName.stringValue = EditorGUI.TextField(rect_name, item_uiName.stringValue);
                    item_Path.stringValue = EditorGUI.TextField(rect_path, item_Path.stringValue);
                    if(GUI.Button(rect_btn, new GUIContent(mImg_Folder)))
                    {
                        string cur_path = item_Path.stringValue;
                        string cur_dir = "Assets/";
                        if (!cur_path.IsNullOrEmpty())
                            cur_dir = Path.GetDirectoryName(cur_path);

                        string select_path = EditorUtility.OpenFilePanelWithFilters("Select UI", cur_dir, new string[] { "UI", "prefab" });
                        if (!select_path.IsNullOrEmpty())
                        {
                            var root_path = Directory.GetCurrentDirectory().Replace("\\", "/");
                            if (select_path.StartsWith(root_path))
                            {
                                select_path = select_path.Substring(root_path.Length + 1, select_path.Length - root_path.Length - 1);
                                select_path = select_path.Replace("\\", "/");
                                item_Path.stringValue = select_path;
                            }
                            else
                                Debug.LogError("Invalid Path: " + select_path);
                        }
                    }
                };
                mReorderableList_Group.onAddCallback = list =>
                {
                    if (list.serializedProperty != null)
                    {
                        list.serializedProperty.arraySize++;
                        list.index = list.serializedProperty.arraySize - 1;

                        SerializedProperty itemData = list.serializedProperty.GetArrayElementAtIndex(list.index);
                        SerializedProperty item_name = itemData.FindPropertyRelative("Name");
                        SerializedProperty item_path = itemData.FindPropertyRelative("Path");
                        item_name.stringValue = null;
                        item_path.stringValue = null;
                    }
                    else
                    {
                        ReorderableList.defaultBehaviours.DoAddButton(list);
                    }
                };
            }

            mReorderableList_Group.DoLayoutList();
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
