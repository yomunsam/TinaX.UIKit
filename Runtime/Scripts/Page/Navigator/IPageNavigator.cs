using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.UIKit.Args;
using TinaX.UIKit.Page.Controller;

namespace TinaX.UIKit.Page.Navigator
{
    /// <summary>
    /// 页面导航器
    /// </summary>
    public interface IPageNavigator
    {
        UniTask<UIPageBase> OpenUIAsync(OpenUIArgs args, CancellationToken cancellationToken = default);
        UniTask<UIPageBase> OpenUIAsync(string pageUri, CancellationToken cancellationToken = default);
        UniTask<UIPageBase> OpenUIAsync(string pageUri, PageControllerBase controller, params object[] displayMessageArgs);
    }

    public interface IPageNavigator<TPage, TOpenUIArgs> : IPageNavigator 
        where TPage : UIPageBase
    {
        UniTask<TPage> OpenUIAsync(TOpenUIArgs args, CancellationToken cancellationToken = default);
    }
}
