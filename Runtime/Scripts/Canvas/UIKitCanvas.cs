using TinaX.UIKit.Page;
using TinaX.UIKit.Page.Group;

namespace TinaX.UIKit.Canvas
{
    public class UIKitCanvas
    {
        public UIKitCanvas() : this(new UIPageGroup()) { }

        public UIKitCanvas(UIPageBase mainPage)
        {
            MainPage = mainPage;
        }

        public UIPageBase MainPage { get; private set; }


    }
}
