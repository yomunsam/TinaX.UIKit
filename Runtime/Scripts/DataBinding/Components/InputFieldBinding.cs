using TinaX.UIKit.DataBinding.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace TinaX.UIKit.DataBinding.Components
{
    [DisallowMultipleComponent]
    [AddComponentMenu("TinaX/UIKit/DataBinding/Input Field")]
    [RequireComponent(typeof(InputField))]
    public class InputFieldBinding : UIBindingBase
    {
        public InputField InputFieldTarget;
        public BidirectionalBinding<string> text = new BidirectionalBinding<string>();


        private void Awake()
        {
            if (this.InputFieldTarget == null)
                this.InputFieldTarget = this.gameObject.GetComponent<InputField>();
            if (InputFieldTarget != null)
            {
                base._UITarget = InputFieldTarget;

                this.text.OnSetValueDelegate += (value) =>
                {
                    InputFieldTarget.text = value;
                };
                InputFieldTarget.onValueChanged.AddListener(value =>
                {
                    this.text.InvokeValueChanged(value);
                });

            }

        }

        private void OnDestroy()
        {
            text?.ClearDelegates();
            InputFieldTarget.onValueChanged.RemoveAllListeners();
            
        }
    }
}