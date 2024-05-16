
using ImageEncryptCompress;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

public class HuffmanCoding
{

    private static HuffmanTree red;
    private static HuffmanTree green;
    private static HuffmanTree blue;
    private static int height;
    private static int width;
    private static List<byte> compressedData;
    public static int numberOfBytes = 0;

    public static void CompressImage(RGBPixel[,] image, ref BinaryWriter writer)
    {
        red = new HuffmanTree(ImageUtilities.GetColorChannelFrequency(image, Color.RED));
        green = new HuffmanTree(ImageUtilities.GetColorChannelFrequency(image, Color.GREEN));
        blue = new HuffmanTree(ImageUtilities.GetColorChannelFrequency(image, Color.BLUE));

        height = image.GetLength(0);
        width = image.GetLength(1);

        compressedData = new List<byte>();

        StringBuilder currentByte = new StringBuilder();
        StringBuilder currentPixel = new StringBuilder();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //concatenate each 8 bits (digits) in one byte
                currentPixel.Append(red.GetCode(image[i, j].red));
                currentPixel.Append(green.GetCode(image[i, j].green));
                currentPixel.Append(blue.GetCode(image[i, j].blue));
                var res = currentPixel.ToString();

                foreach (var d in res)
                {
                    currentByte.Append(d);
                    //current byte is full, write it and start new one
                    if (currentByte.Length == 8)
                    {
                        //counting number of bytes to calcute compression ratio
                        numberOfBytes++;
                        compressedData.Add(Convert.ToByte(currentByte.ToString(), 2));
                        currentByte.Clear();
                    }
                }
                currentPixel.Clear();
            }
        }

        //check for unwritten remaining bytes
        if (currentByte.Length != 0)
        {
            while(currentByte.Length != 8)
                currentByte.Append('0');
            numberOfBytes++;
            compressedData.Add(Convert.ToByte(currentByte.ToString(), 2));
        }
        SaveCompressedImage(compressedData, ref writer);
    }
    public static void SaveCompressedImage(List<byte> compressedData,ref BinaryWriter writer)
    {
        /*
        File structure:
            seed (string) tapPosition(byte)
            Red Channel Tree {EOT = 2}
            Green Channel Tree {EOT = 2}
            Blue Channel Tree
            {EOH = 2}
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
        red.WriteTreeToFile(ref writer);
        writer.Write((byte)2); //indicating end of the red HuffmanTree
        numberOfBytes++;

        green.WriteTreeToFile(ref writer);
        writer.Write((byte)2); //indicating end of the green HuffmanTree
        numberOfBytes++;

        blue.WriteTreeToFile(ref writer);
        writer.Write((byte)2); //indicating end of the Header
        numberOfBytes++;

        //write height and width
        writer.Write(height);
        writer.Write(width);

        foreach(var d in compressedData)
            writer.Write(d);
    }
    public static RGBPixel[,] DecompressImage(ref BinaryReader reader)
    {

        //reading trees
        RetrievedHuffmanTree[] trees = new RetrievedHuffmanTree[3];
        trees[0] = new RetrievedHuffmanTree();
        trees[0].BuildTreeFromFile(ref reader);
        reader.ReadByte();

        trees[1] = new RetrievedHuffmanTree();
        trees[1].BuildTreeFromFile(ref reader);
        reader.ReadByte();

        trees[2] = new RetrievedHuffmanTree();
        trees[2].BuildTreeFromFile(ref reader);
        reader.ReadByte();

        //reading height and width
        height = reader.ReadInt32();
        width = reader.ReadInt32();
        RGBPixel[,] image = new RGBPixel[height, width];

        //reading compressed data
        byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
        reader.Close();

        int i = 0, j = 0, channel = 0;
        var curNode = trees[channel].root;
        foreach (var currentByte in data)
        {
            for (int d = 7; d >= 0; d--)
            {
                bool lit = (currentByte & (1 << d)) > 0;
                if(lit)
                    curNode = curNode.rightChild;
                else
                    curNode = curNode.leftChild;
                //leaf
                if(curNode.leftChild == null)
                {
                    if (channel == 0)
                        image[i, j].red = curNode.data;
                    else if (channel == 1)
                        image[i, j].green = curNode.data;
                    else
                    {
                        image[i, j].blue = curNode.data;
                        j++;
                        if (j == width)
                        {
                            i++;
                            j = 0;
                            if (i == height)
                                break;
                        }
                    }
                    channel = (channel + 1) % 3;
                    curNode = trees[channel].root;
                }
            }
        }
        return image;
    }


}