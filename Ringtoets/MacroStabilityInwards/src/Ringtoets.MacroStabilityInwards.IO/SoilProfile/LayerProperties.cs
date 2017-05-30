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
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.MacroStabilityInwards.IO.Exceptions;
using Ringtoets.MacroStabilityInwards.IO.Properties;
using Ringtoets.MacroStabilityInwards.IO.SoilProfile.Schema;

namespace Ringtoets.MacroStabilityInwards.IO.SoilProfile
{
    /// <summary>
    /// Class responsible for reading layer properties from a database reader.
    /// </summary>
    internal class LayerProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="LayerProperties"/>, which contains properties
        /// that are required to create a complete soil layer.
        /// </summary>
        /// <param name="reader">The <see cref="SQLiteDataReader"/> to obtain the required layer property values from.</param>
        /// <param name="profileName">The profile name used in generating exceptions messages if casting failed.</param>
        /// <exception cref="MacroStabilityInwardsSoilProfileReadException">Thrown when the values in the database cannot be 
        /// casted to the expected column types.</exception>
        internal LayerProperties(IRowBasedDatabaseReader reader, string profileName)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (profileName == null)
            {
                throw new ArgumentNullException(nameof(profileName));
            }

            string readColumn = SoilProfileTableColumns.IsAquifer;
            try
            {
                IsAquifer = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableColumns.MaterialName;
                MaterialName = reader.ReadOrDefault<string>(readColumn);

                readColumn = SoilProfileTableColumns.Color;
                Color = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableColumns.BelowPhreaticLevelDistribution;
                BelowPhreaticLevelDistribution = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableColumns.BelowPhreaticLevelShift;
                BelowPhreaticLevelShift = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableColumns.BelowPhreaticLevelMean;
                BelowPhreaticLevelMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableColumns.BelowPhreaticLevelDeviation;
                BelowPhreaticLevelDeviation = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableColumns.DiameterD70Distribution;
                DiameterD70Distribution = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableColumns.DiameterD70Shift;
                DiameterD70Shift = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableColumns.DiameterD70Mean;
                DiameterD70Mean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableColumns.DiameterD70CoefficientOfVariation;
                DiameterD70CoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableColumns.PermeabilityDistribution;
                PermeabilityDistribution = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableColumns.PermeabilityShift;
                PermeabilityShift = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableColumns.PermeabilityMean;
                PermeabilityMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableColumns.PermeabilityCoefficientOfVariation;
                PermeabilityCoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);
            }
            catch (InvalidCastException e)
            {
                string message = new FileReaderErrorMessageBuilder(reader.Path)
                    .WithSubject(string.Format(Resources.MacroStabilityInwardsSoilProfileReader_SoilProfileName_0_, profileName))
                    .Build(string.Format(Resources.MacroStabilityInwardsSoilProfileReader_Profile_has_invalid_value_on_Column_0_, readColumn));
                throw new MacroStabilityInwardsSoilProfileReadException(message, profileName, e);
            }
        }

        /// <summary>
        /// Gets a <see cref="double"/> value representing  whether the layer is an aquifer.
        /// </summary>
        internal double? IsAquifer { get; private set; }

        /// <summary>
        /// Gets the name of the material that was assigned to the layer.
        /// </summary>
        internal string MaterialName { get; private set; }

        /// <summary>
        /// Gets the value representing the color that was used to represent the layer.
        /// </summary>
        internal double? Color { get; private set; }

        /// <summary>
        /// Gets the distribution for the volumic weight of the layer below the 
        /// phreatic level.
        /// [kN/m³]
        /// </summary>
        internal long? BelowPhreaticLevelDistribution { get; private set; }

        /// <summary>
        /// Gets the shift of the distribution for the volumic weight of the layer 
        /// below the phreatic level.
        /// [kN/m³]
        /// </summary>
        internal double? BelowPhreaticLevelShift { get; private set; }

        /// <summary>
        /// Gets the mean of the distribution for the volumic weight of the layer 
        /// below the phreatic level.
        /// [kN/m³]
        /// </summary>
        internal double? BelowPhreaticLevelMean { get; private set; }

        /// <summary>
        /// Gets the deviation of the distribution for the volumic weight of the layer below the phreatic level.
        /// [kN/m³]
        /// </summary>
        internal double? BelowPhreaticLevelDeviation { get; private set; }

        /// <summary>
        /// Gets the distribution for the mean diameter of small scale tests applied to different kinds of sand, on which the 
        /// formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        internal long? DiameterD70Distribution { get; private set; }

        /// <summary>
        /// Gets the shift of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        internal double? DiameterD70Shift { get; private set; }

        /// <summary>
        /// Gets the mean of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        internal double? DiameterD70Mean { get; private set; }

        /// <summary>
        /// Gets the coefficient of variation of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        internal double? DiameterD70CoefficientOfVariation { get; private set; }

        /// <summary>
        /// Gets the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        internal long? PermeabilityDistribution { get; private set; }

        /// <summary>
        /// Gets the shift of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        internal double? PermeabilityShift { get; private set; }

        /// <summary>
        /// Gets the mean of the distribution for the the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        internal double? PermeabilityMean { get; private set; }

        /// <summary>
        /// Gets the coefficient of variation of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        internal double? PermeabilityCoefficientOfVariation { get; private set; }
    }
}