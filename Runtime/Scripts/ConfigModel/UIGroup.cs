using System;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.UIKit
{
    [CreateAssetMenu(menuName ="TinaX/UIKit/UI Group", fileName ="UIGroup",order = 121)]
    public class UIGroup : ScriptableObject
    {
        public List<Item> Items;

        [Serializable]
        public struct Item
        {
            public string Name;
            public string Path;
        }


        private Dictionary<string, string> _dict_name_path;
        private Dictionary<string,string> mDict_Name_Path
        {
            get
            {
                if(_dict_name_path == null)
                {
                    _dict_name_path = new Dictionary<string, string>();
                    if(Items != null)
                    {
                        foreach (var item in Items)
                        {
                            if (item.Name.IsNullOrEmpty() || item.Path.IsNullOrEmpty())
                                continue;
                            if (mDict_Name_Path.ContainsKey(item.Name))
                                continue;
                            mDict_Name_Path.Add(item.Name, item.Path);
                        }    
                    }
                }
                return _dict_name_path;
            }
        }

        public bool TryGetPath(string uiName,out string uiPath)
        {
            return mDict_Name_Path.TryGetValue(uiName, out uiPath);
        }

    }
}
