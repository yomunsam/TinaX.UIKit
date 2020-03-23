using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.UIKit.Entity;

namespace TinaX.UIKit.Internal
{
    public class UIEntityManager
    {
        /// <summary>
        /// Key: Path
        /// </summary>
        private Dictionary<string, UIEntity> mDict_UIEntitys = new Dictionary<string, UIEntity>();
        List<UIEntity> mList_UIEntitys = new List<UIEntity>();


        public void Register(UIEntity entity)
        {
            lock (this)
            {
                if (!mDict_UIEntitys.ContainsKey(entity.UIPath))
                    mDict_UIEntitys.Add(entity.UIPath, entity);
                if (!mList_UIEntitys.Contains(entity))
                    mList_UIEntitys.Add(entity);
            }
        }

        public bool TryGetEntity(string path, out UIEntity entity)
        {
            lock (this)
            {
                if (mDict_UIEntitys.TryGetValue(path, out entity))
                {
                    if (entity.UIStatue == UIStatus.Unloaded)
                    {
                        mDict_UIEntitys.Remove(entity.UIPath);
                        mList_UIEntitys.Remove(entity);
                        entity = null;
                        return false;
                    }
                    else
                        return true;
                }
                else
                    return false;
            }
        }
        
    }
}
