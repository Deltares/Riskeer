﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Xml;
using Ringtoets.Common.IO.Configurations.Export;
using Ringtoets.StabilityPointStructures.IO.Configurations.Helpers;

namespace Ringtoets.StabilityPointStructures.IO.Configurations
{
    /// <summary>
    /// Writer for writing <see cref="StabilityPointStructuresCalculationConfiguration"/> in XML format to file.
    /// </summary>
    public class StabilityPointStructuresCalculationConfigurationWriter : StructureCalculationConfigurationWriter<StabilityPointStructuresCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationConfigurationWriter"/>.
        /// </summary>
        /// <param name="filePath">The path of the file to write to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public StabilityPointStructuresCalculationConfigurationWriter(string filePath) : base(filePath) { }

        protected override void WriteSpecificStructureParameters(StabilityPointStructuresCalculationConfiguration configuration, XmlWriter writer)
        {
            WriteElementWhenContentAvailable(writer,
                                             StabilityPointStructuresConfigurationSchemaIdentifiers.EvaluationLevelElement,
                                             configuration.EvaluationLevel);
            WriteElementWhenContentAvailable(writer,
                                             StabilityPointStructuresConfigurationSchemaIdentifiers.FactorStormDurationOpenStructureElement,
                                             configuration.FactorStormDurationOpenStructure);
            WriteElementWhenContentAvailable(writer,
                                             StabilityPointStructuresConfigurationSchemaIdentifiers.FailureProbabilityRepairClosureElement,
                                             configuration.FailureProbabilityRepairClosure);

            WriteConfigurationInflowModelTypeWhenAvailable(writer, configuration.InflowModelType);

            WriteConfigurationLoadSchematizationTypeWhenAvailable(writer, configuration.LoadSchematizationType);

            WriteElementWhenContentAvailable(writer,
                                             StabilityPointStructuresConfigurationSchemaIdentifiers.LevellingCountElement,
                                             configuration.LevellingCount);
            WriteElementWhenContentAvailable(writer,
                                             StabilityPointStructuresConfigurationSchemaIdentifiers.ProbabilityCollisionSecondaryStructureElement,
                                             configuration.ProbabilityCollisionSecondaryStructure);
            WriteElementWhenContentAvailable(writer,
                                             StabilityPointStructuresConfigurationSchemaIdentifiers.VerticalDistanceElement,
                                             configuration.VerticalDistance);
            WriteElementWhenContentAvailable(writer,
                                             StabilityPointStructuresConfigurationSchemaIdentifiers.VolumicWeightWaterElement,
                                             configuration.VolumicWeightWater);
        }

        protected override void WriteSpecificStochasts(StabilityPointStructuresCalculationConfiguration configuration, XmlWriter writer)
        {
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.AreaFlowAperturesStochastName,
                                           configuration.AreaFlowApertures);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.BankWidthStochastName,
                                           configuration.BankWidth);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthLinearLoadModelStochastName,
                                           configuration.ConstructiveStrengthLinearLoadModel);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthQuadraticLoadModelStochastName,
                                           configuration.ConstructiveStrengthQuadraticLoadModel);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.DrainCoefficientStochastName,
                                           configuration.DrainCoefficient);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.FailureCollisionEnergyStochastName,
                                           configuration.FailureCollisionEnergy);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.FlowVelocityStructureClosableStochastName,
                                           configuration.FlowVelocityStructureClosable);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelStochastName,
                                           configuration.InsideWaterLevel);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelFailureConstructionStochastName,
                                           configuration.InsideWaterLevelFailureConstruction);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName,
                                           configuration.LevelCrestStructure);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.ShipMassStochastName,
                                           configuration.ShipMass);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.ShipVelocityStochastName,
                                           configuration.ShipVelocity);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityLinearLoadModelStochastName,
                                           configuration.StabilityLinearLoadModel);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityQuadraticLoadModelStochastName,
                                           configuration.StabilityQuadraticLoadModel);
            WriteDistributionWhenAvailable(writer,
                                           StabilityPointStructuresConfigurationSchemaIdentifiers.ThresholdHeightOpenWeirStochastName,
                                           configuration.ThresholdHeightOpenWeir);
        }

        private static void WriteConfigurationLoadSchematizationTypeWhenAvailable(XmlWriter writer, ConfigurationStabilityPointStructuresLoadSchematizationType? configuration)
        {
            if (!configuration.HasValue)
            {
                return;
            }

            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();
            writer.WriteElementString(StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationTypeElement,
                                      converter.ConvertToInvariantString(configuration.Value));
        }

        private static void WriteConfigurationInflowModelTypeWhenAvailable(XmlWriter writer, ConfigurationStabilityPointStructuresInflowModelType? configuration)
        {
            if (!configuration.HasValue)
            {
                return;
            }

            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();
            writer.WriteElementString(StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelTypeElement,
                                      converter.ConvertToInvariantString(configuration.Value));
        }
    }
}