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
using System.Xml.Linq;
using Ringtoets.Common.IO.Configurations.Helpers;

namespace Riskeer.MacroStabilityInwards.IO.Configurations.Helpers
{
    /// <summary>
    /// Extensions methods for macro stability inwards specific <see cref="XElement"/>.
    /// </summary>
    public static class MacroStabilityInwardsXElementExtensions
    {
        /// <summary>
        /// Gets a location input configuration based on the values in the <paramref name="calculationElement"/>.
        /// </summary>
        /// <param name="calculationElement">The element containing values for location input parameters.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsLocationInputConfiguration"/> or <c>null</c> 
        /// when the descendant element was not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when the value for a parameter isn't in the correct format.</exception>
        /// <exception cref="OverflowException">Thrown when the value for a parameter represents a number less
        /// than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        public static MacroStabilityInwardsLocationInputConfiguration GetMacroStabilityInwardsLocationInputConfiguration(
            this XElement calculationElement)
        {
            if (calculationElement == null)
            {
                throw new ArgumentNullException(nameof(calculationElement));
            }

            XElement element = calculationElement.GetDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LocationInputDailyElement);
            if (element == null)
            {
                return null;
            }

            return new MacroStabilityInwardsLocationInputConfiguration
            {
                WaterLevelPolder = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.WaterLevelPolderElement),
                UseDefaultOffsets = element.GetBoolValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.UseDefaultOffsetsElement),
                PhreaticLineOffsetBelowDikeTopAtRiver = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeTopAtRiverElement),
                PhreaticLineOffsetBelowDikeTopAtPolder = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeTopAtPolderElement),
                PhreaticLineOffsetBelowShoulderBaseInside = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowShoulderBaseInsideElement),
                PhreaticLineOffsetBelowDikeToeAtPolder = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeToeAtPolderElement)
            };
        }

        /// <summary>
        /// Gets a location input extreme configuration based on the values in the <paramref name="calculationElement"/>.
        /// </summary>
        /// <param name="calculationElement">The element containing values for location input extreme parameters.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsLocationInputExtremeConfiguration"/> or <c>null</c> 
        /// when the descendant element was not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when the value for a parameter isn't in the correct format.</exception>
        /// <exception cref="OverflowException">Thrown when the value for a parameter represents a number less
        /// than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        public static MacroStabilityInwardsLocationInputExtremeConfiguration GetMacroStabilityInwardsLocationInputExtremeConfiguration(
            this XElement calculationElement)
        {
            if (calculationElement == null)
            {
                throw new ArgumentNullException(nameof(calculationElement));
            }

            XElement element = calculationElement.GetDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LocationInputExtremeElement);
            if (element == null)
            {
                return null;
            }

            return new MacroStabilityInwardsLocationInputExtremeConfiguration
            {
                PenetrationLength = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PenetrationLengthElement),
                WaterLevelPolder = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.WaterLevelPolderElement),
                UseDefaultOffsets = element.GetBoolValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.UseDefaultOffsetsElement),
                PhreaticLineOffsetBelowDikeTopAtRiver = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeTopAtRiverElement),
                PhreaticLineOffsetBelowDikeTopAtPolder = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeTopAtPolderElement),
                PhreaticLineOffsetBelowShoulderBaseInside = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowShoulderBaseInsideElement),
                PhreaticLineOffsetBelowDikeToeAtPolder = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeToeAtPolderElement)
            };
        }

        /// <summary>
        /// Gets a grid configuration based on the values found in the <paramref name="calculationElement"/>.
        /// </summary>
        /// <param name="calculationElement">The element containing values for grid parameters.</param>
        /// <param name="descendantElementName">The name of the descendant to find the parameter values for.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsGridConfiguration"/> or <c>null</c> 
        /// when <paramref name="descendantElementName"/> was not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when the value for a parameter isn't in the correct format.</exception>
        /// <exception cref="OverflowException">Thrown when the value for a parameter represents a number less
        /// than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        public static MacroStabilityInwardsGridConfiguration GetMacroStabilityInwardsGridConfiguration(
            this XElement calculationElement,
            string descendantElementName)
        {
            if (calculationElement == null)
            {
                throw new ArgumentNullException(nameof(calculationElement));
            }

            if (descendantElementName == null)
            {
                throw new ArgumentNullException(nameof(descendantElementName));
            }

            XElement element = calculationElement.GetDescendantElement(descendantElementName);
            if (element == null)
            {
                return null;
            }

            return new MacroStabilityInwardsGridConfiguration
            {
                XLeft = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridXLeftElement),
                XRight = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridXRightElement),
                ZTop = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridZTopElement),
                ZBottom = element.GetDoubleValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridZBottomElement),
                NumberOfHorizontalPoints = element.GetIntegerValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridNumberOfHorizontalPointsElement),
                NumberOfVerticalPoints = element.GetIntegerValueFromDescendantElement(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridNumberOfVerticalPointsElement)
            };
        }
    }
}