using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DelftTools.Utils
{
    /// <summary>
    /// This static helper class tries to guess the datetime format, for example when parsing from a file. It
    /// returns the format itself, not the parsed datetime. This format can be used as an initial guess whenever 
    /// the user must specify the datetime format because the actual format is unknown. It's main method is
    /// TryGuessDateTimeFormat.
    /// 
    /// Should the guessing fail for what you believe is a common date time format, please add the format parts
    /// to the string arrays. Placeholder seperator in formats is '-' (the '-' will be substituted with the strings
    /// from the separator array). 
    /// </summary>
    public static class DateTimeFormatGuesser
    {
        private const string FallbackFormat = "dd-MM-yyyy HH:mm:ss";

        private static IEnumerable<string> GetMatchingDateTimeFormats(IEnumerable<string> formatsToTry,
                                                                      string dateTimeString)
        {
            foreach (var format in formatsToTry)
            {
                DateTime result;
                if (DateTime.TryParseExact(dateTimeString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    yield return format;
                }
            }
        }

        public static bool TryGuessDateTimeFormat(string dateTimeString, out string outFormat)
        {
            return TryGuessDateTimeFormat(new[] {dateTimeString}, out outFormat);
        }

        public static bool TryGuessDateTimeFormat(IEnumerable<string> dateTimeStrings, out string outFormat)
        {
            var guessedFormat =
                DateTimeFormats.GetAllFormats().FirstOrDefault(f => dateTimeStrings.All(dt => CanParseExact(dt, f)));

            if (guessedFormat != null)
            {
                outFormat = guessedFormat;
                return true;
            }
            outFormat = FallbackFormat;
            return false;
        }

        private static bool CanParseExact(string dateTime, string format)
        {
            DateTime dummy;
            return DateTime.TryParseExact(dateTime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dummy);
        }
    }
}
