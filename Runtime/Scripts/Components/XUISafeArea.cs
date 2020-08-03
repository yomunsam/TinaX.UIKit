using UnityEngine;

namespace TinaX.UIKit.Components
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [AddComponentMenu("TinaX/UIKit/Components/x UISafeArea")]
    public class XUISafeArea : MonoBehaviour
    {
        public bool IgnoreX;
        public bool IgnoreY;

        RectTransform m_RectTransform;
        Rect m_LastSafeArea = Rect.zero;
        Vector2Int m_LastScreenSize = new Vector2Int(0, 0);
        ScreenOrientation m_LastOrientation = ScreenOrientation.AutoRotation;

        private void Awake()
        {
            m_RectTransform = this.GetComponent<RectTransform>();

            m_RectTransform.anchorMin = Vector2.zero;
            m_RectTransform.anchorMax = Vector2.one;
        }

        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            var safeArea = Screen.safeArea;

            if(safeArea != m_LastSafeArea
                || Screen.width != m_LastScreenSize.x
                || Screen.height != m_LastScreenSize.y
                || Screen.orientation != m_LastOrientation)
            {
                m_LastScreenSize.x = Screen.width;
                m_LastScreenSize.y = Screen.height;
                m_LastOrientation = Screen.orientation;

                ApplySafeArea(safeArea);
            }
        }

        private void ApplySafeArea(Rect rect)
        {
            m_LastSafeArea = rect;

            if (IgnoreX)
            {
                rect.x = 0;
                rect.width = Screen.width;
            }

            if (IgnoreY)
            {
                rect.y = 0;
                rect.height = Screen.height;
            }

            Vector2 anchorMin = rect.position;
            Vector2 anchorMax = rect.position + rect.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            m_RectTransform.anchorMin = anchorMin;
            m_RectTransform.anchorMax = anchorMax;
        }
    }
}

