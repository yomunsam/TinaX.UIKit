namespace TinaX.UIKit.Page.View
{
    public interface IViewProvider
    {
    }

    public interface IViewProvider<T> : IViewProvider where T : PageView
    {

    }
}
