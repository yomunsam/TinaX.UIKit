namespace TinaX.UIKit
{
    public interface IUIEntity
    {
        UIPage UIPage { get;}
        bool Closed { get; }

        void Close(params object[] args);
        void Hide();
        void Show();
    }
}
