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

        /// <summary>
        /// 创建UIEntity对象的流程
        /// </summary>
        public const string CreateUIEntity = @"xUIKit_CreateUIEntity";

        /// <summary>
        /// 加载Prefab的流程
        /// </summary>
        public const string LoadPrefab = @"xUIKit_LoadPrefab";

        /// <summary>
        /// 把Prefab在对应位置布置好GameObject的流程
        /// </summary>
        public const string Instantiates = @"xUIKit_Instantiates";

        /// <summary>
        /// 处理UI Main Handler为xBehaviour的情况
        /// </summary>
        public const string XBehaviour = @"xUIKit_xBehaviour";

        /// <summary>
        /// UI遮罩
        /// </summary>
        public const string UIMask = @"xUIKit_UIMask";

        /// <summary>
        /// 发送打开UI事件
        /// </summary>
        public const string SendMessage = @"xUIKit_SendMessage";


        /// <summary>
        /// 收尾工作
        /// </summary>
        public const string Finish = @"xUIKit_Finish";
    }
}
