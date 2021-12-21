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
            m_RootGroup = rootGroup;
            this.Name = this.GetHashCode().ToString();
        }

        public UIKitCanvas(UIPageGroup rootGroup, string name)
        {
            m_RootGroup = rootGroup;
            this.Name = name;
        }

        public UIKitCanvas(UIPageBase mainPage, UIPageGroup rootGroup, string name)
        {
            m_RootGroup = rootGroup;
            rootGroup.Push(mainPage);
            this.Name = name;
        }

        protected UIPageGroup m_RootGroup;

        public virtual UIPageGroup RootGroup
        {
            get { return m_RootGroup; }
            protected set { m_RootGroup = value; }
        }

        public string Name { get; private set; }


        public override string ToString()
        {
            return $"[TinaX UIKit Canvas]{this.Name}";
        }
    }
#nullable restore
}
