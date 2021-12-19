using TinaX.UIKit.Page;
using TinaX.UIKit.Page.Group;

namespace TinaX.UIKit.Canvas
{
#nullable enable

    public class UIKitCanvas
    {
        public UIKitCanvas() : this(new UIPageGroup()) { }

        public UIKitCanvas(UIPageGroup rootGroup)
        {
            RootGroup = rootGroup;
            this.Name = this.GetHashCode().ToString();
        }

        public UIKitCanvas(UIPageGroup rootGroup, string name)
        {
            RootGroup = rootGroup;
            this.Name = name;
        }

        public UIKitCanvas(UIPageBase mainPage, UIPageGroup rootGroup, string name)
        {
            this.RootGroup = rootGroup;
            rootGroup.Push(mainPage);
            this.Name = name;
        }

        public UIPageGroup RootGroup { get; private set; }

        public string Name { get; private set; }


        public override string ToString()
        {
            return $"[TinaX UIKit Canvas]{this.Name}";
        }
    }
#nullable restore
}
