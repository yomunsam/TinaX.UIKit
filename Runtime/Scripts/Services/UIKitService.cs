using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Container;
using TinaX.Options;
using TinaX.Systems.Pipeline;
using TinaX.UIKit.Canvas;
using TinaX.UIKit.Consts;
using TinaX.UIKit.Options;
using TinaX.UIKit.Page;
using TinaX.UIKit.Page.Controller;
using TinaX.UIKit.Pipelines.GetUIPage;
using TinaX.UIKit.Providers.UIKitProvider;
using UnityEngine;

namespace TinaX.UIKit.Services
{
    public class UIKitService : IUIKit, IUIKitInternalService
    {
        private readonly UIKitOptions m_Options;
        private readonly UIKitProvidersManager m_UIKitProvidersManager;
        private readonly IXCore m_XCore;
        private readonly UIKitCanvasManager m_UIKitCanvasManager = new UIKitCanvasManager();

        public UIKitService(IOptions<UIKitOptions> options,
            UIKitProvidersManager uIKitProvidersManager,
            IXCore core)
        {
            m_Options = options.Value;
            this.m_UIKitProvidersManager = uIKitProvidersManager;
            this.m_XCore = core;
        }

        private bool m_Initialized;
        private XPipeline<IGetUIPageAsyncHandler> m_GetUIPageAsyncPipeline = new XPipeline<IGetUIPageAsyncHandler>();


        public IServiceContainer Services => m_XCore.Services;


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

            //准备GetUIPage管线
            m_UIKitProvidersManager.ConfigureGetUIPagePipeline(m_GetUIPageAsyncPipeline, m_XCore.Services);

            m_Initialized = true;
        }

        //public async UniTask<TPage> LoadUIAsync<TPage>(string uri, CancellationToken cancellationToken = default) 
        //    where TPage : UIPageBase
        //{

        //}

        #region UIKit Canvas
        public void RegisterUIKitCanvas(UIKitCanvas canvas)
        {
            m_UIKitCanvasManager.Add(canvas);
        }

        #endregion

        public UniTask<UIPageBase> GetUIPageAsync(string pageUri, CancellationToken cancellationToken = default)
        {
            return DoGetUIPageAsync(new GetUIPagePayload(pageUri.Trim()), cancellationToken);
        }
        
        public UniTask<UIPageBase> GetUIPageAsync(string pageUri, PageControllerBase controller, CancellationToken cancellationToken = default)
        {
            var payload = new GetUIPagePayload(pageUri.Trim())
            {
                PageController = controller,
            };
            return DoGetUIPageAsync(payload, cancellationToken);
        }

        public UniTask<UIPageBase> GetUIPageAsync(GetUIPageArgs args, CancellationToken cancellationToken = default)
        {
            var payload = new GetUIPagePayload(args.PageUri.Trim())
            {
                PageController = args.PageController
            };
            return DoGetUIPageAsync(payload, cancellationToken);
        }
        

        private async UniTask<UIPageBase> DoGetUIPageAsync(GetUIPagePayload payload, CancellationToken cancellationToken = default)
        {
            //上下文
            var context = new GetUIPageContext(m_XCore.Services);

            //开始队列
            await m_GetUIPageAsyncPipeline.StartAsync(async handler =>
            {
                await handler.GetPageAsync(context, payload, cancellationToken);
                return !context.BreakPipeline; //返回值表示pipeline是否继续
            });

            return payload.UIPage;
        }
    }
}
