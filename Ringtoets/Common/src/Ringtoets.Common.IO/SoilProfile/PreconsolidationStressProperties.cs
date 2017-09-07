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
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Class responsible for reading preconsolidation stress properties from a 
    /// database reader.
    /// </summary>
    public class PreconsolidationStressProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="PreconsolidationStressProperties"/>
        /// which contains properties that are required to create preconsolidation
        /// stresses for a soil layer.
        /// </summary>
        /// <param name="reader">The <see cref="IRowBasedDatabaseReader"/> to obtain
        /// the preconsolidation stress properties from.</param>
        /// <param name="profileName">The profile name used for generating exception messages
        /// if reading property vailes fails.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters
        /// are <c>null</c>.</exception>
        /// <exception cref="SoilProfileReadException">Thrown when the values in the database
        /// cannot be casted to the expected column types.</exception>
        public PreconsolidationStressProperties(IRowBasedDatabaseReader reader, string profileName)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (profileName == null)
            {
                throw new ArgumentNullException(nameof(profileName));
            }

            string readColumn = null;
            try
            {
                readColumn = PreconsolidationStressTableDefinitions.PreconsolidationStressXCoordinate;
                XCoordinate = reader.ReadOrDefault<double?>(readColumn);
                readColumn = PreconsolidationStressTableDefinitions.PreconsolidationStressZCoordinate;
                ZCoordinate = reader.ReadOrDefault<double?>(readColumn);

                readColumn = PreconsolidationStressTableDefinitions.PreconsolidationStressCalculationValue;
                PreconsolidationStressCalculationValue = reader.ReadOrDefault<double?>(readColumn);

                readColumn = PreconsolidationStressTableDefinitions.PreconsolidationStressDistribution;
                PreconsolidationStressDistributionType = reader.ReadOrDefault<long?>(readColumn);
                readColumn = PreconsolidationStressTableDefinitions.PreconsolidationStressMean;
                PreconsolidationStressMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = PreconsolidationStressTableDefinitions.PreconsolidationStressCoefficientOfVariation;
                PreconsolidationStressCoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);
                readColumn = PreconsolidationStressTableDefinitions.PreconsolidationStressShift;
                PreconsolidationStressShift = reader.ReadOrDefault<double?>(readColumn);
            }
            catch (InvalidCastException e)
            {
                string message = new FileReaderErrorMessageBuilder(reader.Path)
                    .WithSubject(string.Format(Resources.SoilProfileReader_SoilProfileName_0_, profileName))
                    .Build(string.Format(Resources.SoilProfileReader_Profile_has_invalid_value_on_Column_0_, readColumn));
                throw new SoilProfileReadException(message, profileName, e);
            }
        }

        /// <summary>
        /// Gets the value representing the X coordinate of the preconsolidation stress location.
        /// [m]
        /// </summary>
        public double? XCoordinate { get; }

        /// <summary>
        /// Gets the value representing the Z coordinate of the preconsolidation stress location.
        /// [m]
        /// </summary>
        public double? ZCoordinate { get; }

        /// <summary>
        /// Gets the value representing the calculation value of the preconsolidation stress.
        /// [kN/m�]
        /// </summary>
        public double? PreconsolidationStressCalculationValue { get; }

        /// <summary>
        /// Gets the distribution type of the preconsolidation stress.
        /// </summary>
        public double? PreconsolidationStressDistributionType { get; }

        /// <summary>
        /// Gets the value representing the mean of the distribution for the preconsolidation stress.
        /// [kN/m�]
        /// </summary>
        public double? PreconsolidationStressMean { get; }

        /// <summary>
        /// Gets the value representing the coefficient of variation of the distribution for the preconsolidation stress.
        /// [kN/m�]
        /// </summary>
        public double? PreconsolidationStressCoefficientOfVariation { get; }

        /// <summary>
        /// Gets the value representing the shift of the distribution for the preconsolidation stress.
        /// [kN/m�]
        /// </summary>
        public double? PreconsolidationStressShift { get; }
    }
}