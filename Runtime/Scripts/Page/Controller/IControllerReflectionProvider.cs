using System;

namespace TinaX.UIKit.Page.Controller
{
#nullable enable
    /// <summary>
    /// 控制器反射提供者
    /// </summary>
    public interface IControllerReflectionProvider
    {
        bool TrySendMessage(object controllerObject, ref Type? controllerType, string messageName, object?[]? args);
    }
#nullable restore
}
