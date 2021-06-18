namespace TinaX.UIKit.Router
{
    /// <summary>
    /// UI load path router
    /// </summary>
    public interface IRouter
    {
        bool TryGetUILoadPath(string uiName, out string uiLoadPath);
    }
}
