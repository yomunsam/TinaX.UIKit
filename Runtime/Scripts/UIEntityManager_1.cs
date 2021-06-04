using System.Collections.Generic;
using System.Linq;
using TinaX.UIKit.Entity;

namespace TinaX.UIKit.Internal
{
    public class UIEntityManager
    {
        /// <summary>
        /// Key: Path
        /// </summary>
        private Dictionary<string, List<UIEntity>> mDict_UIEntitys = new Dictionary<string, List<UIEntity>>();
        List<UIEntity> m_List_UIEntitys = new List<UIEntity>();


        public void Register(UIEntity entity)
        {
            lock (this)
            {
                if (!mDict_UIEntitys.ContainsKey(entity.UIPath))
                    mDict_UIEntitys.Add(entity.UIPath, new List<UIEntity>());
                if (!mDict_UIEntitys[entity.UIPath].Contains(entity))
                    mDict_UIEntitys[entity.UIPath].Add(entity);

                if (!m_List_UIEntitys.Contains(entity))
                    m_List_UIEntitys.Add(entity);
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
                            m_List_UIEntitys.Remove(entities[i]);
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
                if (m_List_UIEntitys.Contains(entity))
                    m_List_UIEntitys.Remove(entity);
                if (mDict_UIEntitys.ContainsKey(entity.UIPath))
                {
                    if (mDict_UIEntitys[entity.UIPath].Contains(entity))
                        mDict_UIEntitys[entity.UIPath].Remove(entity);

                    if (mDict_UIEntitys[entity.UIPath].Count == 0)
                        mDict_UIEntitys.Remove(entity.UIPath);
                }
            }
        }
        
        public IEnumerable<UIEntity> FindByUIName(string UIName)
        {
            return m_List_UIEntitys.Where(entity => entity.UIName == UIName);
        }



    }
}
