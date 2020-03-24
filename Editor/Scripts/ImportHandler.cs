using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX;
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
        private static bool refresh = false;

        static List<UIConfig.UIFolderItem> ui_folders = new List<UIConfig.UIFolderItem>();
        public static void RefreshConfig()
        {
            refresh = true;
            if (Config == null)
                Config = XConfig.GetConfig<UIConfig>(UIConst.ConfigPath_Resources, AssetLoadType.Resources, false);
            if (Config == null)
                return;

            ui_folders.Clear();
            foreach(var item in Config.UI_Image_Folders)
            {
                if (!item.Path.StartsWith("Assets/"))
                    continue;

                string path = item.Path;
                if (!path.EndsWith("/"))
                {
                    path = path + "/";
                }
                if(!ui_folders.Any(i => i.Path == path))
                {
                    ui_folders.Add(new UIConfig.UIFolderItem() { Path = path, Atlas = item.Atlas });
                }
            }

        }

        void OnPreprocessTexture()
        {
            if (Config == null && !refresh)
                RefreshConfig();
            if (Config == null)
                return;

            string path = this.assetImporter.assetPath;
            foreach(var item in ui_folders)
            {
                if (path.StartsWith(item.Path))
                {
                    TextureImporter ti = assetImporter as TextureImporter;
                    ti.textureType = TextureImporterType.Sprite;

                    //图集
                    if (item.Atlas)
                    {
                        var path_arr = item.Path.Split('/');
                        var str_arr = path.Split('/');
                        if(str_arr.Length - path_arr.Length >= 1)
                        {
                            //旧 Sprite Packer
                            ti.spritePackingTag = str_arr[path_arr.Length - 1];

                            ////Sprite Atlas
                            //string folder_path = $"{item.Path}{str_arr[path_arr.Length - 1]}";
                            //string atlas_path = $"{folder_path}/{str_arr[path_arr.Length - 1]}.spriteatlas";
                            //var atlas = GetOrCreateAtlas(atlas_path, folder_path);
                        }
                    }
                }
            }
        }



        //SpriteAtlas GetOrCreateAtlas(string path,string folder)
        //{
        //    var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
        //    if (atlas != null)
        //        return atlas;

        //    //Create
        //    atlas = new SpriteAtlas();
        //    atlas.SetIncludeInBuild(true);
        //    AssetDatabase.CreateAsset(atlas, path);

        //    return atlas;
        //}
    }
}
