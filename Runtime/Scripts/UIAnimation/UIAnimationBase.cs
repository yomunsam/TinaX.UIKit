using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TinaX.UIKit.Animation
{
    public class UIAnimationBase : MonoBehaviour
    {
        [System.Serializable]
        public class AnimationFinishEvent : UnityEvent { }

        public float Duration = 1f;

        public string title = "ani";
        public bool playOnAwake = false;
        public bool pingPong = false;
        public AnimationFinishEvent onFinish;



        protected virtual void Awake()
        {
            if (playOnAwake)
            {
                this.Play();
            }
        }

        public virtual void Ready()
        {

        }

        public virtual void Play()
        {

        }

        public virtual void Stop()
        {

        }

        protected void AniFinish()
        {
            this.onFinish?.Invoke();
        }
        
    }
}

