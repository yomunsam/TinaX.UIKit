using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.UIKit.Page.Group;
using TinaX.UIKit.Page.View;

namespace TinaX.UIKit.Page
{
    public abstract class UIPageBase
    {
#nullable enable

        public UIPageBase() { }

        public UIPageBase(IPageViewProvider viewProvider)
        {
            this.m_ViewProvider = viewProvider;
        }

        /// <summary>
        /// Page的View
        /// </summary>
        protected PageView? m_Content { get; set; }

        protected IPageViewProvider? m_ViewProvider { get; set; }

        protected string m_PageUri { get; set; } = string.Empty;

        protected string m_Name { get; set; } = string.Empty;

        public virtual string Name => m_Name;

        /// <summary>
        /// 总Page数量（包括自己和子项）
        /// </summary>
        public virtual int PageCount => 1;

        /// <summary>
        /// 准备好视图到内存（资产加载过程）
        /// </summary>
        /// <returns></returns>
        public abstract UniTask ReadyViewAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 初始化显示一个View
        /// </summary>
        public abstract void DisplayView();



        public abstract void HideView();

        /// <summary>
        /// UIPage被加入一个组
        /// </summary>
        public abstract void OnJoinGroup(UIPageGroup group);

        /// <summary>
        /// UIPage被移出了一个组
        /// </summary>
        /// <param name="group"></param>
        public abstract void OnLeaveGroup(UIPageGroup group);


        


#nullable restore
    }
}
