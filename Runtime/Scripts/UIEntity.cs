using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.UIKit.Internal;
using UnityEngine;

namespace TinaX.UIKit.Entity
{
    public class UIEntity : IUIEntity , IDisposable
    {
        public string UIName;
        public string UIPath;
        public UIStatus UIStatue = UIStatus.Idle;

        public Task OpenUITask = Task.CompletedTask;

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
                    _uiPage.OnPageDestroy = OnUIDestory;
            }
        }
        public GameObject UIGameObject;

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
        public int SortingLayerID => this.UIPage.SortingLayerID;
        public bool AllowMultiple => this.UIPage.AllowMultiple;

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
            if (this.UIGameObject != null)
                this.UIGameObject.Show();
            if (this.mGo_Mask != null)
                this.mGo_Mask.Show();
        }

        public void Hide()
        {
            if (this.UIGameObject != null)
                this.UIGameObject.Hide();
            if (this.mGo_Mask != null)
                this.mGo_Mask.Hide();
        }

        public void Close(params object[] args)
        {
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

            var image = mGo_Mask.GetComponentOrAdd<UnityEngine.UI.Image>();
            image.color = maskColor;
            image.raycastTarget = true;

            var rect_trans = mGo_Mask.GetComponent<RectTransform>();
            rect_trans.anchorMin = Vector2.zero;
            rect_trans.anchorMax = Vector2.one;
            rect_trans.localScale = Vector3.one;
            rect_trans.sizeDelta = Vector2.zero;

            mMaskSmartClose = closeByMask;
            if (closeByMask)
            {
                var btn = mGo_Mask.GetComponentOrAdd<UnityEngine.UI.Button>();
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
        
        public void Dispose()
        {
            UIStatue = UIStatus.Unloaded;
            if (UIGameObject != null)
            {
                UIGameObject.Destroy(this.UIPage.DestroyDelayTime);
                UIGameObject = null;
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
