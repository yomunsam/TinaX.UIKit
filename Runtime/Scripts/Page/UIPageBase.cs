using System.Reflection;
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
    public abstract class UIPageBase
    {
#nullable enable

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

        protected PageControllerBase? m_Controller { get; set; }


        /// <summary>
        /// UI序号
        /// </summary>
        protected int m_Order { get; set; } = 0;

        /// <summary>
        /// UIPage位于哪一个UIKitCanvas上
        /// </summary>
        protected UIKitCanvas? m_Canvas;



        //------公开属性-----------------------------------------------------------------------------------------------

        /// <summary>
        /// View已显示
        /// </summary>
        public bool IsDisplayed => m_Displayed;


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


        //------公开方法-------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 准备好视图到内存（资产加载过程）
        /// </summary>
        /// <returns></returns>
        public abstract UniTask ReadyViewAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 初始化显示一个View
        /// </summary>
        public abstract void DisplayView(object[]? args);



        public abstract void HideView();

        /// <summary>
        /// UIPage被加入一个组
        /// </summary>
        public virtual void OnJoinGroup(UIPageGroup group, object[]? displayMessageArgs)
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
        }


        /// <summary>
        /// 对Controller发出UI OnDisplay消息
        /// </summary>
        public virtual void SendUIDisplayMessage(object[]? args)
        {
            if(m_Controller != null)
            {
                //使用接口
                if(m_Controller is IUIDisplayMessage displayMsg)
                {
                    displayMsg.OnDisplay(args);
                    return;
                }

                //反射调用
                var controllerType = m_Controller.GetType();
                var method = controllerType.GetMethod(UIMessageNameConsts.OnDisplay, BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if(method != null)
                {
                    var methodParams = method.GetParameters();
                    if(methodParams.Length == 0) //无参数
                    {
                        method.Invoke(m_Controller, null);
                        return;
                    }
                }
            }
        }

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

#nullable restore
    }
}
