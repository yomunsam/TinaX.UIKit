namespace TinaX.UIKit.Pipelines.OpenUI
{
    /// <summary>
    /// 通过常量定义UIKit内部的OpenUI相关的所有Handler的名字
    /// </summary>
    public static class OpenUIHandlerNameConst
    {
        /// <summary>
        /// 通过传入的LoadPath得到UI加载路径
        /// </summary>
        public const string GetUILoadPath = @"xUIKit_GetUILoadPath";

        /// <summary>
        /// 检查是否已经打开UI, 如果UI已经打开了，并且该UI设置不允许打开多个的话，则把已存在的UI置顶，并不再加载
        /// </summary>
        public const string CheckLoaded = @"xUIKit_CheckLoaded";
    }
}
