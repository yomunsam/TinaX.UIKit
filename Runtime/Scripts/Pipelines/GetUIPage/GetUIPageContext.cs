using TinaX.Container;

namespace TinaX.UIKit.Pipelines.GetUIPage
{
#nullable enable

    /// <summary>
    /// 获取UIPage 上下文
    /// </summary>
    public class GetUIPageContext
    {
        public GetUIPageContext(IServiceContainer services)
        {
            this.Services = services;
        }

        /// <summary>
        /// 是否终断Pipeline的标记
        /// </summary>
        public bool BreakPipeline { get; set; } = false;

        /// <summary>
        /// 终断Pipeline流程
        /// </summary>
        public void Break() => BreakPipeline = true;
        

        public IServiceContainer Services { get; set; }
        
    }
#nullable restore

}
