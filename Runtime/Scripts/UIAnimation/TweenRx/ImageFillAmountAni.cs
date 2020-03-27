using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TinaX.UIKit.Animation
{
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("TinaX/UIKit/Animation/Image/Image Fill Amount")]
    public class ImageFillAmountAni : UIAnimationBase
    {
        public bool AutoOriginValue = false;
        [Range(0,1)]
        public float FromValue = 0;
        public bool AutoTargetValue = false;
        [Range(0,1)]
        public float ToValue = 1;

        public Tween.EaseType Ease;


        private System.IDisposable _disposable;
        private bool pingpong_switch;
        private Image _image;

        float? origin_value;
        float? target_value;

        public override void Ready()
        {
            if (_image == null) _image = this.GetComponent<Image>();
            if (origin_value == null)
                origin_value = this.AutoOriginValue ? this._image.fillAmount : this.FromValue;
            if (target_value == null)
                target_value = this.AutoTargetValue ? this._image.fillAmount : this.ToValue;

            _image.fillAmount = FromValue;
        }
        public override void Play()
        {
            if (_image == null) _image = this.GetComponent<Image>();
            if (origin_value == null)
                origin_value = this.AutoOriginValue ? this._image.fillAmount : this.FromValue;
            if (target_value == null)
                target_value = this.AutoTargetValue ? this._image.fillAmount : this.ToValue;

            if (!AutoOriginValue)
                this._image.fillAmount = this.FromValue;
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

        private void doNext(float value)
        {
            if (this._image != null)
                this._image.fillAmount = value;
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

