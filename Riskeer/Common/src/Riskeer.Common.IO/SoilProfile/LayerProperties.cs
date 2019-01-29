// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Builders;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// Class responsible for reading layer properties from a database reader.
    /// </summary>
    internal class LayerProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="LayerProperties"/> which contains properties
        /// that are required to create a complete soil layer.
        /// </summary>
        /// <param name="reader">The <see cref="IRowBasedDatabaseReader"/> to obtain the required 
        /// layer property values from.</param>
        /// <param name="profileName">The profile name used for generating exception messages 
        /// if reading property values fails.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is 
        /// <c>null</c>.</exception>
        /// <exception cref="SoilProfileReadException">Thrown when the values in the database 
        /// cannot be casted to the expected column types.</exception>
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

            string readColumn = SoilProfileTableDefinitions.IsAquifer;
            try
            {
                IsAquifer = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.MaterialName;
                MaterialName = reader.ReadOrDefault<string>(readColumn);

                readColumn = SoilProfileTableDefinitions.Color;
                Color = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.BelowPhreaticLevelDistributionType;
                BelowPhreaticLevelDistributionType = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableDefinitions.BelowPhreaticLevelShift;
                BelowPhreaticLevelShift = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.BelowPhreaticLevelMean;
                BelowPhreaticLevelMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.BelowPhreaticLevelDeviation;
                BelowPhreaticLevelDeviation = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.BelowPhreaticLevelCoefficientOfVariation;
                BelowPhreaticLevelCoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.DiameterD70DistributionType;
                DiameterD70DistributionType = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableDefinitions.DiameterD70Shift;
                DiameterD70Shift = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.DiameterD70Mean;
                DiameterD70Mean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.DiameterD70CoefficientOfVariation;
                DiameterD70CoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.PermeabilityDistributionType;
                PermeabilityDistributionType = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableDefinitions.PermeabilityShift;
                PermeabilityShift = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.PermeabilityMean;
                PermeabilityMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.PermeabilityCoefficientOfVariation;
                PermeabilityCoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.UsePop;
                UsePop = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.ShearStrengthModel;
                ShearStrengthModel = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.AbovePhreaticLevelDistributionType;
                AbovePhreaticLevelDistributionType = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableDefinitions.AbovePhreaticLevelMean;
                AbovePhreaticLevelMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.AbovePhreaticLevelCoefficientOfVariation;
                AbovePhreaticLevelCoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.AbovePhreaticLevelShift;
                AbovePhreaticLevelShift = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.CohesionDistributionType;
                CohesionDistributionType = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableDefinitions.CohesionMean;
                CohesionMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.CohesionCoefficientOfVariation;
                CohesionCoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.CohesionShift;
                CohesionShift = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.FrictionAngleDistributionType;
                FrictionAngleDistributionType = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableDefinitions.FrictionAngleMean;
                FrictionAngleMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.FrictionAngleCoefficientOfVariation;
                FrictionAngleCoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.FrictionAngleShift;
                FrictionAngleShift = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.ShearStrengthRatioDistributionType;
                ShearStrengthRatioDistributionType = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableDefinitions.ShearStrengthRatioMean;
                ShearStrengthRatioMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.ShearStrengthRatioCoefficientOfVariation;
                ShearStrengthRatioCoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.ShearStrengthRatioShift;
                ShearStrengthRatioShift = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.StrengthIncreaseExponentDistributionType;
                StrengthIncreaseExponentDistributionType = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableDefinitions.StrengthIncreaseExponentMean;
                StrengthIncreaseExponentMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.StrengthIncreaseExponentCoefficientOfVariation;
                StrengthIncreaseExponentCoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.StrengthIncreaseExponentShift;
                StrengthIncreaseExponentShift = reader.ReadOrDefault<double?>(readColumn);

                readColumn = SoilProfileTableDefinitions.PopDistributionType;
                PopDistributionType = reader.ReadOrDefault<long?>(readColumn);
                readColumn = SoilProfileTableDefinitions.PopMean;
                PopMean = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.PopCoefficientOfVariation;
                PopCoefficientOfVariation = reader.ReadOrDefault<double?>(readColumn);
                readColumn = SoilProfileTableDefinitions.PopShift;
                PopShift = reader.ReadOrDefault<double?>(readColumn);
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
        /// Gets a <see cref="double"/> value representing whether the layer is an aquifer.
        /// </summary>
        public double? IsAquifer { get; }

        /// <summary>
        /// Gets the name of the material that was assigned to the layer.
        /// </summary>
        public string MaterialName { get; }

        /// <summary>
        /// Gets the value representing the color that was used to represent the layer.
        /// </summary>
        public double? Color { get; }

        /// <summary>
        /// Gets the distribution type for the volumic weight of the layer below the 
        /// phreatic level.
        /// </summary>
        public long? BelowPhreaticLevelDistributionType { get; }

        /// <summary>
        /// Gets the shift of the distribution for the volumic weight of the layer 
        /// below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? BelowPhreaticLevelShift { get; }

        /// <summary>
        /// Gets the mean of the distribution for the volumic weight of the layer 
        /// below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? BelowPhreaticLevelMean { get; }

        /// <summary>
        /// Gets the deviation of the distribution for the volumic weight of the layer below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? BelowPhreaticLevelDeviation { get; }

        /// <summary>
        /// Gets the coefficient of variation of the distribution for the volumic weight of the layer below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? BelowPhreaticLevelCoefficientOfVariation { get; }

        /// <summary>
        /// Gets the distribution type for the mean diameter of small scale tests applied to different kinds of sand, on which the 
        /// formula of Sellmeijer has been fit.
        /// </summary>
        public long? DiameterD70DistributionType { get; }

        /// <summary>
        /// Gets the shift of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double? DiameterD70Shift { get; }

        /// <summary>
        /// Gets the mean of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double? DiameterD70Mean { get; }

        /// <summary>
        /// Gets the coefficient of variation of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double? DiameterD70CoefficientOfVariation { get; }

        /// <summary>
        /// Gets the distribution type for the Darcy-speed with which water flows through the aquifer layer.
        /// </summary>
        public long? PermeabilityDistributionType { get; }

        /// <summary>
        /// Gets the shift of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double? PermeabilityShift { get; }

        /// <summary>
        /// Gets the mean of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double? PermeabilityMean { get; }

        /// <summary>
        /// Gets the coefficient of variation of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double? PermeabilityCoefficientOfVariation { get; }

        /// <summary>
        /// Gets the value indicating whether to use POP for the layer.
        /// </summary>
        public double? UsePop { get; }

        /// <summary>
        /// Gets the shear strength model to use for the layer.
        /// </summary>
        public double? ShearStrengthModel { get; }

        /// <summary>
        /// Gets the distribution type for the volumic weight of the layer above the phreatic level.
        /// </summary>
        public long? AbovePhreaticLevelDistributionType { get; }

        /// <summary>
        /// Gets the mean of the distribution for the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? AbovePhreaticLevelMean { get; }

        /// <summary>
        /// Gets the deviation of the distribution for the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? AbovePhreaticLevelCoefficientOfVariation { get; }

        /// <summary>
        /// Gets the shift of the distribution for the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? AbovePhreaticLevelShift { get; }

        /// <summary>
        /// Gets the distribution type for the mean of the distribution for the cohesion.
        /// </summary>
        public long? CohesionDistributionType { get; }

        /// <summary>
        /// Gets the mean of the distribution for the cohesion.
        /// [kN/m³]
        /// </summary>
        public double? CohesionMean { get; }

        /// <summary>
        /// Gets the deviation of the distribution for the cohesion.
        /// [kN/m³]
        /// </summary>
        public double? CohesionCoefficientOfVariation { get; }

        /// <summary>
        /// Gets the shift of the distribution for the cohesion.
        /// [kN/m³]
        /// </summary>
        public double? CohesionShift { get; }

        /// <summary>
        /// Gets the distribution type for the friction angle.
        /// </summary>
        public long? FrictionAngleDistributionType { get; }

        /// <summary>
        /// Gets the mean of the distribution for the friction angle
        /// [°]
        /// </summary>
        public double? FrictionAngleMean { get; }

        /// <summary>
        /// Gets the deviation of the distribution for the friction angle.
        /// [°]
        /// </summary>
        public double? FrictionAngleCoefficientOfVariation { get; }

        /// <summary>
        /// Gets the shift of the distribution for the friction angle.
        /// [°]
        /// </summary>
        public double? FrictionAngleShift { get; }

        /// <summary>
        /// Gets the distribution type for the ratio of shear strength S
        /// </summary>
        public long? ShearStrengthRatioDistributionType { get; }

        /// <summary>
        /// Gets the mean of the distribution for the ratio of shear strength S
        /// [-]
        /// </summary>
        public double? ShearStrengthRatioMean { get; }

        /// <summary>
        /// Gets the deviation of the distribution for the ratio of shear strength S.
        /// [-]
        /// </summary>
        public double? ShearStrengthRatioCoefficientOfVariation { get; }

        /// <summary>
        /// Gets the shift of the distribution for the ratio of shear strength S.
        /// [-]
        /// </summary>
        public double? ShearStrengthRatioShift { get; }

        /// <summary>
        /// Gets the distribution type for the strength increase exponent (m).
        /// </summary>
        public long? StrengthIncreaseExponentDistributionType { get; }

        /// <summary>
        /// Gets the mean of the distribution for the strength increase exponent (m).
        /// [-]
        /// </summary>
        public double? StrengthIncreaseExponentMean { get; }

        /// <summary>
        /// Gets the deviation of the distribution for the strength increase exponent (m).
        /// [-]
        /// </summary>
        public double? StrengthIncreaseExponentCoefficientOfVariation { get; }

        /// <summary>
        /// Gets the shift of the distribution for the strength increase exponent (m).
        /// [-]
        /// </summary>
        public double? StrengthIncreaseExponentShift { get; }

        /// <summary>
        /// Gets the distribution type for the POP.
        /// </summary>
        public long? PopDistributionType { get; }

        /// <summary>
        /// Gets mean of the distribution for the POP.
        /// [kN/m²]
        /// </summary>
        public double? PopMean { get; }

        /// <summary>
        /// Gets the deviation of the distribution for the POP.
        /// [kN/m²]
        /// </summary>
        public double? PopCoefficientOfVariation { get; }

        /// <summary>
        /// Gets the shift of the distribution for the POP.
        /// [kN/m²]
        /// </summary>
        public double? PopShift { get; }
    }
}