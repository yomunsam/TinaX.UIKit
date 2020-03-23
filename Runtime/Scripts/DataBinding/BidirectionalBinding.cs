using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TinaX.UIKit.DataBinding.Internal
{
    /// <summary>
    /// 双向绑定
    /// </summary>
    public class BidirectionalBinding<T>
    {
        public delegate void SetValueDelegate(T value);
        public delegate void ValueChangedDelegate(T newValue);

        public SetValueDelegate OnSetValueDelegate;
        public ValueChangedDelegate OnValueChangedDelegate;

        public void SetValue(T value)
        {
            OnSetValueDelegate?.Invoke(value);
        }

        public void InvokeValueChanged(T newValue)
        {
            OnValueChangedDelegate?.Invoke(newValue);
        }

        public void OnValueChanged(ValueChangedDelegate callback)
        {
            this.OnValueChangedDelegate += callback;
        }

        public void OnSetValue(SetValueDelegate callback)
        {
            this.OnSetValueDelegate += callback;
        }

        public void ClearDelegates()
        {
            this.OnSetValueDelegate = null;
            this.OnValueChangedDelegate = null;
        }

    }
}

