using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptCompress
{
    public class ImageEncrypterDecrypter
    {
        private static int width;
        private static int height;

        /// <summary>
        /// create a copy of the image to avoid ruin the original image
        /// </summary>
        /// <param name="imageMatrix">the original image</param>
        /// <returns>a copy of the original image</returns>
        private static RGBPixel[,] CopyMatrix(RGBPixel[,] imageMatrix)
        {
            
            RGBPixel[,] copiedImage = new RGBPixel[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    copiedImage[i, j] = imageMatrix[i, j];
                }
            }
            return copiedImage;
        }

        /// <summary>
        /// Encrypt the image by creating a copy of the original image and apply lfsr on it
        /// </summary>
        /// <param name="imageMatrix">the orignal image</param>
        /// <param name="passwordGenerator">object from LFSR class to generate keys</param>
        /// <returns>encrypted image</returns>
        
        public static RGBPixel[,] EncryptDecrypt(RGBPixel[,] imageMatrix, LFSR passwordGenerator)
        {
            width = ImageOperations.GetWidth(imageMatrix);
            height = ImageOperations.GetHeight(imageMatrix);

            RGBPixel[,] encryptedImage = CopyMatrix(imageMatrix);
            
            int numberOfBits = 8;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    encryptedImage[i, j].red   = (byte)(encryptedImage[i, j].red   ^ (byte)passwordGenerator.GenerateKey(numberOfBits));
                    encryptedImage[i, j].green = (byte)(encryptedImage[i, j].green ^ (byte)passwordGenerator.GenerateKey(numberOfBits));
                    encryptedImage[i, j].blue  = (byte)(encryptedImage[i, j].blue  ^ (byte)passwordGenerator.GenerateKey(numberOfBits));
                }
            }
            
            return encryptedImage;
        }
        public static RGBPixel[,] EncryptDecrypt(RGBPixel[,] imageMatrix, AlphaLFSR passwordGenerator)
        {
            width = ImageOperations.GetWidth(imageMatrix);
            height = ImageOperations.GetHeight(imageMatrix);

            RGBPixel[,] encryptedImage = CopyMatrix(imageMatrix);

            int numberOfBits = 8;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    encryptedImage[i, j].red = (byte)(encryptedImage[i, j].red ^ (byte)passwordGenerator.GenerateKey(numberOfBits));
                    encryptedImage[i, j].green = (byte)(encryptedImage[i, j].green ^ (byte)passwordGenerator.GenerateKey(numberOfBits));
                    encryptedImage[i, j].blue = (byte)(encryptedImage[i, j].blue ^ (byte)passwordGenerator.GenerateKey(numberOfBits));
                }
            }

            return encryptedImage;
        }

    }
}
