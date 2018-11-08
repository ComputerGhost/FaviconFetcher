using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaviconFetcher.Utility
{
    class TextParser
    {
        public StreamReader Reader { get; private set; }

        public bool EndOfStream
        {
            get { return Reader.EndOfStream; }
        }

        public TextParser(StreamReader reader)
        {
            Reader = reader;
        }

        // Skip a series of the character.
        public void Skip(char c)
        {
            while (!Reader.EndOfStream && Reader.Peek() == c)
                Reader.Read();
        }

        // Skip a series of whitespace
        public void SkipWhitespace()
        {
            while (!Reader.EndOfStream && char.IsWhiteSpace((char)Reader.Peek()))
                Reader.Read();
        }

        // Read until a needle is found.
        public void SkipUntil(string needle)
        {
            int foundIndex = 0;
            while (!Reader.EndOfStream && foundIndex < needle.Length)
            {
                if (Reader.Read() == needle[foundIndex])
                    ++foundIndex;
                else
                    foundIndex = 0;
            }
        }

        // Read until one of the needles are found, and returns it.
        // Warning: it will break if a needle is an empty string.
        public string SkipUntil(params string[] needles)
        {
            var foundIndices = new int[needles.Length];
            while (!Reader.EndOfStream)
            {
                var currentChar = Reader.Read();
                for (var i = 0; i != needles.Length; ++i)
                {
                    if (currentChar == needles[i][foundIndices[i]])
                    {
                        ++foundIndices[i];
                        if (foundIndices[i] >= needles[i].Length)
                            return needles[i];
                    }
                    else
                        foundIndices[i] = 0;
                }
            }
            return null;
        }

        // Look at the next character, leaving it to be read again.
        public int Peek()
        {
            return Reader.Peek();
        }

        // Read the next character.
        public int Read()
        {
            return Reader.Read();
        }

        // Returns all text up to (and not including) the needle.
        // Also removes the needle from the stream.
        public string ReadUntil(string needle)
        {
            if (Reader.EndOfStream)
                return null;

            var builder = new StringBuilder();
            int foundIndex = 0;
            while (foundIndex < needle.Length)
            {
                var currentChar = Reader.Read();
                if (Reader.EndOfStream)
                    return builder.ToString();

                builder.Append((char)currentChar);

                if (currentChar == needle[foundIndex])
                    ++foundIndex;
                else
                    foundIndex = 0;
            }

            builder.Remove(builder.Length - needle.Length, needle.Length);
            return builder.ToString();
        }

    }
}
