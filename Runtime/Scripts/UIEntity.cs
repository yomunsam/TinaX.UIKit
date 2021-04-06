using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.UIKit.Internal;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TinaX.UIKit.Entity
{
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class UIEntity : IUIEntity , IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        public string UIName;
        public string UIPath;
        public UIStatus UIStatue = UIStatus.Idle;

        public Task LoadUIPrefabTask = Task.CompletedTask;

        private UIPage _uiPage;
        public UIPage UIPage
        {
            get
            {
                return _uiPage;
            }
            set
            {
                _uiPage = value;
                if (value != null)
                    _uiPage.OnDestroyUI += OnUIDestory;
            }
        }
        public GameObject UIGameObject;

        public GameObject UIPrefab;

        public Canvas UICanvas;
        public int SortingOrder
        {
            get=> UICanvas.sortingOrder;
            set
            {
                UICanvas.sortingOrder = value;
                if (mCanvas_Mask != null)
                    mCanvas_Mask.sortingOrder = value - 1;
            }
        }
        public int SortingLayerValue => this.UIPage.SortingLayerValue;
        public int SortingLayerId => this.UIPage.SortingLayerId;
        public bool AllowMultiple => this.UIPage.AllowMultiple;

        public bool Closed => this.UIStatue == UIStatus.Unloaded;

        private GameObject mGo_Mask;
        private Canvas mCanvas_Mask;
        private bool mMaskSmartClose; //click mask to close ui

        private UIManager UIMgr;


        public UIEntity(UIManager mgr,string name, string path)
        {
            this.UIMgr = mgr;
            this.UIName = name;
            this.UIPath = path;
        }

        public void Show()
        {
            if (Closed)
                return;
            if (this.UIGameObject != null)
                this.UIGameObject.Show();
            if (this.mGo_Mask != null)
                this.mGo_Mask.Show();
        }

        public void Hide()
        {
            if (Closed)
                return;
            if (this.UIGameObject != null)
                this.UIGameObject.Hide();
            if (this.mGo_Mask != null)
                this.mGo_Mask.Hide();
        }

        public void Close(params object[] args)
        {
            if (Closed)
                return;
            this.UIMgr?.CloseUI(this, args);
        }

        public void ShowMask(bool closeByMask, Color maskColor)
        {
            if (UIGameObject == null) return;
            string mask_name = "mask_" + UIGameObject.GetInstanceID().ToString();
            mGo_Mask = UIGameObject.transform.parent.gameObject.FindOrCreateGameObject(mask_name)
                .SetLayerRecursive(UIGameObject.layer);

            mCanvas_Mask = mGo_Mask.GetComponentOrAdd<Canvas>();
            mCanvas_Mask.overrideSorting = true;
            mCanvas_Mask.sortingOrder = this.SortingOrder - 1;
            mCanvas_Mask.sortingLayerID = this.SortingLayerId;

            var image = mGo_Mask.GetComponentOrAdd<UnityEngine.UI.Image>();
            image.color = maskColor;
            image.raycastTarget = true;

            var rect_trans = mGo_Mask.GetComponent<RectTransform>();
            rect_trans.anchorMin = Vector2.zero;
            rect_trans.anchorMax = Vector2.one;
            rect_trans.localScale = Vector3.one;
            rect_trans.sizeDelta = Vector2.zero;
            rect_trans.localPosition = Vector3.zero;

            var g = mGo_Mask.GetComponentOrAdd<GraphicRaycaster>();

            mMaskSmartClose = closeByMask;
            if (closeByMask)
            {
                var btn = mGo_Mask.GetComponentOrAdd<UnityEngine.UI.Button>();
                btn.transition = UnityEngine.UI.Selectable.Transition.None;
                btn.targetGraphic = image;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnMaskClicked);
            }
        }

        public void HideMaskIfExist()
        {
            if (mGo_Mask != null)
                mGo_Mask.Hide();
        }

        public void ShowMaskIfExist()
        {
            if (mGo_Mask != null)
                mGo_Mask.Show();
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            if (UIStatue == UIStatus.Unloaded)
                return;

            UIStatue = UIStatus.Unloaded;
            var destroy_delay = this.UIPage.DestroyDelay;
            if (UIGameObject != null)
            {
                //触发CloseUI事件
                this.UIPage.OnCloseUIEvent?.Invoke();
                this.UIPage.OnCloseUI?.Invoke(destroy_delay);


                UIGameObject.Destroy(destroy_delay);
                UIGameObject = null;
            }
            if(UIPrefab != null)
            {
                if(XCore.GetMainInstance().Services.TryGetBuildInService<TinaX.Services.IAssetService>(out var assets))
                {
                    if (this.UIPage.DestroyDelay > 0)
                    {
                        Observable.NextFrame()
                            .Delay(TimeSpan.FromSeconds(destroy_delay))
                            .Subscribe(_ =>
                            {
                                assets.Release(UIPrefab);
                                UIPrefab = null;
                            });
                    }
                    else
                    {
                        assets.Release(UIPrefab);
                        UIPrefab = null;
                    }
                }
                else
                {
                    Debug.LogError("[UIEntity]Connot get build-in service : " + nameof(TinaX.Services.IAssetService));
                    UIPrefab = null;
                }

            }
            UICanvas = null;
            UIPage = null;
        }

        private void OnUIDestory()
        {
            if (mGo_Mask != null)
            {
                mGo_Mask.Destroy();
                mGo_Mask = null;
            }
            mCanvas_Mask = null;
            mMaskSmartClose = false;
        }

        private void OnMaskClicked()
        {
            if (mMaskSmartClose)
                this.Close();
        }

        
    }
}
