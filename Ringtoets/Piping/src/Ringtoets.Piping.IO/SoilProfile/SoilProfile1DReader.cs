using System;
using System.Data.SQLite;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class is responsible for reading 1d profile definitions from the Soil database using a constructed reader
    /// and transform the definitions to a <see cref="PipingSoilProfile"/>.
    /// </summary>
    internal static class SoilProfile1DReader
    {
        /// <summary>
        /// Reads a 1D profile from the given <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">A <see cref="IRowBasedDatabaseReader"/> which is used to read row values from.</param>
        /// <returns>A new <see cref="PipingSoilProfile"/>, which is based on the information from the database.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when reading the profile encountered an unrecoverable error.</exception>
        /// <exception cref="PipingSoilProfileReadException">Thrown when reading the profile encountered a recoverable error.</exception>
        internal static PipingSoilProfile ReadFrom(IRowBasedDatabaseReader reader)
        {
            var criticalProperties = new CriticalProfileProperties(reader);

            var profileName = criticalProperties.ProfileName;
            var requiredProperties = new RequiredProfileProperties(reader, profileName);

            var soilProfileBuilder = new SoilProfileBuilder1D(profileName, requiredProperties.Bottom);

            for (var i = 1; i <= criticalProperties.LayerCount; i++)
            {
                SoilLayer1D soilLayer = ReadSoilLayerFrom(reader, profileName);
                soilProfileBuilder.Add(soilLayer.AsPipingSoilLayer());
                reader.MoveNext();
            }

            return Build(soilProfileBuilder, profileName);
        }

        /// <summary>
        /// Builds a <see cref="SoilLayer1D"/> from the given <paramref name="soilProfileBuilder"/>.
        /// </summary>
        /// <exception cref="PipingSoilProfileReadException">Thrown when building the <see cref="PipingSoilProfile"/> failed.</exception>
        private static PipingSoilProfile Build(SoilProfileBuilder1D soilProfileBuilder, string profileName)
        {
            try
            {
                return soilProfileBuilder.Build();
            }
            catch (SoilProfileBuilderException e)
            {
                throw new PipingSoilProfileReadException(profileName, e.Message, e);
            }
        }

        /// <summary>
        /// Reads a <see cref="SoilLayer1D"/> from the given <paramref name="reader"/>.
        /// </summary>
        /// <exception cref="PipingSoilProfileReadException">Thrown when reading properties of the layers failed.</exception>
        private static SoilLayer1D ReadSoilLayerFrom(IRowBasedDatabaseReader reader, string profileName)
        {
            var properties = new LayerProperties(reader, profileName);

            var pipingSoilLayer = new SoilLayer1D(properties.Top)
            {
                IsAquifer = properties.IsAquifer,
                BelowPhreaticLevel = properties.BelowPhreaticLevel,
                AbovePhreaticLevel = properties.AbovePhreaticLevel,
                DryUnitWeight = properties.DryUnitWeight
            };
            return pipingSoilLayer;
        }

        private class RequiredProfileProperties
        {
            internal readonly double Bottom;

            /// <summary>
            /// Creates a new instance of <see cref="RequiredProfileProperties"/>, which contains properties
            /// that are required to create a complete <see cref="PipingSoilProfile"/>. If these properties
            /// cannot be read, then the reader can proceed to the next profile.
            /// </summary>
            /// <param name="reader">The <see cref="SQLiteDataReader"/> to read the required profile property values from.</param>
            /// <param name="profileName">The profile name used in generating exceptions messages if casting failed.</param>
            /// <exception cref="PipingSoilProfileReadException">Thrown when the values in the database could not be 
            /// casted to the expected column types.</exception>
            internal RequiredProfileProperties(IRowBasedDatabaseReader reader, string profileName)
            {
                string readColumn = SoilProfileDatabaseColumns.Bottom;
                try
                {
                    Bottom = reader.Read<double>(readColumn);
                }
                catch (InvalidCastException e)
                {
                    var message = string.Format(Resources.PipingSoilProfileReader_Profile_0_has_invalid_value_on_Column_1_, profileName, readColumn);
                    throw new PipingSoilProfileReadException(profileName, message, e);
                }
            }
        }

        private class LayerProperties
        {
            internal readonly double Top;
            internal readonly double? IsAquifer;
            internal readonly double? BelowPhreaticLevel;
            internal readonly double? AbovePhreaticLevel;
            internal readonly double? DryUnitWeight;

            /// <summary>
            /// Creates a new instance of <see cref="LayerProperties"/>, which contains properties
            /// that are required to create a complete <see cref="SoilLayer1D"/>. If these properties
            /// cannot be read, then the reader can proceed to the next profile.
            /// </summary>
            /// <param name="reader">The <see cref="SQLiteDataReader"/> to read the required layer property values from.</param>
            /// <param name="profileName">The profile name used in generating exceptions messages if casting failed.</param>
            /// <exception cref="PipingSoilProfileReadException">Thrown when the values in the database could not be 
            /// casted to the expected column types.</exception>
            internal LayerProperties(IRowBasedDatabaseReader reader, string profileName)
            {
                string readColumn = SoilProfileDatabaseColumns.Top;
                try
                {
                    Top = reader.Read<double>(readColumn);

                    readColumn = SoilProfileDatabaseColumns.IsAquifer;
                    IsAquifer = reader.ReadOrNull<double>(readColumn);

                    readColumn = SoilProfileDatabaseColumns.BelowPhreaticLevel;
                    BelowPhreaticLevel = reader.ReadOrNull<double>(readColumn);

                    readColumn = SoilProfileDatabaseColumns.AbovePhreaticLevel;
                    AbovePhreaticLevel = reader.ReadOrNull<double>(readColumn);

                    readColumn = SoilProfileDatabaseColumns.DryUnitWeight;
                    DryUnitWeight = reader.ReadOrNull<double>(readColumn);
                }
                catch (InvalidCastException e)
                {
                    var message = string.Format(Resources.PipingSoilProfileReader_Profile_0_has_invalid_value_on_Column_1_, profileName, readColumn);
                    throw new PipingSoilProfileReadException(profileName, message, e);
                }
            }
        }
    }
}