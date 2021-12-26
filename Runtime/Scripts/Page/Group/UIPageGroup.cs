using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Core.Helper.LogColor;
using UnityEngine;

namespace TinaX.UIKit.Page.Group
{
#nullable enable
    public class UIPageGroup : UIPageBase, IGroup
    {
        protected readonly List<UIPageBase> m_Children = new List<UIPageBase>();

        public UIPageGroup(string name)
        {
            this.m_Name = name;
        }

        public UIPageGroup()
        {
            this.m_Name = this.GetHashCode().ToString();
        }

        

        


        public override int PageSize
            => m_Children.Count + 1; //自己和子项的总数


        public override UniTask ReadyViewAsync(CancellationToken cancellationToken = default)
            => UniTask.CompletedTask;

        

        public override void HideView()
        {
            throw new System.NotImplementedException();
        }



        #region UI Stack

        /// <summary>
        /// UIPage加入组
        /// </summary>
        /// <param name="page"></param>
        /// <param name="displayMessageArgs"></param>
        public virtual void Push(UIPageBase page, object?[]? displayMessageArgs = null)
        {
            m_Children.Add(page);
            page.OnJoinGroup(this, displayMessageArgs);
            ResetOrder(); //Todo:后期可以优化，ResetOrder是针对所有子页的，其实不是很有必要，Push肯定在最末尾。
        }

        /// <summary>
        /// 从组中移除Page
        /// 调用Page的CloseUI会走到这里
        /// </summary>
        /// <param name="page"></param>
        public virtual void Remove(UIPageBase page)
        {
            if (!m_Children.Contains(page))
                return; //和咱没关系.
        }

        /// <summary>
        /// 重设序号
        /// </summary>
        public virtual void ResetOrder()
        {
            var counter = this.Order + 1;
            for(int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].Order = counter;
                counter += m_Children[i].PageSize;
            }
        }

        #endregion

        public override string ToString()
        {
            return $"[Group]{this.m_Name}";
        }

        public virtual UIPageGroup GetLastChildGroup()
        {
            if(m_Children.Count > 0)
            {
                for (var i = m_Children.Count - 1; i >= 0; i--)
                {
                    var child = m_Children[i];
                    if(child is UIPageGroup group)
                    {
                        return group.GetLastChildGroup();
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Debug方法，在控制台中输出UI树结构
        /// </summary>
        public virtual void PrintUITree(int depth = 0)
        {
            var space_str = new string(' ', depth * 4);
            var child_space_str = new string(' ', (depth + 1) * 4);
            Debug.Log($"{space_str}- <color=#{LogColorHelper.Color_Primary_16}>{this.m_Name}</color> <i>(Group)</i> [<color=#{LogColorHelper.Color_Normal_16}>{this.Order}</color>]");
            for (int i = 0; i < m_Children.Count; i++)
            {
                var child = m_Children[i];
                if(child is UIPageGroup)
                {
                    var group = child as UIPageGroup;
                    group!.PrintUITree(depth + 1);
                }
                else
                {
                    Debug.Log($"{child_space_str}- {child.Name} [<color=#{LogColorHelper.Color_Normal_16}>{child.Order}</color>]");
                }
            }
        }

        public override void DisplayView(object?[]? args) { } //Group本身没有View，不需要这个

        public override void ClosePage(object?[]? closeMessageArgs = null)
        {
            throw new System.NotImplementedException();
        }

        public override void DestroyPage()
        {
            throw new System.NotImplementedException();
        }
    }

#nullable restore

}
