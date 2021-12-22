using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Exceptions;
using TinaX.UIKit.Args;
using TinaX.UIKit.Page.Controller;
using TinaX.UIKit.Page.Group;

namespace TinaX.UIKit.Page.Navigator
{
#nullable enable
    /// <summary>
    /// 页 导航器
    /// </summary>
    public class PageNavigator : IPageNavigator
    {
        /// <summary>
        /// 导航器所属的Page
        /// </summary>
        protected UIPageBase m_Page;
        protected IUIKit m_UIKit;

        public PageNavigator(UIPageBase page, IUIKit uiKit)
        {
            m_Page = page;
            m_UIKit = uiKit;
        }

        public virtual async UniTask<UIPageBase> OpenUIAsync(OpenUIArgs args, CancellationToken cancellationToken = default)
        {
            if (m_Page.Parent == null)
                throw new XException($"Page {m_Page.Name} must have parent");
            ProcessArgsBeforeGetPage(ref args);
            var page = await m_UIKit.GetUIPageAsync(args, cancellationToken);
            m_Page.Parent.Push(page, args.UIDisplayArgs);
            return page;
        }

        public virtual UniTask<UIPageBase> OpenUIAsync(string pageUri, CancellationToken cancellationToken = default)
            => this.OpenUIAsync(new OpenUIArgs(pageUri), cancellationToken);

        public virtual UniTask<UIPageBase> OpenUIAsync(string pageUri, PageControllerBase controller, params object[] displayMessageArgs)
        {
            var args = new OpenUIArgs(pageUri)
            {
                PageController = controller,
            };
            if(displayMessageArgs != null && displayMessageArgs.Length > 0)
                args.UIDisplayArgs = displayMessageArgs;
            return this.OpenUIAsync(args);
        }

        protected virtual void ProcessArgsBeforeGetPage(ref OpenUIArgs args)
        {

        }

    }
#nullable restore
}
