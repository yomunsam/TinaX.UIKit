using UniRx;
using UnityEngine;

namespace TinaX.UIKit.Animation
{

    [AddComponentMenu("TinaX/UIKit/Animation/Transform Scale")]
    public class TransformScaleAni : UIAnimationBase
    {
        //public Transform AniTarget;
        //public bool AutoOriginValue = false;
        //public Vector3 FromValue = Vector3.zero;
        //public bool AutoTargetValue = false;
        //public Vector3 ToValue = Vector3.one;

        //public Tween.EaseType Ease;


        //private System.IDisposable _disposable;
        //private bool pingpong_switch;

        //Vector3? origin_value;
        //Vector3? target_value;

        //public override void Ready()
        //{
        //    if (AniTarget == null) AniTarget = this.transform;

        //    origin_value = this.AutoOriginValue ? this.AniTarget.localScale : this.FromValue;
        //    target_value = this.AutoTargetValue ? this.AniTarget.localScale : this.ToValue;

        //    AniTarget.localScale = this.FromValue;
        //}

        //public override void Play()
        //{
        //    if (AniTarget == null) AniTarget = this.transform;
        //    origin_value = this.AutoOriginValue ? this.AniTarget.localScale : this.FromValue;
        //    target_value = this.AutoTargetValue ? this.AniTarget.localScale : this.ToValue;

        //    if (!AutoOriginValue)
        //        this.AniTarget.localScale = this.FromValue;
        //    else
        //    {
        //        this.pingPong = false;
        //        this.AutoTargetValue = false;
        //    }

        //    if (origin_value.Value == target_value.Value)
        //    {
        //        this.AniFinish();
        //        return;
        //    }

        //    _disposable = Tween.Play(
        //        origin_value.Value,
        //        target_value.Value,
        //        this.Duration, this.Ease,
        //        this.DelayBefore)
        //        .Subscribe(doNext, finish);
        //}

        //public override void Stop()
        //{
        //    _disposable?.Dispose();
        //    _disposable = null;
        //    pingpong_switch = false;
        //    base.Stop();
        //}

        //private void doNext(Vector3 value)
        //{
        //    if (this.AniTarget != null)
        //        this.AniTarget.localScale = value;
        //}

        //private void finish()
        //{
        //    if (this.pingPong)
        //    {
        //        this.pingpong_switch = !this.pingpong_switch;
        //        _disposable?.Dispose();
        //        _disposable = Tween.Play(!pingpong_switch ? this.FromValue : this.ToValue,
        //            !pingpong_switch ? this.ToValue : this.FromValue,
        //            this.Duration,
        //            this.Ease)
        //            .Subscribe(doNext, finish);
        //    }
        //    else
        //    {
        //        this.AniFinish();
        //    }
        //}

    }
}

