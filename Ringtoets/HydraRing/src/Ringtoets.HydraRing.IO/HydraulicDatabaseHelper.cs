using System.IO;
using Core.Common.IO.Exceptions;
using log4net;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabaseContext;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabaseContext;

namespace Ringtoets.HydraRing.IO
{
    public static class HydraulicDatabaseHelper
    {
        /// <summary>
        /// Attempts to connect to the <paramref name="filePath"/> as if it is a Hydraulic Boundary Locations 
        /// database with a Hydraulic Location Configurations database next to it.
        /// </summary>
        /// <param name="filePath">The path of the Hydraulic Boundary Locations database file.</param>
        /// <returns>A <see cref="string"/> describing the problem when trying to connect to the <paramref name="filePath"/> 
        /// or <c>null</c> if a connection could be correctly made.</returns>
        public static string ValidatePathForCalculation(string filePath)
        {
            try
            {
                using (var db = new HydraulicBoundarySqLiteDatabaseReader(filePath))
                {
                    db.GetVersion();
                }
                string hlcdFilePath = Path.Combine(Path.GetDirectoryName(filePath), "hlcd.sqlite");
                new HydraulicLocationConfigurationSqLiteDatabaseReader(hlcdFilePath).Dispose();
            }
            catch (CriticalFileReadException e)
            {
                return e.Message;
            }
            return null;
        }
    }
}