using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace TinaX.UIKit.Animation
{

    [AddComponentMenu("TinaX/UIKit/Animation/Transform Scale")]
    public class TransformScaleAni : UIAnimationBase
    {
        public Vector3 FromValue;
        public Vector3 ToValue = Vector3.one;

        public Tween.EaseType Ease;


        private System.IDisposable _disposable;
        private bool pingpong_switch;

        public override void Ready()
        {
            this.transform.localScale = this.FromValue;
        }

        public override void Play()
        {
            this.transform.localScale = this.FromValue;
            _disposable = Tween.Play(this.FromValue, this.ToValue, this.Duration, this.Ease)
                .Subscribe(doNext, finish);

        }

        public override void Stop()
        {
            _disposable?.Dispose();
            _disposable = null;
            pingpong_switch = false;
            base.Stop();
        }

        private void doNext(Vector3 value)
        {
            this.transform.localScale = value;
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

