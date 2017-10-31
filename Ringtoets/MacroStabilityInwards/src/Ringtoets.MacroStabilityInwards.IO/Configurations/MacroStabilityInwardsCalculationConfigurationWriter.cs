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
using Ringtoets.MacroStabilityInwards.IO.Configurations.Helpers;

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

            WriteCalculationElements(writer, configuration);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a grid configuration.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="gridLocationName">The name of the location of the grid.</param>
        /// <param name="configuration">The configuration for the grid that can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> or <paramref name="gridLocationName"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteGridWhenAvailable(XmlWriter writer, string gridLocationName, MacroStabilityInwardsGridConfiguration configuration)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (gridLocationName == null)
            {
                throw new ArgumentNullException(nameof(gridLocationName));
            }

            if (configuration == null)
            {
                return;
            }

            writer.WriteStartElement(gridLocationName);

            if (configuration.XLeft.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridXLeft,
                                          XmlConvert.ToString(configuration.XLeft.Value));
            }
            if (configuration.XRight.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridXRight,
                                          XmlConvert.ToString(configuration.XRight.Value));
            }
            if (configuration.ZTop.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridZTop,
                                          XmlConvert.ToString(configuration.ZTop.Value));
            }
            if (configuration.ZBottom.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridZBottom,
                                          XmlConvert.ToString(configuration.ZBottom.Value));
            }
            if (configuration.NumberOfVerticalPoints.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridNumberOfVerticalPoints,
                                          XmlConvert.ToString(configuration.NumberOfVerticalPoints.Value));
            }
            if (configuration.NumberOfHorizontalPoints.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridNumberOfHorizontalPoints,
                                          XmlConvert.ToString(configuration.NumberOfHorizontalPoints.Value));
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the elements of the <paramref name="configuration"/> in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The calculation configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is closed.</exception>
        private static void WriteCalculationElements(XmlWriter writer, MacroStabilityInwardsCalculationConfiguration configuration)
        {
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.AssessmentLevelElement,
                                             configuration.AssessmentLevel);
            WriteElementWhenContentAvailable(writer,
                                             ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement,
                                             configuration.HydraulicBoundaryLocationName);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SurfaceLineElement,
                                             configuration.SurfaceLineName);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilModelElement,
                                             configuration.StochasticSoilModelName);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilProfileElement,
                                             configuration.StochasticSoilProfileName);

            WriteDikeSoilScenarioWhenAvailable(writer,
                                               MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioElement,
                                               configuration.DikeSoilScenario);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SlipPlaneMinimumDepthElement,
                                             configuration.SlipPlaneMinimumDepth);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SlipPlaneMinimumLengthElement,
                                             configuration.SlipPlaneMinimumLength);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MaximumSliceWidthElement,
                                             configuration.MaximumSliceWidth);

            WriteGridWhenAvailable(writer,
                                   MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LeftGridElement,
                                   configuration.LeftGrid);
            WriteGridWhenAvailable(writer,
                                   MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.RightGridElement,
                                   configuration.RightGrid);

            WriteScenarioWhenAvailable(writer, configuration.Scenario);
        }

        /// <summary>
        /// Writes the <paramref name="dikeSoilScenario"/> in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="elementName">The XML element name.</param>
        /// <param name="dikeSoilScenario">The dike soil scenario to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion of
        /// <paramref name="dikeSoilScenario"/> cannot be performed.</exception>
        private static void WriteDikeSoilScenarioWhenAvailable(XmlWriter writer,
                                                               string elementName,
                                                               ConfigurationDikeSoilScenario? dikeSoilScenario)
        {
            if (!dikeSoilScenario.HasValue)
            {
                return;
            }

            var typeConverter = new ConfigurationDikeSoilScenarioTypeConverter();
            writer.WriteElementString(elementName,
                                      typeConverter.ConvertToInvariantString(dikeSoilScenario.Value));
        }
    }
}