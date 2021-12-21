using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Container;
using TinaX.UIKit.Canvas;
using TinaX.UIKit.Page;
using TinaX.UIKit.Page.Controller;

namespace TinaX.UIKit
{
    public interface IUIKit
    {
        IServiceContainer Services { get; }

        UniTask<UIPageBase> GetUIPageAsync(string pageUri, CancellationToken cancellationToken = default);
        UniTask<UIPageBase> GetUIPageAsync(string pageUri, PageControllerBase controller, CancellationToken cancellationToken = default);
        UniTask<UIPageBase> GetUIPageAsync(GetUIPageArgs args, CancellationToken cancellationToken = default);
        void RegisterUIKitCanvas(UIKitCanvas canvas);
    }
}
