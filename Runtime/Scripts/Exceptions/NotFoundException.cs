namespace TinaX.UIKit.Exceptions
{
    public class NotFoundException : UIKitException
    {
        public NotFoundException(string msg, int errorCode) : base(msg, errorCode) { }
        public NotFoundException(string msg, UIKitErrorCode errorCode): base(msg, errorCode) { }
    }
}
