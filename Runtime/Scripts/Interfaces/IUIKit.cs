using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.UIKit
{
    public interface IUIKit
    {
        Task<IUIEntity> OpenUI(string UIName, params object[] args);
    }
}
