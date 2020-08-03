using UnityEngine;
using UnityEngine.UI;

namespace TinaX.UIKit.Components
{
    //[RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    [AddComponentMenu("TinaX/UIKit/Components/UI Background")]
    public class XUIBackground : XUIComponent
    {
        /// <summary>
        /// 背景横纵比定义
        /// </summary>
        public Vector2Int BgAspectRationDefine;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = this.GetComponent<RectTransform>();
            this.ScaleBackground();
        }

        public void Refresh()
        {
            this.ScaleBackground();
        }


        private void ScaleBackground()
        {

            float screenAspectRation = (float)Screen.width / Screen.height;
            float bgAspectRation = (float)BgAspectRationDefine.x / BgAspectRationDefine.y;

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;


            var ui_root_rectTrans = GetParentCanvas(this.transform).GetComponent<RectTransform>();
            var ui_root_rect = ui_root_rectTrans.rect;
            var cur_ui_size = new Vector2(ui_root_rect.width, ui_root_rect.height);

            if (screenAspectRation > bgAspectRation)
            {
                //屏幕比背景图要宽，优先用背景图宽度填满屏幕
                rectTransform.sizeDelta = new Vector2(cur_ui_size.x, cur_ui_size.x / bgAspectRation);
            }
            else
            {
                //屏幕比背景图要狭窄，优先用背景图高度填满拼命
                rectTransform.sizeDelta = new Vector2(cur_ui_size.y * bgAspectRation, cur_ui_size.y);
            }
        }

        private Canvas GetParentCanvas(Transform trans, Canvas canvas = null)
        {
            var c = trans.GetComponent<Canvas>();
            if(trans.parent != null)
            {
                //递归
                return this.GetParentCanvas(trans.parent, c == null ? canvas : c);
            }
            else
            {
                if (c != null)
                    return c;
                else
                    return canvas;
            }
        }

    }

}
