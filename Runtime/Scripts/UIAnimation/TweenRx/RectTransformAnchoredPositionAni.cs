using UniRx;
using UnityEngine;

namespace TinaX.UIKit.Animation
{
    [AddComponentMenu("TinaX/UIKit/Animation/RectTransform/AnchoredPosition Ani")]
    public class RectTransformAnchoredPositionAni : UIAnimationBase
    {
        //public RectTransform AniTarget;
        //public bool AutoOriginValue = false;
        //public Vector2 FromValue;
        //public bool AutoTargetValue = false;
        //public Vector2 ToValue = Vector2.one;

        //public Tween.EaseType Ease;

        //private System.IDisposable _disposable;
        //private bool pingpong_switch;

        //private Vector2? origin_value;
        //private Vector2? target_value;

        //public override void Ready()
        //{
        //    if (AniTarget == null) AniTarget = this.GetComponent<RectTransform>();
        //    if (AniTarget == null) return;
        //    origin_value = this.AutoOriginValue ? this.AniTarget.anchoredPosition : this.FromValue;
        //    target_value = this.AutoTargetValue ? this.AniTarget.anchoredPosition : this.ToValue;

        //    AniTarget.anchoredPosition = FromValue;
        //}

        //public override void Play()
        //{
        //    if (AniTarget == null) AniTarget = this.GetComponent<RectTransform>();
        //    if (AniTarget == null) return;

        //    origin_value = this.AutoOriginValue ? this.AniTarget.anchoredPosition : this.FromValue;
        //    target_value = this.AutoTargetValue ? this.AniTarget.anchoredPosition : this.ToValue;

        //    if (!AutoOriginValue)
        //        this.AniTarget.anchoredPosition = this.FromValue;
        //    else
        //    {
        //        this.pingPong = false;
        //        this.AutoTargetValue = false;
        //    }

        //    if(origin_value.Value == target_value.Value)
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

        //private void doNext(Vector2 value)
        //{
        //    if (this.AniTarget != null)
        //        this.AniTarget.anchoredPosition = value;
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

