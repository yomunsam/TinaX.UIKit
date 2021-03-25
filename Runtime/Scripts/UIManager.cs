using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX;
using TinaX.Services;
using TinaX.UIKit.Internal;
using TinaX.UIKit.Const;
using UnityEngine.UI;
using TinaX.UIKit.Entity;
using TinaX.XComponent;
using UniRx;
using TinaX.UIKit.Pipelines.OpenUI;
using TinaX.Systems.Pipeline;
using Cysharp.Threading.Tasks;

namespace TinaX.UIKit
{
    public class UIManager : IUIKit, IUIKitInternal
    {
        [Inject]
        public IAssetService Assets { get; set; }

        [Inject]
        public IXCore m_Core { get; set; }

        private UIConfig mConfig;

        private UIGroup mCurUIGroup;
        private UINameMode mUINameMode;
        private string mUIRootDirLoadPath;
        private string mUIRootDirLoadPath_withSlash;

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
        private XPipeline<IOpenUIAsyncHandler> m_OpenUIAsyncPipeline = new XPipeline<IOpenUIAsyncHandler>();


        #endregion

        public Camera UICamera => mScreenUICamera;

        public XPipeline<IOpenUIAsyncHandler> OpenUIAsyncPipeline => m_OpenUIAsyncPipeline;

        public async Task<XException> Start()
        {
            #region config
            if (mInited) return null;
            mConfig = XConfig.GetConfig<UIConfig>(UIConst.ConfigPath_Resources);
            if (mConfig == null)
                return new XException("[TinaX.UIKit] Connot found config file.");

            if (!mConfig.EnableUIKit) return null;

            if (mConfig.UINameMode == UINameMode.UIGroup)
                mCurUIGroup = mConfig.DefaultUIGroup;
            else
            {
                mUIRootDirLoadPath = mConfig.UIRootDirectoryLoadPath;
                if (!mUIRootDirLoadPath.IsNullOrEmpty())
                {
                    if (mUIRootDirLoadPath.EndsWith("/"))
                        mUIRootDirLoadPath = mUIRootDirLoadPath.Substring(0, mUIRootDirLoadPath.Length - 1);
                    mUIRootDirLoadPath_withSlash = mUIRootDirLoadPath + "/";
                }
            }
            mUINameMode = mConfig.UINameMode;

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

            //Pipelines 初始化
            this.OpenUIAsyncConfigure(ref m_OpenUIAsyncPipeline);

            await Task.Yield();
            return null;
        }

        #region OpenUI方法

        //------------UIName 和 参数 -----------------------------------------------------------

        public IUIEntity OpenUI(string UIName, params object[] args)
        {
            string ui_path = null;
            if (mUINameMode == UINameMode.UIGroup)
            {
                if (mCurUIGroup != null)
                {
                    if (!mCurUIGroup.TryGetPath(UIName, out ui_path))
                        throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + UIName, UIKitErrorCode.InvalidUIName);
                }
            }
            else
                ui_path = (mUIRootDirLoadPath.IsNullOrEmpty()) ? UIName : mUIRootDirLoadPath_withSlash + UIName;

            if (ui_path.IsNullOrEmpty())
                throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);

            var entity = this.openUI(ui_path, UIName, null, null, false, false, false, default, args);
            return entity;
        }
        public async Task<IUIEntity> OpenUIAsync(string UIName, params object[] args)
        {
            string ui_path = null;
            if(mUINameMode == UINameMode.UIGroup)
            {
                if(mCurUIGroup != null)
                {
                    if (!mCurUIGroup.TryGetPath(UIName, out ui_path))
                        throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + UIName, UIKitErrorCode.InvalidUIName);
                }
            }
            else
            {
                ui_path = (mUIRootDirLoadPath.IsNullOrEmpty()) ? UIName : mUIRootDirLoadPath_withSlash + UIName;
            }

            if (ui_path.IsNullOrEmpty())
            {
                throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);
            }

            var entity = await openUIAsync(ui_path, UIName, null, null,false, false, false, default, args);
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
            string ui_path = null;
            if (mUINameMode == UINameMode.UIGroup)
            {
                if (mCurUIGroup != null)
                {
                    if (!mCurUIGroup.TryGetPath(UIName, out ui_path))
                        throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + UIName, UIKitErrorCode.InvalidUIName);
                }
            }
            else
                ui_path = (mUIRootDirLoadPath.IsNullOrEmpty()) ? UIName : mUIRootDirLoadPath_withSlash + UIName;

            if (ui_path.IsNullOrEmpty())
                throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);

            return this.openUI(ui_path, UIName, null, behaviour, true, false, false, default, args);
        }

        public IUIEntity OpenUI(string UIName, XBehaviour behaviour, OpenUIParam openUIParam, params object[] args)
        {
            string ui_path = null;
            if (mUINameMode == UINameMode.UIGroup)
            {
                if (mCurUIGroup != null)
                {
                    if (!mCurUIGroup.TryGetPath(UIName, out ui_path))
                        throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + UIName, UIKitErrorCode.InvalidUIName);
                }
            }
            else
                ui_path = (mUIRootDirLoadPath.IsNullOrEmpty()) ? UIName : mUIRootDirLoadPath_withSlash + UIName;

            if (ui_path.IsNullOrEmpty())
                throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);

            return this.openUI(ui_path,
                UIName,
                openUIParam.UIRoot,
                behaviour,
                openUIParam.DependencyInjection,
                openUIParam.UseMask,
                openUIParam.CloseByMask,
                (openUIParam.MaskColor == null) ? mConfig.DefaultUIMaskColor : openUIParam.MaskColor.Value,
                args);
        }

        //public async Task<IUIEntity> OpenUIAsync(string UIName, XBehaviour behaviour, params object[] args)
        //{
        //    string ui_path = null;
        //    if (mUINameMode == UINameMode.UIGroup)
        //    {
        //        if (mCurUIGroup != null)
        //        {
        //            if (!mCurUIGroup.TryGetPath(UIName, out ui_path))
        //                throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + UIName, UIKitErrorCode.InvalidUIName);
        //        }
        //    }
        //    else
        //        ui_path = (mUIRootDirLoadPath.IsNullOrEmpty()) ? UIName : mUIRootDirLoadPath_withSlash + UIName;

        //    if (ui_path.IsNullOrEmpty())
        //        throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);


        //    var entity = await openUIAsync(ui_path, UIName, null, behaviour, true, false, false, default, args);
        //    return entity;
        //}

        public async Task<IUIEntity> OpenUIAsync(string UIName, XBehaviour behaviour, params object[] args)
        {
            //尝试基于Pipeline的新写法

            var entity = await _PipelineOpenUIAsync(uiName: UIName, useMask: false, closeByMask: false, maskColor: null, uiRoot: null, xBehaviour: behaviour, DI: true, openUIParam: null, args);
            return entity;
        }

        public async Task<IUIEntity> OpenUIAsync(string UIName, XBehaviour behaviour, OpenUIParam openUIParam, params object[] args)
        {
            string ui_path = null;
            if (mUINameMode == UINameMode.UIGroup)
            {
                if (mCurUIGroup != null)
                {
                    if (!mCurUIGroup.TryGetPath(UIName, out ui_path))
                        throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + UIName, UIKitErrorCode.InvalidUIName);
                }
            }
            else
                ui_path = (mUIRootDirLoadPath.IsNullOrEmpty()) ? UIName : mUIRootDirLoadPath_withSlash + UIName;

            if (ui_path.IsNullOrEmpty())
                throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);


            var entity = await openUIAsync(ui_path,
                UIName,
                openUIParam.UIRoot,
                behaviour,
                openUIParam.DependencyInjection,
                openUIParam.UseMask,
                openUIParam.CloseByMask,
                (openUIParam.MaskColor == null) ? mConfig.DefaultUIMaskColor : openUIParam.MaskColor.Value,
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
            string ui_path = null;
            if (mUINameMode == UINameMode.UIGroup)
            {
                if (mCurUIGroup != null)
                {
                    if (!mCurUIGroup.TryGetPath(UIName, out ui_path))
                        throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + UIName, UIKitErrorCode.InvalidUIName);
                }
            }
            else
                ui_path = (mUIRootDirLoadPath.IsNullOrEmpty()) ? UIName : mUIRootDirLoadPath_withSlash + UIName;

            if (ui_path.IsNullOrEmpty())
                throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);

            var entity = this.openUI(ui_path, 
                UIName, 
                openUIParam.UIRoot, 
                openUIParam.xBehaviour, 
                openUIParam.DependencyInjection,
                openUIParam.UseMask, 
                openUIParam.CloseByMask, 
                (openUIParam.MaskColor == null) ? mConfig.DefaultUIMaskColor:openUIParam.MaskColor.Value, 
                args);
            return entity;
        }

        public IUIEntity OpenUIWithParam(string UIName, OpenUIParam openUIParam, params object[] args) => OpenUI(UIName, openUIParam, args);

        public async Task<IUIEntity> OpenUIAsync(string UIName, OpenUIParam openUIParam, params object[] args)
        {
            string ui_path = null;
            if (mUINameMode == UINameMode.UIGroup)
            {
                if (mCurUIGroup != null)
                {
                    if (!mCurUIGroup.TryGetPath(UIName, out ui_path))
                        throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + UIName, UIKitErrorCode.InvalidUIName);
                }
            }
            else
            {
                ui_path = (mUIRootDirLoadPath.IsNullOrEmpty()) ? UIName : mUIRootDirLoadPath_withSlash + UIName;
            }

            if (ui_path.IsNullOrEmpty())
            {
                throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);
            }

            var entity = await openUIAsync(ui_path,
                UIName,
                openUIParam.UIRoot,
                openUIParam.xBehaviour,
                openUIParam.DependencyInjection,
                openUIParam.UseMask,
                openUIParam.CloseByMask,
                (openUIParam.MaskColor == null) ? mConfig.DefaultUIMaskColor : openUIParam.MaskColor.Value,
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
            if (entity == null) return;
            this.closeUI(entity, args);
        }

        public void CloseUI(string UIName, params object[] args)
        {
            string ui_path = null;
            if (mUINameMode == UINameMode.UIGroup)
            {
                if (mCurUIGroup != null)
                {
                    if (!mCurUIGroup.TryGetPath(UIName, out ui_path))
                        throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + UIName, UIKitErrorCode.InvalidUIName);
                }
            }
            else
            {
                ui_path = (mUIRootDirLoadPath.IsNullOrEmpty()) ? UIName : mUIRootDirLoadPath_withSlash + UIName;
            }

            if (ui_path.IsNullOrEmpty())
            {
                throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);
            }

            if(this.UIEntities.TryGetEntitys(ui_path, out var entities))
            {
                foreach (var item in entities)
                    this.CloseUI(item, args);
            }
        }


        /// <summary>
        /// 私有方法总入口
        /// </summary>
        /// <param name="UINameOrPath"></param>
        /// <param name="ui_root">只有在UI不是ScreenUI的情况下才需要传递这个值</param>
        private async Task<UIEntity> openUIAsync(string uiPath, string uiName, Transform ui_root, XComponent.XBehaviour xBehaviour, bool inject, bool UseMask, bool CloseByMask, Color maskColor, params object[] args)
        {
            void setTop(UIEntity __entity)
            {
                //置顶
                if (mDict_UILayers.TryGetValue(__entity.SortingLayerValue, out var layer))
                    layer.Top(__entity);
            }
            //加载检查
            if (this.UIEntities.TryGetEntitys(uiPath,out var entities))
            {
                if(entities.Length > 0)
                {
                    if(entities[0].UIStatue == UIStatus.Loaded && !entities[0].AllowMultiple)
                    {
                        setTop(entities[0]);
                        return entities[0];
                    }

                    if(entities[0].UIStatue == UIStatus.Loading)
                    {
                        await entities[0].LoadUIPrefabTask;
                        if (!entities[0].AllowMultiple)
                        {
                            setTop(entities[0]);
                            return entities[0];
                        }
                    }
                }
            }

            //除了上面两种情况，其他都得重新加载
            UIEntity entity = new UIEntity(this, uiName, uiPath);
            entity.LoadUIPrefabTask = doOpenUIAsync(entity, ui_root, xBehaviour, inject, UseMask, CloseByMask, maskColor, args);
            this.UIEntities.Register(entity);

            await entity.LoadUIPrefabTask;

            return entity;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="_ui_root">只有在UI不是ScreenUI的情况下才需要传递这个值</param>
        /// <returns></returns>
        private async Task doOpenUIAsync(UIEntity entity, Transform _ui_root, XComponent.XBehaviour xBehaviour,bool inject ,bool UseMask,bool CloseByMask, Color maskColor, params object[] args)
        {
            if (entity.UIStatue != UIStatus.Loaded && entity.UIStatue != UIStatus.Unloaded)
                entity.UIStatue = UIStatus.Loading;
            if (entity.UIGameObject == null)
            {
                var prefab = await Assets.LoadAsync<GameObject>(entity.UIPath);
                entity.UIPrefab = prefab;
                var uiPage = prefab.GetComponent<UIPage>();
                if (uiPage == null)
                {
                    string ui_name = entity.UIName;
                    Assets.Release(prefab);
                    throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"无法打开UI \"{ui_name}\" , 这不是一个有效的UI页文件。" : $"Unable to open UI \"{ui_name}\", this is not a valid UI Page file."), UIKitErrorCode.InvalidUIPage);
                }
                if (uiPage.ScreenUI)
                {
                    //Screen UI， 使用UIKit Screen UIRoot

                    //if (!SortingLayer.layers.Any(sl=> sl.value == uiPage.SortingLayerValue))
                    //    uiPage.SortingLayerValue = 0;
                    Transform trans_uiroot = getScreenUIRoot(uiPage.SortingLayerId);
                    entity.UIGameObject = UnityEngine.GameObject.Instantiate(prefab, trans_uiroot);
                    if (entity.UIGameObject.name.Length > 7)
                        entity.UIGameObject.Name(entity.UIGameObject.name.Substring(0, entity.UIGameObject.name.Length - 7));

                    
                }
                else
                {
                    //非ScreenUI, 在指定的UIRoot下创建GameObject
                    entity.UIGameObject = UnityEngine.GameObject.Instantiate(prefab, _ui_root);
                    if (entity.UIGameObject.name.Length > 7)
                        entity.UIGameObject.Name(entity.UIGameObject.name.Substring(0, entity.UIGameObject.name.Length - 7));
                }
            }

            entity.UIPage = entity.UIGameObject.GetComponent<UIPage>();
            entity.UICanvas = entity.UIGameObject.GetComponentOrAdd<Canvas>();
            if (entity.UIPage.ScreenUI)
            {
                entity.UICanvas.overrideSorting = true;
                entity.UICanvas.sortingLayerID = entity.UIPage.SortingLayerId;

                //UI层级
                if (!mDict_UILayers.ContainsKey(entity.SortingLayerId))
                    mDict_UILayers.Add(entity.SortingLayerId, new UILayerManager());
                mDict_UILayers[entity.SortingLayerId].Register(entity);
            }

            //xbehaviour
            if (xBehaviour != null)
            {
                if (inject)
                    XCore.GetMainInstance().InjectObject(xBehaviour); //依赖注入，Services
                if (xBehaviour is XUIBehaviour)
                {
                    var uibehaviour = xBehaviour as XUIBehaviour;
                    uibehaviour.UIEntity = entity;
                    entity.UIPage.TrySetXBehavior(uibehaviour, inject);
                }
                else
                    entity.UIPage.TrySetXBehavior(xBehaviour, inject);
            }

            //mask
            if (UseMask)
                entity.ShowMask(CloseByMask, maskColor);

            //OpenUI事件
            if (args != null && args.Length > 0)
            {
                entity.UIPage.SendOpenUIMessage(args);
            }
            entity.UIStatue = UIStatus.Loaded;
            entity.LoadUIPrefabTask = Task.CompletedTask;
        }

        private UIEntity openUI(string uiPath, string uiName, Transform ui_root, XComponent.XBehaviour xBehaviour, bool inject, bool UseMask, bool CloseByMask, Color maskColor, params object[] args)
        {
            void setTop(UIEntity __entity)
            {
                //置顶
                if (mDict_UILayers.TryGetValue(__entity.SortingLayerValue, out var layer))
                    layer.Top(__entity);
            }

            //加载检查
            if (this.UIEntities.TryGetEntitys(uiPath, out var entities))
            {
                if (entities.Length > 0)
                {
                    if (entities[0].UIStatue == UIStatus.Loaded && !entities[0].AllowMultiple)
                    {
                        setTop(entities[0]);
                        return entities[0];
                    }
                }
            }

            /*
             * 异步加载一个UI开始后，立即同步加载同一个UI,这种情况不管。约定不应该在开发的时候出现这种情况
             */

            UIEntity entity = new UIEntity(this, uiName, uiPath);

            
            if (entity.UIGameObject == null)
            {
                var prefab = Assets.Load<GameObject>(entity.UIPath);
                var uiPage = prefab.GetComponent<UIPage>();
                if (uiPage == null)
                {
                    string ui_name = entity.UIName;
                    Assets.Release(prefab);
                    throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"无法打开UI \"{ui_name}\" , 这不是一个有效的UI页文件。" : $"Unable to open UI \"{ui_name}\", this is not a valid UI Page file."), UIKitErrorCode.InvalidUIPage);
                }

                if (uiPage.ScreenUI)
                {
                    //Screen UI， 使用UIKit Screen UIRoot
                    //if (!SortingLayer.layers.Any(sl => sl.value == uiPage.SortingLayerValue))
                    //    uiPage.SortingLayerValue = 0;
                    Transform trans_uiroot = getScreenUIRoot(uiPage.SortingLayerId);
                    entity.UIGameObject = UnityEngine.Object.Instantiate(prefab, trans_uiroot);
                    if (entity.UIGameObject.name.Length > 7)
                        entity.UIGameObject.Name(entity.UIGameObject.name.Substring(0, entity.UIGameObject.name.Length - 7));
                    
                }
                else
                {
                    //非ScreenUI, 在指定的UIRoot下创建GameObject
                    entity.UIGameObject = UnityEngine.Object.Instantiate(prefab, ui_root);
                    if (entity.UIGameObject.name.Length > 7)
                        entity.UIGameObject.Name(entity.UIGameObject.name.Substring(0, entity.UIGameObject.name.Length - 7));
                }
                Assets.Release(prefab);
            }

            entity.UIPage = entity.UIGameObject.GetComponent<UIPage>();
            entity.UICanvas = entity.UIGameObject.GetComponentOrAdd<Canvas>();
            if (entity.UIPage.ScreenUI)
            {
                entity.UICanvas.overrideSorting = true;
                entity.UICanvas.sortingLayerID = entity.UIPage.SortingLayerId;

                //UI层级
                if (!mDict_UILayers.ContainsKey(entity.SortingLayerId))
                    mDict_UILayers.Add(entity.SortingLayerId, new UILayerManager());
                mDict_UILayers[entity.SortingLayerId].Register(entity);
            }

            //xbehaviour
            if (xBehaviour != null)
            {
                if (inject)
                    XCore.GetMainInstance().InjectObject(xBehaviour); //依赖注入，Services

                if (xBehaviour is XUIBehaviour)
                {
                    var uibehaviour = xBehaviour as XUIBehaviour;
                    uibehaviour.UIEntity = entity;
                    entity.UIPage.TrySetXBehavior(uibehaviour, inject);
                }
                else
                    entity.UIPage.TrySetXBehavior(xBehaviour, inject);
            }

            //mask
            if (UseMask)
                entity.ShowMask(CloseByMask, maskColor);

            //OpenUI事件
            if (args != null && args.Length > 0)
            {
                entity.UIPage.SendOpenUIMessage(args);
            }
            entity.UIStatue = UIStatus.Loaded;
            entity.LoadUIPrefabTask = Task.CompletedTask;

            this.UIEntities.Register(entity);
            return entity;
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
#if TINAX_DEBUG_DEV
                Debug.Log("[UIKIT]进入处理LoadPath的Pipeline流程");
#endif
                switch (mUINameMode)
                {
                    case UINameMode.UIGroup:
                        if (mCurUIGroup != null)
                        {
                            if (mCurUIGroup.TryGetPath(payload.UIName, out var _loadpath))
                                payload.UILoadPath = _loadpath;
                            else
                                throw new UIKitException("[TinaX.UIKit] Invalid UIName : " + payload.UIName, UIKitErrorCode.InvalidUIName);
                        }
                        else
                            throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"无效的设置，UI组为空" : $"Invalid configuration: UI Group is empty."), UIKitErrorCode.InvalidUIGroup);

                        break;

                    case UINameMode.RelativeDirectory:
                        payload.UILoadPath = (mUIRootDirLoadPath.IsNullOrEmpty()) ? payload.UIName : mUIRootDirLoadPath_withSlash + payload.UIName;
                        break;
                }

                if (payload.UILoadPath.IsNullOrEmpty())
                    throw new UIKitException("[TinaX.UIKit] " + (IsChinese ? $"未能获取到UI \"{payload.UIName}\" 的加载路径，请检查设置或传入参数" : $"Cannot get UI Path by UI Name \"{payload.UIName}\", Please check config or args."), UIKitErrorCode.ConnotGetUIPath);

                return Task.FromResult<bool>(true);
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
                    await payload.UIEntity.LoadUIPrefabTask;
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
            pipeline.AddLast(new GeneralOpenUIAsyncHandler(OpenUIHandlerNameConst.Instantiates, (payload, next) =>
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

