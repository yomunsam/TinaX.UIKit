using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.XComponent;
using System;
using TinaX.UIKit.Animation;
using System.Linq;

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

        //public int SortingLayerValue;

        //private int? _sortingLayerId;
        //private int? _sortingLayerIdByValue;

        //public int SortingLayerId
        //{ 
        //    get
        //    {
        //        if (_sortingLayerId == null || _sortingLayerIdByValue == null || _sortingLayerIdByValue.Value != SortingLayerValue)
        //        {
        //            try
        //            {
        //                var layer = SortingLayer.layers.Where(sl => sl.value == this.SortingLayerValue).First();
        //                _sortingLayerId = layer.id;
        //                _sortingLayerIdByValue = this.SortingLayerValue;
        //            }
        //            catch
        //            {
        //                this.SortingLayerValue = 0;
        //                _sortingLayerId = 0;
        //                _sortingLayerIdByValue = this.SortingLayerValue;
        //            }
        //        }
        //        return _sortingLayerId.Value;
        //    }
        //}

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
                    return UIExitAni.GetDurationTime();
            }
        }

        public Action OnPageDestroy;


        private bool mUI_Ani_Show_Playing = false;
        private bool mUI_Ani_Exit_Playing = false;

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
            //var layer = SortingLayer.layers.Where(sl => sl.value == this.SortingLayerValue).FirstOrDefault();
            //this.GetComponent<Canvas>().

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

        public void TryPlayExitAni()
        {
            if (mUI_Ani_Show_Playing && UIShowAni != null)
            {
                UIShowAni.Stop();
                mUI_Ani_Show_Playing = false;
            }

            if (UIExitAni != null && !mUI_Ani_Exit_Playing)
            {
                mUI_Ani_Exit_Playing = true;
                UIExitAni.onFinish.RemoveListener(OnExitUIAniFinish);
                UIExitAni.onFinish.AddListener(OnExitUIAniFinish);
                UIExitAni.Play();
            }
        }

        private void OnShowUIAniFinish()
        {
            mUI_Ani_Show_Playing = false;
            this.SendMsg(UIEventConst.ShowUIAnimationFinish);
        }

        private void OnExitUIAniFinish()
        {
            mUI_Ani_Exit_Playing = false;
        }

    }
}

