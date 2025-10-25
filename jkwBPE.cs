//
//  jkwBPE.cs
//  this is a simple implementation of byte pair encoding
//  probably not the most efficient one
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text;


namespace jkwBPE
{
    public class Token
    {
        public Token(string token)
        {
            Value = token;
        }
        public Token(char c)
        {
            Value = c.ToString();
        }
        public Token(string first, string second)
        {
            Value = $"{first}{second}";
        }
        public Token(Token firstToken, Token secondToken)
        {
            Value = $"{firstToken.Value}{secondToken.Value}";
        }

        public string Value { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Token token)
            {
                return this.Value == token.Value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }

    public class jkwBPE
    {
        public List<Token> Vocabulary { get; set; }

        public jkwBPE()
        {
            Vocabulary = new List<Token>();
        }
        private List<Token> TokenizeString(string text)
        {
            List<Token> tokens = new List<Token>();

            foreach (char c in text)
            {
                tokens.Add(new Token(c));
            }

            return tokens;
        }
        private void InitializeVocabulary(List<Token> tokens)
        {
            foreach (var token in tokens)
            {
                Vocabulary.Add(new Token(token.Value));
            }
        }
        private List<Token> MakePairs(List<Token> tokens)
        {
            List<Token> pairs = new List<Token>();

            for (var counter = 0; counter < tokens.Count - 1; counter++)
            {
                var firstToken = tokens[counter];
                var secondToken = tokens[counter + 1]; //just gotta make sure we stop before we go past the end
                pairs.Add(new Token($"{firstToken.Value}{secondToken.Value}"));
            }
            return pairs;
        }

        private Dictionary<string, int> CountPairs(List<Token> pairs)
        {
            Dictionary<string, int> pairCount = new Dictionary<string, int>();

            foreach (Token pair in pairs)
            {
                if (false == pairCount.ContainsKey(pair.Value))
                {
                    pairCount.Add(pair.Value, 1);
                }
                else
                {
                    var count = pairCount[pair.Value];
                    pairCount[pair.Value] = count + 1;
                }
            }

            return pairCount;
        }

        private Token GetMostFrequentTokenPair(Dictionary<string, int> pairs)
        {
            Token mostFrequentToken = new Token(pairs.Keys.First<string>());
            int mostFrequentCount = pairs[mostFrequentToken.Value];
            foreach (string token in pairs.Keys)
            {
                var count = pairs[token];
                if (count > mostFrequentCount)
                {
                    mostFrequentCount = count;
                    mostFrequentToken = new Token(token);
                }
            }
            Vocabulary.Add(mostFrequentToken);
            return mostFrequentToken;
        }

        private List<Token> MergeMostFrequentTokenPairs(List<Token> tokens, out bool NoPairs)
        {
            List<Token> pairs = MakePairs(tokens);
            Dictionary<String, int> tokenFrequencyCount = CountPairs(pairs);
            Token mostFrequentPair = GetMostFrequentTokenPair(tokenFrequencyCount);

            NoPairs = true;
            List<Token> mergedTokens = new List<Token>();
            for (var counter = 0; counter < tokens.Count; counter++)
            {
                var first = tokens[counter];
                if(counter+1 == tokens.Count)
                {
                    // dealing with the last token in the chain
                    mergedTokens.Add(first);
                    break;
                }
                var second = tokens[counter+1];
                var paired = new Token(first, second);

                if (paired.Value == mostFrequentPair.Value)
                {
                    //merge the two tokens into a single token
                    mergedTokens.Add(paired);
                    NoPairs = false; 
                    counter++; //we need to step past this pair
                }
                else
                {
                    // it's not what we're looking for so just add the first one to the collection
                    mergedTokens.Add(first);
                }
            }

            return mergedTokens;
        }
        private void PrintPairs(List<Token> pairs, bool noPairsFound)
        {
            Console.Write("Pairs: ");
            foreach (Token token in pairs)
            {
                Console.Write($"{token.Value}, ");
            }
            Console.WriteLine($" - {noPairsFound.ToString()}");
        }
        public List<Token> BytePairEncodeString(string content, int iterations)
        {
            //first tokenize the string
            List<Token> tokens = TokenizeString(content);
            InitializeVocabulary(tokens);
            // while we find pairs to merge and we're belong the iterations
            bool NoPairsFound = false;
            var counter = 0;
            while (false == NoPairsFound && counter < iterations)
            {
                tokens = MergeMostFrequentTokenPairs(tokens, out NoPairsFound);
                PrintPairs(tokens, NoPairsFound);
                counter++;
            }

            return tokens;
        }
    }
}