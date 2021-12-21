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

    }
#nullable restore
}
