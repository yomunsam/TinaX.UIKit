using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Container;
using TinaX.UIKit.Providers.PageProvider;

namespace TinaX.UIKit.Providers.UIKitProvider
{

    /// <summary>
    /// UIKit 提供者 管理器
    /// </summary>
    public class UIKitProvidersManager
    {
        private readonly List<IUIKitProvider> m_Providers;

        public UIKitProvidersManager()
        {
            m_Providers = new List<IUIKitProvider>();
        }

        public UIKitProvidersManager(int capacity)
        {
            m_Providers = new List<IUIKitProvider>(capacity);
        }

        public UIKitProvidersManager(IEnumerable<IUIKitProvider> collection)
        {
            m_Providers = new List<IUIKitProvider>(collection);
        }


        public void Add(IUIKitProvider provider)
        {
            if(provider == null)
                throw new ArgumentNullException(nameof(provider));
            m_Providers.AddIfNotExist(provider);
        }

        public void AddRange(IEnumerable<IUIKitProvider> providers)
        {
            if(providers == null)
                throw new ArgumentNullException(nameof(providers));

            var privider_enumerator = providers.GetEnumerator();
            while (privider_enumerator.MoveNext())
            {
                m_Providers.AddIfNotExist(privider_enumerator.Current);
            }
        }

        public async UniTask StartAllAsync(IServiceContainer services, CancellationToken cancellationToken = default)
        {
            if (m_Providers.Count == 0)
                return;
            List<UniTask> tasks = new List<UniTask>(m_Providers.Count);
            for (int i = 0; i < m_Providers.Count; i++)
            {
                tasks.Add(m_Providers[i].StartAsync(services, cancellationToken));
            }
            await UniTask.WhenAll(tasks);
        }


    }
}
