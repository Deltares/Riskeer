using System;
using System.Data.SQLite;

namespace Riskeer.Storage.Core
{
    /// <summary>
    /// Class containing helper methods related to <see cref="SQLiteConnection"/> instances.
    /// </summary>
    internal class SQLiteConnectionHelper
    {
        /// <summary>
        /// Method for forcefully disposing any active <see cref="SQLiteConnection"/>.
        /// </summary>
        /// <remarks>
        /// As an alternative to waiting for garbage collection during idle time, calling this
        /// method will result in removing redundant file locks instantly.
        /// </remarks>
        internal static void ForcefullyDisposeSQLiteConnection()
        {
            SQLiteConnection.ClearAllPools();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}