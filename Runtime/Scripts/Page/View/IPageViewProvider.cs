using System.Threading;
using Cysharp.Threading.Tasks;

namespace TinaX.UIKit.Page.View
{
    public interface IPageViewProvider
    {
        /// <summary>
        /// 是否支持异步方法
        /// </summary>
        bool SupportAsynchronous { get; }

        PageView GetPageView(UIPageBase page);

        UniTask<PageView> GetPageViewAsync(UIPageBase page, CancellationToken cancellationToken = default); 

        string ViewUri { get; }
    }

    public interface IPageViewProvider<TView, TPage> : IPageViewProvider 
        where TView : PageView 
        where TPage : UIPageBase
    {
        TView GetPageViewGeneric(TPage page);

        UniTask<TView> GetPageViewGenericAsync(TPage page, CancellationToken cancellationToken = default);
    }
}
