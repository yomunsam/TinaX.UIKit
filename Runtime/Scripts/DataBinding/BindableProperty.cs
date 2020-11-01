using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.UIKit.DataBinding
{
    public class BindableProperty<T> : IDisposable
    {
        public delegate void ValueChangedDelegate(T oldValue, T newValue);
        public ValueChangedDelegate ValueChanged;

        private T _value;
        private bool disposedValue;

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

        public void OnValueChanged(ValueChangedDelegate callback)
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //释放托管状态(托管对象)
                }

                // 释放未托管的资源(未托管的对象)并替代终结器
                // 将大型字段设置为 null
                ValueChanged = null;

                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~BindableProperty()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

