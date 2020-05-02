//using System;
//using System.ComponentModel;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//
//namespace FormatPreservingEncryption
//{
//    public class A2
//    {
//        public int BlockSize { get; set; } = 4; // bytes
//        public int Rounds { get; set; } = 10;
//
//        private UInt32[] GetBlocks(string text)
//        {
//            var plainTextBytes = Encoding.ASCII.GetBytes(text).ToList();
//            var numberOfBlocks = (int) Math.Ceiling(plainTextBytes.Count / (double) BlockSize);
//            var neededPaddingLength = numberOfBlocks * BlockSize - plainTextBytes.Count;
//            for (int i = 0; i < neededPaddingLength; i++)
//            {
//                plainTextBytes.Insert(0, new byte());
//            }
//
//            var blocks = new UInt32[numberOfBlocks];
//            for (int i = 0; i < numberOfBlocks; i++)
//            {
//                blocks[i] = ToInt32(plainTextBytes.Skip(i * BlockSize).Take(BlockSize).ToArray());
//            }
//
//            return blocks;
//        }
//
//        private UInt32 ToInt32(byte[] block)
//        {
//            var result = 0;
//            for (int i = 0; i < BlockSize; i++)
//            {
//                result += block[i] * (int) Math.Pow(256, BlockSize - i - 1);
//            }
//
//            return (uint) result;
//        }
//
//        private int Split(int n)
//        {
//            return (int) Math.Floor((double) n / 2);
//        }
//
//        public string Encrypt(string X, string T, string K)
//        {
//            var rawBlocks = GetBlocks(X);
//            var cipheredBlocks = new uint[rawBlocks.Length];
//            for (int i = 0; i < cipheredBlocks.Length; i++)
//            {
//                cipheredBlocks[i] = F(rawBlocks[i], K);
//            }
//
//        }
//
//        private uint F(uint originalBlock, string key)
//        {
//            throw new NotImplementedException();
//        }
//
//        // UInt16 is half of a block. 
//        private UInt16 RoundFunction(UInt16 halfBlock, UInt16 key)
//        {
//            UInt16 result;
//            result = (UInt16) (halfBlock ^ key);
//            result = (UInt16) (result << 5);
//            return result;
//        }
//    }
//}