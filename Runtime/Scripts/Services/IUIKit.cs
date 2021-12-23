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
        /// <summary>
        /// TinaX的服务容器
        /// </summary>
        IServiceContainer Services { get; }

        /// <summary>
        /// 默认的控制器反射提供者
        /// 可以在框架启动前通过注册自己的服务来修改它。
        /// </summary>
        IControllerReflectionProvider DefaultControllerReflectionProvider { get; }

        UniTask<UIPageBase> GetUIPageAsync(string pageUri, CancellationToken cancellationToken = default);
        UniTask<UIPageBase> GetUIPageAsync(string pageUri, PageControllerBase controller, CancellationToken cancellationToken = default);
        UniTask<UIPageBase> GetUIPageAsync(GetUIPageArgs args, CancellationToken cancellationToken = default);
        void RegisterUIKitCanvas(UIKitCanvas canvas);
    }
}
