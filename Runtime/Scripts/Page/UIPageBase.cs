using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.UIKit.Canvas;
using TinaX.UIKit.Consts;
using TinaX.UIKit.Page.Controller;
using TinaX.UIKit.Page.Group;
using TinaX.UIKit.Page.View;
using TinaX.UIKit.UIMessage;

namespace TinaX.UIKit.Page
{
    public abstract class UIPageBase : IPage
    {
#nullable enable

        //------固定字段--------------------------------------------------------------------------------------------
        protected List<Action<IPage>> m_OnDisplayedActions = new List<Action<IPage>>();
        protected List<Action<IPage>> m_OnHiddenActions = new List<Action<IPage>>();
        protected List<Action<IPage>> m_OnShowedActions = new List<Action<IPage>>();
        protected List<Action<IPage>> m_OnClosedActions = new List<Action<IPage>>();
        protected List<Action<IPage>> m_OnActiveActions = new List<Action<IPage>>(); //活跃，当UI页被置顶时调用


        //------构造函数--------------------------------------------------------------------------------------------
        public UIPageBase()
        {
        }

        public UIPageBase(string pageUri, IPageViewProvider viewProvider)
        {
            this.m_PageUri = pageUri;
            this.m_ViewProvider = viewProvider;
        }

        public UIPageBase(string pageUri, IPageViewProvider viewProvider, PageControllerBase? controller)
        {
            this.m_PageUri = pageUri;
            this.m_ViewProvider = viewProvider;
            m_Controller = controller;

            if (controller!= null)
            {
                controller.Page = this;
                m_Name = controller.GetType().Name;
            }
        }

        //------内部字段------------------------------------------------------------------------------------------------

        /// <summary>
        /// 是否已显示
        /// </summary>
        protected bool m_Displayed = false;

        /// <summary>
        /// 是否已关闭
        /// </summary>
        protected bool m_Closed = false;

        protected bool m_Closing = false; //是否正在关闭，因为播放UI动画等原因，关闭可能需要有延时

        /// <summary>
        /// Page所属的组（可能为空）
        /// </summary>
        protected UIPageGroup? m_Parent;

        /// <summary>
        /// Page的View
        /// </summary>
        protected PageView? m_Content { get; set; }

        protected IPageViewProvider? m_ViewProvider { get; set; }

        protected string m_PageUri { get; set; } = string.Empty;

        protected string m_Name { get; set; } = string.Empty;

        protected PageControllerBase? m_Controller;

        protected Type? m_ControllerType;


        /// <summary>
        /// UI序号
        /// </summary>
        protected int m_Order { get; set; } = 0;

        /// <summary>
        /// UIPage位于哪一个UIKitCanvas上
        /// </summary>
        protected UIKitCanvas? m_Canvas;

        /// <summary>
        /// UI页关闭延迟秒数
        /// </summary>
        protected float m_CloseDelaySeconds = 0;

        //------公开属性-----------------------------------------------------------------------------------------------

        /// <summary>
        /// View已显示
        /// </summary>
        public bool IsDisplayed => m_Displayed;

        public bool IsClosed => m_Closed;


        public PageControllerBase? Controller => m_Controller;

        public virtual UIPageGroup? Parent => m_Parent;

        public virtual string Name => m_Name;
        public virtual string PageUri => m_PageUri;


        /// <summary>
        /// 总Page的尺寸（包括自己和子项）
        /// </summary>
        public virtual int PageSize => 1;

        /// <summary>
        /// UI序号
        /// </summary>
        public virtual int Order
        {
            get => m_Order;
            set
            {
                m_Order = value;
                m_Content?.SetOrder(value);
            }
        }


        /// <summary>
        /// UIPage位于哪一个UIKitCanvas上
        /// </summary>
        public virtual UIKitCanvas? Canvas
        {
            get
            {
                if (m_Canvas == null)
                    m_Canvas = FindCanvas();
                return m_Canvas;
            }
            set
            {
                m_Canvas = value;
            }
        }


        //UI显示状态---------

        public virtual bool IsViewHidden => m_Content?.IsHidden ?? false;

        /// <summary>
        /// 控制器反射提供者
        /// </summary>
        public IControllerReflectionProvider? ControllerReflectionProvider { get; set; }

        /// <summary>
        /// UI关闭延迟时间
        /// </summary>
        public TimeSpan CloseDelayTime => TimeSpan.FromSeconds(m_CloseDelaySeconds);

        //------公开方法-------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 准备好视图到内存（资产加载过程）
        /// </summary>
        /// <returns></returns>
        public abstract UniTask ReadyViewAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 初始化显示一个View
        /// </summary>
        public abstract void DisplayView(object?[]? args);



        public abstract void HideView();

        /// <summary>
        /// 从Page关闭UIPage的调用入口
        /// </summary>
        public abstract void ClosePage(params object?[] closeMessageArgs);


        /// <summary>
        /// UIPage被加入一个组
        /// </summary>
        public virtual void OnJoinGroup(UIPageGroup group, object?[]? displayMessageArgs)
        {
            m_Parent = group;
        }

        /// <summary>
        /// UIPage被移出了一个组
        /// </summary>
        /// <param name="group"></param>
        public virtual void OnLeaveGroup(UIPageGroup group)
        {
            if (m_Parent != null && m_Parent == group)
                m_Parent = null;
            m_Canvas = null;//如果我们离开了一个组，说明我们以前进入过一个组，这就说明我们肯定不是Canvas的根级组，所以要把Canvas清空；
        }


        /// <summary>
        /// 对Controller发出UI OnDisplay消息
        /// </summary>
        public virtual bool SendUIDisplayMessage(object?[]? args)
        {
            if(m_Controller != null)
            {
                //使用专有接口
                if(m_Controller is IUIDisplayMessage displayMsg)
                {
                    displayMsg.OnDisplayed(args);
                    return true;
                }

                //没有专有接口，使用通用方式传递消息
                return this.SendMessage(UIMessageNameConsts.OnDisplayed, args);
            }

            return false;
        }

        public virtual bool SendUIClosedMessage(object?[]? args)
        {
            if(m_Controller == null)
            {
                //专有接口
                if(m_Controller is IUIClosedMessage closedMsg)
                {
                    closedMsg.OnClosed(args);
                    return true;
                }

                //没有专用接口，尝试通用方式传递消息
                return this.SendMessage(UIMessageNameConsts.OnClosed, args);
            }

            return false;
        }

        /// <summary>
        /// 向Controller发送消息
        /// </summary>
        /// <param name="messageName">消息名</param>
        /// <param name="args">消息参数</param>
        /// <returns>是否发送成功</returns>
        public virtual bool SendMessage(string messageName, object?[]? args)
        {
            //首先尝试反射调用
            if (ControllerReflectionProvider != null) 
            {
                if(m_Controller != null)
                {
                    if (ControllerReflectionProvider.TrySendMessage(m_Controller, ref m_ControllerType, messageName, args))
                    {
                        return true; //这儿调用成功，方法返回了
                    }
                }
            }
            return false;
        }

        //UI关闭延迟时间------

        /// <summary>
        /// 设置 关闭UI页延迟时间
        /// 默认情况下，如果调用了多次的话，会用时间长的覆盖时间短的
        /// </summary>
        /// <param name="seconds">时间（秒）</param>
        /// <param name="cover">覆盖，强制使用本次设置的时间为UI页的延迟时间</param>
        public void SetCloseDelayTime(float seconds, bool cover = false)
        {
            if(cover || seconds > m_CloseDelaySeconds)
                m_CloseDelaySeconds = seconds;
        }

        /// <summary>
        /// 在UI页现有的延迟时间基础上添加指定的延迟时间
        /// </summary>
        /// <param name="seconds"></param>
        public void AddCloseDelayTime(float seconds)
        {
            m_CloseDelaySeconds += seconds;
        }

        //------UI Actions
        public void OnPageDisplayed(Action<IPage> action)
        {
            if(!m_OnDisplayedActions.Contains(action))
                m_OnDisplayedActions.Add(action);
        }

        public void OnPageClosed(Action<IPage> action)
        {
            if(!m_OnClosedActions.Contains(action))
                m_OnClosedActions.Add(action);
        }


        //------------私有方法们---------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 寻找Canvas
        /// </summary>
        /// <returns></returns>
        protected virtual UIKitCanvas? FindCanvas()
        {
            if (m_Parent == null)
                return m_Canvas;
            else
                return m_Parent.FindCanvas();
        }

        protected virtual void InvokeOnDisplayedActions()
        {
            for(int i = 0; i < m_OnDisplayedActions.Count; i++)
            {
                m_OnDisplayedActions[i]?.Invoke(this);
            }
        }

        protected virtual void InvokeOnClosedActions()
        {
            for (int i = 0; i < m_OnClosedActions.Count; i++)
            {
                m_OnClosedActions[i]?.Invoke(this);
            }
        }
    }
#nullable restore

}
