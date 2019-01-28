// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
    public class MacroStabilityInwardsCalculationConfigurationWriter
        : CalculationConfigurationWriter<MacroStabilityInwardsCalculationConfiguration>
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

        protected override void WriteCalculation(MacroStabilityInwardsCalculationConfiguration configuration,
                                                 XmlWriter writer)
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
        private static void WriteCalculationElements(XmlWriter writer,
                                                     MacroStabilityInwardsCalculationConfiguration configuration)
        {
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.WaterLevelElement,
                                             configuration.AssessmentLevel);
            WriteElementWhenContentAvailable(writer,
                                             ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElementNew,
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

            WriteDikeSoilScenarioWhenContentAvailable(writer,
                                                      MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioElement,
                                                      configuration.DikeSoilScenario);

            WriteWaterStresses(writer, configuration);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SlipPlaneMinimumDepthElement,
                                             configuration.SlipPlaneMinimumDepth);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SlipPlaneMinimumLengthElement,
                                             configuration.SlipPlaneMinimumLength);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MaximumSliceWidthElement,
                                             configuration.MaximumSliceWidth);

            WriteZones(writer, configuration);

            WriteGrid(writer, configuration);

            WriteScenarioWhenAvailable(writer, configuration.Scenario);
        }

        /// <summary>
        /// Writes the <paramref name="dikeSoilScenario"/> in XML format to file when it has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="elementName">The XML element name.</param>
        /// <param name="dikeSoilScenario">The dike soil scenario to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion of
        /// <paramref name="dikeSoilScenario"/> cannot be performed.</exception>
        private static void WriteDikeSoilScenarioWhenContentAvailable(XmlWriter writer,
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

        #region Zones

        /// <summary>
        /// Writes the zone related properties in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteZones(XmlWriter writer,
                                       MacroStabilityInwardsCalculationConfiguration configuration)
        {
            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZonesElement);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.CreateZonesElement,
                                             configuration.CreateZones);
            WriteZoningBoundariesDeterminationTypeWhenContentAvailable(
                writer,
                MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZoningBoundariesDeterminationTypeElement,
                configuration.ZoningBoundariesDeterminationType);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZoneBoundaryLeft,
                                             configuration.ZoneBoundaryLeft);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZoneBoundaryRight,
                                             configuration.ZoneBoundaryRight);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the <paramref name="zoningBoundariesDeterminationType"/> in XML format to file when it has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="elementName">The XML element name.</param>
        /// <param name="zoningBoundariesDeterminationType">The zoning boundaries determination type to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion of
        /// <paramref name="zoningBoundariesDeterminationType"/> cannot be performed.</exception>
        private static void WriteZoningBoundariesDeterminationTypeWhenContentAvailable(XmlWriter writer,
                                                                                       string elementName,
                                                                                       ConfigurationZoningBoundariesDeterminationType? zoningBoundariesDeterminationType)
        {
            if (!zoningBoundariesDeterminationType.HasValue)
            {
                return;
            }

            var typeConverter = new ConfigurationZoningBoundariesDeterminationTypeConverter();
            writer.WriteElementString(elementName,
                                      typeConverter.ConvertToInvariantString(zoningBoundariesDeterminationType.Value));
        }

        #endregion

        #region Write water stresses

        /// <summary>
        /// Writes the water stress related properties in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteWaterStresses(XmlWriter writer,
                                               MacroStabilityInwardsCalculationConfiguration configuration)
        {
            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.WaterStressesElement);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.WaterLevelRiverAverageElement,
                                             configuration.WaterLevelRiverAverage);

            WriteDrainageConstruction(writer, configuration);

            WriteMinimumLevelPhreaticLine(writer, configuration);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.AdjustPhreaticLine3And4ForUpliftElement,
                                             configuration.AdjustPhreaticLine3And4ForUplift);

            WriteLeakageLengthPhreaticLine3Configuration(writer, configuration);
            WriteLeakageLengthPhreaticLine4Configuration(writer, configuration);
            WritePiezometricHeadPhreaticLine2Configuration(writer, configuration);

            WriteLocationDailyInputWhenAvailable(writer, configuration.LocationInputDaily);

            WriteLocationExtremeInputWhenAvailable(writer, configuration.LocationInputExtreme);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the piezometric head phreatic line 2 related properties in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WritePiezometricHeadPhreaticLine2Configuration(XmlWriter writer,
                                                                           MacroStabilityInwardsCalculationConfiguration configuration)
        {
            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine2PiezometricHeadElement);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineInwardsElement,
                                             configuration.PiezometricHeadPhreaticLine2Inwards);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOutwardsElement,
                                             configuration.PiezometricHeadPhreaticLine2Outwards);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the leakage length phreatic line 3 related properties in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteLeakageLengthPhreaticLine3Configuration(XmlWriter writer,
                                                                         MacroStabilityInwardsCalculationConfiguration configuration)
        {
            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine3LeakageLengthElement);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineInwardsElement,
                                             configuration.LeakageLengthInwardsPhreaticLine3);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOutwardsElement,
                                             configuration.LeakageLengthOutwardsPhreaticLine3);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the leakage length phreatic line 4 related properties in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteLeakageLengthPhreaticLine4Configuration(XmlWriter writer,
                                                                         MacroStabilityInwardsCalculationConfiguration configuration)
        {
            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine4LeakageLengthElement);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineInwardsElement,
                                             configuration.LeakageLengthInwardsPhreaticLine4);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOutwardsElement,
                                             configuration.LeakageLengthOutwardsPhreaticLine4);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a location input configuration for daily conditions in XML format to file when 
        /// <paramref name="configuration"/> has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration for the location input that can be <c>null</c>.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteLocationDailyInputWhenAvailable(XmlWriter writer,
                                                                 MacroStabilityInwardsLocationInputConfiguration configuration)
        {
            if (configuration == null)
            {
                return;
            }

            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LocationInputDailyElement);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.WaterLevelPolderElement,
                                             configuration.WaterLevelPolder);

            WriteLocationLocationInputOffset(writer, configuration);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a location input configuration for extreme conditions in XML format to file 
        /// when <paramref name="configuration"/> has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration for the location input that can be <c>null</c>.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteLocationExtremeInputWhenAvailable(XmlWriter writer,
                                                                   MacroStabilityInwardsLocationInputExtremeConfiguration configuration)
        {
            if (configuration == null)
            {
                return;
            }

            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LocationInputExtremeElement);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.WaterLevelPolderElement,
                                             configuration.WaterLevelPolder);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PenetrationLengthElement,
                                             configuration.PenetrationLength);

            WriteLocationLocationInputOffset(writer, configuration);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the offset of a location input configuration in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration for the location input that can be <c>null</c>.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteLocationLocationInputOffset(XmlWriter writer,
                                                             MacroStabilityInwardsLocationInputConfiguration configuration)
        {
            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LocationInputOffsetElement);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.UseDefaultOffsetsElement,
                                             configuration.UseDefaultOffsets);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeTopAtRiverElement,
                                             configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeTopAtPolderElement,
                                             configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowShoulderBaseInsideElement,
                                             configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeToeAtPolderElement,
                                             configuration.PhreaticLineOffsetBelowDikeToeAtPolder);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the drainage construction related properties in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteDrainageConstruction(XmlWriter writer,
                                                      MacroStabilityInwardsCalculationConfiguration configuration)
        {
            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DrainageConstructionElement);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DrainageConstructionPresentElement,
                                             configuration.DrainageConstructionPresent);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.XCoordinateDrainageConstructionElement,
                                             configuration.XCoordinateDrainageConstruction);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZCoordinateDrainageConstructionElement,
                                             configuration.ZCoordinateDrainageConstruction);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the phreatic line 1 minimum level related properties in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteMinimumLevelPhreaticLine(XmlWriter writer,
                                                          MacroStabilityInwardsCalculationConfiguration configuration)
        {
            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine1MinimumLevelElement);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MinimumLevelPhreaticLineAtDikeTopPolderElement,
                                             configuration.MinimumLevelPhreaticLineAtDikeTopPolder);
            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MinimumLevelPhreaticLineAtDikeTopRiverElement,
                                             configuration.MinimumLevelPhreaticLineAtDikeTopRiver);

            writer.WriteEndElement();
        }

        #endregion

        #region Write grids

        /// <summary>
        /// Writes the grid related properties in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteGrid(XmlWriter writer,
                                      MacroStabilityInwardsCalculationConfiguration configuration)
        {
            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridsElement);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MoveGridElement,
                                             configuration.MoveGrid);
            WriteGridDeterminationTypeWhenContentAvailable(writer,
                                                           MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeElement,
                                                           configuration.GridDeterminationType);

            WriteTangentLine(writer, configuration);

            WriteGridConfigurationWhenAvailable(writer,
                                                MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LeftGridElement,
                                                configuration.LeftGrid);
            WriteGridConfigurationWhenAvailable(writer,
                                                MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.RightGridElement,
                                                configuration.RightGrid);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the <paramref name="gridDeterminationType"/> in XML format to file when it has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="elementName">The XML element name.</param>
        /// <param name="gridDeterminationType">The grid determination type to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion of
        /// <paramref name="gridDeterminationType"/> cannot be performed.</exception>
        private static void WriteGridDeterminationTypeWhenContentAvailable(XmlWriter writer,
                                                                           string elementName,
                                                                           ConfigurationGridDeterminationType? gridDeterminationType)
        {
            if (!gridDeterminationType.HasValue)
            {
                return;
            }

            var typeConverter = new ConfigurationGridDeterminationTypeConverter();
            writer.WriteElementString(elementName,
                                      typeConverter.ConvertToInvariantString(gridDeterminationType.Value));
        }

        #region Write tangent line

        /// <summary>
        /// Writes the tangent line related properties in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteTangentLine(XmlWriter writer,
                                             MacroStabilityInwardsCalculationConfiguration configuration)
        {
            writer.WriteStartElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineElement);

            WriteTangentLineDeterminationTypeWhenContentAvailable(writer,
                                                                  MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeElement,
                                                                  configuration.TangentLineDeterminationType);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineZTopElement,
                                             configuration.TangentLineZTop);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineZBottomElement,
                                             configuration.TangentLineZBottom);

            WriteElementWhenContentAvailable(writer,
                                             MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineNumberElement,
                                             configuration.TangentLineNumber);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the <paramref name="tangentLineDeterminationType"/> in XML format to file when it has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="elementName">The XML element name.</param>
        /// <param name="tangentLineDeterminationType">The tangent line determination type to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion of
        /// <paramref name="tangentLineDeterminationType"/> cannot be performed.</exception>
        private static void WriteTangentLineDeterminationTypeWhenContentAvailable(XmlWriter writer,
                                                                                  string elementName,
                                                                                  ConfigurationTangentLineDeterminationType? tangentLineDeterminationType)
        {
            if (!tangentLineDeterminationType.HasValue)
            {
                return;
            }

            var typeConverter = new ConfigurationTangentLineDeterminationTypeConverter();
            writer.WriteElementString(elementName,
                                      typeConverter.ConvertToInvariantString(tangentLineDeterminationType.Value));
        }

        #endregion

        /// <summary>
        /// Writes a grid configuration in XML format to file when <paramref name="configuration"/> has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="gridLocationName">The name of the location of the grid.</param>
        /// <param name="configuration">The configuration for the grid that can be <c>null</c>.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        private static void WriteGridConfigurationWhenAvailable(XmlWriter writer,
                                                                string gridLocationName,
                                                                MacroStabilityInwardsGridConfiguration configuration)
        {
            if (configuration == null)
            {
                return;
            }

            writer.WriteStartElement(gridLocationName);

            if (configuration.XLeft.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridXLeftElement,
                                          XmlConvert.ToString(configuration.XLeft.Value));
            }

            if (configuration.XRight.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridXRightElement,
                                          XmlConvert.ToString(configuration.XRight.Value));
            }

            if (configuration.ZTop.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridZTopElement,
                                          XmlConvert.ToString(configuration.ZTop.Value));
            }

            if (configuration.ZBottom.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridZBottomElement,
                                          XmlConvert.ToString(configuration.ZBottom.Value));
            }

            if (configuration.NumberOfVerticalPoints.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridNumberOfVerticalPointsElement,
                                          XmlConvert.ToString(configuration.NumberOfVerticalPoints.Value));
            }

            if (configuration.NumberOfHorizontalPoints.HasValue)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridNumberOfHorizontalPointsElement,
                                          XmlConvert.ToString(configuration.NumberOfHorizontalPoints.Value));
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}