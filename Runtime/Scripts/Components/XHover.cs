using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TinaX.UIKit.Animation;

namespace TinaX.UIKit.Components
{
    [AddComponentMenu("TinaX/UIKit/Components/X Hover")]
    public class XHover : XUIComponent, IPointerEnterHandler, IPointerExitHandler
    {
        #region Ani
        //public UIAnimationBase EnterAnimation;
        //public UIAnimationBase ExitAnimation;
        #endregion

        [Space(10)]
        public UnityEvent OnEnter = new UnityEvent();
        public UnityEvent OnExit = new UnityEvent();
        

        public void OnPointerEnter(PointerEventData eventData)
        {
            #region Ani
            //if (ExitAnimation != null)
            //    ExitAnimation.Stop();
            //if (EnterAnimation != null)
            //    EnterAnimation.Play();
            #endregion

            OnEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            #region Ani
            //if (EnterAnimation != null)
            //    EnterAnimation.Stop();
            //if (ExitAnimation != null)
            //    ExitAnimation.Play();
            #endregion

            OnExit?.Invoke();
        }
    }
}
