using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.XComponent;
using System;
using TinaX.UIKit.Animation;

namespace TinaX.UIKit
{
    public class UIPage : MonoBehaviour
    {
        public bool ScreenUI = true;
        public bool FullScreenUI = false;

        public int SortingLayerID = 0;
        public bool AllowMultiple = false;
        public UnityEngine.Object UIMainHandler;

        public UIAnimationBase UIShowAni;
        public UIAnimationBase UIExitAni;

        public float DestroyDelayTime
        {
            get
            {
                if (UIExitAni == null)
                    return 0;
                else
                    return UIExitAni.Duration;
            }
        }

        public Action OnPageDestroy;

        private bool mUI_Ani_Show_Playing = false;

        public void TrySetXBehavior(XComponent.XBehaviour xBehaviour)
        {
            if(UIMainHandler != null)
            {
                if (UIMainHandler is TinaX.XComponent.XComponent)
                {
                    var xcomponent = UIMainHandler as TinaX.XComponent.XComponent;
                    if(xcomponent.Behaviour == null)
                    {
                        xcomponent.AddBehaviour(xBehaviour, true);
                    }
                }
            }
        }

        public void SendOpenUIMessage(params object[] args)
        {
            this.SendMsg(UIEventConst.OpenUI, args);
        }

        public void SendCloseUIMessage(params object[] args)
        {
            this.SendMsg(UIEventConst.CloseUI, args);
        }



        private void SendMsg(string msg_name, params object[] args)
        {
            if (UIMainHandler != null)
            {
                if (UIMainHandler is XComponentScriptBase)
                {
                    var xcomponent = UIMainHandler as XComponentScriptBase;
                    xcomponent.SendMsg(msg_name, args);
                }
                else if (UIMainHandler is MonoBehaviour)
                {
                    var mb = UIMainHandler as MonoBehaviour;
                    mb.SendMessage(msg_name, args);
                }
            }
        }

        private void Awake()
        {
            if(UIShowAni != null)
            {
                mUI_Ani_Show_Playing = true;
                UIShowAni.onFinish.AddListener(OnShowUIAniFinish);
                UIShowAni.Play();
            }
        }


        private void OnDestroy()
        {
            this.OnPageDestroy?.Invoke();
            if(mUI_Ani_Show_Playing && UIShowAni != null)
            {
                UIShowAni.Stop();
                mUI_Ani_Show_Playing = false;
            }
        }

        private void OnShowUIAniFinish()
        {
            mUI_Ani_Show_Playing = false;
            this.SendMsg(UIEventConst.ShowUIAnimationFinish);
        }

    }
}

