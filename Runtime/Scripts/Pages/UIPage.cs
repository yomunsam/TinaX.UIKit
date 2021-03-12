using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.XComponent;
using System;
//using TinaX.UIKit.Animation;
using System.Linq;
using TinaX.UIKit.UnityEvents;

namespace TinaX.UIKit
{
    public class UIPage : MonoBehaviour
    {
        public bool ScreenUI = true;
        public bool FullScreenUI = false;

        public int SortingLayerId;


        private SortingLayer? _sortingLayer;
        private int? _sortingLayerById; //上面的这个Layer信息是从哪获取到的

        public SortingLayer SortingLayer
        {
            get
            {
                if(_sortingLayer == null || _sortingLayerById == null || _sortingLayerById.Value != SortingLayerId)
                {
                    try
                    {
                        _sortingLayer = SortingLayer.layers.Where(sl => sl.id == this.SortingLayerId).Single();
                        _sortingLayerById = _sortingLayer.Value.id;
                    }
                    catch
                    {
                        _sortingLayer = SortingLayer.layers.FirstOrDefault();
                        SortingLayerId = _sortingLayer.Value.id;
                        _sortingLayerById = SortingLayerId;
                    }
                }
                return _sortingLayer.Value;
            }
        }


        public int SortingLayerValue => this.SortingLayer.value;

        public string SortingLayerName => this.SortingLayer.name;


        public bool AllowMultiple = false;
        public UnityEngine.Object UIMainHandler;

        public UIPageEventBase OnOpenUIEvent = new UIPageEventBase();
        public UIPageEventBase OnShowUIEvent = new UIPageEventBase();
        public UIPageEventBase OnHideUIEvent = new UIPageEventBase();
        public UIPageEventBase OnCloseUIEvent = new UIPageEventBase();
        public UIPageEventBase OnDestroyUIEvent = new UIPageEventBase();

        public Action OnOpenUI { get; set; }
        public Action OnShowUI { get; set; }
        public Action OnHideUI { get; set; }
        public Action<float> OnCloseUI { get; set; } //float参数为UI延迟销毁时间
        public Action OnDestroyUI { get; set; }


        /// <summary>
        /// UI GameObject销毁的延迟时间，可用于诸如UI关闭动画等场合
        /// </summary>
        public float DestroyDelay { get; set; } = 0;


        

        public void TrySetXBehavior(XComponent.XBehaviour xBehaviour, bool inject = true)
        {
            if(UIMainHandler != null)
            {
                if (UIMainHandler is TinaX.XComponent.XComponent)
                {
                    var xcomponent = UIMainHandler as TinaX.XComponent.XComponent;
                    if(xcomponent.Behaviour == null)
                    {
                        xcomponent.AddBehaviour(xBehaviour, inject);
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
                    xcomponent.SendQueueMsg(msg_name, args);
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
            OnOpenUI?.Invoke();
        }


        private void OnDestroy()
        {
            this.OnDestroyUI?.Invoke();
            var emmList = this.OnDestroyUI.GetInvocationList();
            this.OnDestroyUIEvent?.Invoke();
            //OnCloseUI?.Invoke();
        }

    }
}

