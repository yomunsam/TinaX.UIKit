using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.UIKit
{
    public interface IUIEntity
    {
        UIPage UIPage { get;}

        void Close(params object[] args);
        void Hide();
        void Show();
    }
}
