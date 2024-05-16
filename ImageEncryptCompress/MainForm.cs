using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageEncryptCompress
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        RGBPixel[,] encryptedImage;
        string fileName;
        string filePath;
        string binaryFilePath;
        double originalSize;
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                fileName = "";
                filePath = OpenedFilePath;
                bool start = false;
                for (int i = OpenedFilePath.Length - 1; i >= 0; i--)
                {
                    if (OpenedFilePath[i] == '\\')
                        break;
                    if (OpenedFilePath[i] == '.')
                    {
                        start = true;
                        continue;
                    }
                    if(start)
                        fileName += OpenedFilePath[i];
                }
                originalSize = new FileInfo(OpenedFilePath).Length;
                var tmp = fileName.ToCharArray();
                Array.Reverse(tmp);
                fileName = new string(tmp);

                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
        }
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ImageMatrix == null || seed.Text.Length == 0 || tap.Text.Length == 0 ||
                Byte.Parse(tap.Text) >= seed.Text.Length)
            {
                MessageBox.Show("Invalid input", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            HuffmanCoding.numberOfBytes = 0;

            Stopwatch sw = new Stopwatch();
            sw = Stopwatch.StartNew();

            LFSR lfsr = new LFSR(seed.Text, Int32.Parse(tap.Text));
            encryptedImage = ImageEncrypterDecrypter.EncryptDecrypt(ImageMatrix, lfsr);
            ImageOperations.DisplayImage(encryptedImage, pictureBox2);

            BinaryWriter writer = new BinaryWriter(File.Open(fileName + ".bin", FileMode.Create));
            //Wrting seed and tap position to the binary file
            writer.Write(seed.Text);
            writer.Write(Byte.Parse(tap.Text));

            //Writing Huffman trees and the compressed image to binary file
            HuffmanCoding.CompressImage(ImageMatrix, ref writer);
            writer.Close();

            Console.WriteLine(HuffmanCoding.numberOfBytes);
            sw.Stop();
            double time = sw.ElapsedMilliseconds;
            time /= 1e3;

            double compressionRatio = Math.Round((originalSize / HuffmanCoding.numberOfBytes), 3);
            string caption = $"Exection Time in Seconds = {time} seconds\n" +
                             $"Compressed Binary File Size = {HuffmanCoding.numberOfBytes} bytes\n" +
                             $"Compression Ratio = {compressionRatio}";
            MessageBox.Show(caption, "Encryption & Compression Exection Time",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
                string destination = "", cur = saveFileDialog1.FileName;
                for (int i = cur.Length - 1; i >= 0; i--)
                {
                    if (cur[i] == '\\')
                    {
                        for (int j = 0; j < i; j++)
                            destination += cur[j];
                        break;
                    }
                }
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ImageMatrix == null || binaryFilePath == null)
            {
                MessageBox.Show("Invalid input", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw = Stopwatch.StartNew();
            BinaryReader reader = new BinaryReader(File.Open(binaryFilePath, FileMode.Open));

            string seed = reader.ReadString();
            byte tap = reader.ReadByte();
            LFSR lfsr = new LFSR(seed, tap);

            encryptedImage = ImageEncrypterDecrypter.EncryptDecrypt(ImageMatrix, lfsr);
            ImageOperations.DisplayImage(HuffmanCoding.DecompressImage(ref reader), pictureBox2);

            sw.Stop();
            double time = sw.ElapsedMilliseconds;
            time /= 1e3;

            double compressionRatio = Math.Round((HuffmanCoding.numberOfBytes / originalSize) * 100, 3);
            string caption = $"Exection Time in Seconds = {time} seconds\n" +
                             $"Compression Ratio = {compressionRatio}";
            MessageBox.Show(caption, "Decryption & Decompression Exection Time",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        
        //open binary file
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                binaryFilePath = openFileDialog1.FileName;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        //Encrypt only
        private void button5_Click(object sender, EventArgs e)
        {
            if (ImageMatrix == null || seed.Text.Length == 0 || tap.Text.Length == 0 ||
                 Byte.Parse(tap.Text) >= seed.Text.Length)
            {
                MessageBox.Show("Invalid input", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            HuffmanCoding.numberOfBytes = 0;

            Stopwatch sw = new Stopwatch();
            sw = Stopwatch.StartNew();

            LFSR lfsr = new LFSR(seed.Text, Int32.Parse(tap.Text));
            encryptedImage = ImageEncrypterDecrypter.EncryptDecrypt(ImageMatrix, lfsr);
            ImageOperations.DisplayImage(encryptedImage, pictureBox2);

            sw.Stop();
            double time = sw.ElapsedMilliseconds;
            time /= 1e3;

            string caption = $"Encryption Exection Time in Seconds = {time} seconds\n";
            MessageBox.Show(caption, "Encryption Exection Time",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //compress only
        private void button6_Click(object sender, EventArgs e)
        {
            if (ImageMatrix == null || seed.Text.Length == 0 || tap.Text.Length == 0 ||
                 Byte.Parse(tap.Text) >= seed.Text.Length)
            {
                MessageBox.Show("Invalid input", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            HuffmanCoding.numberOfBytes = 0;

            Stopwatch sw = new Stopwatch();
            sw = Stopwatch.StartNew();

            BinaryWriter writer = new BinaryWriter(File.Open(fileName + ".bin", FileMode.Create));
            //Wrting seed and tap position to the binary file
            writer.Write(seed.Text);
            writer.Write(Byte.Parse(tap.Text));

            //Writing Huffman trees and the compressed image to binary file
            HuffmanCoding.CompressImage(ImageMatrix, ref writer);
            writer.Close();

            sw.Stop();
            double time = sw.ElapsedMilliseconds;
            time /= 1e3;

            double compressionRatio = Math.Round((originalSize / HuffmanCoding.numberOfBytes), 3);
            string caption = $"Compression Exection Time in Seconds = {time} seconds\n" +
                             $"Compressed Binary File Size = {HuffmanCoding.numberOfBytes} bytes\n" +
                             $"Compression Ratio = {compressionRatio}";
            MessageBox.Show(caption, "Compression Exection Time",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Decompress only
        private void button7_Click(object sender, EventArgs e)
        {
            if (binaryFilePath == null)
            {
                MessageBox.Show("Invalid input", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw = Stopwatch.StartNew();
            BinaryReader reader = new BinaryReader(File.Open(binaryFilePath, FileMode.Open));

            string seed = reader.ReadString();
            byte tap = reader.ReadByte();

            ImageOperations.DisplayImage(HuffmanCoding.DecompressImage(ref reader), pictureBox2);

            sw.Stop();
            double time = sw.ElapsedMilliseconds;
            time /= 1e3;

            double compressionRatio = Math.Round((HuffmanCoding.numberOfBytes / originalSize) * 100, 3);
            string caption = $"Decompression Exection Time in Seconds = {time} seconds\n";
            MessageBox.Show(caption, "Decompression Exection Time",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (ImageMatrix == null || binaryFilePath == null)
            {
                MessageBox.Show("Invalid input", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw = Stopwatch.StartNew();
            BinaryReader reader = new BinaryReader(File.Open(binaryFilePath, FileMode.Open));

            string seed = reader.ReadString();
            byte tap = reader.ReadByte();
            LFSR lfsr = new LFSR(seed, tap);
            Console.WriteLine(seed + " " + tap);
            reader.Close();

            encryptedImage = ImageEncrypterDecrypter.EncryptDecrypt(ImageMatrix, lfsr);
            ImageOperations.DisplayImage(encryptedImage, pictureBox2);

            sw.Stop();
            double time = sw.ElapsedMilliseconds;
            time /= 1e3;

            double compressionRatio = Math.Round((HuffmanCoding.numberOfBytes / originalSize) * 100, 3);
            string caption = $"Decryption Exection Time in Seconds = {time} seconds\n";
            MessageBox.Show(caption, "Decryption Exection Time",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (ImageMatrix == null || seed.Text.Length == 0 || tap.Text.Length == 0 ||
                 Byte.Parse(tap.Text) >= seed.Text.Length * 6)
            {
                MessageBox.Show("Invalid input", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            HuffmanCoding.numberOfBytes = 0;

            Stopwatch sw = new Stopwatch();
            sw = Stopwatch.StartNew();

            AlphaLFSR lfsr = new AlphaLFSR(seed.Text, Int32.Parse(tap.Text));
            encryptedImage = ImageEncrypterDecrypter.EncryptDecrypt(ImageMatrix, lfsr);
            ImageOperations.DisplayImage(encryptedImage, pictureBox2);

            sw.Stop();
            double time = sw.ElapsedMilliseconds;
            time /= 1e3;

            string caption = $"Encryption Exection Time in Seconds = {time} seconds\n";
            MessageBox.Show(caption, "Encryption Exection Time",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}