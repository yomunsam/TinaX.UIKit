using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.UIKit.Internal
{
    public interface IUIKitInternal
    {
        XException GetStartException();
        Task<bool> Start();
    }
}
