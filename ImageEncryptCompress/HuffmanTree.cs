using Priority_Queue;
using System.Collections.Generic;
using System.IO;

public class HuffmanTree
{
    private HuffmanNode root;
    private readonly int[] histogram;
    private string[] compressedCodes;

    public HuffmanTree(int[] histogram)
    {
        this.histogram = new int[Constants.MAX_VALUE];
        histogram.CopyTo(this.histogram, 0);
        compressedCodes = new string[Constants.MAX_VALUE];
        BuildHuffmanTree();
        BuildHuffmanCodes();
    }

    /// <summary>
    /// Builds a Huffman tree for efficient data compression using a priority queue.
    /// </summary>
    private void BuildHuffmanTree()
    {
        SimplePriorityQueue<HuffmanNode, int> pq = new SimplePriorityQueue<HuffmanNode, int>();
        for (int i = 0; i < histogram.Length; i++)
        {
            if (histogram[i] != 0)
                pq.Enqueue(new HuffmanNode((byte)i, histogram[i]), histogram[i]);
        }

        while (pq.Count > 1)
        {
            HuffmanNode leftNode = pq.First;
            pq.Dequeue();

            HuffmanNode rightNode = pq.First;
            pq.Dequeue();

            //INTERNAL_NODE to indicate to internal node: does not represent any value
            HuffmanNode parent = new HuffmanNode(Constants.INTERNAL_NODE, leftNode, rightNode);

            pq.Enqueue(parent, parent.frequency);
        }
        root = pq.First;
    }

    /// <summary>
    /// Traverses the Huffman tree using Breadth-First Search (BFS) and assigns each color value (0-255) its corresponding Huffman code
    /// I am using BFS due to it being more intuitive in complexity analysis, since it's iterative
    /// </summary>
    private void BuildHuffmanCodes()
    {
        // I am storing the Huffman code as a string since each node has a new Huffman code
        // They can't share the same StringBuilder, using StringBuilder here isn't suitable
        // HuffmanNode, HuffmanCode when reached this node
        Queue<KeyValuePair<HuffmanNode, string>> queue = new Queue<KeyValuePair<HuffmanNode, string>>();

        queue.Enqueue(new KeyValuePair<HuffmanNode, string>(root, ""));
        
        HuffmanNode currentNode;
        string currentHuffmanCode;

        while (queue.Count != 0)
        {
            currentNode = queue.Peek().Key;
            currentHuffmanCode = queue.Peek().Value;

            queue.Dequeue();

            // This is because in the Huffman tree if a node has a left child, then it for sure has a right child
            if (currentNode.leftChild != null)
            {
                queue.Enqueue(new KeyValuePair<HuffmanNode, string>(currentNode.leftChild, currentHuffmanCode + '0'));
                queue.Enqueue(new KeyValuePair<HuffmanNode, string>(currentNode.rightChild, currentHuffmanCode + '1'));
            }
            else
            {
                compressedCodes[currentNode.data] = currentHuffmanCode;
            }
        }
    }

    /// <summary>
    /// Returns the Huffman code corresponding to the given 8-bit color value (0-255)
    /// </summary>
    /// <param name="colorValue">8-bit color value</param>
    /// <returns>Huffman code corresponding to the provided color value</returns>
    public string GetCode(byte colorValue)
    {
        return compressedCodes[colorValue];
    }

    /// <summary>
    /// Interface to use the WriteTreeToFile(HuffmanNode node, BinaryWriter writer) function
    /// </summary>
    /// <param name="writer">The BinaryWriter object used to write data to the binary file file</param>
    public void WriteTreeToFile(ref BinaryWriter writer)
    {
        WriteTreeToFile(root, ref writer);
    }

    /// <summary>
    /// Writes the Huffman tree structure to a file in a pre-order traversal.
    /// </summary>
    /// <param name="node">The current HuffmanNode to be written</param>
    /// <param name="writer">The BinaryWriter object used to write data to the binary file</param>
    private void WriteTreeToFile(HuffmanNode node, ref BinaryWriter writer)
    {
        if (node == null)
            return;
        if (node.leftChild == null)
        {
            HuffmanCoding.numberOfBytes += 2;
            writer.Write((byte)1);
            writer.Write(node.data);
            return;
        }
        HuffmanCoding.numberOfBytes++;
        writer.Write((byte)0);
        //Writing sub-trees
        WriteTreeToFile(node.leftChild, ref writer);
        WriteTreeToFile(node.rightChild, ref writer);
    }
}