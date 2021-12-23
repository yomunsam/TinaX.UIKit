using TinaX.UIKit.Page;
using TinaX.UIKit.Page.Controller;

namespace TinaX.UIKit.Pipelines.GetUIPage
{
#nullable enable
    public class GetUIPagePayload
    {

        public GetUIPagePayload(GetUIPageArgs args, IControllerReflectionProvider defaultControllerReflectionProvider)
        {
            this.GetUIPageArgs = args;
            PageUriLower = args.PageUri.ToLower();
            DefaultControllerReflectionProvider = defaultControllerReflectionProvider;
        }

        public GetUIPageArgs GetUIPageArgs;

        public string PageUriLower { get; set; }

        public IControllerReflectionProvider DefaultControllerReflectionProvider;

        /// <summary>
        /// 存放得到的UIPage
        /// </summary>
        public UIPageBase? UIPage { get; set; }
    }

#nullable restore
}
