using System;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class describes properties which are critical when reading soil profiles from a data source.
    /// If obtaining properties could not be obtained, then it is impossible to guarantee a correct import.
    /// </summary>
    internal class CriticalProfileProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="CriticalProfileProperties"/>, which contains properties
        /// that are critical for reading profiles. If these properties cannot be read, then something
        /// went wrong while querying the database.
        /// </summary>
        /// <param name="reader">The <see cref="IRowBasedDatabaseReader"/> from which to obtain the critical properties.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the values in the database could not be 
        /// casted to the expected column types.</exception>
        internal CriticalProfileProperties(IRowBasedDatabaseReader reader)
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

        /// <summary>
        /// The name of the profile to read
        /// </summary>
        internal string ProfileName { get; private set; }

        /// <summary>
        /// The number of layers that the profile to read has
        /// </summary>
        internal long LayerCount { get; private set; }
    }
}