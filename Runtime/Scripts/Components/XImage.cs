using System;
using System.Threading.Tasks;
using TinaX.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TinaX.UIKit.Components
{
    [AddComponentMenu("TinaX/UIKit/Components/X Image")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class XImage : XUIComponent
    {
        private Image m_Target;

        private Sprite m_LastLoadAsset; //最后一次加载的资产

        private IAssetService _assetService;
        private IAssetService m_Assets
        {
            get
            {
                if (_assetService == null)
                {
                    if (XCore.MainInstance != null)
                        XCore.MainInstance.Services.TryGetBuildInService<IAssetService>(out _assetService);
                }
                return _assetService;
            }
        }

        private void Awake()
        {
            m_Target = this.GetComponent<Image>();
        }

        private void OnDestroy()
        {
            if (m_LastLoadAsset != null)
                m_Assets.Release(m_LastLoadAsset);
            m_LastLoadAsset = null;
        }


        public void SetSpritePath(string loadPath)
        {
            if(m_Assets == null)
            {
                Debug.LogError($"[x Image ({this.gameObject.name})]Unable to get a valid built-in assets service interface");
                return;
            }

            //顺序是先加载新的资产，最后处理“如果有旧的资产，就卸载”的逻辑。这样可以避免有可能的卸载后立即再次加载，减少资源浪费
            var sprite = m_Assets.Load<Sprite>(loadPath);
            m_Target.sprite = sprite;

            if(m_LastLoadAsset != null)
            {
                m_Assets.Release(m_LastLoadAsset);
                m_LastLoadAsset = null;
            }

            m_LastLoadAsset = sprite;
        }

        public async Task SetSpritePathAsync(string loadPath)
        {
            if (m_Assets == null)
            {
                Debug.LogError($"[x Image ({this.gameObject.name})]Unable to get a valid built-in assets service interface");
                return;
            }

            //顺序是先加载新的资产，最后处理“如果有旧的资产，就卸载”的逻辑。这样可以避免有可能的卸载后立即再次加载，减少资源浪费
            var sprite = await m_Assets.LoadAsync<Sprite>(loadPath);
            m_Target.sprite = sprite;

            if (m_LastLoadAsset != null)
            {
                m_Assets.Release(m_LastLoadAsset);
                m_LastLoadAsset = null;
            }
            m_LastLoadAsset = sprite;
        }

        public void SetSpritePathAsync(string loadPath, Action<XException> callback)
        {
            this.SetSpritePathAsync(loadPath)
                .ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(_ =>
                {
                    callback?.Invoke(null);
                },err =>
                {
                    if (err is XException)
                        callback?.Invoke(err as XException);
                    else
                        throw err;
                });
        }

    }
}
