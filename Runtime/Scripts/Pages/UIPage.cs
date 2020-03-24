using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.XComponent;
using System;

namespace TinaX.UIKit
{
    public class UIPage : MonoBehaviour
    {
        public bool ScreenUI = true;
        public bool FullScreenUI = false;

        public int SortingLayerID = 0;
        public bool AllowMultiple = false;
        public UnityEngine.Object UIMainHandler;

        public Action OnDestroy;

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
            if(UIMainHandler != null)
            {
                if (UIMainHandler is XComponentScriptBase)
                {
                    var xcomponent = UIMainHandler as XComponentScriptBase;
                    xcomponent.SendMsg(UIEventConst.OpenUI, args);
                }
                else if( UIMainHandler is MonoBehaviour)
                {
                    var mb = UIMainHandler as MonoBehaviour;
                    mb.SendMessage(UIEventConst.OpenUI, args);
                }
            }
        }

        public void SendCloseUIMessage(params object[] args)
        {
            if (UIMainHandler != null)
            {
                if (UIMainHandler is XComponentScriptBase)
                {
                    var xcomponent = UIMainHandler as XComponentScriptBase;
                    xcomponent.SendMsg(UIEventConst.CloseUI, args);
                }
                else if (UIMainHandler is MonoBehaviour)
                {
                    var mb = UIMainHandler as MonoBehaviour;
                    mb.SendMessage(UIEventConst.CloseUI, args);
                }
            }
        }


    }
}

