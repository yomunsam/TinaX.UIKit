using TinaX.UIKit.Page.Navigator;

namespace TinaX.UIKit.Page.Controller
{
#nullable enable

    /// <summary>
    /// 页控制器 抽象基类
    /// </summary>
    public abstract class PageControllerBase
    {
        /// <summary>
        /// UI页
        /// </summary>
        public UIPageBase? Page { get; set; }

        /// <summary>
        /// 页面导航
        /// </summary>
        public IPageNavigator? Navigation { get; set; }

    }

#nullable restore
}
