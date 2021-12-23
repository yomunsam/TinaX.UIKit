using TinaX.UIKit.Page.Controller;

namespace TinaX.UIKit
{
#nullable enable
    public class GetUIPageArgs
    {
        public GetUIPageArgs(string pagrUri)
        {
            PageUri = pagrUri;
        }

        public string PageUri { get; set; }

        public PageControllerBase? PageController { get; set; }

        /// <summary>
        /// 指定控制器反射提供者
        /// </summary>
        public IControllerReflectionProvider? ControllerReflectionProvider { get; set; }

    }
#nullable restore
}
