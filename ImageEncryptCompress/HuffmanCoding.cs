﻿
using ImageEncryptCompress;
using System;
using System.IO;
using System.Text;

public class HuffmanCoding
{

    private static HuffmanTree red;
    private static HuffmanTree green;
    private static HuffmanTree blue;
    public static int numberOfBytes = 0;

    public static void CompressImage(RGBPixel[,] image, ref BinaryWriter writer)
    {
        /*
        File structure:
            seed (int32) tapPosition(byte)
            Red Channel Tree {EOT = 2}
            Green Channel Tree {EOT = 2}
            Blue Channel Tree
            {EOH = 3}
            height width
            image[0,0].red image[0,0].green image[0,0].blue image[0,1].red...
            .
            .
            .
        ------------------------------------------------------------------------
        Tree strucutre in the file:
            - pre-order traversal
            - 0: internal node, 1: leaf node, 2: end of the tree
            - All bytes
            - End it with 2
        ------------------------------------------------------------------------
        Image structure in the file:
            - Concating each 8 bits into one byte
            - All strings
        */
        red = new HuffmanTree(ImageUtilities.GetColorChannelFrequency(image, Color.RED));
        green = new HuffmanTree(ImageUtilities.GetColorChannelFrequency(image, Color.GREEN));
        blue = new HuffmanTree(ImageUtilities.GetColorChannelFrequency(image, Color.BLUE));

        red.WriteTreeToFile(ref writer);
        writer.Write((byte)2); //indicating end of the red HuffmanTree
        numberOfBytes++;

        green.WriteTreeToFile(ref writer);
        writer.Write((byte)2); //indicating end of the green HuffmanTree
        numberOfBytes++;

        blue.WriteTreeToFile(ref writer);
        writer.Write((byte)3); //indicating end of the Header
        numberOfBytes++;

        WriteImageToFile(image, ref writer);
    }
    private static void WriteImageToFile(RGBPixel[,] image, ref BinaryWriter writer)
    {
        int height = image.GetLength(0);
        int width = image.GetLength(1);
        StringBuilder currentByte = new StringBuilder();
        StringBuilder currentPixel = new StringBuilder();

        //write height and width
        writer.Write(height);
        writer.Write(width);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //concate each 8 bits in one byte
                currentPixel.Append(red.GetCode(image[i, j].red));
                currentPixel.Append(green.GetCode(image[i, j].green));
                currentPixel.Append(blue.GetCode(image[i, j].blue));
                var res = currentPixel.ToString();

                foreach (var d in res)
                {
                    currentByte.Append(d);
                    if (currentByte.Length == 8)
                    {
                        numberOfBytes++;
                        writer.Write(Convert.ToByte(currentByte.ToString(), 2));
                        currentByte.Clear();
                    }
                }
                currentPixel.Clear();
            }
        }
        if (currentByte.Length != 0)
        {
            numberOfBytes++;
            writer.Write(Convert.ToByte(currentByte.ToString(), 2));
            currentByte.Clear();
        }
    }
}