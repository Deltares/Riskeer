// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.Primitives;

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
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">Thrown when reading the profile encountered an unrecoverable error.</exception>
        /// <exception cref="PipingSoilProfileReadException">Thrown when reading the profile encountered a recoverable error.</exception>
        internal static PipingSoilProfile ReadFrom(IRowBasedDatabaseReader reader)
        {
            var criticalProperties = new CriticalProfileProperties(reader);

            var profileId = criticalProperties.ProfileId;
            var profileName = criticalProperties.ProfileName;
            var requiredProperties = new RequiredProfileProperties(reader, profileName);

            var soilProfileBuilder = new SoilProfileBuilder1D(profileName, requiredProperties.Bottom, profileId);

            for (var i = 1; i <= criticalProperties.LayerCount; i++)
            {
                SoilLayer1D soilLayer = ReadSoilLayerFrom(reader, profileName);
                soilProfileBuilder.Add(soilLayer.AsPipingSoilLayer());
                reader.MoveNext();
            }

            return Build(soilProfileBuilder, reader.Path, profileName);
        }

        /// <summary>
        /// Builds a <see cref="SoilLayer1D"/> from the given <paramref name="soilProfileBuilder"/>.
        /// </summary>
        /// <exception cref="PipingSoilProfileReadException">Thrown when building the <see cref="PipingSoilProfile"/> failed.</exception>
        private static PipingSoilProfile Build(SoilProfileBuilder1D soilProfileBuilder, string path, string profileName)
        {
            try
            {
                return soilProfileBuilder.Build();
            }
            catch (SoilProfileBuilderException e)
            {
                var message = new FileReaderErrorMessageBuilder(path)
                    .WithSubject(string.Format(Resources.PipingSoilProfileReader_SoilProfileName_0_, profileName))
                    .Build(e.Message);
                throw new PipingSoilProfileReadException(profileName, message, e);
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
                    var message = new FileReaderErrorMessageBuilder(reader.Path)
                        .WithSubject(String.Format(Resources.PipingSoilProfileReader_SoilProfileName_0_, profileName))
                        .Build(string.Format(Resources.PipingSoilProfileReader_Profile_has_invalid_value_on_Column_0_, readColumn));
                    throw new PipingSoilProfileReadException(profileName, message, e);
                }
            }
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
                    var message = new FileReaderErrorMessageBuilder(reader.Path)
                        .WithSubject(string.Format(Resources.PipingSoilProfileReader_SoilProfileName_0_, profileName))
                        .Build(string.Format(Resources.PipingSoilProfileReader_Profile_has_invalid_value_on_Column_0_, readColumn));
                    throw new PipingSoilProfileReadException(profileName, message, e);
                }
            }
        }
    }
}