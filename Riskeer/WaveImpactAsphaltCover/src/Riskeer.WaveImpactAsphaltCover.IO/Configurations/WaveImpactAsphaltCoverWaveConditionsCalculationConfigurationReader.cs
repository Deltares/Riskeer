﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Revetment.IO.Configurations;
using Riskeer.WaveImpactAsphaltCover.IO.Properties;
using RiskeerRevetmentIOResources = Riskeer.Revetment.IO.Properties.Resources;

namespace Riskeer.WaveImpactAsphaltCover.IO.Configurations
{
    /// <summary>
    /// This class reads a wave conditions calculation configuration from XML and creates a collection of corresponding
    /// <see cref="IConfigurationItem"/>, typically containing one or more <see cref="WaveConditionsCalculationConfiguration"/>.
    /// </summary>
    public class WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationReader : WaveConditionsCalculationConfigurationReader<WaveConditionsCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationReader"/>.
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
        public WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationReader(string xmlFilePath)
            : base(xmlFilePath, new[]
            {
                Resources.GolfklappenOpAsfaltBekledingConfiguratieSchema_0,
                Resources.GolfklappenOpAsfaltBekledingConfiguratieSchema_1,
                Resources.GolfklappenOpAsfaltBekledingConfiguratieSchema
            }, new[]
            {
                RiskeerRevetmentIOResources.BekledingenConfiguratieBasisSchema0To1,
                RiskeerRevetmentIOResources.BekledingenConfiguratieBasisSchema1To2
            }) {}

        protected override WaveConditionsCalculationConfiguration ParseCalculationElement(XElement calculationElement)
        {
            var configuration = new WaveConditionsCalculationConfiguration(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value);
            ParseCalculationElementData(calculationElement, configuration);
            return configuration;
        }
    }
}