using TinaX.UIKit.Page.Group;

namespace TinaX.UIKit.Page
{
#nullable enable
    public interface IPage
    {
        UIPageGroup? Parent { get; }
        string Name { get; }
        string PageUri { get; }
        int PageSize { get; }

        /// <summary>
        /// 页面被关闭了吗
        /// </summary>
        bool IsClosed { get; }

        void ClosePage(params object?[] closeMessageArgs);
        bool SendMessage(string messageName, object?[]? args);
    }
#nullable restore
}
