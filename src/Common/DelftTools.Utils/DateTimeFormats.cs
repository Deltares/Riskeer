using System.Collections.Generic;
using System.Linq;

namespace DelftTools.Utils
{
    public static class DateTimeFormats
    {
        private static readonly string[] LikelyTimeSeperators = { ":", "." };
        private static readonly string[] LikelyDateSeperators = { "-", "/", " " };
        private static readonly string[] LikelyTimeFormats = {
            "HH-mm-ss", "H-mm-ss", "hh-mm-ss tt", "h-mm-ss tt", "HH-mm", "hh-mm tt", "H-mm", "h-mm tt"
        };
        private static readonly string[] LikelyDateFormats = {
            "dd-MM-yyyy", "MM-dd-yyyy", "yyyy-MM-dd",
            "d-M-yyyy", "M-d-yyyy", "yyyy-M-d",
            "dd-MM-yy", "MM-dd-yy", "yy-MM-dd",
            "d-M-yy", "M-d-yy", "yy-M-d",
        };
        
        public static IEnumerable<string> GetAllFormats()
        {
            return GetDateTimeFormats(true).Concat(GetDateTimeFormats(false)).Concat(DatePermutations);
        }

        private static IEnumerable<string> GetDateTimeFormats(bool dateFirst)
        {
            foreach (var time in TimePermutations)
            {
                foreach (var date in DatePermutations)
                {
                    yield return dateFirst ? date + " " + time : time + " " + date;
                }
            }
        }

        private static IEnumerable<string> timePermutations;
        private static IEnumerable<string> TimePermutations
        {
            get {
                return timePermutations ??
                       (timePermutations = GeneratePermutations(LikelyTimeFormats, LikelyTimeSeperators));
            }
        }

        private static IEnumerable<string> datePermutations;
        private static IEnumerable<string> DatePermutations
        {
            get
            {
                return datePermutations ??
                       (datePermutations = GeneratePermutations(LikelyDateFormats, LikelyDateSeperators));
            }
        }

        private static IEnumerable<string> GeneratePermutations(IEnumerable<string> formats, IEnumerable<string> separators)
        {
            return from format in formats from sep in separators select format.Replace("-", sep);
        }
    }
}