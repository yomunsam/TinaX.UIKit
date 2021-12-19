using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace TinaX.UIKit.Page.Group
{
#nullable enable
    public class UIPageGroup : UIPageBase
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


        protected UIPageGroup? m_Parent;

        public override int PageCount
            => m_Children.Count + 1; //自己和子项的总数


        public override UniTask ReadyViewAsync(CancellationToken cancellationToken = default)
            => UniTask.CompletedTask;

        public override void DisplayView()
        {
            throw new System.NotImplementedException();
        }

        public override void HideView()
        {
            throw new System.NotImplementedException();
        }

        public override void OnJoinGroup(UIPageGroup group)
        {
            m_Parent = group;
        }

        public override void OnLeaveGroup(UIPageGroup group)
        {
            m_Parent = null;
        }


        public virtual void Push(UIPageBase page)
        {
            m_Children.Add(page);
            page.OnJoinGroup(this);
        }


        public override string ToString()
        {
            return $"[Group]{this.m_Name}";
        }
    }

#nullable restore

}
