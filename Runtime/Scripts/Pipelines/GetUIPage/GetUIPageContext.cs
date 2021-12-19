using TinaX.Container;
using TinaX.UIKit.Page;

namespace TinaX.UIKit.Pipelines.GetUIPage
{
    /// <summary>
    /// 获取UIPage 上下文
    /// </summary>
    public class GetUIPageContext
    {
        /// <summary>
        /// 是否终断Pipeline的标记
        /// </summary>
        public bool BreakPipeline { get; set; } = false;

        /// <summary>
        /// 终断Pipeline流程
        /// </summary>
        public void Break() => BreakPipeline = true;
        public void Break(UIPageBase page)
        {
            this.UIPageReuslt = page;
            BreakPipeline = true;
        }

        public IServiceContainer Services { get; set; }


        public UIPageBase UIPageReuslt { get; set; }
    }
}
