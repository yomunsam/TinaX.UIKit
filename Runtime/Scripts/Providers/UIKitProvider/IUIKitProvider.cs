using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Container;
using TinaX.Core.Behaviours;
using TinaX.Systems.Pipeline;
using TinaX.UIKit.Pipelines.GetUIPage;

namespace TinaX.UIKit.Providers.UIKitProvider
{
    public interface IUIKitProvider
    {
        string ProviderName { get; }

        void ConfigureServices(IServiceContainer services);

        void ConfigureBehaviours(IBehaviourManager behaviour, IServiceContainer services);

        UniTask StartAsync(IServiceContainer services, CancellationToken cancellationToken = default);

        void ConfigureGetUIPagePipeline(ref XPipeline<IGetUIPageAsyncHandler> pipeline, ref IServiceContainer services);
    }
}
