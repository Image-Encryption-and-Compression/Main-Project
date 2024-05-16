using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ImageEncryptCompress
{
    public class LFSR
    {

        //- Class Data Members --/
        private int tapPosition;
        private int lengthOfSeed;
        private ulong seed;
      
        // Constructor
        public LFSR(string STR_seed, int tapPosition)
        {
            this.lengthOfSeed = STR_seed.Length;

            this.tapPosition = tapPosition;
            this.seed = Convert.ToUInt64(STR_seed,2); 
        }


        //- Class Methods --/

        /// <summary>
        /// This Function shift the leftmost Bit of the seed & Append the returned value of XOR operation in the seed 
        /// and replacing the tap value (only in the function) to shift from left side of string not right side  
        /// </summary>
        /// <returns> Return the a Bit to Be used in generateKey() function </returns>
        public int ShiftBit()
        {

            int bit;
            ulong copy = seed;
            ulong x = (ulong) 1 << lengthOfSeed - 1;
            ulong y = (ulong) 1 << tapPosition;

            if (((seed & x) > 0 && (copy & y) > 0) || ((seed & x) == 0 && (copy & y) == 0))
                bit = 0;
            else
                bit = 1;

            ulong z = ((ulong)1 << lengthOfSeed - 1) - 1;
            seed &= z;

            seed <<= 1;

            seed += (ulong)bit;

            return bit;
        }


        /// <summary>
        /// This Function is to generate a new Binary Key of K Bits
        /// </summary>
        /// <param name= "K"> the lenght of the Binary Key</param>
        /// <returns> Return a Decimal Key to be used in encrypting the image components </returns>

        public byte GenerateKey(int k)
        {
            byte key = 0;
            for (int i = 1; i <= k; i++)
            {
                byte bit = (byte)ShiftBit();

                key *= 2;
                key += bit;
            } 
            return key;
        }

    }
}


