using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Container;
using TinaX.Core.Behaviours;

namespace TinaX.UIKit.Providers.UIKitProvider
{
    public interface IUIKitProvider
    {
        string ProviderName { get; }

        void ConfigureServices(IServiceContainer services);

        void ConfigureBehaviours(IBehaviourManager behaviour, IServiceContainer services);

        UniTask StartAsync(IServiceContainer services, CancellationToken cancellationToken = default);
    }
}
