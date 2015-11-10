using System;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.SoilProfile
{
    internal class CriticalProfileProperties
    {
        internal readonly string ProfileName;
        internal readonly long LayerCount;

        /// <summary>
        /// Creates a new instance of <see cref="CriticalProfileProperties"/>, which contains properties
        /// that are critical for reading profiles. If these properties cannot be read, then something
        /// went wrong while querying the database.
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="CriticalFileReadException">Thrown when the values in the database could not be 
        /// casted to the expected column types.</exception>
        internal CriticalProfileProperties(IRowBasedReader reader)
        {
            try
            {
                ProfileName = reader.Read<string>(SoilProfileDatabaseColumns.ProfileName);
                LayerCount = reader.Read<long>(SoilProfileDatabaseColumns.LayerCount);
            }
            catch (InvalidCastException e)
            {
                throw new CriticalFileReadException(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column, e);
            }
        }
    }
}