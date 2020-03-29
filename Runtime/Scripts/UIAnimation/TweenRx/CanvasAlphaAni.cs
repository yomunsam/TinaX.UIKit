using UnityEngine;
using UniRx;

namespace TinaX.UIKit.Animation
{
    [AddComponentMenu("TinaX/UIKit/Animation/Canvas Alpha")]
    public class CanvasAlphaAni : UIAnimationBase
    {
        public CanvasGroup AniTarget;
        public bool AutoOriginValue = false;
        [Range(0,1)]
        public float FromValue = 0;
        public bool AutoTargetValue = false;
        [Range(0, 1)]
        public float ToValue = 1;

        public Tween.EaseType Ease;

        private CanvasGroup _canvasGroup;
        private System.IDisposable _disposable;

        private bool pingpong_switch;
        float? origin_value;
        float? target_value;

        public override void Ready()
        {
            if (AniTarget == null) AniTarget = this.transform.GetComponentOrAdd<CanvasGroup>();
            origin_value = this.AutoOriginValue ? this.AniTarget.alpha : this.FromValue;
            target_value = this.AutoTargetValue ? this.AniTarget.alpha : this.ToValue;

            AniTarget.alpha = FromValue;
        }

        public override void Play()
        {
            if (AniTarget == null) AniTarget = this.transform.GetComponentOrAdd<CanvasGroup>();
            origin_value = this.AutoOriginValue ? this.AniTarget.alpha : this.FromValue;
            target_value = this.AutoTargetValue ? this.AniTarget.alpha : this.ToValue;

            if (!AutoOriginValue)
                this.AniTarget.alpha = this.FromValue;
            else
            {
                this.pingPong = false;
                this.AutoTargetValue = false;
            }

            if (origin_value.Value == target_value.Value)
            {
                this.AniFinish();
                return;
            }

            _disposable = Tween.Play(
                origin_value.Value,
                target_value.Value,
                this.Duration, this.Ease,
                this.DelayBefore)
                .Subscribe(doNext, onCompleted);


        }

        public override void Stop()
        {
            _disposable?.Dispose();
            _disposable = null;
            pingpong_switch = false;
        }

        private void doNext(float value)
        {
            if (this.AniTarget != null)
                this.AniTarget.alpha = value;
        }

        private void onCompleted()
        {
            if (this.pingPong)
            {
                this.pingpong_switch = !this.pingpong_switch;
                _disposable?.Dispose();
                _disposable = Tween.Play(!pingpong_switch ? this.FromValue : this.ToValue,
                    !pingpong_switch ? this.ToValue : this.FromValue,
                    this.Duration,
                    this.Ease)
                    .Subscribe(doNext, onCompleted);
            }
            else
            {
                this.AniFinish();
            }
        }

    }
}

