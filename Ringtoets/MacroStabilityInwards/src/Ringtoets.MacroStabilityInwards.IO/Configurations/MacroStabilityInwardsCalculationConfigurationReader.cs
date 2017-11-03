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
using Ringtoets.MacroStabilityInwards.IO.Configurations.Helpers;
using Ringtoets.MacroStabilityInwards.IO.Properties;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// This class reads a macro stability inwards calculation configuration from XML and creates a collection of corresponding
    /// <see cref="IConfigurationItem"/>, typically containing one or more <see cref="MacroStabilityInwardsCalculationConfiguration"/>.
    /// </summary>
    public class MacroStabilityInwardsCalculationConfigurationReader
        : CalculationConfigurationReader<MacroStabilityInwardsCalculationConfiguration>
    {
        private const string scenarioSchemaName = "ScenarioSchema.xsd";
        private const string waternetCreatorSchemaSchemaName = "MacroStabiliteitBinnenwaartsWaterspanningenSchema.xsd";
        private const string slopeStabilityZonesSchemaName = "MacroStabiliteitBinnenwaartsZonesSchema.xsd";
        private const string slopeStabilityGridsSchemaName = "MacroStabiliteitBinnenwaartsGridsSchema.xsd";

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
                   new Dictionary<string, string>
                   {
                       {
                           scenarioSchemaName, RingtoetsCommonIOResources.ScenarioSchema
                       },
                       {
                           waternetCreatorSchemaSchemaName, Resources.MacroStabiliteitBinnenwaartsWaterspanningenSchema
                       },
                       {
                           slopeStabilityZonesSchemaName, Resources.MacroStabiliteitBinnenwaartsZonesSchema
                       },
                       {
                           slopeStabilityGridsSchemaName, Resources.MacroStabiliteitBinnenwaartsGridsSchema
                       }
                   }) {}

        protected override MacroStabilityInwardsCalculationConfiguration ParseCalculationElement(XElement calculationElement)
        {
            var configuration = new MacroStabilityInwardsCalculationConfiguration(
                calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value)
            {
                AssessmentLevel = calculationElement.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.AssessmentLevelElement),
                HydraulicBoundaryLocationName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement),
                SurfaceLineName = calculationElement.GetStringValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SurfaceLineElement),
                StochasticSoilModelName = calculationElement.GetStringValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilModelElement),
                StochasticSoilProfileName = calculationElement.GetStringValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilProfileElement),
                DikeSoilScenario = (ConfigurationDikeSoilScenario?) calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationDikeSoilScenarioTypeConverter>(
                    MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioElement),

                WaterLevelRiverAverage = calculationElement.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.WaterLevelRiverAverageElement),
                DrainageConstructionPresent = calculationElement.GetBoolValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DrainageConstructionPresentElement),
                XCoordinateDrainageConstruction = calculationElement.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.XCoordinateDrainageConstructionElement),
                ZCoordinateDrainageConstruction = calculationElement.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZCoordinateDrainageConstructionElement),

                AdjustPhreaticLine3And4ForUplift = calculationElement.GetBoolValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.AdjustPhreaticLine3And4ForUpliftElement),

                PhreaticLine2 = calculationElement.GetMacroStabilityInwardsPhreaticLineConfiguration(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine2PiezometricHeadElement),
                PhreaticLine3 = calculationElement.GetMacroStabilityInwardsPhreaticLineConfiguration(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine3LeakageLengthElement),
                PhreaticLine4 = calculationElement.GetMacroStabilityInwardsPhreaticLineConfiguration(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine4LeakageLengthElement),

                LocationInputDaily = calculationElement.GetMacroStabilityInwardsLocationInputConfiguration(),
                LocationInputExtreme = calculationElement.GetMacroStabilityInwardsLocationInputExtremeConfiguration(),

                SlipPlaneMinimumDepth = calculationElement.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SlipPlaneMinimumDepthElement),
                SlipPlaneMinimumLength = calculationElement.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SlipPlaneMinimumLengthElement),
                MaximumSliceWidth = calculationElement.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MaximumSliceWidthElement),

                Scenario = calculationElement.GetScenarioConfiguration()
            };

            SetPhreaticLine1Properties(configuration, calculationElement);
            SetZonesProperties(configuration, calculationElement);
            SetGridProperties(configuration, calculationElement);

            return configuration;
        }

        /// <summary>
        /// Sets the phreatic line 1 related parameters to <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to set to the phreatic line 1 properties, if any.</param>
        /// <param name="calculationElement">The <see cref="XElement"/> that contains the phreatic line 1 element.</param>
        /// <exception cref="FormatException">Thrown when the value for a parameter isn't in the correct format.</exception>
        /// <exception cref="OverflowException">Thrown when the value for a parameter represents a number less
        /// than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when any conversion cannot be performed.</exception>
        private static void SetPhreaticLine1Properties(MacroStabilityInwardsCalculationConfiguration configuration, XElement calculationElement)
        {
            XElement phreaticLine1Element = calculationElement.GetDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine1MinimumLevelElement);
            if (phreaticLine1Element == null)
            {
                return;
            }

            configuration.MinimumLevelPhreaticLineAtDikeTopPolder = phreaticLine1Element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MinimumLevelPhreaticLineAtDikeTopPolderElement);
            configuration.MinimumLevelPhreaticLineAtDikeTopRiver = phreaticLine1Element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MinimumLevelPhreaticLineAtDikeTopRiverElement);
        }

        /// <summary>
        /// Sets the zone related parameters to <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to set to the zone properties, if any.</param>
        /// <param name="calculationElement">The <see cref="XElement"/> that contains the zone element.</param>
        /// <exception cref="FormatException">Thrown when the value for a parameter isn't in the correct format.</exception>
        private static void SetZonesProperties(MacroStabilityInwardsCalculationConfiguration configuration, XElement calculationElement)
        {
            XElement zonesElement = calculationElement.GetDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZonesElement);
            if (zonesElement == null)
            {
                return;
            }

            configuration.CreateZones = zonesElement.GetBoolValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.CreateZonesElement);
        }

        /// <summary>
        /// Sets the grid related parameters to <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to set to the grid properties, if any.</param>
        /// <param name="calculationElement">The <see cref="XElement"/> that contains the grid element.</param>
        /// <exception cref="FormatException">Thrown when the value for a parameter isn't in the correct format.</exception>
        /// <exception cref="OverflowException">Thrown when the value for a parameter represents a number less
        /// than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when any conversion cannot be performed.</exception>
        private static void SetGridProperties(MacroStabilityInwardsCalculationConfiguration configuration, XElement calculationElement)
        {
            XElement gridElement = calculationElement.GetDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridsElement);
            if (gridElement == null)
            {
                return;
            }

            configuration.MoveGrid = gridElement.GetBoolValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MoveGridElement);
            configuration.GridDeterminationType = (ConfigurationGridDeterminationType?) gridElement.GetConvertedValueFromDescendantStringElement<ConfigurationGridDeterminationTypeConverter>(
                MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeElement);

            SetTangentLineProperties(configuration, gridElement);

            configuration.LeftGrid = gridElement.GetMacroStabilityInwardsGridConfiguration(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LeftGridElement);
            configuration.RightGrid = gridElement.GetMacroStabilityInwardsGridConfiguration(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.RightGridElement);
        }

        /// <summary>
        /// Sets the tangent line related parameters to <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to set to the tangent line properties, if any.</param>
        /// <param name="gridElement">The <see cref="XElement"/> that contains the tangent line element.</param>
        /// <exception cref="FormatException">Thrown when the value for a parameter isn't in the correct format.</exception>
        /// <exception cref="OverflowException">Thrown when the value for a parameter represents a number less
        /// than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when any conversion cannot be performed.</exception>
        private static void SetTangentLineProperties(MacroStabilityInwardsCalculationConfiguration configuration, XElement gridElement)
        {
            XElement tangentLineElement = gridElement.GetDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineElement);
            if (tangentLineElement == null)
            {
                return;
            }

            configuration.TangentLineDeterminationType = (ConfigurationTangentLineDeterminationType?) tangentLineElement.GetConvertedValueFromDescendantStringElement<ConfigurationTangentLineDeterminationTypeConverter>(
                MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeElement);
            configuration.TangentLineZTop = tangentLineElement.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineZTopElement);
            configuration.TangentLineZBottom = tangentLineElement.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineZBottomElement);
            configuration.TangentLineNumber = tangentLineElement.GetIntegerValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineNumberElement);
        }
    }
}