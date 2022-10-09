using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    [AddComponentMenu("ML Agents/Shadow Ratio Sensor Component")]
    public class ShadowRatioSensorComponent : MonoBehaviour
    {
        [SerializeField]
        Camera moduleCamera;

        [SerializeField]
        RenderTexture renderTexture;

        private Texture2D texture2D;
        private Rect rect;
        private Rect cropRect;
        private int totalPixels;
        private byte[] rawByteData;
        public float shadowRatio;
        
        void Awake()
        {   
            // Get initial render texture rectangle
            int initialWidth = renderTexture.width;
            int initialHeight = renderTexture.height;
            rect = new Rect(0, 0, initialWidth, initialHeight);

            // Get crop rectangle from camera component
            moduleCamera = GetComponentInChildren<Camera>();
            int cropWidth = moduleCamera.pixelWidth;
            int cropHeight = moduleCamera.pixelHeight;
            totalPixels = cropWidth * cropHeight;
            cropRect = new Rect(0, 0, cropWidth, cropHeight);
        }

        int ConvertGrayscale(Color32 pixel)
        {
            // Converting to weighted grayscale
            float pixelValue = (
                0.299f * pixel.r + 0.587f * pixel.g + 0.114f * pixel.b);
            return Mathf.RoundToInt(pixelValue);            
        }

        Texture2D toTexture2D(RenderTexture renderTexture, Rect rect)
        {
            Texture2D texture2D = new Texture2D(
                (int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(rect, 0, 0);
            texture2D.Apply();
            return texture2D;
        }

        Texture2D CropTexture(Texture2D initialTexture, Rect cropRect)
        {
            Texture2D newTexture = new Texture2D(
                (int)cropRect.width, (int)cropRect.height, TextureFormat.RGBA32, false);
            Color[] newPixels = initialTexture.GetPixels(
                (int)cropRect.x, (int)cropRect.y, (int)cropRect.width, (int)cropRect.height);
            newTexture.SetPixels(newPixels);
            newTexture.Apply();
            return newTexture;
        }

        void ComputeShadowRatio(Texture2D croppedTexture)
        {
            // Get RGBA pixels from cropped texture
            Color32[] pixels = croppedTexture.GetPixels32();
            
            // Count number of black pixels RGB(0,0,0)
            int shadowedPixels = 0;
            foreach (Color32 pixel in pixels)
            {
                int pixelValue = ConvertGrayscale(pixel);
                if (pixelValue == 0)
                {
                    shadowedPixels++;
                }
            }

            // Compute shadow ratio, avoid divide by zero
            if (shadowedPixels != 0)
            {
                shadowRatio = (float)shadowedPixels / (float)totalPixels;               
            } else
            {
                shadowRatio = 0.0f;
            }    
        }

        void SaveImage(Texture2D croppedTexture)
        {
            rawByteData = ImageConversion.EncodeToJPG(croppedTexture);
            string fileName = string.Format("Assets/Data/Images/{0}.jpg", gameObject.name);
            File.WriteAllBytes(fileName, rawByteData);
        }

        void Update()
        {
            // Read render texture onto initial texture
            Texture2D initialTexture = toTexture2D(renderTexture, rect);

            // Crop initial texture
            Texture2D croppedTexture = CropTexture(initialTexture, cropRect);

            // Compute shadow ratio from cropped texture
            ComputeShadowRatio(croppedTexture);

            // Save cropped texture to jpeg files
            SaveImage(croppedTexture);
        }
    }
}