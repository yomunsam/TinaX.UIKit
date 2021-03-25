using System;
using System.Threading.Tasks;

namespace TinaX.UIKit.Pipelines.OpenUI
{
    /// <summary>
    /// 通用 OpenUI管线处理器（异步）
    /// </summary>
    public class GeneralOpenUIAsyncHandler : IOpenUIAsyncHandler
    {
        public delegate Task<bool> DoOpenUIAsyncDelegate(OpenUIPayload payload, IOpenUIAsyncHandler next);

        public string HandlerName { get; private set; }

        public DoOpenUIAsyncDelegate DoOpenUIAsync { get; private set; }

        public GeneralOpenUIAsyncHandler(string name, DoOpenUIAsyncDelegate func)
        {

            this.HandlerName = name ?? throw new ArgumentNullException(nameof(name));
            this.DoOpenUIAsync = func ?? throw new ArgumentNullException(nameof(func));
        }

        public Task<bool> OpenUIAsync(OpenUIPayload payload, IOpenUIAsyncHandler next)
        {
            return DoOpenUIAsync(payload, next);
        }
    }
}
