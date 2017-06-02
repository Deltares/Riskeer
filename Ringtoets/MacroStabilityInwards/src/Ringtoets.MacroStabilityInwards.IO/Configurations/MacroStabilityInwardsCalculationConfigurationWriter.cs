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
using System.Xml;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Export;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// Writer for writing a macro stability inwards calculation configuration to XML.
    /// </summary>
    public class MacroStabilityInwardsCalculationConfigurationWriter : CalculationConfigurationWriter<MacroStabilityInwardsCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationConfigurationWriter"/>.
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
        public MacroStabilityInwardsCalculationConfigurationWriter(string filePath) : base(filePath) {}

        protected override void WriteCalculation(MacroStabilityInwardsCalculationConfiguration configuration, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.CalculationElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, configuration.Name);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.AssessmentLevelElement,
                                             configuration.AssessmentLevel);
            WriteElementWhenContentAvailable(writer,
                                             ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement,
                                             configuration.HydraulicBoundaryLocation);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SurfaceLineElement,
                                             configuration.SurfaceLine);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilModelElement,
                                             configuration.StochasticSoilModel);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilProfileElement,
                                             configuration.StochasticSoilProfile);

            writer.WriteEndElement();
        }
    }
}