namespace TinaX.UIKit.Args
{
#nullable enable
    public class OpenUIArgs : GetUIPageArgs
    {
        public OpenUIArgs(string pageUri) : base(pageUri)
        {

        }

        /// <summary>
        /// 显示UI参数（启动参数）
        /// </summary>
        public object[]? UIDisplayArgs { get; set; }

    }
#nullable restore
}
