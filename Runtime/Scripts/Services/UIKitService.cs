using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Options;
using TinaX.UIKit.Consts;
using TinaX.UIKit.Options;
using TinaX.UIKit.Page;
using TinaX.UIKit.Providers.UIKitProvider;
using UnityEngine;

namespace TinaX.UIKit.Services
{
    public class UIKitService : IUIKit, IUIKitInternalService
    {
        private readonly UIKitOptions m_Options;
        private readonly UIKitProvidersManager m_UIKitProvidersManager;
        private readonly IXCore m_XCore;

        public UIKitService(IOptions<UIKitOptions> options,
            UIKitProvidersManager uIKitProvidersManager,
            IXCore core)
        {
            m_Options = options.Value;
            this.m_UIKitProvidersManager = uIKitProvidersManager;
            this.m_XCore = core;
        }


        private bool m_Initialized;


        public async UniTask StartAsync(CancellationToken cancellationToken = default)
        {
            if (m_Initialized)
                return;
            if (!m_Options.Enable)
                return;

            //启动UIKit 提供者
#if TINAX_DEV
            Debug.Log($"[{UIKitConsts.ModuleName}]启动所有UIKit Provider");
#endif
            await m_UIKitProvidersManager.StartAllAsync(m_XCore.Services, cancellationToken);


            m_Initialized = true;
        }

        //public async UniTask<TPage> LoadUIAsync<TPage>(string uri, CancellationToken cancellationToken = default) 
        //    where TPage : UIPageBase
        //{

        //}
        
        public UniTask<UIPageBase> GetUIPageAsync(string pageUri, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        

    }
}
