using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.UIKit.Page;
using TinaX.UIKit.Page.Controller;

namespace TinaX.UIKit
{
    public interface IUIKit
    {
        UniTask<UIPageBase> GetUIPageAsync(string pageUri, CancellationToken cancellationToken = default);
        UniTask<UIPageBase> GetUIPageAsync(string pageUri, PageControllerBase controller, CancellationToken cancellationToken = default);
    }
}
