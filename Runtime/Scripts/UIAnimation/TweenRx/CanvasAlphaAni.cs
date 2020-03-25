using UnityEngine;
using UniRx;

namespace TinaX.UIKit.Animation
{
    [AddComponentMenu("TinaX/UIKit/Animation/Canvas Alpha")]
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasAlphaAni : UIAnimationBase
    {
        [Range(0,1)]
        public float FromAlpha = 0;
        [Range(0, 1)]
        public float ToAlpha = 1;

        public Tween.EaseType Ease;

        private CanvasGroup _canvasGroup;
        private System.IDisposable _disposable;

        private bool pingpong_switch;

        public override void Ready()
        {
            if (_canvasGroup == null) { _canvasGroup = this.gameObject.GetComponent<CanvasGroup>(); }
            _canvasGroup.alpha = FromAlpha;

            base.Ready();
        }

        public override void Play()
        {
            if(_canvasGroup == null) { _canvasGroup = this.gameObject.GetComponent<CanvasGroup>(); }

            _canvasGroup.alpha = FromAlpha;

            _disposable = Tween.Play(FromAlpha, ToAlpha, this.Duration, this.Ease)
                .Subscribe(OnNext, OnCompleted);

            base.Play();
        }

        public override void Stop()
        {
            _disposable?.Dispose();
            _disposable = null;
            pingpong_switch = false;
            base.Stop();
        }

        private void OnNext(float value)
        {
            _canvasGroup.alpha = value;
        }

        private void OnCompleted()
        {
            if (this.pingPong)
            {
                pingpong_switch = !pingpong_switch;
                _disposable?.Dispose();
                _disposable = Tween.Play(
                    !pingpong_switch ? FromAlpha : ToAlpha,
                    !pingpong_switch ? ToAlpha : FromAlpha,
                    this.Duration,
                    this.Ease)
                .Subscribe(OnNext, OnCompleted);
            }
            else
            {
                //完成动画
                this.AniFinish();
            }
        }
    }
}

