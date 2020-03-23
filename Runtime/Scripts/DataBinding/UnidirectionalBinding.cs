using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TinaX.UIKit.DataBinding.Internal
{
    /// <summary>
    /// 单向绑定
    /// </summary>
    public class UnidirectionalBinding<T>
    {
        public delegate void SetValueDelegate(T value);
        public SetValueDelegate OnSetValueDelegate;

        public void SetValue(T value)
        {
            this.OnSetValueDelegate?.Invoke(value);
        }

        public void OnSetValue(SetValueDelegate callback)
        {
            OnSetValueDelegate += callback;
        }

        public void ClearDelegates() { OnSetValueDelegate = null; }
    }

}
