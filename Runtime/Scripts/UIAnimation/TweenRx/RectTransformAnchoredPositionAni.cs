using UniRx;
using UnityEngine;

namespace TinaX.UIKit.Animation
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("TinaX/UIKit/Animation/RectTransform/AnchoredPosition Ani")]
    public class RectTransformAnchoredPositionAni : UIAnimationBase
    {
        public bool AutoOriginValue = false;
        public Vector2 FromValue;
        public bool AutoTargetValue = false;
        public Vector2 ToValue = Vector2.one;

        public Tween.EaseType Ease;

        private System.IDisposable _disposable;
        private bool pingpong_switch;
        private RectTransform _rectTransform;

        private Vector2? origin_value;
        private Vector2? target_value;

        public override void Ready()
        {
            if (_rectTransform == null) _rectTransform = this.GetComponent<RectTransform>();
            if (origin_value == null)
                origin_value = this.AutoOriginValue ? this._rectTransform.anchoredPosition : this.FromValue;
            if (target_value == null)
                target_value = this.AutoTargetValue ? this._rectTransform.anchoredPosition : this.ToValue;

            _rectTransform.anchoredPosition = FromValue;
        }

        public override void Play()
        {
            if (_rectTransform == null) _rectTransform = this.GetComponent<RectTransform>();

            if (origin_value == null)
                origin_value = this.AutoOriginValue ? this._rectTransform.anchoredPosition : this.FromValue;
            if (target_value == null)
                target_value = this.AutoTargetValue ? this._rectTransform.anchoredPosition : this.ToValue;

            if (!AutoOriginValue)
                this._rectTransform.anchoredPosition = this.FromValue;
            else
            {
                this.pingPong = false;
                this.AutoTargetValue = false;
            }

            if(origin_value.Value == target_value.Value)
            {
                this.AniFinish();
                return;
            }

            _disposable = Tween.Play(
                origin_value.Value,
                target_value.Value,
                this.Duration, this.Ease)
                .Subscribe(doNext, finish);
        }


        public override void Stop()
        {
            _disposable?.Dispose();
            _disposable = null;
            pingpong_switch = false;
            base.Stop();
        }

        private void doNext(Vector2 value)
        {
            if (this._rectTransform != null)
                this._rectTransform.anchoredPosition = value;
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

