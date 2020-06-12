using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.UIKit.Entity;

namespace TinaX.UIKit.Internal
{
    public interface IUIKitInternal
    {
        void CloseUI(UIEntity entity, params object[] args);
        Task<XException> Start();
    }
}
