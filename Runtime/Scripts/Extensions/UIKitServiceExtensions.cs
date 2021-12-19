using System;
using TinaX.Options;
using TinaX.UIKit;
using TinaX.UIKit.Builder;
using TinaX.UIKit.Options;
using TinaX.UIKit.Providers.UIKitProvider;

namespace TinaX.Services
{
    public static class UIKitServiceExtensions
    {

        public static IXCore AddUIKit(this IXCore core, Action<UIKitBuilder> uikitBuilder)
        {
            //---------------------------------------------------------------------------------
            //因为这个Builder不复杂，所有我们就直接在这儿顺便实现builder模式里的Director，不另外写个class了

            var builder = new UIKitBuilder(core.Services);
            uikitBuilder?.Invoke(builder);
            //Options
            if(!core.Services.TryGet<IOptions<UIKitOptions>>(out _))
            {
                core.Services.AddOptions();
                core.Services.Configure<UIKitOptions>(options => { });
            }
            //UIKit Providers
            var uikitProviders = builder.GetUIKitProviders();
            for(int i = 0; i < uikitProviders.Count; i++)
            {
                uikitProviders[i].ConfigureServices(core.Services);
                uikitProviders[i].ConfigureBehaviours(core.Behaviour, core.Services);
            }
            var uiKitProviderManager = new UIKitProvidersManager(uikitProviders);
            core.Services.Singleton<UIKitProvidersManager>(uiKitProviderManager);

            //------------------------------------------------------------------------
            core.AddModule(new UIKitModule());

            return core;
        }

    }
}
