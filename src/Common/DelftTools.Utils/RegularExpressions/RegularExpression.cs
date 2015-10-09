using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DelftTools.Utils.RegularExpressions
{
    public static class RegularExpression
    {
        public const string CharactersAndQuote = @"['A-Za-z0-9\s\(\)-\\/\.\+\<\>,\|_&;:\[\]]*";
        public const string Integer = @"[0-9-]+";
        public const string Float = @"[0-9\.-]+"; // TODO: This is no float representation. What about "---0065---...---"?
        public const string OptionalFloat = @"[0-9\.-]*";
        public const string Characters = @"[#A-Za-z0-9\s\(\)-\\/\.\+\<\>,\|_&;:\[\]]*";
        // added support for extended ascii characters [192-259] À-ə
        public const string ExtendedCharacters = @"[#A-Za-zÀ-ə0-9\s\(\)-\\/\.\+\<\>,\|_&;:\[\]!=\""]*";
        public const string AnyNonGreedy = @".*?";
        public const string Scientific = @"\s*[\-\+]?([0-9]*\.[0-9]+|[0-9]+\.|[0-9]+)([eE][\-\+]?[0-9]+)?\s*";
        // Somewhat nasty RE for scientific, to support for base numbers as 1., 1.23, .4 
        public const string FileName = @"[#A-Za-zÀ-ə0-9\s\(\)-\\/\.\|_&~]*";

        public const string EndOfLine = @"\s*($|(\n|\r\n?))";

        // can be a memory leak, clear it regularly
        private static readonly IDictionary<string, Regex> expressions = new Dictionary<string, Regex>();

        public static Match GetFirstMatch(string pattern, string sourceText)
        {
            var matches = GetMatches(pattern, sourceText);
            return matches.OfType<Match>().FirstOrDefault();
        }

        public static void ClearExpressionsCache()
        {
            expressions.Clear();
        }

        public static MatchCollection GetMatches(string pattern, string sourceText)
        {
            Regex regex;

            if (!expressions.TryGetValue(pattern, out regex))
            {
                regex = new Regex(pattern, RegexOptions.Singleline);
                expressions[pattern] = regex;
            }

            MatchCollection matches = regex.Matches(sourceText);
            return matches;
        }

        public static string GetCharacters(string name)
        {
            return String.Format(@"{0}\s+'(?<{0}>" + Characters + @")'\s?", name);
        }

        public static string GetExtendedCharacters(string name)
        {
            return String.Format(@"{0}\s+'(?<{0}>" + ExtendedCharacters + @")'\s?", name);
        }

        /// <summary>
        /// parses integer variable optionally followed by a string : ' quote is required to avoid greediness
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static string GetIntegerOptionallyExtendedCharacters(string integer, string characters)
        {
            return String.Format(@"{0}\s+(?<{0}>" + Integer + @")\s?('(?<{1}>" + ExtendedCharacters + @")')?",
                                 integer, characters);
        }

        public static string GetInteger(string name)
        {
            //return String.Format(@"{0}\s'?(?<{0}>" + Integer + @")'?\s?", name);
            return String.Format(@"{0}\s+'?(?<{0}>" + Integer + @")+'?\s?", name);
        }

        // replaced \s with \s* to support multiple spaces
        /// <summary>
        /// Creates a regular expression for matching a white-space seperated key-value pair.
        /// </summary>
        /// <param name="name">The key</param>
        /// <returns>The regex for matching a key-value pair with value being a decimal number.</returns>
        /// <remarks>This method will not match to floating point numbers in scientific notations (i.e. 1.2e+3) correctly.
        /// The string 'A 1.2e+3' will be matched as 'A 1.2'.</remarks>
        /// <example>Calling this method with <paramref name="name"/> being 'A' will match a string
        /// like 'A   -1.234' or 'A 2' but not 'A234.2', 'B 1.2'.</example>
        /// <seealso cref="GetScientific"/>
        /// <seealso cref="GetFloatOptional"/>
        public static string GetFloat(string name)
        {
            return String.Format(@"{0}\s+(?<{0}>" + Float + @")+\s?", name);
        }

        public static string GetScientific(string name)
        {
            return String.Format(@"{0}\s+(?<{0}>" + Scientific + @")+", name);
        }

        public static string GetScientificArray(string name, int length)
        {
            return String.Format(@"\s+{0}\s+(?<{0}>(" + Scientific + @"\s+){1})", name, "{" + length + "}");
        }

        public static string GetFloatOptional(string name)
        {
            return String.Format(@"({0}\s+(?<{0}>" + Float + @")\s?)?", name);
        }

        public static string ParseFieldAsString(string field, string record)
        {
            var matches = GetMatches(GetCharacters(field), record);
            return matches.Count > 0 ? matches[0].Groups[field].Value : "";
        }

        public static string ParseFieldAsIntegerOrString(string field, string record)
        {
            var match = GetFirstMatch(GetInteger(field), record);
            if (match == null)
            {
                return ParseFieldAsString(field, record);
            }

            return int.Parse(match.Groups[field].Value).ToString();
        }

        public static double ParseFieldAsDouble(string field, string record)
        {
            var match = GetFirstMatch(GetScientific(field), record);
            return match == null ? 0.0 : ConversionHelper.ToDouble(match.Groups[field].Value);
        }

        public static int ParseFieldAsInt(string field, string record)
        {
            var match = GetFirstMatch(GetInteger(field), record);
            return match == null ? 0 : int.Parse(match.Groups[field].Value);
        }

        /// <summary>
        /// Retrieves a float value if it is available in the math.
        /// example:
        ///  culvert.BedLevelRight = RegularExpression.ParseSingle(match, "rl", culvert.BedLevelRight);
        /// </summary>
        /// <param name="match"></param>
        /// <param name="field">name of the field </param>
        /// <param name="defaultValue">default value</param>
        /// <returns>Value of the field if found else default value</returns>
        public static float ParseSingle(Match match, string field, float defaultValue)
        {
            return match.Groups[field].Success ? ConversionHelper.ToSingle(match.Groups[field].Value) : defaultValue;
        }

        public static double ParseDouble(Match match, string field, double defaultValue)
        {
            return match.Groups[field].Success ? ConversionHelper.ToDouble(match.Groups[field].Value) : defaultValue;
        }

        /// <summary>
        /// Retrieves a float value if it is available in the math.
        /// example:
        ///  culvert.Direction = RegularExpression.ParseInt(match, "rt", culvert.Direction);
        /// </summary>
        /// <param name="match"></param>
        /// <param name="field">name of the field </param>
        /// <param name="defaultValue">default value</param>
        /// <returns>Value of the field if found else default value</returns>
        public static int ParseInt(Match match, string field, int defaultValue)
        {
            return match.Groups[field].Success ? Convert.ToInt32(match.Groups[field].Value) : defaultValue;
        }

        /// <summary>
        /// Retrieves a float value if it is available in the math.
        /// example:
        ///  culvert.CrossSectionId = RegularExpression.ParseString(match, "si", culvert.CrossSectionId);
        /// </summary>
        /// <param name="match"></param>
        /// <param name="field">name of the field </param>
        /// <param name="defaultValue">default value</param>
        /// <returns>Value of the field if found else default value</returns>
        public static string ParseString(Match match, string field, string defaultValue)
        {
            return match.Groups[field].Success ? match.Groups[field].Value : defaultValue;
        }
    }
}