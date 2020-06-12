using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX;
using TinaX.Services;
using TinaX.UIKit;
using TinaX.UIKit.Const;
using TinaX.UIKit.Internal;

namespace TinaX.UIKit
{
    [XServiceProviderOrder(100)]
    public class UIKitProvider : IXServiceProvider
    {

        public string ServiceName => UIConst.ServiceName;

        public Task<XException> OnInit(IXCore core)
            => Task.FromResult<XException>(null);


        public void OnServiceRegister(IXCore core)
        {
            core.Services.Singleton<IUIKit, UIManager>()
                .SetAlias<IUIKitInternal>();
        }


        public Task<XException> OnStart(IXCore core)
        {
            return core.Services.Get<IUIKitInternal>().Start();
        }
        
        public void OnQuit() { }

        public Task OnRestart() => Task.CompletedTask;


    }
}
