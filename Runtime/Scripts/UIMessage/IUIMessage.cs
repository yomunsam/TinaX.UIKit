namespace TinaX.UIKit.UIMessage
{
#nullable enable

    /// <summary>
    /// UI消息总接口
    /// </summary>
    public interface IUIMessage
    {
        void OnUIMessage(string messageName, object[]? args);
    }
#nullable restore
}
