using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.UIKit
{
    public class UIKit : CatLib.Facade<IUIKit>
    {
        public static IUIKit Instance => UIKit.That;
        public static IUIKit I => UIKit.That;
    }
}
