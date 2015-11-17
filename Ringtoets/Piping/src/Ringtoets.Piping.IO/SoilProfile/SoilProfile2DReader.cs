using System;
using System.Data.SQLite;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// This class is responsible for reading 2d profile definitions from the Soil database using a constructed reader
    /// and transform the definitions to a <see cref="PipingSoilProfile"/>.
    /// </summary>
    internal static class SoilProfile2DReader
    {
        /// <summary>
        /// Reads information for a profile from the database and creates a <see cref="PipingSoilProfile"/> based on the information.
        /// </summary>
        /// <param name="reader">A <see cref="IRowBasedDatabaseReader"/> which is used to read row values from.</param>
        /// <returns>A new <see cref="PipingSoilProfile"/>, which is based on the information from the database.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when reading the profile encountered an unrecoverable error.</exception>
        /// <exception cref="PipingSoilProfileReadException">Thrown when
        /// <list type="bullet">
        /// <item>a layer's geometry could not be parsed as XML;</item>
        /// <item>the parsed geometry did not contain loops;</item>
        /// <item>after reading the layers, no layers were added to be build;</item>
        /// <item>unexpected values were encountered for layer properties</item>
        /// </list> 
        /// </exception>
        internal static PipingSoilProfile ReadFrom(IRowBasedDatabaseReader reader)
        {
            var criticalProperties = new CriticalProfileProperties(reader);
            var requiredProperties = new RequiredProfileProperties(reader, criticalProperties.ProfileName);

            try
            {
                var soilProfileBuilder = new SoilProfileBuilder2D(criticalProperties.ProfileName, requiredProperties.IntersectionX);

                for (int i = 1; i <= criticalProperties.LayerCount; i++)
                {
                    var pipingSoilLayer2D = ReadPiping2DSoilLayer(reader, criticalProperties.ProfileName);
                    soilProfileBuilder.Add(pipingSoilLayer2D);
                    reader.MoveNext();
                }

                return soilProfileBuilder.Build();
            }
            catch (SoilProfileBuilderException e)
            {
                throw new PipingSoilProfileReadException(criticalProperties.ProfileName, e.Message, e);
            }
            catch (ArgumentException e)
            {
                throw new PipingSoilProfileReadException(criticalProperties.ProfileName, e.Message, e);
            }
        }

        /// <summary>
        /// Reads a soil layer from a 2d profile in the database.
        /// </summary>
        /// <param name="reader">The <see cref="SQLiteDataReader"/> to read the layer property values from.</param>
        /// <param name="profileName">The profile name used in generating exceptions messages if casting failed.</param>
        /// <returns>A new <see cref="SoilLayer2D"/> instance, based on the information read from the database.</returns>
        /// <exception cref="PipingSoilProfileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>A column for a layer property did not contain a value of the expected type.</item>
        /// <item>Thrown when the read geometry does not contain segments that form form a loop for either the inner or outer loop.</item>
        /// </list></exception>
        private static SoilLayer2D ReadPiping2DSoilLayer(IRowBasedDatabaseReader reader, string profileName)
        {
            var properties = new LayerProperties(reader, profileName);

            SoilLayer2D pipingSoilLayer;
            try
            {
                var geometryValue = ReadGeometryFrom(reader, profileName);
                pipingSoilLayer = new SoilLayer2DReader(geometryValue).Read();
            }
            catch (SoilLayer2DConversionException e)
            {
                throw new PipingSoilProfileReadException(profileName, e.Message, e);
            }

            if (pipingSoilLayer != null)
            {
                pipingSoilLayer.IsAquifer = properties.IsAquifer;
                pipingSoilLayer.BelowPhreaticLevel = properties.BelowPhreaticLevel;
                pipingSoilLayer.AbovePhreaticLevel = properties.AbovePhreaticLevel;
                pipingSoilLayer.DryUnitWeight = properties.DryUnitWeight;
            }
            return pipingSoilLayer;
        }

        /// <summary>
        /// Reads the geometry for a layer from the current <paramref name="reader"/>
        /// </summary>
        /// <param name="reader">The <see cref="SQLiteDataReader"/> to read the geometry value from.</param>
        /// <param name="profileName">The profile name used in generating exceptions messages if casting failed.</param>
        /// <returns></returns>
        private static byte[] ReadGeometryFrom(IRowBasedDatabaseReader reader, string profileName)
        {
            try
            {
                return reader.Read<byte[]>(SoilProfileDatabaseColumns.LayerGeometry);
            }
            catch (InvalidCastException e)
            {
                var message = string.Format(
                    Resources.PipingSoilProfileReader_Profile_0_has_invalid_value_on_column_1_, 
                    profileName, 
                    SoilProfileDatabaseColumns.LayerGeometry
                );
                throw new PipingSoilProfileReadException(profileName, message, e);
            }
        }
        

        private class RequiredProfileProperties
        {
            internal readonly double IntersectionX;

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
                string readColumn = SoilProfileDatabaseColumns.IntersectionX;
                try
                {
                    IntersectionX = reader.Read<double>(readColumn);
                }
                catch (InvalidCastException e)
                {
                    var message = string.Format(Resources.PipingSoilProfileReader_Profile_0_has_invalid_value_on_column_1_, profileName, readColumn);
                    throw new PipingSoilProfileReadException(profileName, message, e);
                }
            }
        }

        private class LayerProperties
        {
            internal readonly double? IsAquifer;
            internal readonly double? BelowPhreaticLevel;
            internal readonly double? AbovePhreaticLevel;
            internal readonly double? DryUnitWeight;

            /// <summary>
            /// Creates a new instance of <see cref="LayerProperties"/>, which contains properties
            /// that are required to create a complete <see cref="SoilLayer2D"/>. If these properties
            /// cannot be read, then the reader can proceed to the next profile.
            /// </summary>
            /// <param name="reader">The <see cref="SQLiteDataReader"/> to read the required layer property values from.</param>
            /// <param name="profileName">The profile name used in generating exceptions messages if casting failed.</param>
            /// <exception cref="PipingSoilProfileReadException">Thrown when the values in the database could not be 
            /// casted to the expected column types.</exception>
            internal LayerProperties(IRowBasedDatabaseReader reader, string profileName)
            {
                string readColumn = SoilProfileDatabaseColumns.IsAquifer;
                try
                {
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
                    var message = string.Format(Resources.PipingSoilProfileReader_Profile_0_has_invalid_value_on_column_1_, profileName, readColumn);
                    throw new PipingSoilProfileReadException(profileName, message, e);
                }
            }
        }
    }
}