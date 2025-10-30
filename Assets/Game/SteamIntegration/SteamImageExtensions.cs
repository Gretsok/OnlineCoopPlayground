using Steamworks.Data;
using UnityEngine;

namespace Game.SteamIntegration
{
    public static class SteamImageExtensions
    {
        public static Texture2D ToTexture2D(this Image? a_image)
        {
            if (a_image == null)
                return null;
            
            var texture = new Texture2D((int)a_image?.Width, (int)a_image?.Height, TextureFormat.RGBA32, false, true);

            texture.LoadRawTextureData(a_image?.Data);

            texture.Apply();

            texture = FlipTexture(texture);

            return texture;
        }

        private static Texture2D FlipTexture(Texture2D a_original)
        {
            var flipped = new Texture2D(a_original.width, a_original.height);
            var pixels = a_original.GetPixels();
            System.Array.Reverse(pixels); // Inverse les lignes
            flipped.SetPixels(pixels);
            flipped.Apply();
            return flipped;
        }
    }
}