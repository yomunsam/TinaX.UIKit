using System.Collections.Generic;

namespace TinaX.UIKit.Page.Group
{
    public class UIPageGroup : UIPageBase
    {
        private readonly List<UIPageBase> m_Children = new List<UIPageBase>();

        public UIPageGroup()
        {

        }

        private UIPageGroup m_Parent;

    }
}
