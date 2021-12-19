using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.UIKit.Page;

namespace TinaX.UIKit
{
    public interface IUIKit
    {
        UniTask<UIPageBase> GetUIPageAsync(string pageUri, CancellationToken cancellationToken = default);
    }
}
