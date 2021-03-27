using System.Threading.Tasks;

namespace TinaX.UIKit.Pipelines.OpenUI
{

    public interface IOpenUIAsyncHandler
    {
        string HandlerName { get; }

        /// <summary>
        /// 执行加载管线中的内容
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>继续执行返回true，中断流程返回false</returns>
        Task<bool> OpenUIAsync(OpenUIPayload payload, IOpenUIAsyncHandler next);
    }
}
