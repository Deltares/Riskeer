// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Data.SQLite;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.MacroStabilityInwards.IO.Builders;
using Ringtoets.MacroStabilityInwards.IO.Exceptions;
using Ringtoets.MacroStabilityInwards.IO.Properties;
using Ringtoets.MacroStabilityInwards.IO.SoilProfile.Schema;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.SoilProfile
{
    /// <summary>
    /// This class is responsible for reading 2d profile definitions from the Soil database using a constructed reader
    /// and transform the definitions to a <see cref="MacroStabilityInwardsSoilProfile1D"/>.
    /// </summary>
    internal static class SoilProfile2DReader
    {
        /// <summary>
        /// Reads information for a profile from the database and creates a <see cref="MacroStabilityInwardsSoilProfile1D"/> based on the information.
        /// </summary>
        /// <param name="reader">A <see cref="IRowBasedDatabaseReader"/> which is used to read row values from.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilProfile1D"/>, which is based on the information from the database.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when encountering an unrecoverable error while reading the profile.</exception>
        /// <exception cref="MacroStabilityInwardsSoilProfileReadException">Thrown when
        /// <list type="bullet">
        /// <item>a layer's geometry cannot be parsed as XML;</item>
        /// <item>the parsed geometry does not contain loops;</item>
        /// <item>after reading the layers, no layers are added to be build;</item>
        /// <item>unexpected values are encountered for layer properties;</item>
        /// <item>stochastic layer properties are not defined with a lognormal distribution
        /// or is shifted when it should not be</item>
        /// </list> 
        /// </exception>
        internal static MacroStabilityInwardsSoilProfile1D ReadFrom(IRowBasedDatabaseReader reader)
        {
            var criticalProperties = new CriticalProfileProperties(reader);
            var requiredProperties = new RequiredProfileProperties(reader, criticalProperties.ProfileName);

            try
            {
                var soilProfileBuilder = new SoilProfileBuilder2D(criticalProperties.ProfileName, requiredProperties.IntersectionX, criticalProperties.ProfileId);

                for (var i = 1; i <= criticalProperties.LayerCount; i++)
                {
                    SoilLayer2D soilLayer2D = ReadMacroStabilityInwards2DSoilLayer(reader, criticalProperties.ProfileName);
                    soilProfileBuilder.Add(soilLayer2D);
                    reader.MoveNext();
                }

                return soilProfileBuilder.Build();
            }
            catch (SoilProfileBuilderException e)
            {
                throw CreateMacroStabilityInwardsSoilProfileReadException(reader.Path, criticalProperties.ProfileName, e);
            }
            catch (ArgumentException e)
            {
                throw CreateMacroStabilityInwardsSoilProfileReadException(reader.Path, criticalProperties.ProfileName, e);
            }
        }

        /// <summary>
        /// Reads a soil layer from a 2d profile in the database.
        /// </summary>
        /// <param name="reader">The <see cref="SQLiteDataReader"/> to read the layer property values from.</param>
        /// <param name="profileName">The profile name used in generating exceptions messages if casting failed.</param>
        /// <returns>A new <see cref="SoilLayer2D"/> instance, based on the information read from the database.</returns>
        /// <exception cref="MacroStabilityInwardsSoilProfileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>A column for a layer property did not contain a value of the expected type.</item>
        /// <item>The database contains an invalid XML definition for geometry.</item>
        /// <item>The read geometry does not contain segments that form a loop for either the inner or outer loop.</item>
        /// </list></exception>
        private static SoilLayer2D ReadMacroStabilityInwards2DSoilLayer(IRowBasedDatabaseReader reader, string profileName)
        {
            var properties = new LayerProperties(reader, profileName);

            SoilLayer2D soilLayer;
            try
            {
                byte[] geometryValue = ReadGeometryFrom(reader, profileName);
                soilLayer = new SoilLayer2DReader().Read(geometryValue);
            }
            catch (SoilLayerConversionException e)
            {
                throw CreateMacroStabilityInwardsSoilProfileReadException(reader.Path, profileName, e);
            }

            if (soilLayer != null)
            {
                soilLayer.IsAquifer = properties.IsAquifer;
                soilLayer.MaterialName = properties.MaterialName;
                soilLayer.Color = properties.Color;
            }
            return soilLayer;
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
                return reader.Read<byte[]>(SoilProfileTableColumns.LayerGeometry);
            }
            catch (InvalidCastException e)
            {
                throw CreateMacroStabilityInwardsSoilProfileReadException(reader.Path, profileName, e);
            }
        }

        private static MacroStabilityInwardsSoilProfileReadException CreateMacroStabilityInwardsSoilProfileReadException(string filePath, string profileName, string errorMessage, Exception innerException)
        {
            string message = new FileReaderErrorMessageBuilder(filePath)
                .WithSubject(string.Format(Resources.MacroStabilityInwardsSoilProfileReader_SoilProfileName_0_, profileName))
                .Build(errorMessage);
            return new MacroStabilityInwardsSoilProfileReadException(message, profileName, innerException);
        }

        private static MacroStabilityInwardsSoilProfileReadException CreateMacroStabilityInwardsSoilProfileReadException(string filePath, string profileName, Exception innerException)
        {
            string message = new FileReaderErrorMessageBuilder(filePath)
                .WithSubject(string.Format(Resources.MacroStabilityInwardsSoilProfileReader_SoilProfileName_0_, profileName))
                .Build(innerException.Message);
            return new MacroStabilityInwardsSoilProfileReadException(message, profileName, innerException);
        }

        private class RequiredProfileProperties
        {
            internal readonly double IntersectionX;

            /// <summary>
            /// Creates a new instance of <see cref="RequiredProfileProperties"/>, which contains properties
            /// that are required to create a complete <see cref="MacroStabilityInwardsSoilProfile1D"/>. If these properties
            /// cannot be read, then the reader can proceed to the next profile.
            /// </summary>
            /// <param name="reader">The <see cref="SQLiteDataReader"/> to read the required profile property values from.</param>
            /// <param name="profileName">The profile name used in generating exceptions messages if casting failed.</param>
            /// <exception cref="MacroStabilityInwardsSoilProfileReadException">Thrown when the values in the database could not be 
            /// casted to the expected column types.</exception>
            internal RequiredProfileProperties(IRowBasedDatabaseReader reader, string profileName)
            {
                const string readColumn = SoilProfileTableColumns.IntersectionX;
                try
                {
                    IntersectionX = reader.Read<double>(readColumn);
                }
                catch (InvalidCastException e)
                {
                    string message = string.Format(Resources.MacroStabilityInwardsSoilProfileReader_Profile_has_invalid_value_on_Column_0_, readColumn);
                    throw CreateMacroStabilityInwardsSoilProfileReadException(reader.Path, profileName, message, e);
                }
            }
        }
    }
}