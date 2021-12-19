using System;
using System.Collections.Generic;
using System.ComponentModel;
using TinaX.Container;
using TinaX.Options;
using TinaX.UIKit.Options;
using TinaX.UIKit.Providers.UIKitProvider;

namespace TinaX.UIKit.Builder
{
    public class UIKitBuilder
    {
        private readonly List<IUIKitProvider> m_Providers = new List<IUIKitProvider>();

        public UIKitBuilder(IServiceContainer services)
            => Services = services ?? throw new ArgumentNullException(nameof(services));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly IServiceContainer Services;

        public UIKitBuilder AddUIKitProvider(IUIKitProvider provider)
        {
            if(provider == null)
                throw new ArgumentNullException(nameof(provider));

            m_Providers.AddIfNotExist(provider);
            return this;
        }

        public UIKitBuilder Configure(Action<UIKitOptions> configuration)
        {
            if(configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.Services.AddOptions();
            this.Services.Configure<UIKitOptions>(configuration);
            return this;
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<IUIKitProvider> GetUIKitProviders()
            => m_Providers;

    }
}
