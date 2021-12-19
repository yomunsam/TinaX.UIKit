using System.Threading;
using Cysharp.Threading.Tasks;

namespace TinaX.UIKit.Pipelines.GetUIPage
{
    public interface IGetUIPageAsyncHandler
    {
        string HandlerName { get; }

        UniTask GetPageAsync(string pageUri, GetUIPageContext context, CancellationToken cancellationToken);
    }
}
