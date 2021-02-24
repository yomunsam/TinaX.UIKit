using TinaX.UIKit;

namespace TinaX.Services
{
    public static class UIKitServiceExtension
    {
        public static IXCore UseUIKit(this IXCore core)
        {
            core.RegisterServiceProvider(new UIKitProvider());
            return core;
        }
    }
}
