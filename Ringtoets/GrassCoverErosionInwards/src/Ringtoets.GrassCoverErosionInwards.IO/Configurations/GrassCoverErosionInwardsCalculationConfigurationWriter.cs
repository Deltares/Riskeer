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
using Ringtoets.GrassCoverErosionInwards.IO.Configurations.Helpers;

namespace Ringtoets.GrassCoverErosionInwards.IO.Configurations
{
    /// <summary>
    /// Writer for writing a grass cover erosion inwards calculation configuration to XML.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationConfigurationWriter : CalculationConfigurationWriter<GrassCoverErosionInwardsCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationConfigurationWriter"/>.
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
        public GrassCoverErosionInwardsCalculationConfigurationWriter(string filePath) : base(filePath) {}

        protected override void WriteCalculation(GrassCoverErosionInwardsCalculationConfiguration configuration, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.CalculationElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, configuration.Name);

            WriteElementWhenContentAvailable(
                writer,
                ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElementOld,
                configuration.HydraulicBoundaryLocationName);

            WriteElementWhenContentAvailable(
                writer,
                GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.DikeProfileElement,
                configuration.DikeProfileId);
            WriteElementWhenContentAvailable(
                writer,
                ConfigurationSchemaIdentifiers.Orientation,
                configuration.Orientation);
            WriteElementWhenContentAvailable(
                writer,
                GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.DikeHeightElement,
                configuration.DikeHeight);

            if (configuration.ShouldOvertoppingOutputIllustrationPointsBeCalculated.HasValue)
            {
                WriteElementWhenContentAvailable(
                    writer,
                    GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.ShouldOvertoppingOutputIllustrationPointsBeCalculatedElement,
                    XmlConvert.ToString(configuration.ShouldOvertoppingOutputIllustrationPointsBeCalculated.Value));
            }

            if (configuration.ShouldDikeHeightIllustrationPointsBeCalculated.HasValue)
            {
                WriteElementWhenContentAvailable(
                    writer,
                    GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.ShouldDikeHeightIllustrationPointsBeCalculatedElementElement,
                    XmlConvert.ToString(configuration.ShouldDikeHeightIllustrationPointsBeCalculated.Value));
            }

            if (configuration.ShouldOvertoppingRateIllustrationPointsBeCalculated.HasValue)
            {
                WriteElementWhenContentAvailable(
                    writer,
                    GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.ShouldOvertoppingRateIllustrationPointsBeCalculatedElement,
                    XmlConvert.ToString(configuration.ShouldOvertoppingRateIllustrationPointsBeCalculated.Value));
            }
            WriteConfigurationLoadSchematizationTypeWhenAvailable(
                writer,
                GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.DikeHeightCalculationTypeElement,
                configuration.DikeHeightCalculationType);

            WriteConfigurationLoadSchematizationTypeWhenAvailable(
                writer,
                GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.OvertoppingRateCalculationTypeElement,
                configuration.OvertoppingRateCalculationType);

            WriteWaveReductionWhenAvailable(writer, configuration.WaveReduction);

            writer.WriteStartElement(ConfigurationSchemaIdentifiers.StochastsElement);

            WriteDistributionWhenAvailable(
                writer,
                GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.CriticalFlowRateStochastName,
                configuration.CriticalFlowRate);

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the <paramref name="calculationType"/> in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="elementName">The XML element name.</param>
        /// <param name="calculationType">The calculation type to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion of
        /// <paramref name="calculationType"/> cannot be performed.</exception>
        private static void WriteConfigurationLoadSchematizationTypeWhenAvailable(
            XmlWriter writer,
            string elementName,
            ConfigurationHydraulicLoadsCalculationType? calculationType)
        {
            if (!calculationType.HasValue)
            {
                return;
            }

            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();
            writer.WriteElementString(elementName,
                                      converter.ConvertToInvariantString(calculationType.Value));
        }
    }
}