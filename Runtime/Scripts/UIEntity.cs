using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.UIKit.Internal;
using UnityEngine;

namespace TinaX.UIKit.Entity
{
    public class UIEntity : IUIEntity
    {
        public string UIName;
        public string UIPath;
        public UIStatus UIStatue = UIStatus.Idle;

        public Task OpenUITask = Task.CompletedTask;

        public UIPage UIPage;
        public GameObject UIGameObject;

        public Canvas UICanvas;
        public int SortingOrder
        {
            get=> UICanvas.sortingOrder;
            set
            {
                UICanvas.sortingOrder = value;
            }
        }
        public int SortingLayerID => this.UIPage.SortingLayerID;

        public UIEntity(string name, string path)
        {
            this.UIName = name;
            this.UIPath = path;
        }
    }
}
