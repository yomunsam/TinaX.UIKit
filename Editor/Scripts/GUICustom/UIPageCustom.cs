using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using TinaX.UIKit;
using TinaX.XComponent;
using TinaX.UIKit.Animation;
using TinaXEditor.Utils;
using UnityEngine.Rendering;

namespace TinaXEditor.UIKit
{
    [CustomEditor(typeof(UIPage),true)]
    public class UIPageCustom : Editor
    {
        private UIPage _target;
        private SerializedObject _target_seriablizedObj;

        private SerializedProperty _mainUIHandler;
        private SerializedProperty _fullScreenUI;
        private SerializedProperty _screenUI;
        private SerializedProperty _allowMultiple;
        private SerializedProperty _sortingLayer;
        private SerializedProperty _uiShowAni;
        private SerializedProperty _uiExitAni;

        public override void OnInspectorGUI()
        {
            if (_target == null)
                _target = (UIPage)target;
            if (_target_seriablizedObj == null)
                _target_seriablizedObj = new SerializedObject(_target);
            EditorGUILayout.BeginVertical();

            //处理者
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(I18Ns.UIHandler, I18Ns.UIHandler_Tooltips), GUILayout.MaxWidth(100));
            if (_mainUIHandler == null && _target_seriablizedObj != null)
                _mainUIHandler = _target_seriablizedObj.FindProperty("UIMainHandler");
            EditorGUILayout.PropertyField(_mainUIHandler, GUIContent.none);
            //_target.UIMainHandler = EditorGUILayout.ObjectField(_target.UIMainHandler, typeof(UnityEngine.MonoBehaviour), true);
            EditorGUILayout.EndHorizontal();

            //处理者的奇怪判断
            if(_mainUIHandler.objectReferenceValue == null)
            {
                var xc = _target.gameObject.GetComponent<XComponentScriptBase>();
                if(xc != null)
                {
                    if(GUILayout.Button(string.Format(I18Ns.Btn_SetHandler,xc.GetType().Name)))
                    {
                        _mainUIHandler.objectReferenceValue = xc;
                    }
                }
            }
            else
            {
                if (!_mainUIHandler.objectReferenceValue.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                {
                    //无效警告
                    EditorGUILayout.LabelField(I18Ns.UIHandlerInvalid, Styles.label_warning);
                }
            }

            //全屏
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(I18Ns.FullScreenUI, I18Ns.FullScreenUI_ToolTips), GUILayout.MaxWidth(100));
            if (_fullScreenUI == null && _target_seriablizedObj != null)
                _fullScreenUI = _target_seriablizedObj.FindProperty("FullScreenUI");
            EditorGUILayout.PropertyField(_fullScreenUI, GUIContent.none);
            //_target.FullScreenUI = EditorGUILayout.Toggle(_target.FullScreenUI);
            EditorGUILayout.EndHorizontal();

            //Screen UI
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Screen UI", GUILayout.MaxWidth(100));
            if (_screenUI == null && _target_seriablizedObj != null)
                _screenUI = _target_seriablizedObj.FindProperty("ScreenUI");
            EditorGUILayout.PropertyField(_screenUI, GUIContent.none);

            //_target.ScreenUI = EditorGUILayout.Toggle(_target.ScreenUI);
            EditorGUILayout.EndHorizontal();

            //多开
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(I18Ns.MultipleUI, I18Ns.MultipleUI_Tooltips), GUILayout.MaxWidth(100));
            if (_allowMultiple == null && _target_seriablizedObj != null)
                _allowMultiple = _target_seriablizedObj.FindProperty("AllowMultiple");
            EditorGUILayout.PropertyField(_allowMultiple, GUIContent.none);
            //_target.AllowMultiple = EditorGUILayout.Toggle(_target.AllowMultiple);
            EditorGUILayout.EndHorizontal();

            //SortingLayer
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("SortingLayer", GUILayout.MaxWidth(100));
            if (_sortingLayer == null && _target_seriablizedObj != null)
                _sortingLayer = _target_seriablizedObj.FindProperty("SortingLayerId");

            //int sorting_layer_value = 0;


            //if (SortingLayer.layers.Any(sl => sl.value == _sortingLayer.intValue))
            //    sorting_layer_value = _sortingLayer.intValue;
            //var sorting_layers = SortingLayer.layers;
            //var sorting_layer_names = sorting_layers.Select(s => s.name).ToArray();
            //sorting_layer_value = EditorGUILayout.Popup(sorting_layer_value, sorting_layer_names);
            //_sortingLayer.intValue = sorting_layer_value;

            var sorting_layers = SortingLayer.layers;
            var sorting_layer_ids = sorting_layers.Select(sl => sl.id).ToArray();
            var sorting_layer_names = sorting_layers.Select(sl => sl.name).ToArray();

            int sorting_layer_select_index = 0;
            for(var i = 0; i < sorting_layer_ids.Length; i++)
            {
                if(sorting_layer_ids[i] == _sortingLayer.intValue)
                {
                    sorting_layer_select_index = i;
                    break;
                }
            }
            sorting_layer_select_index = EditorGUILayout.Popup(sorting_layer_select_index, sorting_layer_names);
            _sortingLayer.intValue = sorting_layer_ids[sorting_layer_select_index];



            EditorGUILayout.EndHorizontal();

            //动画-----------------------------------------------------------------------
            GUILayout.Space(10);
            EditorGUILayout.LabelField("UI Animation", EditorStyles.largeLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(I18Ns.UIShow_Ani), GUILayout.MaxWidth(120));
            if (_uiShowAni == null && _target_seriablizedObj != null)
                _uiShowAni = _target_seriablizedObj.FindProperty("UIShowAni");
            EditorGUILayout.PropertyField(_uiShowAni, GUIContent.none);

            //_target.UIShowAni = (UIAnimationBase)EditorGUILayout.ObjectField(_target.UIShowAni,typeof(UIAnimationBase),true);
            EditorGUILayout.EndHorizontal();
            if (_uiShowAni.objectReferenceValue != null)
                ((UIAnimationBase)_uiShowAni.objectReferenceValue).playOnAwake = false;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(I18Ns.UIExit_Ani), GUILayout.MaxWidth(120));
            if (_uiExitAni == null && _target_seriablizedObj != null)
                _uiExitAni = _target_seriablizedObj.FindProperty("UIExitAni");
            EditorGUILayout.PropertyField(_uiExitAni, GUIContent.none);
            //_target.UIExitAni = (UIAnimationBase)EditorGUILayout.ObjectField(_target.UIExitAni, typeof(UIAnimationBase), true);
            EditorGUILayout.EndHorizontal();
            if (_uiExitAni.objectReferenceValue != null)
                ((UIAnimationBase)_uiExitAni.objectReferenceValue).pingPong = false;
            //if (_target.UIExitAni != null)
            //    _target.UIExitAni.pingPong = false;

            //音效


            if (GUILayout.Button("Edit In Window"))
            {
                InspectorInWindow.ShowInspector(target);
            }
            EditorGUILayout.EndVertical();

            if (_target_seriablizedObj != null)
                _target_seriablizedObj.ApplyModifiedProperties();
        }

        static class Styles
        {
            

            private static GUIStyle _label_warning;
            public static GUIStyle label_warning
            {
                get
                {
                    if (_label_warning == null)
                    {
                        _label_warning = new GUIStyle(EditorStyles.label);
                        _label_warning.normal.textColor = TinaX.Internal.XEditorColorDefine.Color_Warning;
                    }
                    return _label_warning;
                }
            }
        }
        static class I18Ns
        {
            private static bool? _isChinese;
            private static bool IsChinese
            {
                get
                {
                    if (_isChinese == null)
                    {
                        _isChinese = (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified);
                    }
                    return _isChinese.Value;
                }
            }

            private static bool? _nihongo_desuka;
            private static bool NihongoDesuka
            {
                get
                {
                    if (_nihongo_desuka == null)
                        _nihongo_desuka = (Application.systemLanguage == SystemLanguage.Japanese);
                    return _nihongo_desuka.Value;
                }
            }

            public static string UIHandler
            {
                get
                {
                    if (IsChinese)
                        return "UI处理者";
                    if (NihongoDesuka)
                        return "UIハンドラー";
                    return "UI Handler";
                }
            }

            public static string UIHandler_Tooltips
            {
                get
                {
                    if (IsChinese)
                        return "UI处理者可以接收到UI相关事件和参数，如打开UI、关闭UI、动画结束 等。";
                    if (NihongoDesuka)
                        return "UIハンドラーは、「UIオープン時」、「UIクローズ時」、「UIアニメーション終了時」などのUI関連のイベントとパラメーターを受け取ることができます。";
                    return "UI handlers can receive UI related events and parameters, such as \"on ui open\", \"on ui close\", and \"on ui animation finish\".";
                }
            }

            public static string Btn_SetHandler
            {
                get
                {
                    if (IsChinese)
                        return "设置\"{0}\"为UI处理者";
                    return "Set \"{0}\" as UI Handler";
                }
            }

            public static string UIHandlerInvalid
            {
                get
                {
                    if (IsChinese)
                        return "UI处理者是无效的";
                    if (NihongoDesuka)
                        return "UIハンドラーが無効です";
                    return "UI handler is invalid";
                }
            }

            public static string FullScreenUI
            {
                get
                {
                    if (IsChinese)
                        return "全屏UI";
                    if (NihongoDesuka)
                        return "フルスクリーン";
                    return "Full Screen";
                }
            }

            public static string FullScreenUI_ToolTips
            {
                get
                {
                    if (IsChinese)
                        return "当打开一个新的全屏UI之后，上一个全屏UI会被隐藏";
                    if (NihongoDesuka)
                        return "新しいフルスクリーンUIを開くと、前のフルスクリーンUIは非表示になります";
                    return "When a new full-screen UI is opened, the previous full-screen UI will be hidden";
                }
            }
            public static string MultipleUI
            {
                get
                {
                    if (IsChinese)
                        return "多开UI";
                    if (NihongoDesuka)
                        return "複数を開く";
                    return "Open Multiple";
                }
            }

            public static string MultipleUI_Tooltips
            {
                get
                {
                    if (IsChinese)
                        return "当调用多次打开UI的方法以打开同一个UI时，如果勾选此选项，可以打开多个UI页，否则会把已经打开UI页置顶。";
                    if (NihongoDesuka)
                        return "UIを複数回開いて同じUIを開くメソッドを呼び出す場合、このオプションをオンにすると、複数のUIページを開くことができます。それ以外の場合は、すでに開いているUIページが上に表示されます。";
                    return "When calling the method of opening the UI multiple times to open the same UI, if this option is checked, multiple UI pages can be opened, otherwise the already opened UI pages will be topped.";
                }
            }

            public static string UIShow_Ani
            {
                get
                {
                    if (IsChinese)
                        return "UI显示动画";
                    if (NihongoDesuka)
                        return "UI表示アニメーション";
                    return "UI Show Animation";
                }
            }


            public static string UIExit_Ani
            {
                get
                {
                    if (IsChinese)
                        return "UI退出动画";
                    if (NihongoDesuka)
                        return "UI終了アニメーション";
                    return "UI Exit Animation";
                }
            }
        }
    }
}

