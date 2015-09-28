using System;
using System.Globalization;
using System.Threading;

namespace DelftTools.Utils
{
    public static class CultureUtils
    {
        /// <summary>
        /// Use in combination with a using statement. For the duration of the using statement, the current thread is 
        /// switched to the specific culture. The original culture is restored at the end of the using statement.
        /// </summary>
        /// <param name="cultureName">eg nl-NL, en-US</param>
        /// <returns></returns>
        public static IDisposable SwitchToCulture(string cultureName)
        {
            return new CultureSwitch(CultureInfo.GetCultureInfo(cultureName));
        }

        /// <summary>
        /// Use in combination with a using statement. For the duration of the using statement, the current thread is 
        /// switched to the invariant culture. The original culture is restored at the end of the using statement.
        /// </summary>
        /// <returns></returns>
        public static IDisposable SwitchToInvariantCulture()
        {
            return new CultureSwitch(CultureInfo.InvariantCulture);
        }

        private class CultureSwitch : IDisposable
        {
            private readonly CultureInfo oldCulture;

            public CultureSwitch(CultureInfo cultureInfo)
            {
                oldCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = cultureInfo;
            }

            public void Dispose()
            {
                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }
    }
}