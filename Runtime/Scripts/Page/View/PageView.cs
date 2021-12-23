namespace TinaX.UIKit.Page.View
{
#nullable enable
    public abstract class PageView
    {
        //------------固定字段------------------------------------------------------------------------------------
        protected readonly string m_ViewUri;
        protected readonly UIPageBase m_Page;

        //------------构造函数----------------------------------------------------------------------------------
        public PageView(UIPageBase page)
        {
            m_ViewUri = string.Empty;
            this.m_Page = page;
        }
        public PageView(string viewUri, UIPageBase page)
        {
            m_ViewUri = viewUri;
            this.m_Page = page;
        }

        //------------内部字段------------------------------------------------------------------------------------

        /// <summary>
        /// 视图是否被隐藏（显示之后再隐藏才叫隐藏）
        /// </summary>
        protected bool m_Hidden;

        //------------公开属性---------------------------------------------------------------------------------------

        public virtual string ViewUri => m_ViewUri;

        public virtual bool IsHidden => m_Hidden;

        //------------公开方法---------------------------------------------------------------------------------------

        /// <summary>
        /// Page 入栈时调用
        /// </summary>
        public abstract void Display(object?[]? args);

        /// <summary>
        /// 设置序号，如UI显示顺序等和它油管
        /// </summary>
        /// <param name="order"></param>
        public abstract void SetOrder(int order);


        

        //------------内部方法---------------------------------------------------------------------------------------

    }

#nullable restore
}
