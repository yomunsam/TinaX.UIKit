using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX;
using TinaX.UIKit;
using TinaX.UIKit.Const;
using TinaX.UIKit.Internal;
using TinaXEditor.UIKit.Const;
using UnityEditor;
using UnityEngine;

namespace TinaXEditor.UIKit.Internal
{
    public static class UIKitProjectSetting
    {
        private static bool m_DataRefreshed = false;
        private static UIConfig m_Config;
        private static SerializedObject m_Config_SerObj;

        [SettingsProvider]
        public static SettingsProvider XRuntimeSetting()
        {
            return new SettingsProvider(UIKitEditorConst.ProjectSetting_Node, SettingsScope.Project, new string[] { "Nekonya", "TinaX", "UI", "TinaX.UIKit", "UIKit" })
            {
                label = "X UIKit",
                guiHandler = (searchContent) =>
                {
                    if (!m_DataRefreshed) refreshData();
                    EditorGUILayout.BeginVertical(Styles.body);
                    if (m_Config == null)
                    {
                        GUILayout.Space(20);
                        GUILayout.Label(I18Ns.NoConfig);
                        if (GUILayout.Button(I18Ns.BtnCreateConfigFile, Styles.style_btn_normal, GUILayout.MaxWidth(120)))
                        {
                            m_Config = XConfig.CreateConfigIfNotExists<UIConfig>(UIConst.ConfigPath_Resources, AssetLoadType.Resources);
                            refreshData();
                        }
                    }
                    else
                    {
                        GUILayout.Space(20);
                        //Enable
                        m_Config.EnableUIKit = EditorGUILayout.ToggleLeft(I18Ns.EnableUIKit, m_Config.EnableUIKit);

                        //UI Name Mode
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(I18Ns.UINameMode, GUILayout.MaxWidth(200));
                        m_Config.UINameMode = (UINameMode)EditorGUILayout.EnumPopup(m_Config.UINameMode,GUILayout.MaxWidth(180));
                        EditorGUILayout.EndHorizontal();

                        //Default UI Group 
                        if (m_Config.UINameMode == UINameMode.UIGroup)
                        {
                            GUILayout.Space(3);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(I18Ns.DefaultUIGroup, GUILayout.MaxWidth(200));
                            m_Config.DefaultUIGroup = (UIGroup)EditorGUILayout.ObjectField(m_Config.DefaultUIGroup,typeof(UIGroup),false, GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();
                        }

                        if(m_Config.UINameMode == UINameMode.RelativeDirectory)
                        {
                            GUILayout.Space(3);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(I18Ns.UIRootDirLoadPath, GUILayout.MaxWidth(200));
                            m_Config.UIRootDirectoryLoadPath = EditorGUILayout.TextField(m_Config.UIRootDirectoryLoadPath);
                            EditorGUILayout.EndHorizontal();
                            m_Config.DefaultUIGroup = null;
                        }

                        //Use UICamera
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(I18Ns.UseDefaultUICamera, GUILayout.MaxWidth(200));
                        m_Config.UseUICamera = EditorGUILayout.Toggle(m_Config.UseUICamera, GUILayout.MaxWidth(180));
                        EditorGUILayout.EndHorizontal();

                        if (m_Config.UseUICamera)
                        {
                            GUILayout.Space(3);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(I18Ns.UICameraConfig, GUILayout.MaxWidth(200));
                            m_Config.UICameraConfig = (UICameraConfig)EditorGUILayout.ObjectField(m_Config.UICameraConfig, typeof(UICameraConfig), false, GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();
                            
                        }


#if ENABLE_LEGACY_INPUT_MANAGER
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(I18Ns.AutoCreateEventSystem, GUILayout.MaxWidth(200));
                        m_Config.AutoCreateEventSystem = EditorGUILayout.Toggle(m_Config.AutoCreateEventSystem, GUILayout.MaxWidth(180));
                        EditorGUILayout.EndHorizontal();
#endif

                        //Default Mask Color
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(I18Ns.DefaultMaskColor, GUILayout.MaxWidth(200));
                        m_Config.DefaultUIMaskColor = EditorGUILayout.ColorField(m_Config.DefaultUIMaskColor, GUILayout.MaxWidth(180));
                        EditorGUILayout.EndHorizontal();

                        #region Canvas Scaler
                        GUILayout.Space(10);
                        GUILayout.Label("Canvas Scaler", EditorStyles.boldLabel);


                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("UI Scale Mode", GUILayout.MaxWidth(200));
                        EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("UICanvasScalerMode"),GUIContent.none, GUILayout.MaxWidth(180));
                        EditorGUILayout.EndHorizontal();

                        if(m_Config.UICanvasScalerMode == UnityEngine.UI.CanvasScaler.ScaleMode.ConstantPixelSize)
                        {
                            //Scale Factor
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("UI Scale Factor", GUILayout.MaxWidth(200));
                            EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("UIScaleFactor"), GUIContent.none, GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();
                        }
                        else if (m_Config.UICanvasScalerMode == UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize)
                        {
                            //Reference Resolution
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Reference Resolution", GUILayout.MaxWidth(200));
                            EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("ReferenceResolution"), GUIContent.none, GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();

                            //Reference Resolution
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Screen Match Mode", GUILayout.MaxWidth(200));
                            EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("ScreenMatchMode"), GUIContent.none, GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Match (Width <-> Height)", GUILayout.MaxWidth(200));
                            m_Config.CanvasScalerMatchWidthOrHeight = EditorGUILayout.Slider(m_Config.CanvasScalerMatchWidthOrHeight, 0, 1, GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();

                        }else if(m_Config.UICanvasScalerMode == UnityEngine.UI.CanvasScaler.ScaleMode.ConstantPhysicalSize)
                        {
                            //PhySical Unit
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("PhySical Unit", GUILayout.MaxWidth(200));
                            EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("PhySicalUnit"), GUIContent.none, GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();

                            //Fallback Screen DPI
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Fallback Screen DPI", GUILayout.MaxWidth(200));
                            EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("FallbackScreenDPI"), GUIContent.none, GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();

                            //Default Sprite DPI
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Default Sprite DPI", GUILayout.MaxWidth(200));
                            EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("DefaultSpriteDPI"), GUIContent.none, GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();
                        }

                        GUILayout.Space(5);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Reference Pixels Per Unit", GUILayout.MaxWidth(200));
                        EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("ReferencePixelsPerUnit"), GUIContent.none, GUILayout.MaxWidth(180));
                        EditorGUILayout.EndHorizontal();

                        #endregion

                        GUILayout.Space(20);
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                        var ui_img_folder = m_Config_SerObj.FindProperty("UI_Image_Folders");
                        EditorGUILayout.PropertyField(ui_img_folder);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(I18Ns.UseLegacySpritePacker, GUILayout.MaxWidth(200));
                        EditorGUILayout.PropertyField(m_Config_SerObj.FindProperty("UseLegacySpritePacker"), GUIContent.none);
                        EditorGUILayout.EndHorizontal();


                        if (m_Config_SerObj.hasModifiedProperties)
                            m_Config_SerObj.ApplyModifiedProperties();
                        else
                            m_Config_SerObj.Update();
                    }
                    EditorGUILayout.EndVertical();

                    
                },
                deactivateHandler = () =>
                {
                    if (m_Config != null)
                    {
                        EditorUtility.SetDirty(m_Config);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        TinaXEditor.UIKit.Internal.ImportHandler.RefreshConfig();
                    }
                },
            };
        }

        private static void refreshData()
        {
            m_Config = XConfig.GetConfig<UIConfig>(UIConst.ConfigPath_Resources, AssetLoadType.Resources, false);
            if (m_Config != null)
                m_Config_SerObj = new SerializedObject(m_Config);


            m_DataRefreshed = true;
        }

        private static class Styles
        {
            private static GUIStyle _style_btn_normal; //字体比原版稍微大一号
            public static GUIStyle style_btn_normal
            {
                get
                {
                    if (_style_btn_normal == null)
                    {
                        _style_btn_normal = new GUIStyle(GUI.skin.button);
                        _style_btn_normal.fontSize = 13;
                    }
                    return _style_btn_normal;
                }
            }

            private static GUIStyle _body;
            public static GUIStyle body
            {
                get
                {
                    if (_body == null)
                    {
                        _body = new GUIStyle();
                        _body.padding.left = 10;
                    }
                    return _body;
                }
            }
        }


        private static class I18Ns
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

            public static string NoConfig
            {
                get
                {
                    if (IsChinese)
                        return "在首次使用TinaX UIKit的设置工具前，请先创建配置文件";
                    if (NihongoDesuka)
                        return "TinaX UIKitセットアップツールを初めて使用する前に、構成ファイルを作成してください";
                    return "Before using the TinaX UIKit setup tool for the first time, please create a configuration file";
                }
            }

            public static string BtnCreateConfigFile
            {
                get
                {
                    if (IsChinese)
                        return "创建配置文件";
                    if (NihongoDesuka)
                        return "構成ファイルを作成する";
                    return "Create Configure File";
                }
            }

            public static string EnableUIKit
            {
                get
                {
                    if (IsChinese)
                        return "启用 UIKit";
                    if (NihongoDesuka)
                        return "UIKitを有効にする";
                    return "Enable UIKit";
                }
            }

            public static string UINameMode
            {
                get
                {
                    if (IsChinese)
                        return "UI命名模式";
                    if (NihongoDesuka)
                        return "UI命名パターン";
                    return "UI naming mode";
                }
            }

            public static string DefaultUIGroup
            {
                get
                {
                    if (IsChinese)
                        return "默认UI组";
                    if (NihongoDesuka)
                        return "デフォルトUIグループ";
                    return "Default UI Group";
                }
            }

            public static string UIRootDirLoadPath
            {
                get
                {
                    if (IsChinese)
                        return "UI根目录加载地址";
                    if (NihongoDesuka)
                        return "UIルートディレクトリのロードパス";
                    return "UI Root Directory load path";
                }
            }
            public static string UseDefaultUICamera
            {
                get
                {
                    if (IsChinese)
                        return "使用 UICamera";
                    if (NihongoDesuka)
                        return "UICameraを使用する";
                    return "Use UI Camera";
                }
            }

            public static string DefaultMaskColor
            {
                get
                {
                    if (IsChinese)
                        return "默认UIMask颜色";
                    return "Default UIMask Color";
                }
            }

            public static string UICameraConfig
            {
                get
                {
                    if (IsChinese)
                        return "UICamera设置";
                    if (NihongoDesuka)
                        return "UICameraの設定";
                    return "UICamera Config";
                }
            }

            public static string AutoCreateEventSystem
            {
                get
                {
                    if (IsChinese)
                        return "自动创建EventSystem";
                    if (NihongoDesuka)
                        return "EventSystemを自動的に作成する";
                    return "Auto Create EventSystem";
                }
            }

            public static string UseLegacySpritePacker
            {
                get
                {
                    if (IsChinese)
                        return "使用 Sprite Packer (旧版图集)";
                    return "Use Sprite Packer (Legacy)";
                }
            }
        }
    
    }
}
