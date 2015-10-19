using System;
using System.IO;
using System.Text;
using GisSharpBlog.NetTopologySuite.IO;

// http://java.sun.com/j2se/1.4/docs/api/java/io/StreamTokenizer.html
// a better implementation could be written. Here is a good Java implementation of StreamTokenizer.
// http://www.flex-compiler.lcs.mit.edu/Harpoon/srcdoc/java/io/StreamTokenizer.html
// a C# StringTokenizer
// http://sourceforge.net/snippet/detail.php?type=snippet&id=101171

// Diego Guidi says:    for my port i have used the RTools.Util.StreamTokenized, and seems very powerful!
//                      this code is present only for compatibility with GeoTools.NET code.                        

namespace GisSharpBlog.NetTopologySuite.Utilities
{
    /// <summary>
    /// The StreamTokenizer class takes an input stream and parses it into "tokens", 
    /// allowing the tokens to be read one at a time. 
    /// The parsing process is controlled by a table and a number of flags that can be set to various states. 
    /// The stream tokenizer can recognize identifiers, numbers, quoted strings, and various comment style.
    /// </summary>
    /// <remarks>
    /// This is a crude c# implementation of Java's 
    /// <a href="http://java.sun.com/products/jdk/1.2/docs/api/java/io/StreamTokenizer.html">StreamTokenizer</a> class.
    /// </remarks>
    [Obsolete("This class is only for GeoTools.NET code compatibility: use RTools.Util.StreamTokenizer instead.")]
    public class GeoToolsStreamTokenizer
    {
        private TokenType _currentTokenType;
        private readonly TextReader _reader;
        private string _currentToken;
        private readonly bool _ignoreWhitespace = false;
        private int _lineNumber = 1;
        private int _colNumber = 1;

        /// <summary>
        /// Initializes a new instance of the StreamTokenizer class.
        /// </summary>
        /// <param name="reader">A TextReader with some text to read.</param>
        /// <param name="ignoreWhitespace">Flag indicating whether whitespace should be ignored.</param>
        public GeoToolsStreamTokenizer(TextReader reader, bool ignoreWhitespace)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            _reader = reader;
            _ignoreWhitespace = ignoreWhitespace;
        }

        /// <summary>
        /// The current line number of the stream being read.
        /// </summary>
        public int LineNumber
        {
            get
            {
                return _lineNumber;
            }
        }

        /// <summary>
        /// The current column number of the stream being read.
        /// </summary>
        public int Column
        {
            get
            {
                return _colNumber;
            }
        }

        /// <summary>
        /// If the current token is a number, this field contains the value of that number. 
        /// </summary>
        /// <remarks>
        /// If the current token is a number, this field contains the value of that number. The current token is a number when the value of the ttype field is TT_NUMBER.
        /// </remarks>
        /// <exception cref="FormatException">Current token is not a number in a valid format.</exception>
        public double GetNumericValue()
        {
            string number = GetStringValue();
            if (GetTokenType() == TokenType.Number)
            {
                return double.Parse(number);
            }
            throw new ParseException(String.Format("The token '{0}' is not a number at line {1} column {2}.", number, LineNumber, Column));
        }

        /// <summary>
        /// If the current token is a word token, this field contains a string giving the characters of the word token. 
        /// </summary>
        public string GetStringValue()
        {
            return _currentToken;
        }

        /// <summary>
        /// Gets the token type of the current token.
        /// </summary>
        /// <returns></returns>
        public TokenType GetTokenType()
        {
            return _currentTokenType;
        }

        /// <summary>
        /// Returns the next token.
        /// </summary>
        /// <param name="ignoreWhitespace">Determines is whitespace is ignored. True if whitespace is to be ignored.</param>
        /// <returns>The TokenType of the next token.</returns>
        public TokenType NextToken(bool ignoreWhitespace)
        {
            var nextTokenType = ignoreWhitespace ? NextNonWhitespaceToken() : NextTokenAny();
            return nextTokenType;
        }

        /// <summary>
        /// Returns the next token.
        /// </summary>
        /// <returns>The TokenType of the next token.</returns>
        public TokenType NextToken()
        {
            return NextToken(_ignoreWhitespace);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private TokenType NextTokenAny()
        {
            TokenType nextTokenType = TokenType.Eof;
            char[] chars = new char[1];
            _currentToken = String.Empty;
            _currentTokenType = TokenType.Eof;
            int finished = _reader.Read(chars, 0, 1);

            bool isNumber = false;
            bool isWord = false;
            byte[] ba = null;
            ASCIIEncoding AE = new ASCIIEncoding();
            char[] ascii = null;
            Char currentCharacter;
            Char nextCharacter;
            while (finished != 0)
            {
                // convert int to char
                ba = new Byte[]
                {
                    (byte) _reader.Peek()
                };
                ascii = AE.GetChars(ba);
                currentCharacter = chars[0];
                nextCharacter = ascii[0];
                _currentTokenType = GetType(currentCharacter);
                nextTokenType = GetType(nextCharacter);

                // handling of words with _
                if (isWord && currentCharacter == '_')
                {
                    _currentTokenType = TokenType.Word;
                }
                // handing of words ending in numbers
                if (isWord && _currentTokenType == TokenType.Number)
                {
                    _currentTokenType = TokenType.Word;
                }

                if (_currentTokenType == TokenType.Word && nextCharacter == '_')
                {
                    // enable words with _ inbetween
                    nextTokenType = TokenType.Word;
                    isWord = true;
                }
                if (_currentTokenType == TokenType.Word && nextTokenType == TokenType.Number)
                {
                    // enable words ending with numbers
                    nextTokenType = TokenType.Word;
                    isWord = true;
                }

                // handle negative numbers
                if (currentCharacter == '-' && nextTokenType == TokenType.Number && isNumber == false)
                {
                    _currentTokenType = TokenType.Number;
                    nextTokenType = TokenType.Number;
                }

                // this handles numbers with a decimal point
                if (isNumber && nextTokenType == TokenType.Number && currentCharacter == '.')
                {
                    _currentTokenType = TokenType.Number;
                }
                if (_currentTokenType == TokenType.Number && nextCharacter == '.' && isNumber == false)
                {
                    nextTokenType = TokenType.Number;
                    isNumber = true;
                }

                _colNumber++;
                if (_currentTokenType == TokenType.Eol)
                {
                    _lineNumber++;
                    _colNumber = 1;
                }

                _currentToken = _currentToken + currentCharacter;
                if (_currentTokenType != nextTokenType)
                {
                    finished = 0;
                }
                else if (_currentTokenType == TokenType.Symbol && currentCharacter != '-')
                {
                    finished = 0;
                }
                else
                {
                    finished = _reader.Read(chars, 0, 1);
                }
            }
            return _currentTokenType;
        }

        /// <summary>
        /// Determines a characters type (e.g. number, symbols, character).
        /// </summary>
        /// <param name="character">The character to determine.</param>
        /// <returns>The TokenType the character is.</returns>
        private TokenType GetType(char character)
        {
            if (Char.IsDigit(character))
            {
                return TokenType.Number;
            }
            else if (Char.IsLetter(character))
            {
                return TokenType.Word;
            }
            else if (character == '\n')
            {
                return TokenType.Eol;
            }
            else if (Char.IsWhiteSpace(character) || Char.IsControl(character))
            {
                return TokenType.Whitespace;
            }
            else // (Char.IsSymbol(character))
            {
                return TokenType.Symbol;
            }
        }

        /// <summary>
        /// Returns next token that is not whitespace.
        /// </summary>
        /// <returns></returns>
        private TokenType NextNonWhitespaceToken()
        {
            TokenType tokentype = NextTokenAny();
            while (tokentype == TokenType.Whitespace || tokentype == TokenType.Eol)
            {
                tokentype = NextTokenAny();
            }
            return tokentype;
        }
    }
}