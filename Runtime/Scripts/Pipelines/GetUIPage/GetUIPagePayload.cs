using TinaX.UIKit.Page;
using TinaX.UIKit.Page.Controller;

namespace TinaX.UIKit.Pipelines.GetUIPage
{
#nullable enable
    public class GetUIPagePayload
    {
        public GetUIPagePayload(string pageUri)
        {
            this.PageUri = pageUri;
            PageUriLower = pageUri.ToLower();
        }

        public string PageUri { get; set; }
        public string PageUriLower { get; set; }

        public UIPageBase? UIPage { get; set; }

        public PageControllerBase? PageController { get; set; }
    }

#nullable restore
}
