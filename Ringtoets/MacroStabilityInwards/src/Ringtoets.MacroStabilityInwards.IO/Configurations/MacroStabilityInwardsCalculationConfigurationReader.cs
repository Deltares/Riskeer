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
using System.Collections.Generic;
using System.Xml.Linq;
using Core.Common.Base.IO;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.MacroStabilityInwards.IO.Properties;

namespace Ringtoets.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// This class reads a macro stability inwards calculation configuration from XML and creates a collection of corresponding
    /// <see cref="IConfigurationItem"/>, typically containing one or more <see cref="MacroStabilityInwardsCalculationConfiguration"/>.
    /// </summary>
    public class MacroStabilityInwardsCalculationConfigurationReader : CalculationConfigurationReader<MacroStabilityInwardsCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationConfigurationReader"/>.
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
        internal MacroStabilityInwardsCalculationConfigurationReader(string xmlFilePath)
            : base(xmlFilePath,
                   Resources.MacroStabiliteitBinnenwaartsConfiguratieSchema,
                   new Dictionary<string, string>()) {}

        protected override MacroStabilityInwardsCalculationConfiguration ParseCalculationElement(XElement calculationElement)
        {
            var configuration = new MacroStabilityInwardsCalculationConfiguration(
                calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value)
            {
                AssessmentLevel = calculationElement.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.AssessmentLevelElement),
                HydraulicBoundaryLocationName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement),
                SurfaceLineName = calculationElement.GetStringValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SurfaceLineElement),
                StochasticSoilModelName = calculationElement.GetStringValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilModelElement),
                StochasticSoilProfileName = calculationElement.GetStringValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilProfileElement)
            };

            return configuration;
        }
    }
}