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
        private Dictionary<string, List<UIEntity>> mDict_UIEntitys = new Dictionary<string, List<UIEntity>>();
        List<UIEntity> mList_UIEntitys = new List<UIEntity>();


        public void Register(UIEntity entity)
        {
            lock (this)
            {
                if (!mDict_UIEntitys.ContainsKey(entity.UIPath))
                    mDict_UIEntitys.Add(entity.UIPath, new List<UIEntity>());
                if (!mDict_UIEntitys[entity.UIPath].Contains(entity))
                    mDict_UIEntitys[entity.UIPath].Add(entity);

                if (!mList_UIEntitys.Contains(entity))
                    mList_UIEntitys.Add(entity);
            }
        }

        public bool TryGetEntitys(string path, out UIEntity[] _entities)
        {
            lock (this)
            {
                if(mDict_UIEntitys.TryGetValue(path,out var entities))
                {
                    for(int i = entities.Count -1; i >= 0; i--)
                    {
                        if(entities[i].UIStatue == UIStatus.Unloaded)
                        {
                            mList_UIEntitys.Remove(entities[i]);
                            entities.RemoveAt(i);
                        }
                    }
                    if(entities.Count == 0)
                    {
                        mDict_UIEntitys.Remove(path);
                        _entities = null;
                        return false;
                    }
                    else
                    {
                        _entities = entities.ToArray();
                        return true;
                    }
                }
                else
                {
                    _entities = null;
                    return false;
                }
            }
        }

        public void Remove(UIEntity entity)
        {
            lock (this)
            {
                if (mList_UIEntitys.Contains(entity))
                    mList_UIEntitys.Remove(entity);
                if (mDict_UIEntitys.ContainsKey(entity.UIPath))
                {
                    if (mDict_UIEntitys[entity.UIPath].Contains(entity))
                        mDict_UIEntitys[entity.UIPath].Remove(entity);

                    if (mDict_UIEntitys[entity.UIPath].Count == 0)
                        mDict_UIEntitys.Remove(entity.UIPath);
                }
            }
        }
        
    }
}
