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
        IXCore XCore { get; }
        UniTask<IPage> OpenUIAsync(OpenUIArgs args, CancellationToken cancellationToken = default);
        UniTask<IPage> OpenUIAsync(string pageUri, CancellationToken cancellationToken = default);
        UniTask<IPage> OpenUIAsync(string pageUri, PageControllerBase controller, params object[] displayMessageArgs);
    }

    public interface IPageNavigator<TPage, TOpenUIArgs> : IPageNavigator
        where TPage : IPage
    {
        UniTask<TPage> OpenUIAsync(TOpenUIArgs args, CancellationToken cancellationToken = default);
    }
}
