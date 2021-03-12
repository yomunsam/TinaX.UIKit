using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using TinaX.Systems;

namespace TinaX.UIKit.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    [AddComponentMenu("TinaX/UIKit/Components/X Button")]
    public class XButton : XUIComponent , IPointerDownHandler , IPointerUpHandler
    {
        [Tooltip("Time (seconds) to judge as \"LongTap\", if 0, it will never trigger \n判定为 “LongTap” 的时间（秒），如果为0则永远不会触发")]
        public float LongTapTime = 1.5f;

        #region Ani
        //[Space(10)]
        //public UIAnimationBase ClickAnimation;
        //public UIAnimationBase DownAnimation;
        //public UIAnimationBase LongTapAnimation;
        #endregion

        #region Events
        [Space(10)]
        public UnityEvent OnClick = new UnityEvent();
        [Tooltip("Button Down | 按下")]
        public UnityEvent OnDown = new UnityEvent();
        [Tooltip("Long Tap | 长按")]
        public UnityEvent OnLongTap = new UnityEvent();

        public UnityEvent onClick => this.OnClick; //兼容Unity风格的小驼峰
        #endregion

        public Action OnClickAction { get; set; }
        public Action OnDownAction { get; set; }
        public Action OnLongTapAction { get; set; }

        private Button _target_btn;


        private void Awake()
        {
            _target_btn = this.GetComponent<Button>();
            _target_btn.onClick.AddListener(onBtnClick);
        }

        private void OnDestroy()
        {
            _target_btn.onClick.RemoveListener(onBtnClick);

            _target_btn = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_target_btn.IsInteractable() && (eventData.button != PointerEventData.InputButton.Middle && eventData.button != PointerEventData.InputButton.Right))
            {
                //down 相关
                
                if (OnDown != null)
                    OnDown.Invoke();
                OnDownAction?.Invoke();

                //long tap 相关
                if(this.LongTapTime > 0)
                {
                    
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        private void onBtnClick()
        {
            if (this.OnClick != null)
                this.OnClick.Invoke();
            this.OnClickAction?.Invoke();
        }

        
    }
}

