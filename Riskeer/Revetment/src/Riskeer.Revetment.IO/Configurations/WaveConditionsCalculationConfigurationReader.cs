// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Revetment.IO.Configurations.Converters;
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
        private const string revetmentBaseVersion1SchemaName = "BekledingenConfiguratieBasisSchema.xsd";

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfigurationReader{T}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to the XML file.</param>
        /// <param name="mainSchemaDefinitions">A <c>string</c> representing the main schema definition.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not pass the schema validation.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain configuration elements.</item>
        /// <item><paramref name="mainSchemaDefinitions"/> is invalid.</item>
        /// </list>
        /// </exception>
        protected WaveConditionsCalculationConfigurationReader(string xmlFilePath, string[] mainSchemaDefinitions)
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
                            revetmentBaseVersion1SchemaName, Resources.BekledingenConfiguratieBasisSchema
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
                    }, Resources.BekledingenConfiguratieSchema0To1)
            }) {}

        protected abstract override T ParseCalculationElement(XElement calculationElement);

        protected void ParseCalculationElementData(XElement calculationElement, T configuration)
        {
            configuration.HydraulicBoundaryLocationName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement);
            configuration.UpperBoundaryRevetment = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.UpperBoundaryRevetment);
            configuration.LowerBoundaryRevetment = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.LowerBoundaryRevetment);
            configuration.UpperBoundaryWaterLevels = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.UpperBoundaryWaterLevels);
            configuration.LowerBoundaryWaterLevels = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.LowerBoundaryWaterLevels);
            configuration.StepSize = (ConfigurationWaveConditionsInputStepSize?) calculationElement.GetConvertedValueFromDescendantDoubleElement<ConfigurationWaveConditionsInputStepSizeConverter>(WaveConditionsCalculationConfigurationSchemaIdentifiers.StepSize);
            configuration.ForeshoreProfileId = calculationElement.GetStringValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.ForeshoreProfile);
            configuration.Orientation = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.Orientation);
            configuration.WaveReduction = calculationElement.GetWaveReductionParameters();
        }
    }
}