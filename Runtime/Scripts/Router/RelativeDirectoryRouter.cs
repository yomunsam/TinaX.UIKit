namespace TinaX.UIKit.Router
{
    /// <summary>
    /// 相对目录路由
    /// </summary>
    public class RelativeDirectoryRouter : IRouter
    {
        private readonly string m_UIRootDirLoadPath; //不带斜杠
        private readonly string m_UIRootDirLoadPathWithSlash; //带斜杠的根目录
        public RelativeDirectoryRouter(string UIRootDirectoryLoadPath)
        {
            m_UIRootDirLoadPath = UIRootDirectoryLoadPath;
            if (!m_UIRootDirLoadPath.IsNullOrEmpty())
            {
                if (m_UIRootDirLoadPath.EndsWith("/"))
                    m_UIRootDirLoadPath = m_UIRootDirLoadPath.Substring(0, m_UIRootDirLoadPath.Length - 1);
                m_UIRootDirLoadPathWithSlash = m_UIRootDirLoadPath + "/";
            }
        }

        public bool TryGetUILoadPath(string uiName, out string uiLoadPath)
        {
            uiLoadPath = m_UIRootDirLoadPath.IsNullOrEmpty() ? uiName : (m_UIRootDirLoadPathWithSlash + uiName);
            return true;
        }
    }
}
