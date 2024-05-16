using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RetrievedHuffmanTree
{
    public HuffmanNode root;
    public RetrievedHuffmanTree()
    {
        root = null;
    }

    /// <summary>
    /// Interface to use the BuildTreeFromFile(HuffmanNode node, BinaryReader reader) function
    /// </summary>
    /// <param name="reader"></param>
    public void BuildTreeFromFile(ref BinaryReader reader)
    {
        root = ConstructTree(ref reader);
    }

    /// <summary>
    /// Reads and re-constructs the HuffmanTree from the binary file
    /// </summary>
    /// <param name="node">The current HuffmanNode to be read</param>
    /// <param name="reader">The BinaryReader object used to read data from the binary file</param>
    /// <returns>The root of the RetrivedHuffmanTree</returns>
    private HuffmanNode ConstructTree(ref BinaryReader reader)
    {
        //0: internal node
        //1: child node
        //2: end of tree

        byte nodeType = reader.ReadByte();
        if (nodeType == 2)
            return new HuffmanNode(0, 0);

        if (nodeType == 0)
        {
            //build sub-trees
            HuffmanNode leftChild = ConstructTree(ref reader);
            HuffmanNode rightChild = ConstructTree(ref reader);
            return new HuffmanNode(Constants.INTERNAL_NODE, leftChild, rightChild);
        }
        //frquency doesn't matter here. We already used it in building it for the first time
        return new HuffmanNode(reader.ReadByte(), 0);
    }
}
