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
 * 全屏UI隐藏规则：
 * 
 * 无法同时出现两个全屏UI, 当新的全屏UI打开时，之前的全屏UI会不显示。
 * 这里的不显示，与UI接口里的Hide有点区别：
 * 
 * UI接口里的Hide相当于SetActive(false), 这种情况下UI上的脚本就不工作了，
 * 这里的不显示，是把UI的Canvas关掉，让它单纯的不会被渲染出来。
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

        private List<UIEntity> mUIHideStack = new List<UIEntity>(); //Hide

        public void Register(UIEntity entity)
        {
            if (mUIStack.Contains(entity))
            {
                this.Top(entity);
                return;
            }

            int cur_layer = mUIStack.Count * layerCount_per + layerCount_per;

            //全屏UI处理
            if (entity.UIPage.FullScreenUI)
            {
                //最上层的UI是全屏UI，把下层的全屏UI都隐藏掉
                foreach(var ui in mUIStack)
                {
                    if (ui.UIPage.FullScreenUI)
                    {
                        if (!mUIHideStack.Contains(ui))
                            mUIHideStack.Add(ui);

                        //隐藏
                        ui.UICanvas.enabled = false;
                        ui.HideMaskIfExist();
                    }
                }
            }

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


            int max_order = mUIStack.Last().SortingOrder;
            for(var i = mUIStack.Count - 1; i >= 0; i--)
            {
                if(mUIStack[i] != entity)
                {
                    mUIStack[i].SortingOrder -= layerCount_per; //在主角上面的所有UI的order往下降一个单位
                }
                else
                {
                    entity.SortingOrder = max_order;
                    //调换stack中的次序，置顶的放到最后面
                    var temp = mUIStack.Last();
                    if (temp.UIPage.FullScreenUI)
                    {
                        //如果当前顶层级UI是个全屏UI,我们得隐藏掉它
                        if (!mUIHideStack.Contains(temp))
                            mUIHideStack.Add(temp);
                        temp.UICanvas.enabled = false;
                        temp.HideMaskIfExist();
                    }
                    mUIStack[mUIStack.Count - 1] = mUIStack[i];
                    mUIStack[i] = temp;
                    if(mUIStack[mUIStack.Count - 1].UIPage.FullScreenUI)
                    {
                        //如果当前顶层UI是全屏UI, 并且原来被隐藏的话，把它放出来
                        if (mUIHideStack.Contains(mUIStack[mUIStack.Count - 1]))
                            mUIHideStack.Remove(mUIStack[mUIStack.Count - 1]);
                        mUIStack[mUIStack.Count - 1].UICanvas.enabled = true;
                        mUIStack[mUIStack.Count - 1].ShowMaskIfExist();
                    }
                    break;
                }
            }

        }

        public void Remove(UIEntity entity)
        {
            if (!mUIStack.Contains(entity)) return;
            if (entity.UIPage.FullScreenUI)
            {
                if(!mUIHideStack.Contains(entity))
                {
                    //这是当前显示的一个全屏UI,得把上一个放出来
                    if(mUIHideStack.Count > 0)
                    {
                        var last = mUIHideStack.Last();
                        mUIHideStack.Remove(last);

                        last.ShowMaskIfExist();
                        last.UICanvas.enabled = true;
                    }
                }
            }
            for(var i = mUIStack.Count -1; i >= 0; i--)
            {
                if(mUIStack[i] != entity)
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
