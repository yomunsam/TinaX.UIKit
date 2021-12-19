using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.UIKit.Page;

namespace TinaX.UIKit.Providers.PageProvider
{
    public interface IPageProvider
    {
#nullable enable
        UniTask<UIPageBase?> TryGetPage(string pageUri, CancellationToken cancellationToken = default);
#nullable restore
    }
}
