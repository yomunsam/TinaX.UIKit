using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.UIKit.Router
{
    /// <summary>
    /// UI load path router
    /// </summary>
    public interface IRouter
    {
        public bool TryGetUILoadPath(string uiName, out string uiLoadPath);
    }
}
