namespace TinaX.UIKit.Page.View
{
#nullable enable
    public abstract class PageView
    {
        protected readonly string m_ViewUri;
        protected readonly UIPageBase m_Page;

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


        public virtual string ViewUri => m_ViewUri;


        /// <summary>
        /// Page 入栈时调用
        /// </summary>
        public abstract void Display();
    }

#nullable restore
}
