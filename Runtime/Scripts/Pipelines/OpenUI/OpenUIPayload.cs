using TinaX.UIKit.Entity;

namespace TinaX.UIKit.Pipelines.OpenUI
{
    public class OpenUIPayload : OpenUIParam
    {
        /// <summary>
        /// 能从资源接口上加载出UI prefab来的路径
        /// </summary>
        public string UILoadPath { get; set; }

        public object[] OpenUIArgs { get; set; } //打开UI的附带参数


        public UIEntity UIEntity { get; set; } //应该是Pipeline上的某个handler流程把它变出来的

    }
}
