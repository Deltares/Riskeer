﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.Xml.Linq;
using Core.Common.Base.IO;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.Revetment.IO.Properties;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Revetment.IO.Configurations
{
    /// <summary>
    /// This class reads a wave conditions calculation configuration from XML and creates a collection of corresponding
    /// <see cref="IConfigurationItem"/>, typically containing one or more <see cref="WaveConditionsCalculationConfiguration"/>.
    /// </summary>
    /// <typeparam name="T">The type of the calculation configuration.</typeparam>
    public abstract class WaveConditionsCalculationConfigurationReader<T> : CalculationConfigurationReader<T>
        where T : WaveConditionsCalculationConfiguration
    {
        private const string hbLocationVersion0SchemaName = "HbLocatieSchema_0.xsd";
        private const string hbLocationVersion1SchemaName = "HbLocatieSchema.xsd";
        private const string orientationSchemaName = "OrientatieSchema.xsd";
        private const string foreshoreProfileSchemaName = "VoorlandProfielSchema.xsd";
        private const string waveReductionSchemaName = "GolfReductieSchema.xsd";
        private const string revetmentBaseVersion0SchemaName = "BekledingenConfiguratieBasisSchema_0.xsd";
        private const string revetmentBaseVersion1SchemaName = "BekledingenConfiguratieBasisSchema_1.xsd";
        private const string revetmentBaseVersion2SchemaName = "BekledingenConfiguratieBasisSchema_2.xsd";
        private const string revetmentBaseVersion3SchemaName = "BekledingenConfiguratieBasisSchema.xsd";

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfigurationReader{T}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to the XML file.</param>
        /// <param name="mainSchemaDefinitions">An <see cref="Array"/> of <see cref="string"/> representing the main schema definitions.</param>
        /// <param name="migrationScripts">An <see cref="Array"/> of <see cref="string"/> representing the migration scripts.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlFilePath"/> is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="migrationScripts"/> contains <c>null</c> elements.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not pass the schema validation.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain configuration elements.</item>
        /// <item><paramref name="mainSchemaDefinitions"/> is invalid.</item>
        /// </list>
        /// </exception>
        protected WaveConditionsCalculationConfigurationReader(string xmlFilePath, string[] mainSchemaDefinitions, string[] migrationScripts)
            : base(xmlFilePath, new[]
            {
                new CalculationConfigurationSchemaDefinition(
                    0, mainSchemaDefinitions[0],
                    new Dictionary<string, string>
                    {
                        {
                            revetmentBaseVersion0SchemaName, Resources.BekledingenConfiguratieBasisSchema_0
                        },
                        {
                            hbLocationVersion0SchemaName, RiskeerCommonIOResources.HbLocatieSchema_0
                        },
                        {
                            orientationSchemaName, RiskeerCommonIOResources.OrientatieSchema
                        },
                        {
                            foreshoreProfileSchemaName, RiskeerCommonIOResources.VoorlandProfielSchema
                        },
                        {
                            waveReductionSchemaName, RiskeerCommonIOResources.GolfReductieSchema
                        }
                    }, string.Empty),
                new CalculationConfigurationSchemaDefinition(
                    1, mainSchemaDefinitions[1],
                    new Dictionary<string, string>
                    {
                        {
                            revetmentBaseVersion1SchemaName, Resources.BekledingenConfiguratieBasisSchema_1
                        },
                        {
                            hbLocationVersion1SchemaName, RiskeerCommonIOResources.HbLocatieSchema
                        },
                        {
                            orientationSchemaName, RiskeerCommonIOResources.OrientatieSchema
                        },
                        {
                            foreshoreProfileSchemaName, RiskeerCommonIOResources.VoorlandProfielSchema
                        },
                        {
                            waveReductionSchemaName, RiskeerCommonIOResources.GolfReductieSchema
                        }
                    }, migrationScripts[0]),
                new CalculationConfigurationSchemaDefinition(
                    2, mainSchemaDefinitions[2],
                    new Dictionary<string, string>
                    {
                        {
                            revetmentBaseVersion2SchemaName, Resources.BekledingenConfiguratieBasisSchema_2
                        },
                        {
                            hbLocationVersion1SchemaName, RiskeerCommonIOResources.HbLocatieSchema
                        },
                        {
                            orientationSchemaName, RiskeerCommonIOResources.OrientatieSchema
                        },
                        {
                            foreshoreProfileSchemaName, RiskeerCommonIOResources.VoorlandProfielSchema
                        },
                        {
                            waveReductionSchemaName, RiskeerCommonIOResources.GolfReductieSchema
                        }
                    }, migrationScripts[1]),
                new CalculationConfigurationSchemaDefinition(
                    3, mainSchemaDefinitions[3],
                    new Dictionary<string, string>
                    {
                        {
                            revetmentBaseVersion3SchemaName, Resources.BekledingenConfiguratieBasisSchema
                        },
                        {
                            hbLocationVersion1SchemaName, RiskeerCommonIOResources.HbLocatieSchema
                        },
                        {
                            orientationSchemaName, RiskeerCommonIOResources.OrientatieSchema
                        },
                        {
                            foreshoreProfileSchemaName, RiskeerCommonIOResources.VoorlandProfielSchema
                        },
                        {
                            waveReductionSchemaName, RiskeerCommonIOResources.GolfReductieSchema
                        }
                    }, migrationScripts[2])
            }) {}

        protected abstract override T ParseCalculationElement(XElement calculationElement);

        protected void ParseCalculationElementData(XElement calculationElement, T configuration)
        {
            configuration.HydraulicBoundaryLocationName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement);
            configuration.TargetProbability = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.TargetProbability);
            configuration.UpperBoundaryRevetment = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.UpperBoundaryRevetment);
            configuration.LowerBoundaryRevetment = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.LowerBoundaryRevetment);
            configuration.UpperBoundaryWaterLevels = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.UpperBoundaryWaterLevels);
            configuration.LowerBoundaryWaterLevels = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.LowerBoundaryWaterLevels);
            configuration.StepSize = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.StepSize);
            configuration.ForeshoreProfileId = calculationElement.GetStringValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.ForeshoreProfile);
            configuration.Orientation = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.Orientation);
            configuration.WaveReduction = calculationElement.GetWaveReductionParameters();
        }
    }
}