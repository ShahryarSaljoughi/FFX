using System;

namespace FormatPreservingEncryption
{
    public class FormatPreservingEncryptionService
    {
        public int BlockSize { get; set; } // number of bits
        public string AllowedChars { get; set; }
        public uint CipherWidth { get; set; }
        public string Key { get; set; }
        private int Rounds = 10;

        public string Encrypt(string plainText)
        {
            var n = plainText.Length;
            throw new NotImplementedException();
        }

        private uint Split(uint length)
        {
            return length / 2;
        }
    }
}