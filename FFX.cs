using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

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
            // the block size is not necessarily even, hence, we are having an unbalanced Feistel network:

            var (left, right, remainder) = originalBlock.SplitInHalf();

            for (int i = 0; i < RoundNumber; i++)
            {
                var permutedLeftPart = RoundFunction(left);
                var temp = MergeBlockParts(permutedLeftPart, right);
                if (i < RoundNumber - 1)
                {
                    right = left;
                    left = temp;
                }
                else
                {
                    right = temp;
                }
            }

            var result = left + right;
            if (!(remainder is null))
            {
                result += remainder;
            }

            return result.ToBlock();
        }

        /// <summary>
        /// This is equivalent to XOR operation, but customized for our requirements.
        /// </summary>
        /// <param name="permutedLeftPart"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private BlockPart MergeBlockParts(BlockPart permutedLeftPart, BlockPart right)
        {
            if (permutedLeftPart.BlockSize != right.BlockSize)
                throw new Exception("operands should have the same block size to be merged");
            var result = new BlockPart() {BlockSize = permutedLeftPart.BlockSize};
            result.Characters = permutedLeftPart.Characters
                .Zip(right.Characters, (x, y) => (ushort) ((x + y) % Radix))
                .ToArray();
            return result;
        }

        private BlockPart RoundFunction(BlockPart b)
        {
            var result = new BlockPart(b.BlockSize);
            for (int i = 0; i < b.BlockSize; i++)
            {
                result.Characters[i] = (ushort) (Radix - b.Characters[i]);
            }

            return result;
        }

        private BlockPart AddBlockParts(BlockPart b1, BlockPart b2)
        {
            throw new NotImplementedException();
        }

        private ICollection<Block> GetBlocks(ICollection<ushort> text)
        {
            var numberOfNeededBlocks = (int) Math.Ceiling(text.Count / (double) BlockSize);
            var blocks = new Block[numberOfNeededBlocks];
            for (int i = 0; i < numberOfNeededBlocks; i++)
            {
                blocks[i] = new Block
                {
                    Characters = text.Skip(i * BlockSize).Take(BlockSize).PadIfNeeded(BlockSize, (ushort) 0).ToArray(),
                    BlockSize = BlockSize
                };
            }

            return blocks;
        }

        private ICollection<ushort> ConvertTextToNumbers(string text)
        {
            return text.Select(c => _charToNumAlphabetMapping[c]).ToArray();
        }

        private string ConvertNumbersToText(IEnumerable<ushort> numbers)
        {
            return new string(numbers.Select(n => _numToCharMapping[n]).ToArray());
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

        /// <summary>
        /// if block size is even, remainder is null. otherwise, it will be a BlockPart holding the last character of this block.
        /// remainder BlockPart, won't participate in ciphering. it will remain untouched in result. later we can use secret key to decide which character,
        /// will be considered as the remainder BlockPart.
        /// this is because, left and right parts should be of the same length for the cipher to work.
        /// </summary>
        /// <returns></returns>
        public (BlockPart left, BlockPart right, BlockPart remainder) SplitInHalf()
        {
            BlockPart left, right, remainder;
            if (BlockSize % 2 == 0)
            {
                remainder = null;
                left = this.SubRange<BlockPart>(0, BlockSize / 2);
                right = this.SubRange<BlockPart>(BlockSize / 2, BlockSize / 2);
            }
            else
            {
                remainder = this.SubRange<BlockPart>(BlockSize - 1, 1);
                left = this.SubRange<BlockPart>(0, (BlockSize - 1) / 2);
                right = this.SubRange<BlockPart>((BlockSize - 1) / 2, (BlockSize - 1) / 2);
            }

            return (left, right, remainder);
        }
    }

    class BlockPart : BlockBase<BlockPart>
    {
        public Block ToBlock() => new Block() {BlockSize = this.BlockSize, Characters = this.Characters};

        public static BlockPart operator +(BlockPart b1, BlockPart b2)
        {
            var result = new BlockPart();
            result.BlockSize = b1.BlockSize + b2.BlockSize;
            result.Characters = b1.Characters.Concat(b2.Characters).ToArray();
            return result;
        }

        public BlockPart()
        {
        }

        public BlockPart(int blockSize)
        {
            BlockSize = blockSize;
            Characters = new ushort[blockSize];
        }
    }

    class BlockBase<T> : BlockBase where T : BlockBase, new()
    {
        public void CopyTo(T source, out T destination)
        {
            destination = new T();
            destination.Characters = new ushort[source.Characters.Length];
            source.Characters.CopyTo(destination.Characters, 0);
        }

        /// <summary>
        /// start index is zero-based and is included in the returned Block.
        /// </summary>
        /// <typeparam name="U">type of output. must inherit from BlockBase</typeparam>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public U SubRange<U>(int start, int? count) where U : BlockBase, new()
        {
            if (count is null)
            {
                count = Characters.Length - start;
            }

            var result = new U();
            result.BlockSize = count.Value;
            result.Characters = Characters.Skip(start).Take(count.Value).ToArray();
            return result;
        }
    }

    public class BlockBase
    {
        public int BlockSize;
        public ushort[] Characters { get; set; }
    }

    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> PadIfNeeded<T>(this IEnumerable<T> input, int size, T padWidth)
        {
            var collection = input as T[] ?? input.ToArray();
            if (collection.Count() == size)
            {
                return collection;
            }

            var result = new List<T>(collection);
            result.AddRange(Enumerable.Repeat(padWidth, size - result.Count));
            return result;
        }
    }
}