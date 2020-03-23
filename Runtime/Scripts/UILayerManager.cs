using System.Collections.Generic;
using TinaX.UIKit.Entity;
using System.Linq;

/*
 * 层级号分配规则
 * 
 * 每个UI占用10个layer数，
 * 
 * layer本体占用在10的整数倍上，并向前后各预留5个数位
 * 
 * 如，第一个UI，本体占用layer为10，向前后预留5位，则6到15号都属于该UI占用的layer index数。
 * 
 * 特殊分配：
 * 
 * 【UI遮罩】UI遮罩位于每个UI的下方，占用Layer为 UI本体 layer index - 2 
 * 
 */

namespace TinaX.UIKit.Internal
{
    /// <summary>
    /// UI层级管理器
    /// </summary>
    internal class UILayerManager
    {
        private const int layerCount_per = 10;

        private List<UIEntity> mUIStack = new List<UIEntity>();

        public void Register(UIEntity entity)
        {
            if (mUIStack.Contains(entity))
            {
                this.Top(entity);
                return;
            }

            int cur_layer = mUIStack.Count * layerCount_per + layerCount_per;
            mUIStack.Add(entity);

            entity.SortingOrder = cur_layer;
        }

        /// <summary>
        /// UI置顶
        /// </summary>
        /// <param name="entity"></param>
        public void Top(UIEntity entity)
        {
            if (mUIStack == null && mUIStack.Count == 0)
                return;
            if (mUIStack.Last() == entity)
                return; //已经在顶了，不需要指定
            if (!mUIStack.Contains(entity)) return;


            int max_layer = mUIStack.Last().SortingLayerID;
            for(var i = mUIStack.Count - 1; i >= 0; i--)
            {
                if(mUIStack[i] != entity)
                {
                    mUIStack[i].SortingOrder -= layerCount_per; //在主角上面的所有UI的order往下降一个单位
                }
                else
                {
                    entity.SortingOrder = max_layer;
                    //调换stack中的次序，置顶的放到最后面
                    var temp = mUIStack.Last();
                    mUIStack[mUIStack.Count - 1] = mUIStack[i];
                    mUIStack[i] = temp;
                    break;
                }
            }

        }

        public void Remove(UIEntity entity)
        {
            if (!mUIStack.Contains(entity)) return;
            for(var i = mUIStack.Count -1; i >= 0; i--)
            {
                if(mUIStack[i] == entity)
                {
                    //把在要Remove的entity上面的每个entiy，每个层级挨个缩小一个单位
                    mUIStack[i].SortingOrder -= layerCount_per;
                }
                else
                {
                    mUIStack.RemoveAt(i);
                    break;
                }
            }
        }

    }
}
