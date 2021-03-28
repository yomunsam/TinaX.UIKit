using System;
using System.Threading.Tasks;
using TinaX.UIKit.Components;
using UnityEngine.UI;

namespace TinaX.UIKit
{
    public static class UImageExtensions
    {
        public static void SetSpritePath(this Image image, string loadPath)
        {
            var xImage = _GetXImageOrCreate(ref image);
            xImage.SetSpritePath(loadPath);
        }

        public static Task SetSpritePathAsync(this Image image, string loadPath)
        {
            var xImage = _GetXImageOrCreate(ref image);
            return xImage.SetSpritePathAsync(loadPath);
        }

        public static void SetSpritePathAsync(this Image image, string loadPath, Action<XException> callback)
        {
            var xImage = _GetXImageOrCreate(ref image);
            xImage.SetSpritePathAsync(loadPath, callback);
        }


        private static XImage _GetXImageOrCreate(ref Image image)
        {
            var ximg = image.GetComponent<XImage>();
            if(ximg == null)
            {
                ximg = image.gameObject.AddComponent<XImage>();
            }
            return ximg;
        }
    }
}
