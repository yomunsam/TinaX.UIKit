using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.UIKit.Args;
using TinaX.UIKit.Page;
using TinaX.UIKit.Page.Controller;

namespace TinaX.UIKit.Builder.GetPageBuilders
{
    public class UIKitServiceGetPageBuilder
    {
        private readonly IUIKit m_UIKit;
        private readonly GetUIPageArgs m_GetPageArgs;


        public UIKitServiceGetPageBuilder(IUIKit uikit, string pageUri)
        {
            this.m_UIKit = uikit;
            m_GetPageArgs = new OpenUIArgs(pageUri);
        }

        public UIKitServiceGetPageBuilder SetPageUri(string pageUri)
        {
            this.m_GetPageArgs.PageUri = pageUri;
            return this;
        }

        public UIKitServiceGetPageBuilder SetController(PageControllerBase controller)
        {
            this.m_GetPageArgs.PageController = controller;
            return this;
        }


        public UniTask<UIPageBase> GetPageAsync(CancellationToken cancellationToken = default)
        {
            return m_UIKit.GetUIPageAsync(m_GetPageArgs, cancellationToken);
        }
    }
}
