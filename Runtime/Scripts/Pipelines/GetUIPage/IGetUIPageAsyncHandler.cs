using System.Threading;
using Cysharp.Threading.Tasks;

namespace TinaX.UIKit.Pipelines.GetUIPage
{
    public interface IGetUIPageAsyncHandler
    {
        string HandlerName { get; }

        UniTask GetPageAsync(GetUIPageContext context, GetUIPagePayload payload, CancellationToken cancellationToken);
    }
}
