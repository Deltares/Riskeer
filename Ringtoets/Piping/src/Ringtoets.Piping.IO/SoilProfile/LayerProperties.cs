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
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.SoilProfile
{
    /// <summary>
    /// Class responsible for reading layer properties from a database reader.
    /// </summary>
    internal class LayerProperties
    {
        internal readonly double? IsAquifer;
        internal readonly string MaterialName;
        internal readonly double? Color;
        internal readonly double? AbovePhreaticLevel;
        internal readonly double? DryUnitWeight;

        internal readonly long? BelowPhreaticLevelDistribution;
        internal readonly double? BelowPhreaticLevelShift;
        internal readonly double? BelowPhreaticLevelMean;
        internal readonly double? BelowPhreaticLevelDeviation;

        internal readonly long? DiameterD70Distribution;
        internal readonly double? DiameterD70Shift;
        internal readonly double? DiameterD70Mean;
        internal readonly double? DiameterD70Deviation;

        internal readonly long? PermeabilityDistribution;
        internal readonly double? PermeabilityShift;
        internal readonly double? PermeabilityMean;
        internal readonly double? PermeabilityDeviation;

        /// <summary>
        /// Creates a new instance of <see cref="LayerProperties"/>, which contains properties
        /// that are required to create a complete soil layer. If these properties
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
                IsAquifer = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileDatabaseColumns.MaterialName;
                MaterialName = reader.ReadOrDefault<string>(readColumn);

                readColumn = SoilProfileDatabaseColumns.Color;
                Color = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileDatabaseColumns.AbovePhreaticLevel;
                AbovePhreaticLevel = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileDatabaseColumns.DryUnitWeight;
                DryUnitWeight = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileDatabaseColumns.BelowPhreaticLevelDistribution;
                BelowPhreaticLevelDistribution = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileDatabaseColumns.BelowPhreaticLevelShift;
                BelowPhreaticLevelShift = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileDatabaseColumns.BelowPhreaticLevelMean;
                BelowPhreaticLevelMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileDatabaseColumns.BelowPhreaticLevelDeviation;
                BelowPhreaticLevelDeviation = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileDatabaseColumns.DiameterD70Distribution;
                DiameterD70Distribution = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileDatabaseColumns.DiameterD70Shift;
                DiameterD70Shift = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileDatabaseColumns.DiameterD70Mean;
                DiameterD70Mean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileDatabaseColumns.DiameterD70Deviation;
                DiameterD70Deviation = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileDatabaseColumns.PermeabilityDistribution;
                PermeabilityDistribution = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileDatabaseColumns.PermeabilityShift;
                PermeabilityShift = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileDatabaseColumns.PermeabilityMean;
                PermeabilityMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileDatabaseColumns.PermeabilityDeviation;
                PermeabilityDeviation = reader.ReadOrDefault<double?>(readColumn);
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