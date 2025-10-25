
using System;
using System.Security.Principal;

namespace jkwBPE
{
    public class Program
    {
        public static void Main()
        {
            var bpe = new jkwBPE();
            var tokens = bpe.BytePairEncodeString("hickerory dickerory dock", 10);
            Console.Write("Tokens: ");
            foreach (Token token in tokens)
            {
                Console.Write($"{token.Value}, ");
            }
            Console.WriteLine("");

            Console.Write("Vocabulary: ");
            foreach(var token in bpe.Vocabulary)
            {
                Console.Write($"{token.Value}, ");
            }
            Console.WriteLine("");
        }
    }
}