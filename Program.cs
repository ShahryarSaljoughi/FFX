using System;
using System.Linq;
using FormatPreservignEncryption;

namespace FormatPreservingEncryption
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new[]
            {
                "10000", "10001", "10002", "10003", "10004", "10005", "10006", "10007", "10008", "10009", "10010",
                "10011", "10012"
            };
            var fiveDigitCipher = new FFX(new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'});
            fiveDigitCipher.BlockSize = 5;
            var result = input.Select(fiveDigitCipher.EncryptText).ToArray();

            var fourDigitCipher = new FFX(new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'});
            fourDigitCipher.BlockSize = 4;
            var result2 = new[] {"1000", "1001", "1002", "1003", "1111", "1112", "2222", "2223", "3456", "3457", "9980"}
                .Select(fourDigitCipher.EncryptText).ToArray();
            Console.WriteLine(result);
            Console.WriteLine(result2);
//            var ciphered = ?
        }
    }
}