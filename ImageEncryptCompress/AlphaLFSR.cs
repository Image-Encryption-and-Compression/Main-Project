using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptCompress
{
    public class AlphaLFSR
    {


        private int tapPosition;
        private int lengthOfSeed;
        private int lengthOfExtendedSeed;

        private Int32[] seed;

        private string alphaNumericString = "0123456789+-abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int alphaNumericBits = 6;
        private Dictionary<char, int> alphaNumericDictionary;

        // Constructor
        public AlphaLFSR(string STR_seed, int tapPosition)
        {
            this.lengthOfSeed = STR_seed.Length;
            this.lengthOfExtendedSeed = lengthOfSeed * alphaNumericBits;

            this.tapPosition = lengthOfExtendedSeed - tapPosition - 1;

            this.seed = new Int32[lengthOfExtendedSeed];

            int lengthOfAlphaNumericString = alphaNumericString.Length;
            alphaNumericDictionary = new Dictionary<char, int>();
            for (int i = 0; i < lengthOfAlphaNumericString; i++)
            {
                alphaNumericDictionary[alphaNumericString[i]] = i;
            }

            for (int i = 0; i < lengthOfSeed; i++)
            {
                int number = alphaNumericDictionary[STR_seed[i]];
                int [] bits = ToBits(number);
                for (int j = 0; j < alphaNumericBits; j++)
                {
                    this.seed[(i * 6) + j] = bits[j];
                }
            }
        }

        public int ShiftBit()
        {
            int Bit = (seed[0] ^ seed[tapPosition]);

            for (int i = 0; i < lengthOfExtendedSeed - 1; i++)
            {
                seed[i] = seed[i + 1];
            }
            
            seed[lengthOfExtendedSeed - 1] = Bit;

            return Bit;
        }

        public int GenerateKey(int k)
        {
            int key = 0;
            for (int i = 1; i <= k; i++)
            {
                int Bit = ShiftBit();
                key *= 2;
                key += Bit;
            }
            return key;
        }



        private int[] ToBits(int x)
        {
            string binaryString = Convert.ToString(x, 2).PadLeft(alphaNumericBits, '0'); 
            int[] bits = new int[alphaNumericBits];

            for (int i = 0; i < alphaNumericBits; i++)
            {
                bits[i] = int.Parse(binaryString[i].ToString()); 
            }

            return bits;
        }
    }
}
