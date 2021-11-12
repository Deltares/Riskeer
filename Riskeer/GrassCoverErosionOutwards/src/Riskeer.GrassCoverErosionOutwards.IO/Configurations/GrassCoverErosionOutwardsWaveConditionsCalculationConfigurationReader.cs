﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Xml.Linq;
using Core.Common.Base.IO;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.GrassCoverErosionOutwards.IO.Configurations.Converters;
using Riskeer.GrassCoverErosionOutwards.IO.Properties;
using Riskeer.Revetment.IO.Configurations;
using RiskeerRevetmentIOResources = Riskeer.Revetment.IO.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.IO.Configurations
{
    /// <summary>
    /// This class reads a wave conditions calculation configuration from XML and creates a collection of corresponding
    /// <see cref="IConfigurationItem"/>, typically containing one or more <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationReader : WaveConditionsCalculationConfigurationReader<GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationReader"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to the XML file.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not pass the schema validation.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain configuration elements.</item>
        /// </list>
        /// </exception>
        public GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationReader(string xmlFilePath)
            : base(xmlFilePath, new[]
            {
                Resources.GEBUConfiguratieSchema_0,
                Resources.GEBUConfiguratieSchema_1,
                Resources.GEBUConfiguratieSchema
            }, new[]
            {
                RiskeerRevetmentIOResources.BekledingenConfiguratieBasisSchema0To1,
                Resources.GEBUConfiguratieSchema1To2
            }) {}

        protected override GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration ParseCalculationElement(XElement calculationElement)
        {
            var configuration = new GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value);
            ParseCalculationElementData(calculationElement, configuration);
            configuration.CalculationType = (ConfigurationGrassCoverErosionOutwardsCalculationType?)
                calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter>(
                    WaveConditionsCalculationConfigurationSchemaIdentifiers.CalculationType);
            return configuration;
        }
    }
}