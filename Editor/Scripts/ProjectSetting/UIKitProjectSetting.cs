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

namespace Packages.io.nekonya.tinax.uikit.Editor.Scripts.ProjectSetting
{
    public static class UIKitProjectSetting
    {
        private static bool mDataRefreshed = false;
        private static UIConfig mConfig;
        private static SerializedObject mConfig_SerObj;

        [SettingsProvider]
        public static SettingsProvider XRuntimeSetting()
        {
            return new SettingsProvider(UIKitEditorConst.ProjectSetting_Node, SettingsScope.Project, new string[] { "Nekonya", "TinaX", "UI", "TinaX.UIKit", "UIKit" })
            {
                label = "X UIKit",
                guiHandler = (searchContent) =>
                {
                    if (!mDataRefreshed) refreshData();
                    EditorGUILayout.BeginVertical(Styles.body);
                    if (mConfig == null)
                    {
                        GUILayout.Space(20);
                        GUILayout.Label(I18Ns.NoConfig);
                        if (GUILayout.Button(I18Ns.BtnCreateConfigFile, Styles.style_btn_normal, GUILayout.MaxWidth(120)))
                        {
                            mConfig = XConfig.CreateConfigIfNotExists<UIConfig>(UIConst.ConfigPath_Resources, AssetLoadType.Resources);
                            refreshData();
                        }
                    }
                    else
                    {
                        GUILayout.Space(20);
                        //Enable
                        mConfig.EnableUIKit = EditorGUILayout.ToggleLeft(I18Ns.EnableUIKit, mConfig.EnableUIKit);

                        //UI Name Mode
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(I18Ns.UINameMode, GUILayout.MaxWidth(200));
                        mConfig.UINameMode = (UINameMode)EditorGUILayout.EnumPopup(mConfig.UINameMode,GUILayout.MaxWidth(180));
                        EditorGUILayout.EndHorizontal();

                        //Default UI Group 
                        if (mConfig.UINameMode == UINameMode.UIGroup)
                        {
                            GUILayout.Space(3);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(I18Ns.DefaultUIGroup, GUILayout.MaxWidth(200));
                            mConfig.DefaultUIGroup = (UIGroup)EditorGUILayout.ObjectField(mConfig.DefaultUIGroup,typeof(UIGroup),false, GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();
                        }

                        if(mConfig.UINameMode == UINameMode.RelativeDirectory)
                        {
                            GUILayout.Space(3);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(I18Ns.UIRootDirLoadPath, GUILayout.MaxWidth(200));
                            mConfig.UIRootDirectoryLoadPath = EditorGUILayout.TextField(mConfig.UIRootDirectoryLoadPath);
                            EditorGUILayout.EndHorizontal();
                            mConfig.DefaultUIGroup = null;
                        }

                        //Use UICamera
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(I18Ns.UseDefaultUICamera, GUILayout.MaxWidth(200));
                        mConfig.UseUICamera = EditorGUILayout.Toggle(mConfig.UseUICamera, GUILayout.MaxWidth(180));
                        EditorGUILayout.EndHorizontal();

                        if (mConfig.UseUICamera)
                        {
                            GUILayout.Space(3);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(I18Ns.UICameraConfig, GUILayout.MaxWidth(200));
                            mConfig.UICameraConfig = (UICameraConfig)EditorGUILayout.ObjectField(mConfig.UICameraConfig, typeof(UICameraConfig), false, GUILayout.MaxWidth(180));
                            EditorGUILayout.EndHorizontal();
                            
                        }


#if ENABLE_LEGACY_INPUT_MANAGER
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(I18Ns.AutoCreateEventSystem, GUILayout.MaxWidth(200));
                        mConfig.AutoCreateEventSystem = EditorGUILayout.Toggle(mConfig.AutoCreateEventSystem, GUILayout.MaxWidth(180));
                        EditorGUILayout.EndHorizontal();
#endif


                        GUILayout.Space(20);
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                        var ui_img_folder = mConfig_SerObj.FindProperty("UI_Image_Folders");
                        EditorGUILayout.PropertyField(ui_img_folder);

                        if (mConfig_SerObj.hasModifiedProperties)
                            mConfig_SerObj.ApplyModifiedProperties();
                        else
                            mConfig_SerObj.Update();
                    }
                    EditorGUILayout.EndVertical();

                    
                },
                deactivateHandler = () =>
                {
                    if (mConfig != null)
                    {
                        EditorUtility.SetDirty(mConfig);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        TinaXEditor.UIKit.Internal.ImportHandler.RefreshConfig();
                    }
                },
            };
        }

        private static void refreshData()
        {
            mConfig = XConfig.GetConfig<UIConfig>(UIConst.ConfigPath_Resources, AssetLoadType.Resources, false);
            if (mConfig != null)
                mConfig_SerObj = new SerializedObject(mConfig);


            mDataRefreshed = true;
        }

        static class Styles
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
        }
    
    }
}
