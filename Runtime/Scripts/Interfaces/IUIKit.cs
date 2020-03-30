using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.UIKit.Entity;
using TinaX.XComponent;

namespace TinaX.UIKit
{
    public interface IUIKit
    {
        void CloseUI(UIEntity entity, params object[] args);
        void CloseUI(string UIName, params object[] args);
        IUIEntity OpenUI(string UIName, params object[] args);
        IUIEntity OpenUI(string UIName, OpenUIParam openUIParam, params object[] args);
        IUIEntity OpenUI<T>(string UIName, params object[] args) where T : XBehaviour;
        IUIEntity OpenUIWithBehaviourType(string UIName, Type xBehaviourType, params object[] args);
        IUIEntity OpenUI(string UIName, XBehaviour behaviour, params object[] args);
        Task<IUIEntity> OpenUIAsync(string UIName, params object[] args);
        void OpenUIAsync(string UIName, Action<IUIEntity, XException> callback, params object[] args);
        Task<IUIEntity> OpenUIAsync(string UIName, OpenUIParam openUIParam, params object[] args);
        void OpenUIAsync(string UIName, OpenUIParam openUIParam, Action<IUIEntity, XException> callback, params object[] args);
        Task<IUIEntity> OpenUIAsync<T>(string UIName, params object[] args) where T : XBehaviour;
        Task<IUIEntity> OpenUIWithBehaviourTypeAsync(string UIName, Type xBehaviourType, params object[] args);
        Task<IUIEntity> OpenUIAsync(string UIName, XBehaviour behaviour, params object[] args);
        void OpenUIWithBehaviourTypeAsync(string UIName, Type xBehaviourType, Action<IUIEntity, XException> callback, params object[] args);
        void OpenUIAsync(string UIName, XBehaviour behaviour, Action<IUIEntity, XException> callback, params object[] args);
        IUIEntity OpenUI<T>(string UIName, OpenUIParam openUIParam, params object[] args) where T : XBehaviour;
        IUIEntity OpenUIWithBehaviourType(string UIName, Type xBehaviourType, OpenUIParam openUIParam, params object[] args);
        IUIEntity OpenUI(string UIName, XBehaviour behaviour, OpenUIParam openUIParam, params object[] args);
        Task<IUIEntity> OpenUIAsync<T>(string UIName, OpenUIParam openUIParam, params object[] args) where T : XBehaviour;
        Task<IUIEntity> OpenUIWithBehaviourTypeAsync(string UIName, Type xBehaviourType, OpenUIParam openUIParam, params object[] args);
        Task<IUIEntity> OpenUIAsync(string UIName, XBehaviour behaviour, OpenUIParam openUIParam, params object[] args);
        void OpenUIWithBehaviourTypeAsync(string UIName, Type xBehaviourType, OpenUIParam openUIParam, Action<IUIEntity, XException> callback, params object[] args);
        void OpenUIAsync(string UIName, XBehaviour behaviour, OpenUIParam openUIParam, Action<IUIEntity, XException> callback, params object[] args);
    }
}
