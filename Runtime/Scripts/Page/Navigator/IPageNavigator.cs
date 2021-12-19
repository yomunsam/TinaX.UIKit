using Cysharp.Threading.Tasks;

namespace TinaX.UIKit.Page.Navigator
{
    /// <summary>
    /// 页面导航器
    /// </summary>
    public interface IPageNavigator
    {
        UniTask PushAsync(UIPageBase page);
    }
}
