using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.UIKit.Args;
using TinaX.UIKit.Page;
using TinaX.UIKit.Page.Controller;
using TinaX.UIKit.Page.Navigator;

namespace TinaX.UIKit.Builder.OpenUIBuilders
{
#nullable enable
    public class PageNavigatorOpenUIBuilder
    {
        private readonly IPageNavigator m_Navigator;
        private readonly OpenUIArgs m_OpenUIArgs;
        public PageNavigatorOpenUIBuilder(IPageNavigator navigator, string pageUri)
        {
            this.m_Navigator = navigator;
            m_OpenUIArgs = new OpenUIArgs(pageUri);
        }

        public PageNavigatorOpenUIBuilder SetPageUri(string pageUri)
        {
            this.m_OpenUIArgs.PageUri = pageUri;
            return this;
        }

        public PageNavigatorOpenUIBuilder SetController(PageControllerBase controller)
        {
            this.m_OpenUIArgs.PageController = controller;
            return this;
        }

        public PageNavigatorOpenUIBuilder SetDisplayMessageArgs(params object[] args)
        {
            this.m_OpenUIArgs.UIDisplayArgs = args;
            return this;
        }


        public UniTask<UIPageBase> OpenUIAsync(CancellationToken cancellationToken = default)
        {
            return m_Navigator.OpenUIAsync(m_OpenUIArgs, cancellationToken);
        }

    }
#nullable restore
}
