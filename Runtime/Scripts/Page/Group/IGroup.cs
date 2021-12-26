namespace TinaX.UIKit.Page
{
    public interface IGroup : IPage
    {
        void Push(UIPageBase page, object[] displayMessageArgs = null);
    }
}
