using System;

namespace FormatPreservingEncryption
{
    class Program
    {
        static void Main(string[] args)
        {
            var encryptor = new Numeric5digitStringEncryptor();
            var e = encryptor.Encrypt("1");
            Console.WriteLine(e);
        }
    }
}
