using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX;

namespace TinaX.UIKit
{
    public class UIKitException : XException
    {
        public UIKitException(string msg, int errorCode) : base(msg, errorCode) { }

        public UIKitException(string msg, UIKitErrorCode errorCode):base (msg, (int)errorCode) { }
    }
}
