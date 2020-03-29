using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TinaX;

namespace TinaX.UIKit.Animation
{
    [AddComponentMenu("TinaX/UIKit/Animation/Image/Image Color")]
    public class ImageColorAni : UIAnimationBase
    {
        public Image AniTarget;
        public bool AutoOriginValue = false;
        public Color FromValue;
        public bool AutoTargetValue = false;
        public Color ToValue;

        public Tween.EaseType Ease;


        private System.IDisposable _disposable;
        private bool pingpong_switch;

        Color? origin_value;
        Color? target_value;

        public override void Ready()
        {
            if (AniTarget == null) AniTarget = this.transform.GetComponentOrAdd<Image>();
            origin_value = this.AutoOriginValue ? this.AniTarget.color : this.FromValue;
            target_value = this.AutoTargetValue ? this.AniTarget.color : this.ToValue;

            AniTarget.color = FromValue;
        }

        public override void Play()
        {
            if (AniTarget == null) AniTarget = this.transform.GetComponentOrAdd<Image>();
            origin_value = this.AutoOriginValue ? this.AniTarget.color : this.FromValue;
            target_value = this.AutoTargetValue ? this.AniTarget.color : this.ToValue;

            if (!AutoOriginValue)
                this.AniTarget.color = this.FromValue;
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
                .Subscribe(doNext, finish);
        }


        public override void Stop()
        {
            _disposable?.Dispose();
            _disposable = null;
            pingpong_switch = false;
        }

        private void doNext(Color value)
        {
            if (this.AniTarget != null)
                this.AniTarget.color = value;
        }

        private void finish()
        {
            if (this.pingPong)
            {
                this.pingpong_switch = !this.pingpong_switch;
                _disposable?.Dispose();
                _disposable = Tween.Play(!pingpong_switch ? this.FromValue : this.ToValue,
                    !pingpong_switch ? this.ToValue : this.FromValue,
                    this.Duration,
                    this.Ease)
                    .Subscribe(doNext, finish);
            }
            else
            {
                this.AniFinish();
            }
        }


    }
}

