using System;

namespace Wti.Service
{
    /// <summary>
    /// The <see cref="DateTimeService"/> class is responsible for providing general <see cref="DateTime"/> operations.
    /// </summary>
    public static class DateTimeService
    {
        /// <summary>
        /// Gets the current time in HH:mm:ss format.
        /// </summary>
        public static String CurrentTimeAsString
        {
            get
            {
                return DateTime.Now.ToString("HH:mm:ss");
            }
        }
    }
}