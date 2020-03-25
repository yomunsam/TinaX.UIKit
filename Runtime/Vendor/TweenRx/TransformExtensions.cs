/*
 * 此处代码改写自TweenRx （https://github.com/fumobox/TweenRx）
 * The code here is rewritten from TweenRx
 */

using System;
using UniRx;
using UnityEngine;

namespace TinaX.UIKit.Animation
{
    public static class TransformExtensions
    {
        public static IDisposable SubscribeToLocalPosition(this IObservable<Vector3> source, Transform transform)
        {
            return source.SubscribeWithState(transform, (x, t) => t.localPosition = x);
        }

        public static IObservable<Vector3> DoToLocalPosition(this IObservable<Vector3> source, Transform transform)
        {
            return source.Do(x => transform.localPosition = x);
        }
    }
}