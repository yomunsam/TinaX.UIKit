using System.Threading;
using Cysharp.Threading.Tasks;

namespace TinaX.UIKit.Services
{
    public interface IUIKitInternalService
    {
        UniTask StartAsync(CancellationToken cancellationToken = default);
    }
}
