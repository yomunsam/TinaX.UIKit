using System.Collections.Generic;
using System.Linq;

namespace TinaX.UIKit.Canvas
{
    public class UIKitCanvasManager
    {
        private readonly List<UIKitCanvas> m_Canvas = new List<UIKitCanvas>();

        public UIKitCanvasManager()
        {

        }


        public void Add(UIKitCanvas canvas)
        {
            if(!m_Canvas.Contains(canvas))
                m_Canvas.Add(canvas);
        }

        public bool TryGet(string canvasName, out UIKitCanvas canvas)
        {
            canvas = m_Canvas.FirstOrDefault(c => c.Name == canvasName);
            return canvas != null;
        }
    }
}
