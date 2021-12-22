using TinaX.UIKit.Builder.OpenUIBuilders;
using TinaX.UIKit.Page.Navigator;

namespace TinaX.UIKit
{
    public static class PageNavigatorExtensions
    {
        //public static UniTask<UIPageBase> OpenUIAsync(this IPageNavigator navigator, string pageUri, CancellationToken cancellationToken = default)
        //{
        //    var openUIArgs = new OpenUIArgs(pageUri);
        //    return navigator.OpenUIAsync(openUIArgs, cancellationToken);
        //}

        public static PageNavigatorOpenUIBuilder CreateOpenUI(this IPageNavigator navigator, string pageUri)
            => new PageNavigatorOpenUIBuilder(navigator, pageUri);

    }
}
