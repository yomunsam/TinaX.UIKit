using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TinaX.Services;
using TinaX.Systems.Pipeline;
using TinaX.UIKit.Const;
using TinaX.UIKit.Entity;
using TinaX.UIKit.Internal;
using TinaX.UIKit.Pipelines.OpenUI;
using TinaX.UIKit.Router;
using TinaX.XComponent;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TinaX.UIKit
{
    public class UIManager : IUIKit, IUIKitInternal
    {
        [Inject]
        public IAssetService Assets { get; set; }

        [Inject]
        public IXCore m_Core { get; set; }

        private UIConfig mConfig;

        //private UIGroup mCurUIGroup;
        //private UINameMode mUINameMode;
        //private string mUIRootDirLoadPath;
        //private string mUIRootDirLoadPath_withSlash;

        /// <summary>
        /// UIName到加载地址的路由接口
        /// </summary>
        private IRouter m_Router;

        private bool mInited = false;

        private GameObject mUIKit_Root_Go;
        //private Canvas mScreenUIRoot_Canvas;
        private Camera mScreenUICamera;
        private Dictionary<int, GameObject> mDict_UIRoot_SortingLayer_Go = new Dictionary<int, GameObject>();
        private Dictionary<int, Canvas> mDict_UIRootCanvas_SortingLayer_Canvas = new Dictionary<int, Canvas>();
        /// <summary>
        /// Key: SortingLayer
        /// </summary>
        private Dictionary<int, UILayerManager> mDict_UILayers = new Dictionary<int, UILayerManager>();

        private UIEntityManager UIEntities = new UIEntityManager();

        #region Pipelines

        /// <summary>
        /// 执行“异步打开UI”流程时候的管线
        /// </summary>
        private readonly XPipeline<IOpenUIAsyncHandler> m_OpenUIAsyncPipeline = new XPipeline<IOpenUIAsyncHandler>();

        /// <summary>
        /// 执行“同步打开UI”流程时候的管线
        /// </summary>
        private readonly XPipeline<IOpenUIHandler> m_OpenUIPipeline = new XPipeline<IOpenUIHandler>();


        #endregion

        public Camera UICamera => mScreenUICamera;

        public XPipeline<IOpenUIAsyncHandler> OpenUIAsyncPipeline => m_OpenUIAsyncPipeline;
        public XPipeline<IOpenUIHandler> OpenUIPipeline => m_OpenUIPipeline;

        public UIManager()
        {
            //Pipelines 初始化
            this.OpenUIAsyncConfigure(ref m_OpenUIAsyncPipeline);
            this.OpenUIConfigure(ref m_OpenUIPipeline);
        }

        public async Task<XException> Start()
        {
            #region config
            if (mInited) return null;
            mConfig = XConfig.GetConfig<UIConfig>(UIConst.ConfigPath_Resources);
            if (mConfig == null)
                return new XException("[TinaX.UIKit] Connot found config file.");

            if (!mConfig.EnableUIKit) return null;

            if(!m_Core.Services.TryGet<IRouter>(out m_Router))
            {
                //没有发现开发者外部注入的路由，所以使用内置路由
                switch (mConfig.UINameMode)
                {
                    default:
                        throw new UIKitException("Unknow UI Name Mode: " + mConfig.UINameMode.ToString(), UIKitErrorCode.Unknow);
                    case UINameMode.UIGroup:
                        m_Router = new UIGroupRouter(mConfig.DefaultUIGroup);
                        break;
                    case UINameMode.RelativeDirectory:
                        m_Router = new RelativeDirectoryRouter(mConfig.UIRootDirectoryLoadPath);
                        break;
                }
            }

            //if (mConfig.UINameMode == UINameMode.UIGroup)
            //    mCurUIGroup = mConfig.DefaultUIGroup;
            //else
            //{
            //    mUIRootDirLoadPath = mConfig.UIRootDirectoryLoadPath;
            //    if (!mUIRootDirLoadPath.IsNullOrEmpty())
            //    {
            //        if (mUIRootDirLoadPath.EndsWith("/"))
            //            mUIRootDirLoadPath = mUIRootDirLoadPath.Substring(0, mUIRootDirLoadPath.Length - 1);
            //        mUIRootDirLoadPath_withSlash = mUIRootDirLoadPath + "/";
            //    }
            //}
            //mUINameMode = mConfig.UINameMode;

            #endregion


            //Init UIKit GameObjects
            #region UIKit GameObjects
            mUIKit_Root_Go = XCore.GetMainInstance().BaseGameObject
                .FindOrCreateGameObject("UIKit")
                .SetPosition(new Vector3(-9999, -9999, -9999));

            if (mConfig.UseUICamera)
            {
                var camera_config = mConfig.UICameraConfig;
                if (camera_config == null)
                    camera_config = new UICameraConfig();

                mScreenUICamera = mUIKit_Root_Go.FindOrCreateGameObject("UICamera")
                    .AddComponent<Camera>();
                mScreenUICamera.clearFlags = camera_config.clearFlags;
                mScreenUICamera.backgroundColor = camera_config.backgroundColor;
                mScreenUICamera.cullingMask = camera_config.cullingMask;
                mScreenUICamera.orthographic = camera_config.orthographic;
                mScreenUICamera.orthographicSize = camera_config.orthographicSize;
                mScreenUICamera.nearClipPlane = camera_config.nearClipPlane;
                mScreenUICamera.farClipPlane = camera_config.farClipPlane;
                mScreenUICamera.depth = camera_config.depth;
                mScreenUICamera.renderingPath = camera_config.renderingPath;
                mScreenUICamera.targetTexture = camera_config.targetTexture;
                mScreenUICamera.useOcclusionCulling = camera_config.useOcclusionCulling;
                mScreenUICamera.allowHDR = camera_config.allowHDR;
                mScreenUICamera.allowMSAA = camera_config.allowMSAA;

                
            }

            //Default UIRoot
            refreshUIRoot(0);

            //EventSystem
#if ENABLE_LEGACY_INPUT_MANAGER
            if (mConfig.AutoCreateEventSystem)
            {
                var es_go = GameObjectHelper.FindOrCreateGameObject("EventSystem");
                var event_system = es_go.GetComponentOrAdd<UnityEngine.EventSystems.EventSystem>();
                event_system.sendNavigationEvents = true;
                event_system.pixelDragThreshold = 10;
                var input_module = es_go.GetComponentOrAdd<UnityEngine.EventSystems.StandaloneInputModule>();
            }
#endif

            #endregion

            

            await Task.Yield();
            return null;
        }

        

        #region OpenUI方法

        //------------UIName 和 参数 -----------------------------------------------------------

        public IUIEntity OpenUI(string UIName, params object[] args)
        {
            var entity = this._PipelineOpenUI(uiName: UIName,
                useMask: false,
                closeByMask: false,
                maskColor: null,
                uiRoot: null,
                xBehaviour: null,
                DI: true,
                openUIParam: null,
                args);
            return entity;
        }
        public async Task<IUIEntity> OpenUIAsync(string UIName, params object[] args)
        {
            var entity = await _PipelineOpenUIAsync(uiName: UIName,
                useMask: false,
                closeByMask: false,
                maskColor: null,
                uiRoot: null,
                xBehaviour: null,
                DI: true,
                openUIParam: null,
                args);
            return entity;
        }
        public void OpenUIAsync(string UIName, Action<IUIEntity, XException> callback, params object[] args)
        {
            this.OpenUIAsync(UIName, args)
                .ToObservable()
                .ObserveOnMainThread()
                .Subscribe(entity =>
                {
                    callback?.Invoke(entity, null);
                },
                e => 
                {
                    if (e is XException)
                        callback?.Invoke(null, e as XException);
                    else
                        Debug.LogException(e);
                });
        }
        //------------------------------------------------------------------------------------------------------
        
        public IUIEntity OpenUI(string UIName, XBehaviour behaviour, params object[] args)
        {
            return this._PipelineOpenUI(uiName: UIName,
                useMask: false,
                closeByMask: false,
                maskColor: null,
                uiRoot: null,
                xBehaviour: behaviour,
                DI: true,
                openUIParam: null,
                args);
        }

        public IUIEntity OpenUI(string UIName, XBehaviour behaviour, OpenUIParam openUIParam, params object[] args)
        {
            //有oepnUIParam的话，优先使用这个
            if (openUIParam.UIName.IsNullOrEmpty())
                openUIParam.UIName = UIName;
            if (openUIParam.xBehaviour == null)
                openUIParam.xBehaviour = behaviour;

            return this._PipelineOpenUI(uiName: default,
                useMask: default,
                closeByMask: default,
                maskColor: default,
                uiRoot: default,
                xBehaviour: default,
                DI: default,
                openUIParam: openUIParam,
                args);
        }

        public async Task<IUIEntity> OpenUIAsync(string UIName, XBehaviour behaviour, params object[] args)
        {
            //尝试基于Pipeline的新写法

            var entity = await _PipelineOpenUIAsync(uiName: UIName, 
                useMask: false, 
                closeByMask: false, 
                maskColor: null, 
                uiRoot: null, 
                xBehaviour: behaviour, 
                DI: true, 
                openUIParam: null, 
                args);
            return entity;
        }

        public async Task<IUIEntity> OpenUIAsync(string UIName, XBehaviour behaviour, OpenUIParam openUIParam, params object[] args)
        {
            //有oepnUIParam的话，优先使用这个
            if (openUIParam.UIName.IsNullOrEmpty())
                openUIParam.UIName = UIName;
            if (openUIParam.xBehaviour == null)
                openUIParam.xBehaviour = behaviour;

            var entity = await _PipelineOpenUIAsync(uiName: default,
                useMask: default,
                closeByMask: default,
                maskColor: default,
                uiRoot: default,
                xBehaviour: default,
                DI: default,
                openUIParam: openUIParam,
                args);
            return entity;
        }

        public void OpenUIAsync(string UIName, XBehaviour behaviour, Action<IUIEntity, XException> callback, params object[] args)
        {
            this.OpenUIAsync(UIName, behaviour, args)
                .ToObservable()
                .ObserveOnMainThread()
                .Subscribe(entity =>
                {
                    callback?.Invoke(entity, null);
                },
                e =>
                {
                    if (e is XException)
                        callback?.Invoke(null, e as XException);
                    else
                        Debug.LogException(e);
                });
        }

        public void OpenUIAsync(string UIName, XBehaviour behaviour, OpenUIParam openUIParam, Action<IUIEntity, XException> callback, params object[] args)
        {
            this.OpenUIAsync(UIName, behaviour, openUIParam, args)
                .ToObservable()
                .ObserveOnMainThread()
                .Subscribe(entity =>
                {
                    callback?.Invoke(entity, null);
                },
                e =>
                {
                    if (e is XException)
                        callback?.Invoke(null, e as XException);
                    else
                        Debug.LogException(e);
                });
        }


        //------------全都有-------------------------------------------------------------------------------------
        public IUIEntity OpenUI(string UIName, OpenUIParam openUIParam, params object[] args)
        {
            //有oepnUIParam的话，优先使用这个
            if (openUIParam.UIName.IsNullOrEmpty())
                openUIParam.UIName = UIName;

            return this._PipelineOpenUI(uiName: default,
                useMask: default,
                closeByMask: default,
                maskColor: default,
                uiRoot: default,
                xBehaviour: default,
                DI: default,
                openUIParam: openUIParam,
                args);
        }

        public IUIEntity OpenUIWithParam(string UIName, OpenUIParam openUIParam, params object[] args) => OpenUI(UIName, openUIParam, args);

        public async Task<IUIEntity> OpenUIAsync(string UIName, OpenUIParam openUIParam, params object[] args)
        {
            if (openUIParam.UIName.IsNullOrEmpty())
                openUIParam.UIName = UIName;

            var entity = await _PipelineOpenUIAsync(uiName: default,
                useMask: default,
                closeByMask: default,
                maskColor: default,
                uiRoot: default,
                xBehaviour: default,
                DI: default,
                openUIParam: openUIParam,
                args);
            return entity;
        }

        public void OpenUIAsync(string UIName, OpenUIParam openUIParam, Action<IUIEntity, XException> callback, params object[] args)
        {
            this.OpenUIAsync(UIName, openUIParam, args)
                .ToObservable()
                .ObserveOnMainThread()
                .Subscribe(
                entity =>
                {
                    callback?.Invoke(entity, null);
                },
                e =>
                {
                    if (e is XException)
                        callback?.Invoke(null, e as XException);
                    else
                        Debug.LogException(e);
                });
        }

        public void OpenUIWithParamAsync(string UIName, OpenUIParam openUIParam, Action<IUIEntity, XException> callback, params object[] args) => OpenUIAsync(UIName, openUIParam, callback, args);

        #endregion

        public void CloseUI(UIEntity entity , params object[] args)
        {
            if (entity == null) 
                return;
            if (entity.Closed)
                return;
            this.closeUI(entity, args);
        }

        public void CloseUI(string UIName, params object[] args)
        {
            //string ui_path = null;
            //if (mUINameMode == UINameMode.UIGroup)
            //{
            //    if (mCurUIGroup != null)
            //    {
            //        if (!mCurUIGroup.TryGetPath(UIName, out ui_path))
            //            throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + UIName, UIKitErrorCode.InvalidUIName);
            //    }
            //}
            //else
            //{
            //    ui_path = (mUIRootDirLoadPath.IsNullOrEmpty()) ? UIName : mUIRootDirLoadPath_withSlash + UIName;
            //}

            //if (ui_path.IsNullOrEmpty())
            //{
            //    throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);
            //}

            //if(this.UIEntities.TryGetEntitys(ui_path, out var entities))
            //{
            //    foreach (var item in entities)
            //        this.CloseUI(item, args);
            //}

            var entities = this.UIEntities.FindByUIName(UIName).ToArray();
            if(entities != null && entities.Length > 0)
            {
                for (var i = entities.Length - 1; i >= 0; i--)
                    this.closeUI(entities[i], args);
            }
        }

        public void SetUIRouter(IRouter router)
        {
            m_Router = router ?? throw new ArgumentNullException(nameof(router));
        }
        

        /// <summary>
        /// [新] Pipeline实现版本私有方法总入口
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="useMask"></param>
        /// <param name="closeByMask"></param>
        /// <param name="maskColor"></param>
        /// <param name="uiRoot"></param>
        /// <param name="xBehaviour"></param>
        /// <param name="DI">启用依赖注入</param>
        /// <param name="openUIParam"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task<UIEntity> _PipelineOpenUIAsync(string uiName, //广义上的UIName, 可能是UI Group里的Name,也有可能是别的命名方式定义的UI Name，甚至直接是路径
            bool useMask,
            bool closeByMask,
            Color? maskColor,
            Transform uiRoot,
            XBehaviour xBehaviour,
            bool DI,
            OpenUIParam openUIParam,
            object[] args)
        {
            OpenUIPayload payload = new OpenUIPayload(); 
            if(openUIParam != null) //openUIParam里的参数优先级大于零散参数
            {
                //要是Unity有AutoMapper用就好了, 先手动抄一遍吧，以后想想看有啥好方法优化一下
                payload.UIName = openUIParam.UIName;
                payload.UseMask = openUIParam.UseMask;
                payload.CloseByMask = openUIParam.CloseByMask;
                payload.MaskColor = openUIParam.MaskColor;
                payload.UIRoot = openUIParam.UIRoot;
                payload.xBehaviour = openUIParam.xBehaviour;
                payload.DependencyInjection = openUIParam.DependencyInjection;
            }
            else
            {
                payload.UIName = uiName;
                payload.UseMask = useMask;
                payload.CloseByMask = closeByMask;
                payload.MaskColor = maskColor;
                payload.UIRoot = uiRoot;
                payload.xBehaviour = xBehaviour;
                payload.DependencyInjection = DI;
            }

            payload.OpenUIArgs = args;

            //挨个跑一下Pipeline
            await m_OpenUIAsyncPipeline.StartAsync((handler, next) =>
            {
                return handler.OpenUIAsync(payload, next);
            });

            //然后照理说，Pipeline中会把payload里的UIEntity赋值，我们就不用管了
            return payload.UIEntity;
        }

        private UIEntity _PipelineOpenUI(string uiName, //广义上的UIName, 可能是UI Group里的Name,也有可能是别的命名方式定义的UI Name，甚至直接是路径
            bool useMask,
            bool closeByMask,
            Color? maskColor,
            Transform uiRoot,
            XBehaviour xBehaviour,
            bool DI,
            OpenUIParam openUIParam,
            object[] args)
        {
            OpenUIPayload payload = new OpenUIPayload();
            if (openUIParam != null) //openUIParam里的参数优先级大于零散参数
            {
                //要是Unity有AutoMapper用就好了, 先手动抄一遍吧，以后想想看有啥好方法优化一下
                payload.UIName = openUIParam.UIName;
                payload.UseMask = openUIParam.UseMask;
                payload.CloseByMask = openUIParam.CloseByMask;
                payload.MaskColor = openUIParam.MaskColor;
                payload.UIRoot = openUIParam.UIRoot;
                payload.xBehaviour = openUIParam.xBehaviour;
                payload.DependencyInjection = openUIParam.DependencyInjection;
            }
            else
            {
                payload.UIName = uiName;
                payload.UseMask = useMask;
                payload.CloseByMask = closeByMask;
                payload.MaskColor = maskColor;
                payload.UIRoot = uiRoot;
                payload.xBehaviour = xBehaviour;
                payload.DependencyInjection = DI;
            }

            payload.OpenUIArgs = args;

            //挨个跑一下Pipeline
            m_OpenUIPipeline.Start((handler, next) =>
            {
                return handler.OpenUI(ref payload, next);
            });

            //然后照理说，Pipeline中会把payload里的UIEntity赋值，我们就不用管了
            return payload.UIEntity;
        }


        /// <summary>
        /// 关闭UI  私有方法总入口
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="args"></param>
        private void closeUI(UIEntity entity, params object[] args)
        {
            //退还layer
            if(mDict_UILayers.TryGetValue(entity.SortingLayerValue,out var layer))
            {
                layer.Remove(entity);
            }
            //移除登记
            UIEntities.Remove(entity);
            //传递参数
            if (args != null && args.Length>0)
                entity.UIPage.SendCloseUIMessage(args);
            //
            entity.Dispose();
            entity = null;
        }

        private Transform getScreenUIRoot(int sortingLayerId)
        {
            if (mDict_UIRoot_SortingLayer_Go.TryGetValue(sortingLayerId, out var go))
                return go.transform;
            else
            {
                refreshUIRoot(sortingLayerId);
                return mDict_UIRoot_SortingLayer_Go[sortingLayerId].transform;
            }
        }

        private void refreshUIRoot(int sortingLayerId)
        {
            if (SortingLayer.IsValid(sortingLayerId))
            {
                GameObject ui_root_go;
                if (!mDict_UIRoot_SortingLayer_Go.TryGetValue(sortingLayerId, out ui_root_go))
                {
                    ui_root_go = mUIKit_Root_Go.FindOrCreateGameObject("UIRoot_" + sortingLayerId,
                        typeof(Canvas),
                        typeof(CanvasScaler),
                        typeof(GraphicRaycaster))
                        .SetLocalPosition(Vector3.zero);
                    mDict_UIRoot_SortingLayer_Go.Add(sortingLayerId, ui_root_go);
                }

                var canvas = ui_root_go.GetComponentOrAdd<Canvas>();
                if (!mDict_UIRootCanvas_SortingLayer_Canvas.ContainsKey(sortingLayerId))
                    mDict_UIRootCanvas_SortingLayer_Canvas.Add(sortingLayerId, canvas);

                canvas.sortingLayerID = sortingLayerId;
                if(mScreenUICamera == null)
                {
                    canvas.worldCamera = null;
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                }
                else
                {
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = mScreenUICamera;
                }

                var canvasScaler = ui_root_go.GetComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = mConfig.UICanvasScalerMode;
                canvasScaler.scaleFactor = mConfig.UIScaleFactor;
                canvasScaler.referencePixelsPerUnit = mConfig.ReferencePixelsPerUnit;

                canvasScaler.referenceResolution = mConfig.ReferenceResolution;
                canvasScaler.screenMatchMode = mConfig.ScreenMatchMode;
                canvasScaler.matchWidthOrHeight = mConfig.CanvasScalerMatchWidthOrHeight;

                canvasScaler.physicalUnit = mConfig.PhySicalUnit;
                canvasScaler.fallbackScreenDPI = mConfig.FallbackScreenDPI;
                canvasScaler.defaultSpriteDPI = mConfig.DefaultSpriteDPI;
            }
        }

        /// <summary>
        /// 配置“异步打开UI”的管线内容
        /// </summary>
        /// <param name="pipeline"></param>
        private void OpenUIAsyncConfigure(ref XPipeline<IOpenUIAsyncHandler> pipeline)
        {
            //处理UILoadPath
            pipeline.AddLast(new GeneralOpenUIAsyncHandler(OpenUIHandlerNameConst.GetUILoadPath, (payload, next) =>
            {
                return Task.FromResult<bool>(_Pipeline_Func_GetUILoadPath(payload));
            }));

            //检查是否已加载 (检查是否已经打开UI, 如果UI已经打开了，并且该UI设置不允许打开多个的话，则把已存在的UI置顶，并不再加载
            pipeline.AddLast(new GeneralOpenUIAsyncHandler(OpenUIHandlerNameConst.CheckLoaded, async (payload, next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]进入判断是否重复加载的Pipeline流程");
#endif
                void setTop(UIEntity __entity)
                {
                    //置顶
                    if (mDict_UILayers.TryGetValue(__entity.SortingLayerValue, out var layer))
                        layer.Top(__entity);
                }

                if (this.UIEntities.TryGetEntitys(payload.UILoadPath, out var entities))
                {
                    if(entities.Length > 0)
                    {
                        if(entities[0].UIStatue == UIStatus.Loaded && !entities[0].AllowMultiple)
                        {
                            payload.UIEntity = entities[0];
                            await UniTask.NextFrame();
                            setTop(payload.UIEntity);
                            await UniTask.NextFrame();
                            return false; //中断后续操作
                        }

                        if(entities[0].UIStatue == UIStatus.Loading)
                        {
                            await entities[0].LoadUIPrefabTask;
                            if (!entities[0].AllowMultiple)
                            {
                                payload.UIEntity = entities[0];
                                await UniTask.NextFrame();
                                setTop(payload.UIEntity);
                                return false; //中断后续操作
                            } //否则继续加载这个UI
                        }
                    }
                }

                return true;
            }));

            //创建UIEntity的流程
            pipeline.AddLast(new GeneralOpenUIAsyncHandler(OpenUIHandlerNameConst.CreateUIEntity, (payload, next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]进入创建UIEntity的Pipeline流程");
#endif
                payload.UIEntity = new UIEntity(this, payload.UIName, payload.UILoadPath);
                payload.UIEntity.LoadUIPrefabTask = DoLoadUIPrefabAsync(payload.UIEntity);
                this.UIEntities.Register(payload.UIEntity);

                return Task.FromResult(true);
            }));

            //加载Prefab的流程
            pipeline.AddLast(new GeneralOpenUIAsyncHandler(OpenUIHandlerNameConst.LoadPrefab, async (payload, next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]进入加载UI Prefab的Pipeline流程");
#endif
                if(payload.UIEntity.UIPrefab == null)
                {
                    try
                    {
                        await payload.UIEntity.LoadUIPrefabTask;
                    }
                    catch(Exception e)
                    {
                        Debug.LogException(e);
                        return false;
                    }
                }
                
                return true;
            }));

            //从prefab到GameObject的流程
            pipeline.AddLast(new GeneralOpenUIAsyncHandler(OpenUIHandlerNameConst.Instantiates, (payload, next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]进入从UI prefab 到 GameObject的Pipeline流程");
#endif
                var uiPage = payload.UIEntity.UIPrefab.GetComponent<UIPage>();
                if (uiPage == null)
                {
                    string ui_name = payload.UIName;
                    Assets.Release(payload.UIEntity.UIPrefab);
                    payload.UIEntity.UIPrefab = null;
                    throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"无法打开UI \"{ui_name}\" , 这不是一个有效的UI页文件。" : $"Unable to open UI \"{ui_name}\", this is not a valid UI Page file."), UIKitErrorCode.InvalidUIPage);
                }

                if (uiPage.ScreenUI)
                {
                    Transform trans_UIRoot = getScreenUIRoot(uiPage.SortingLayerId);
                    payload.UIEntity.UIGameObject = UnityEngine.GameObject.Instantiate(payload.UIEntity.UIPrefab, trans_UIRoot);
                    if (payload.UIEntity.UIGameObject.name.Length > 7)
                        payload.UIEntity.UIGameObject.Name(payload.UIEntity.UIGameObject.name.Substring(0, payload.UIEntity.UIGameObject.name.Length - 7));
                }
                else
                {
                    //非ScreenUI, 在指定的UIRoot下创建GameObject
                    payload.UIEntity.UIGameObject = UnityEngine.GameObject.Instantiate(payload.UIEntity.UIPrefab, payload.UIRoot);
                    if (payload.UIEntity.UIGameObject.name.Length > 7)
                        payload.UIEntity.UIGameObject.Name(payload.UIEntity.UIGameObject.name.Substring(0, payload.UIEntity.UIGameObject.name.Length - 7));
                }

                payload.UIEntity.UIPage = payload.UIEntity.UIGameObject.GetComponent<UIPage>();
                payload.UIEntity.UICanvas = payload.UIEntity.UIGameObject.GetComponentOrAdd<Canvas>();

                if (payload.UIEntity.UIPage.ScreenUI)
                {
                    payload.UIEntity.UICanvas.overrideSorting = true;
                    payload.UIEntity.UICanvas.sortingLayerID = payload.UIEntity.UIPage.SortingLayerId;

                    //UI层级
                    if (!mDict_UILayers.ContainsKey(payload.UIEntity.SortingLayerId))
                        mDict_UILayers.Add(payload.UIEntity.SortingLayerId, new UILayerManager());
                    mDict_UILayers[payload.UIEntity.SortingLayerId].Register(payload.UIEntity);
                }

                return Task.FromResult(true);
            }));

            //xBehaviour，处理UI Main Handler为xBehaviour的情况
            pipeline.AddLast(new GeneralOpenUIAsyncHandler(OpenUIHandlerNameConst.XBehaviour, (payload, next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]pipeline处理UI Main Handler是xBehaviour的情况");
#endif
                if(payload.xBehaviour != null)
                {
                    if (payload.DependencyInjection)
                        m_Core.Services.Inject(payload.xBehaviour);
                    if(payload.xBehaviour is XUIBehaviour)
                    {
                        var uiBehaviour = payload.xBehaviour as XUIBehaviour;
                        uiBehaviour.UIEntity = payload.UIEntity;
                    }
                    payload.UIEntity.UIPage.TrySetXBehavior(payload.xBehaviour, payload.DependencyInjection);
                }

                return Task.FromResult(true);
            }));

            //处理UI遮罩
            pipeline.AddLast(new GeneralOpenUIAsyncHandler(OpenUIHandlerNameConst.UIMask, (payload, next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]Pipeline处理UI遮罩");
#endif
                if (payload.UseMask)
                {
                    payload.UIEntity.ShowMask(payload.CloseByMask, payload.MaskColor ?? mConfig.DefaultUIMaskColor);
                }

                return Task.FromResult(true);
            }));

            //处理UI发送事件相关
            pipeline.AddLast(new GeneralOpenUIAsyncHandler(OpenUIHandlerNameConst.SendMessage, (payload, next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]Pipeline处理UI打开事件");
#endif
                if(payload.OpenUIArgs != null && payload.OpenUIArgs.Length > 0)
                {
                    payload.UIEntity.UIPage.SendOpenUIMessage(payload.OpenUIArgs);
                }

                return Task.FromResult(true);
            }));

            //收尾工作
            pipeline.AddLast(new GeneralOpenUIAsyncHandler(OpenUIHandlerNameConst.Finish, (payload, next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]Pipeline收尾工作");
#endif
                payload.UIEntity.UIStatue = UIStatus.Loaded;
                payload.UIEntity.LoadUIPrefabTask = Task.CompletedTask;

                return Task.FromResult(true);
            }));

        }

        /// <summary>
        /// 配置“同步打开UI”的管线内容
        /// </summary>
        /// <param name="pipeline"></param>
        private void OpenUIConfigure(ref XPipeline<IOpenUIHandler> pipeline)
        {
            //处理UILoadPath
            pipeline.AddLast(new GeneralOpenUIHandler(OpenUIHandlerNameConst.GetUILoadPath, (ref OpenUIPayload payload, IOpenUIHandler next) =>
            {
                return _Pipeline_Func_GetUILoadPath(payload);
            }));

            //检查是否已加载 (检查是否已经打开UI, 如果UI已经打开了，并且该UI设置不允许打开多个的话，则把已存在的UI置顶，并不再加载
            pipeline.AddLast(new GeneralOpenUIHandler(OpenUIHandlerNameConst.CheckLoaded, (ref OpenUIPayload payload, IOpenUIHandler next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]进入判断是否重复加载的Pipeline流程");
#endif
                void setTop(UIEntity __entity)
                {
                    //置顶
                    if (mDict_UILayers.TryGetValue(__entity.SortingLayerValue, out var layer))
                        layer.Top(__entity);
                }

                if (this.UIEntities.TryGetEntitys(payload.UILoadPath, out var entities))
                {
                    if (entities.Length > 0)
                    {
                        if (entities[0].UIStatue == UIStatus.Loaded && !entities[0].AllowMultiple)
                        {
                            payload.UIEntity = entities[0];
                            setTop(payload.UIEntity);
                            return false; //中断后续操作
                        }
                    }
                }

                /*
                 * 异步加载一个UI开始后，立即同步加载同一个UI,这种情况不管。约定不应该在开发的时候出现这种情况
                 */

                return true;
            }));

            //创建UIEntity的流程
            pipeline.AddLast(new GeneralOpenUIHandler(OpenUIHandlerNameConst.CreateUIEntity, (ref OpenUIPayload payload, IOpenUIHandler next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]进入创建UIEntity的Pipeline流程");
#endif
                payload.UIEntity = new UIEntity(this, payload.UIName, payload.UILoadPath);
                payload.UIEntity.LoadUIPrefabTask = Task.CompletedTask;
                this.UIEntities.Register(payload.UIEntity);

                return true;
            }));

            //加载Prefab的流程
            pipeline.AddLast(new GeneralOpenUIHandler(OpenUIHandlerNameConst.LoadPrefab, (ref OpenUIPayload payload, IOpenUIHandler next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]进入加载UI Prefab的Pipeline流程");
#endif
                if (payload.UIEntity.UIPrefab == null)
                {
                    payload.UIEntity.UIPrefab = this.Assets.Load<GameObject>(payload.UIEntity.UIPath);
                }

                return true;
            }));

            //从prefab到GameObject的流程
            pipeline.AddLast(new GeneralOpenUIHandler(OpenUIHandlerNameConst.Instantiates, (ref OpenUIPayload payload, IOpenUIHandler next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]进入从UI prefab 到 GameObject的Pipeline流程");
#endif
                var uiPage = payload.UIEntity.UIPrefab.GetComponent<UIPage>();
                if (uiPage == null)
                {
                    string ui_name = payload.UIName;
                    Assets.Release(payload.UIEntity.UIPrefab);
                    payload.UIEntity.UIPrefab = null;
                    throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"无法打开UI \"{ui_name}\" , 这不是一个有效的UI页文件。" : $"Unable to open UI \"{ui_name}\", this is not a valid UI Page file."), UIKitErrorCode.InvalidUIPage);
                }

                if (uiPage.ScreenUI)
                {
                    Transform trans_UIRoot = getScreenUIRoot(uiPage.SortingLayerId);
                    payload.UIEntity.UIGameObject = UnityEngine.GameObject.Instantiate(payload.UIEntity.UIPrefab, trans_UIRoot);
                    if (payload.UIEntity.UIGameObject.name.Length > 7)
                        payload.UIEntity.UIGameObject.Name(payload.UIEntity.UIGameObject.name.Substring(0, payload.UIEntity.UIGameObject.name.Length - 7));
                }
                else
                {
                    //非ScreenUI, 在指定的UIRoot下创建GameObject
                    payload.UIEntity.UIGameObject = UnityEngine.GameObject.Instantiate(payload.UIEntity.UIPrefab, payload.UIRoot);
                    if (payload.UIEntity.UIGameObject.name.Length > 7)
                        payload.UIEntity.UIGameObject.Name(payload.UIEntity.UIGameObject.name.Substring(0, payload.UIEntity.UIGameObject.name.Length - 7));
                }

                payload.UIEntity.UIPage = payload.UIEntity.UIGameObject.GetComponent<UIPage>();
                payload.UIEntity.UICanvas = payload.UIEntity.UIGameObject.GetComponentOrAdd<Canvas>();

                if (payload.UIEntity.UIPage.ScreenUI)
                {
                    payload.UIEntity.UICanvas.overrideSorting = true;
                    payload.UIEntity.UICanvas.sortingLayerID = payload.UIEntity.UIPage.SortingLayerId;

                    //UI层级
                    if (!mDict_UILayers.ContainsKey(payload.UIEntity.SortingLayerId))
                        mDict_UILayers.Add(payload.UIEntity.SortingLayerId, new UILayerManager());
                    mDict_UILayers[payload.UIEntity.SortingLayerId].Register(payload.UIEntity);
                }

                return true;
            }));

            //xBehaviour，处理UI Main Handler为xBehaviour的情况
            pipeline.AddLast(new GeneralOpenUIHandler(OpenUIHandlerNameConst.XBehaviour, (ref OpenUIPayload payload, IOpenUIHandler next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]pipeline处理UI Main Handler是xBehaviour的情况");
#endif
                if (payload.xBehaviour != null)
                {
                    if (payload.DependencyInjection)
                        m_Core.Services.Inject(payload.xBehaviour);
                    if (payload.xBehaviour is XUIBehaviour)
                    {
                        var uiBehaviour = payload.xBehaviour as XUIBehaviour;
                        uiBehaviour.UIEntity = payload.UIEntity;
                    }
                    payload.UIEntity.UIPage.TrySetXBehavior(payload.xBehaviour, payload.DependencyInjection);
                }

                return true;
            }));

            //处理UI遮罩
            pipeline.AddLast(new GeneralOpenUIHandler(OpenUIHandlerNameConst.UIMask, (ref OpenUIPayload payload, IOpenUIHandler next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]Pipeline处理UI遮罩");
#endif
                if (payload.UseMask)
                {
                    payload.UIEntity.ShowMask(payload.CloseByMask, payload.MaskColor ?? mConfig.DefaultUIMaskColor);
                }

                return true;
            }));

            //处理UI发送事件相关
            pipeline.AddLast(new GeneralOpenUIHandler(OpenUIHandlerNameConst.SendMessage, (ref OpenUIPayload payload, IOpenUIHandler next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]Pipeline处理UI打开事件");
#endif
                if (payload.OpenUIArgs != null && payload.OpenUIArgs.Length > 0)
                {
                    payload.UIEntity.UIPage.SendOpenUIMessage(payload.OpenUIArgs);
                }

                return true;
            }));

            //收尾工作
            pipeline.AddLast(new GeneralOpenUIHandler(OpenUIHandlerNameConst.Finish, (ref OpenUIPayload payload, IOpenUIHandler next) =>
            {
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]Pipeline收尾工作");
#endif
                payload.UIEntity.UIStatue = UIStatus.Loaded;
                payload.UIEntity.LoadUIPrefabTask = Task.CompletedTask;

                return true;
            }));

        }


        

        /// <summary>
        /// 执行加载UIPrefab的方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>

        private async Task DoLoadUIPrefabAsync(UIEntity entity)
        {
            var prefab = await this.Assets.LoadAsync<GameObject>(entity.UIPath);
            entity.UIPrefab = prefab;
        }


        #region OpenUI Pipeline Functions
        //OpenUI Pipeline可共用的部分放在这里

        private bool _Pipeline_Func_GetUILoadPath(OpenUIPayload payload)
        {
#if TINAX_DEBUG_DEV
            Debug.Log("[UIKIT]进入处理LoadPath的Pipeline流程");
#endif
            if (m_Router.TryGetUILoadPath(payload.UIName, out string _loadPath))
            {
                payload.UILoadPath = _loadPath;
            }
            else
            {
                throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + payload.UIName, UIKitErrorCode.InvalidUIName);
            }


            if (payload.UILoadPath.IsNullOrEmpty())
                throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{payload.UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{payload.UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);

            return true;
        }

        #endregion


        private static bool? _ischinese;
        private static bool IsChinese
        {
            get
            {
                if (_ischinese == null)
                {
                    _ischinese = (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified);
                }
                return _ischinese.Value;
            }
        }
    }
}

