using TinaX.UIKit.Builder.GetPageBuilders;

namespace TinaX.UIKit
{
    public static class UIKitServiceExtensions
    {
        public static UIKitServiceGetPageBuilder CreateGetPage(this IUIKit uikit, string pageUri)
        {
            return new UIKitServiceGetPageBuilder(uikit, pageUri);
        }
    }
}
