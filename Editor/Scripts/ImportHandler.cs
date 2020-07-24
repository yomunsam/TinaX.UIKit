using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinaX;
using TinaX.IO;
using TinaX.UIKit.Const;
using TinaX.UIKit.Internal;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace TinaXEditor.UIKit.Internal
{
    public class ImportHandler : AssetPostprocessor
    {
        public static UIConfig Config;
        private static bool _Refresh = false;

        private static List<UIConfig.UIFolderItem> s_ImageFolder = new List<UIConfig.UIFolderItem>();
        /// <summary>
        /// 使用旧图集系统
        /// </summary>
        private static bool s_UseLegacyAtlas = false;

        private static string[] lastImportedAssets;
        private static DateTime lastAllPostprocessTime = DateTime.UtcNow;

        public static void RefreshConfig()
        {
            _Refresh = true;
            if (Config == null)
                Config = XConfig.GetConfig<UIConfig>(UIConst.ConfigPath_Resources, AssetLoadType.Resources, false);
            if (Config == null)
                return;
            s_UseLegacyAtlas = Config.UseLegacySpritePacker;
            s_ImageFolder.Clear();
            if (Config.UI_Image_Folders == null || Config.UI_Image_Folders.Count < 0) return;
            foreach (var item in Config.UI_Image_Folders)
            {
                if (!item.Path.StartsWith("Assets/"))
                    continue;

                string path = item.Path;
                if (!path.EndsWith("/"))
                {
                    path = path + "/";
                }
                if (!s_ImageFolder.Any(i => i.Path == path))
                {
                    s_ImageFolder.Add(new UIConfig.UIFolderItem() { Path = path, Atlas = item.Atlas });
                }
            }

        }

        /// <summary>
        /// 导入Texture 前置处理
        /// </summary>
        public void OnPreprocessTexture()
        {
            if (Config == null && !_Refresh)
                RefreshConfig();
            if (Config == null)
                return;

            if (InImageDir(this.assetPath, out bool isAtlas))
            {
                TextureImporter ti = this.assetImporter as TextureImporter;
                ti.textureType = TextureImporterType.Sprite;

                if (isAtlas && s_UseLegacyAtlas)
                {
                    //处理旧版精灵图集 | Handle Sprite Packer (Legacy)
                    string dir_name = GetDirectoryNameWithoutPath(this.assetPath);
                    ti.spritePackingTag = dir_name;
                }
            }

            #region Legacy Code

            //string path = this.assetImporter.assetPath;

            //foreach(var item in s_ImageFolder)
            //{
            //    if (path.StartsWith(item.Path))
            //    {
            //        TextureImporter ti = assetImporter as TextureImporter;
            //        ti.textureType = TextureImporterType.Sprite;

            //        //图集
            //        if (item.Atlas)
            //        {
            //            var path_arr = item.Path.Split('/');
            //            var str_arr = path.Split('/');
            //            if(str_arr.Length - path_arr.Length >= 1)
            //            {
            //                //旧 Sprite Packer
            //                ti.spritePackingTag = str_arr[path_arr.Length - 1];

            //                ////Sprite Atlas
            //                //string folder_path = $"{item.Path}{str_arr[path_arr.Length - 1]}";
            //                //string atlas_path = $"{folder_path}/{str_arr[path_arr.Length - 1]}.spriteatlas";
            //                //var atlas = GetOrCreateAtlas(atlas_path, folder_path);
            //            }
            //        }
            //    }
            //}

            #endregion
        }




        /// <summary>
        /// Unity编辑器会反射调用它
        /// 总资源后处理
        /// </summary>
        /// <param name="importedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (Config == null && !_Refresh)
                RefreshConfig();
            if (Config == null)
                return;

            #region 防重复
            /*
             * 【Unity 2019.4 目前为发现下述现象】【好吧，有时候还是会有这个问题】
             * 在早期Unity版本中，Windows平台上，同一次资源导入可能会导致多次OnPostprocessAllAssets被调用。
             * 所以会有如下代码来防止对同一次资源导入进行多次处理。
             */

            if (lastImportedAssets != null)
            {
                //Debug.Log("需要进行重复判断");
                //Debug.Log("上次导入处理时间：" + lastAllPostprocessTime);
                //Debug.Log("本次导入处理时间：" + DateTime.UtcNow);
                //Debug.Log("时差：" + (DateTime.UtcNow - lastAllPostprocessTime).TotalSeconds + "秒");
                if (lastImportedAssets.Length == importedAssets.Length)
                {
                    bool flag = false; //如果对比结果不一致，这置为true
                    for (var i = 0; i < importedAssets.Length; i++)
                    {
                        if (!importedAssets[i].Equals(lastImportedAssets[i]))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        lastImportedAssets = importedAssets;
                        lastAllPostprocessTime = DateTime.UtcNow;
                        return;
                    }
                }
            }
            #endregion

            if (importedAssets.Length > 0)
            {
                HandleUIImagesImport(importedAssets);
            }
            else
            {
                if(deletedAssets.Length > 0)
                {
                    HandleUIImagesDelete(deletedAssets);
                }
            }

            lastAllPostprocessTime = DateTime.UtcNow;
            lastImportedAssets = importedAssets;
        }

        static void HandleUIImagesImport(string[] importedAssets)
        {
            List<string[]> handleAtlasPath = new List<string[]>(); //需要处理的Atlas路径，数组长度为2，其中[0]为图集路径，[1]为图集所在目录的文件夹路径
            foreach (var path in importedAssets)
            {
                if (InImageDir(path, out bool isAtlas))
                {
                    if (isAtlas)
                    {
                        string atlas_path = GetAltasPath(path, out string _dir);
                        if (!handleAtlasPath.Any(p => p[0].Equals(atlas_path)))
                            handleAtlasPath.Add(new string[2] { atlas_path, _dir });
                    }
                }
            }

            if (handleAtlasPath.Count > 0)
            {
                foreach (var atlasPath in handleAtlasPath)
                {
                    RefreshSpritesInAtlas(atlasPath[0], atlasPath[1]);
                }
            }
        }

        static void HandleUIImagesDelete(string[] deletedAssets)
        {
            List<string[]> handleAtlasPath = new List<string[]>(); //需要处理的Atlas路径，数组长度为2，其中[0]为图集路径，[1]为图集所在目录的文件夹路径
            foreach (var path in deletedAssets)
            {
                if (InImageDir(path, out bool isAtlas))
                {
                    if (isAtlas)
                    {
                        string atlas_path = GetAltasPath(path, out string _dir);
                        if (!handleAtlasPath.Any(p => p[0].Equals(atlas_path)))
                            handleAtlasPath.Add(new string[2] { atlas_path, _dir });
                    }
                }
            }

            if (handleAtlasPath.Count > 0)
            {
                foreach (var atlasPath in handleAtlasPath)
                {
                    //SpriteAtlas sa = GetAtlasOrCreate(atlasPath[0]);
                    RefreshSpritesInAtlas(atlasPath[0], atlasPath[1]);
                }
            }
        }

        /// <summary>
        /// 给定的路径是否在Image（包括Atlas）的配置中
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool InImageDir(string path,out bool atlas)
        {
            var folders = s_ImageFolder
                .Where(f => path.StartsWith(f.Path));
            if (folders.Count() <= 0)
            {
                atlas = false;
                return false;
            }
            var folder = folders.First();
            atlas = folder.Atlas;
            return true;
        }

        /// <summary>
        /// 获取给定路径所属的目录的名称（单纯的目录名称而非路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetDirectoryNameWithoutPath(string path)
        {
            return Path.GetFileName(Path.GetDirectoryName(path));
        }

        private static SpriteAtlas GetAtlasOrCreate(string path)
        {
            var sa = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            if (sa != null)
                return sa;

            //create
            sa = new SpriteAtlas();
            AssetDatabase.CreateAsset(sa, path);
            return sa;
        }

        private static string GetAltasPath(string assetPath, out string rootDir)
        {
            rootDir = Path.GetDirectoryName(assetPath);
            return Path.Combine(rootDir, Path.GetFileName(rootDir) + ".spriteatlas");
        }

        private static void RefreshSpritesInAtlas(string sa_path, string rootDir)
        {
            if (s_UseLegacyAtlas)
                return;

            string[] sprite_guids = AssetDatabase.FindAssets("t:Sprite", new string[] { rootDir });
            List<string> spritePaths = new List<string>();

            #region 整理出sprites的路径
            foreach(var guid in sprite_guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                //剔除子文件夹
                string path_dir = Path.GetDirectoryName(path);
                if (!XPath.IsSubpath(path_dir, rootDir))
                    spritePaths.Add(path);
            }
            #endregion
            if(spritePaths.Count == 0)
            {
                var _sa = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(sa_path);
                if(_sa != null)
                {
                    _sa = null;
                    AssetDatabase.DeleteAsset(sa_path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                return;
            }

            var sa = GetAtlasOrCreate(sa_path);
            List<UnityEngine.Object> sprites = new List<UnityEngine.Object>(); //最终图集里会放这些对象
            UnityEngine.Object[] current_sprites = sa.GetPackables(); //在处理之前，图集里已经存在的对象

            //把当前实际存在的精灵加载出来，添加到sprites这个list
            foreach(var path in spritePaths)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if(sprite != null)
                {
                    if (!sprites.Contains(sprite))
                        sprites.Add(sprite);
                }
            }
            sa.Remove(current_sprites);
            sa.Add(sprites.ToArray());
        }

    }
}
