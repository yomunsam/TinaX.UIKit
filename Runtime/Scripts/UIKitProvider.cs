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
        private IXCore mCore;

        public string ServiceName => UIConst.ServiceName;

        public Task<bool> OnInit()
        {
            mCore = XCore.GetMainInstance();

            return Task.FromResult(true);
        }

        public XException GetInitException() => null;


        public void OnServiceRegister()
        {
            mCore.BindSingletonService<IUIKit, UIManager>().SetAlias<IUIKitInternal>();
        }


        public Task<bool> OnStart()
        {
            return mCore.GetService<IUIKitInternal>().Start();
        }
        public XException GetStartException()
        {
            return mCore.GetService<IUIKitInternal>().GetStartException();
        }



        public void OnQuit() { }

        public Task OnRestart() => Task.CompletedTask;


    }
}
