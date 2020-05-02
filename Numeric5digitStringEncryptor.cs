using System;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace FormatPreservingEncryption
{
    public class Numeric5digitStringEncryptor
    {
        public int BlockSize { get; set; } = 2;
        public int Rounds { get; set; } = 10;

        public string Encrypt(string text)
        {
            var blocks = GetBlocks(text);
            var cipheredBlocks = new Block[blocks.Length];
            for (int i = 0; i < cipheredBlocks.Length; i++)
            {
                cipheredBlocks[i] = F(blocks[i]);
            }

            var cipheredText = cipheredBlocks
                .Select(b => Encoding.ASCII.GetString(b.bytes))
                .Aggregate((s1, s2) => s1 + s2);
            return cipheredText;

        }
        private Block F(Block originalBlock)
        {
            var leftPart = originalBlock.LeftPart;
            var rightPart = originalBlock.RightPart;

            for (int i = 0; i < Rounds; i++)
            {
                var permutedLeftPart = RoundFunction(leftPart);
                var r = permutedLeftPart ^ rightPart;
                if (i < Rounds - 1)
                {
                    rightPart = leftPart;
                    leftPart = r;
                }
                else
                {
                    rightPart = r;
                }
            }

            var result = new Block();
            result.LeftPart = leftPart;
            result.RightPart = rightPart;
            
            return result;
        }

        private Block[] GetBlocks(string text)
        {
            var plainTextBytes = text.ToCharArray().Select(BitConverter.GetBytes)
                .Aggregate((a1, a2) => a1.Concat(a2).ToArray()).ToList();
            var numberOfBlocks = (int) Math.Ceiling(plainTextBytes.Count / (double) BlockSize);
            var neededPaddingLength = numberOfBlocks * BlockSize - plainTextBytes.Count;
            for (int i = 0; i < neededPaddingLength; i++)
            {
                plainTextBytes.Insert(0, new byte());
            }

            var blocks = new Block[numberOfBlocks];
            for (int i = 0; i < numberOfBlocks; i++)
            {
                blocks[i] = new Block();
                blocks[i].bytes = plainTextBytes.Skip(i * BlockSize).Take(BlockSize).ToArray();
            }

            return blocks;
        }

        private HalfBlock RoundFunction(HalfBlock halfBlock)
        {
            var result = new HalfBlock(){bytes = halfBlock.bytes};
            result ^= 255;

            return result;
        }
    }

    public class Block
    {
        public int BlockSize { get; set; } = 2;
        public byte[] bytes { get; set; } = new byte[2];

        public HalfBlock LeftPart
        {
            get => new HalfBlock() {bytes = bytes.Take(BlockSize / 2).ToArray()};
            set
            {
                for (var i = 0; i < BlockSize / 2; i++)
                {
                    bytes[i] = value.bytes[i];
                }
            }
        }

        public HalfBlock RightPart
        {
            get => new HalfBlock() {bytes = bytes.Skip(BlockSize / 2).Take(BlockSize / 2).ToArray()};
            set
            {
                for (var i = 0; i < BlockSize / 2; i++)
                {
                    bytes[i + BlockSize/2] = value.bytes[i];
                }
            }
        }
    }

    public class HalfBlock
    {
        public byte[] bytes { get; set; } = new byte[1];

        public static HalfBlock operator ^(HalfBlock a, HalfBlock b)
        {
            var result = new HalfBlock();
            for (var i = 0; i < result.bytes.Length; i++)
            {
                result.bytes[i] = (byte) (a.bytes[i] ^ b.bytes[i]);
            }

            return result;
        }

        public static HalfBlock operator ^(HalfBlock a, int b)
        {
            var result = new HalfBlock();
            var secondOperandBytes = BitConverter.GetBytes(b);
            var halfBlockSize = 1;
            for (int i = 0; i < halfBlockSize; i++)
            {
                result.bytes[i] = (byte) (secondOperandBytes[i] ^ a.bytes[i]);
            }

            return result;
        }
    }
}