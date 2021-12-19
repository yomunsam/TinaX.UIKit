using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Container;
using TinaX.Core.Behaviours;
using TinaX.Exceptions;
using TinaX.Module;
using TinaX.Modules;
using TinaX.UIKit.Consts;
using TinaX.UIKit.Services;

namespace TinaX.UIKit
{
    [ModuleProviderOrder]
    public class UIKitModule : IModuleProvider
    {
        public string ModuleName => UIKitConsts.ModuleName;

        public UniTask<ModuleBehaviourResult> OnInitAsync(IServiceContainer services, CancellationToken cancellationToken)
            => UniTask.FromResult(ModuleBehaviourResult.CreateSuccess(ModuleName));

        public void ConfigureBehaviours(IBehaviourManager behaviour, IServiceContainer services) { }

        public void ConfigureServices(IServiceContainer services)
        {
            services.Singleton<IUIKit, UIKitService>()
                .SetAlias<IUIKitInternalService>();
        }
        public async UniTask<ModuleBehaviourResult> OnStartAsync(IServiceContainer services, CancellationToken cancellationToken)
        {
            try
            {
                var uikit_internal = services.Get<IUIKitInternalService>();
                await uikit_internal.StartAsync(cancellationToken);
            }
            catch(XException ex)
            {
                return ModuleBehaviourResult.CreateFromException(ModuleName, ex);
            }
            return ModuleBehaviourResult.CreateSuccess(ModuleName);
        }


        public void OnQuit() { }

        public UniTask OnRestartAsync(IServiceContainer services, CancellationToken cancellationToken)
            => UniTask.CompletedTask;

        
    }
}
