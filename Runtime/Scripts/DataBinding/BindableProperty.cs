using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.UIKit.DataBinding
{
    public class BindableProperty<T>
    {
        public delegate void ValueChangedDalegate(T oldValue, T newValue);
        public ValueChangedDalegate ValueChanged;

        private T _value;

        public BindableProperty() { _value = default(T); }
        public BindableProperty(T defaultValue)
        {
            this._value = defaultValue;
        }

        public T Value
        {
            get => this._value;
            set
            {
                if (!object.Equals(_value, value))
                {
                    T old = _value;
                    _value = value;
                    ValueChanged?.Invoke(old, value);
                }
            }
        }

        public void SendValueChanged()
        {
            this.ValueChanged?.Invoke(_value, _value);
        }

        public void OnValueChanged(ValueChangedDalegate callback)
        {
            this.ValueChanged += callback;
        }

        public void ClearDelegates()
        {
            this.ValueChanged = null;
        }

        public override string ToString()
        {
            return (_value != null ? _value.ToString() : string.Empty);
        }


    }
}

