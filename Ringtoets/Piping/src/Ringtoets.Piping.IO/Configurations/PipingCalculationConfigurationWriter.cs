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

namespace Ringtoets.Piping.IO.Configurations
{
    /// <summary>
    /// Writer for writing a piping calculation configuration to XML.
    /// </summary>
    public class PipingCalculationConfigurationWriter : CalculationConfigurationWriter<PipingCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationConfigurationWriter"/>.
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
        public PipingCalculationConfigurationWriter(string filePath) : base(filePath) {}

        protected override void WriteCalculation(PipingCalculationConfiguration configuration, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.CalculationElement);

            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, configuration.Name);

            WriteCalculationElements(writer, configuration);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the elements of the <paramref name="configuration"/> in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The calculation configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is closed.</exception>
        private static void WriteCalculationElements(XmlWriter writer, PipingCalculationConfiguration configuration)
        {
            WriteElementWhenContentAvailable(writer,
                                             PipingCalculationConfigurationSchemaIdentifiers.WaterLevelElement,
                                             configuration.AssessmentLevel);
            WriteElementWhenContentAvailable(writer,
                                             ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElementNew,
                                             configuration.HydraulicBoundaryLocationName);

            WriteElementWhenContentAvailable(writer,
                                             PipingCalculationConfigurationSchemaIdentifiers.SurfaceLineElement,
                                             configuration.SurfaceLineName);
            WriteElementWhenContentAvailable(writer,
                                             PipingCalculationConfigurationSchemaIdentifiers.EntryPointLElement,
                                             configuration.EntryPointL);
            WriteElementWhenContentAvailable(writer,
                                             PipingCalculationConfigurationSchemaIdentifiers.ExitPointLElement,
                                             configuration.ExitPointL);
            WriteElementWhenContentAvailable(writer,
                                             PipingCalculationConfigurationSchemaIdentifiers.StochasticSoilModelElement,
                                             configuration.StochasticSoilModelName);
            WriteElementWhenContentAvailable(writer,
                                             PipingCalculationConfigurationSchemaIdentifiers.StochasticSoilProfileElement,
                                             configuration.StochasticSoilProfileName);

            WriteStochasts(writer, configuration);

            WriteScenarioWhenAvailable(writer, configuration.Scenario);
        }

        /// <summary>
        /// Writes the stochasts elements of the <paramref name="configuration"/> in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The calculation configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is closed.</exception>
        private static void WriteStochasts(XmlWriter writer, PipingCalculationConfiguration configuration)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.StochastsElement);

            WriteDistributionWhenAvailable(writer,
                                           PipingCalculationConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName,
                                           configuration.PhreaticLevelExit);

            WriteDistributionWhenAvailable(writer,
                                           PipingCalculationConfigurationSchemaIdentifiers.DampingFactorExitStochastName,
                                           configuration.DampingFactorExit);

            writer.WriteEndElement();
        }
    }
}