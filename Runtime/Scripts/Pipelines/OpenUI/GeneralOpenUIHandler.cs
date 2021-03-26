using System;

namespace TinaX.UIKit.Pipelines.OpenUI
{
    public class GeneralOpenUIHandler : IOpenUIHandler
    {
        public delegate bool DoOpenUIDelegate(ref OpenUIPayload payload, IOpenUIHandler next);

        public string HandlerName { get; private set; }

        public DoOpenUIDelegate DoOpenUI { get; private set; }

        public GeneralOpenUIHandler(string name, DoOpenUIDelegate func)
        {
            this.HandlerName = name ?? throw new ArgumentNullException(nameof(name));
            this.DoOpenUI = func ?? throw new ArgumentNullException(nameof(func));
        }


        public bool OpenUI(ref OpenUIPayload payload, IOpenUIHandler next)
        {
            return DoOpenUI(ref payload, next);
        }
    }
}
