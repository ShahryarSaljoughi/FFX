using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FormatPreservignEncryption
{
    class FFX
    {
        public int BlockSize;
        private int RoundNumber => 10;
        public int Radix { get; }
        private readonly Dictionary<char, ushort> _charToNumAlphabetMapping;
        private readonly Dictionary<ushort, char> _numToCharMapping;

        public FFX(char[] alphabets)
        {
            Radix = alphabets.Length;
            _charToNumAlphabetMapping = new Dictionary<char, ushort>();
            _numToCharMapping = new Dictionary<ushort, char>();
            for (ushort i = 0; i < alphabets.Length; i++)
            {
                _charToNumAlphabetMapping.Add(alphabets[i], i);
                _numToCharMapping.Add(i, alphabets[i]);
            }
        }

        public string EncryptText(string text)
        {
            /* 1- conversions : text -> numeric -> blocks
               2 - encrypt each block
               3- convert back result: blocks -> numeric -> text*/

            var numericRepresentation = ConvertTextToNumbers(text);
            var plainBlocks = GetBlocks(numericRepresentation);
            var cipheredBlocks = plainBlocks.Select(EncryptBlock).ToArray();
            var cipheredNumericResult = cipheredBlocks.SelectMany(cb => cb.Characters).ToArray();
            var result = ConvertNumbersToText(cipheredNumericResult);
            return result;
        }

        private Block EncryptBlock(Block originalBlock)
        {
            // because the block size is not even, so we are having an unbalanced feistel network:
            // left and right parts do not have the same length:
            var (left, right) = originalBlock.Split(BlockSize / 2);
            var result = new Block() { BlockSize = originalBlock.BlockSize};

            for (int i = 0; i < RoundNumber; i++)
            {
                
            }
        }

        private BlockPart F(BlockPart b)
        {
            throw new NotImplementedException();
        }

        private BlockPart AddBlockParts(BlockPart b1, BlockPart b2)
        {
            throw new NotImplementedException();
        }

        private ICollection<Block> GetBlocks(ICollection<ushort> text)
        {
            var numberOnNeededBlocks = (int) Math.Ceiling(text.Count / (double) BlockSize);
            var blocks = new Block[numberOnNeededBlocks];
            for (int i = 0; i < numberOnNeededBlocks; i++)
            {
                blocks[i] = new Block
                    {Characters = text.Skip(i * BlockSize).Take(BlockSize).ToArray(), BlockSize = BlockSize};
            }

            return blocks;
        }

        private ICollection<ushort> ConvertTextToNumbers(string text)
        {
            return text.Select(c => _charToNumAlphabetMapping[c]).ToArray();
        }

        private string ConvertNumbersToText(IEnumerable<ushort> numbers)
        {
            return numbers.Select(n => _numToCharMapping[n]).ToString();
        }
    }

    class Block : BlockBase<Block>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"> index is zero based</param>
        /// <returns></returns>
        public (BlockPart left, BlockPart right) Split(int index)
        {
            var left = new BlockPart();
            var right = new BlockPart();

            left.Characters = Characters.Take(index + 1).ToArray();
            left.BlockSize = index + 1;
            right.Characters = Characters.Skip(index + 1).Take(BlockSize - index - 1).ToArray();
            right.BlockSize = BlockSize - index - 1;
            return (left, right);
        }

    }

    class BlockPart : BlockBase<BlockPart>
    {
    }

    class BlockBase<T> : BlockBase where T : BlockBase, new()
    {
        public void CopyTo(T source, out T destination)
        {
            destination = new T();
            destination.Characters = new ushort[source.Characters.Length];
            source.Characters.CopyTo(destination.Characters, 0);
        }
    }

    public class BlockBase
    {
        public int BlockSize;
        public ushort[] Characters;
    }
}