using TinaX.XComponent;

namespace TinaX.UIKit
{
    public class XUIBehaviour : XBehaviour
    {
        public IUIEntity UIEntity { get; internal set; }

        protected void Hide()
        {
            UIEntity?.Hide();
        }

        protected void Show() => UIEntity?.Show();

        protected void Close() => UIEntity.Close();
    }
}
