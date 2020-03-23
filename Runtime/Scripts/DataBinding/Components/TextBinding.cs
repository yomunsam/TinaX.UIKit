using TinaX.UIKit.DataBinding.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace TinaX.UIKit.DataBinding.Components
{
    [DisallowMultipleComponent]
    [AddComponentMenu("TinaX/UIKit/DataBinding/Text")]
    [RequireComponent(typeof(Text))]
    public class TextBinding : UIBindingBase
    {
        public Text TextTarget;
        public UnidirectionalBinding<string> text = new UnidirectionalBinding<string>();
        public UnidirectionalBinding<Color> color = new UnidirectionalBinding<Color>();
        public UnidirectionalBinding<int> fontSize = new UnidirectionalBinding<int>();


        private void Awake()
        {
            if (this.TextTarget == null)
                this.TextTarget = this.gameObject.GetComponent<Text>();
            if(TextTarget != null)
            {
                base._UITarget = TextTarget;

                this.text.OnSetValueDelegate += (value) =>
                {
                    TextTarget.text = value;
                };

                this.color.OnSetValueDelegate += (color) =>
                {
                    TextTarget.color = color;
                };

                this.fontSize.OnSetValueDelegate += (size) =>
                {
                    TextTarget.fontSize = size;
                };

            }
            
        }

        private void OnDestroy()
        {
            text?.ClearDelegates();
            color?.ClearDelegates();
            fontSize.ClearDelegates();
        }

    }
}

