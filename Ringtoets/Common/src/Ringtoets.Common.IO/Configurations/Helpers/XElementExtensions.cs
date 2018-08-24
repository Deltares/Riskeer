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
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Ringtoets.Common.IO.Configurations.Helpers
{
    /// <summary>
    /// Extensions methods for <see cref="XElement"/>.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Gets the <see cref="double"/> value from a descendant element.
        /// </summary>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the descendant element.</param>
        /// <param name="descendantElementName">The name of the descendant element.</param>
        /// <returns>The value of the element, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have descendant elements of <paramref name="descendantElementName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when the value isn't in the correct format.</exception>
        /// <exception cref="OverflowException">Thrown when the value represents a number
        /// less than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        public static double? GetDoubleValueFromDescendantElement(this XElement parentElement, string descendantElementName)
        {
            XElement descendantElement = parentElement.GetDescendantElement(descendantElementName);

            return descendantElement != null
                       ? (double?) XmlConvert.ToDouble(descendantElement.Value)
                       : null;
        }

        /// <summary>
        /// Gets the <see cref="int"/> value from a descendant element.
        /// </summary>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the descendant element.</param>
        /// <param name="descendantElementName">The name of the descendant element.</param>
        /// <returns>The value of the element, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have descendant elements of <paramref name="descendantElementName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when the value isn't in the correct format.</exception>
        /// <exception cref="OverflowException">Thrown when the value represents a number
        /// less than <see cref="int.MinValue"/> or greater than <see cref="int.MaxValue"/>.</exception>
        public static int? GetIntegerValueFromDescendantElement(this XElement parentElement, string descendantElementName)
        {
            XElement descendantElement = parentElement.GetDescendantElement(descendantElementName);

            return descendantElement != null
                       ? (int?) XmlConvert.ToInt32(descendantElement.Value)
                       : null;
        }

        /// <summary>
        /// Gets the <see cref="string"/> value from a descendant element.
        /// </summary>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the descendant element.</param>
        /// <param name="descendantElementName">The name of the descendant element.</param>
        /// <returns>The value of the element, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have descendant elements of <paramref name="descendantElementName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static string GetStringValueFromDescendantElement(this XElement parentElement, string descendantElementName)
        {
            XElement descendantElement = parentElement.GetDescendantElement(descendantElementName);

            return descendantElement?.Value;
        }

        /// <summary>
        ///  Gets the <see cref="bool"/> value from a descendant element.
        /// </summary>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the descendant element.</param>
        /// <param name="descendantElementName">The name of the descendant element.</param>
        /// <returns>The <see cref="bool"/> value, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have descendant elements of <paramref name="descendantElementName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when the value does not represent a <see cref="bool"/> value.</exception>
        public static bool? GetBoolValueFromDescendantElement(this XElement parentElement, string descendantElementName)
        {
            XElement descendantElement = parentElement.GetDescendantElement(descendantElementName);

            return descendantElement != null
                       ? (bool?) XmlConvert.ToBoolean(descendantElement.Value)
                       : null;
        }

        /// <summary>
        /// Gets the converted value from a descendant element containing a string.
        /// </summary>
        /// <typeparam name="TConverter">The <see cref="TypeConverter"/> to use</typeparam>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the descendant element.</param>
        /// <param name="descendantElementName">The name of the descendant element.</param>
        /// <returns>The converted value, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have descendant elements of <paramref name="descendantElementName"/>.</returns>
        /// <exception cref="Exception">Thrown when calling <typeparamref name="TConverter"/>.
        /// <see cref="TypeConverter.ConvertFrom(object)"/> results in an exception being thrown.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion cannot be performed.</exception>
        public static object GetConvertedValueFromDescendantStringElement<TConverter>(this XElement parentElement, string descendantElementName)
            where TConverter : TypeConverter, new()
        {
            string stringValue = parentElement.GetStringValueFromDescendantElement(descendantElementName);
            if (stringValue == null)
            {
                return null;
            }

            return new TConverter().ConvertFromInvariantString(stringValue);
        }

        /// <summary>
        /// Gets the converted value from a descendant element containing a double.
        /// </summary>
        /// <typeparam name="TConverter">The <see cref="TypeConverter"/> to use</typeparam>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the descendant element.</param>
        /// <param name="descendantElementName">The name of the descendant element.</param>
        /// <returns>The converted value, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have descendant elements of <paramref name="descendantElementName"/>.</returns>
        /// <exception cref="Exception">Thrown when calling <typeparamref name="TConverter"/>.
        /// <see cref="TypeConverter.ConvertFrom(object)"/> results in an exception being thrown.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when the value from a descendant element is
        /// not in the correct format.</exception>
        /// <exception cref="OverflowException">Thrown when the value from a descendant element 
        /// represents a number less than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion cannot be performed.</exception>
        public static object GetConvertedValueFromDescendantDoubleElement<TConverter>(this XElement parentElement, string descendantElementName)
            where TConverter : TypeConverter, new()
        {
            double? doubleValue = parentElement.GetDoubleValueFromDescendantElement(descendantElementName);
            if (doubleValue == null)
            {
                return null;
            }

            return new TConverter().ConvertFrom(doubleValue);
        }

        /// <summary>
        /// Gets the 'stochast' element from the descendant 'stochasts' element.
        /// </summary>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the descendant element.</param>
        /// <param name="stochastName">The name of the stochast element.</param>
        /// <returns>The stochast element, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have stochast elements with the name <paramref name="stochastName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static XElement GetStochastElement(this XElement parentElement, string stochastName)
        {
            if (parentElement == null)
            {
                throw new ArgumentNullException(nameof(parentElement));
            }

            if (stochastName == null)
            {
                throw new ArgumentNullException(nameof(stochastName));
            }

            return parentElement.Elements(ConfigurationSchemaIdentifiers.StochastsElement)
                                .FirstOrDefault()?
                                .Elements(ConfigurationSchemaIdentifiers.StochastElement)
                                .FirstOrDefault(e => e.Attribute(ConfigurationSchemaIdentifiers.NameAttribute)?.Value == stochastName);
        }

        /// <summary>
        /// Gets a descendant element with the given <paramref name="descendantElementName"/>.
        /// </summary>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the descendant element.</param>
        /// <param name="descendantElementName">The name of the descendant element.</param>
        /// <returns>The element, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have descendant elements of <paramref name="descendantElementName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static XElement GetDescendantElement(this XElement parentElement, string descendantElementName)
        {
            if (parentElement == null)
            {
                throw new ArgumentNullException(nameof(parentElement));
            }

            if (descendantElementName == null)
            {
                throw new ArgumentNullException(nameof(descendantElementName));
            }

            return parentElement.Descendants(descendantElementName).FirstOrDefault();
        }

        /// <summary>
        /// Gets a stochast configuration based on the values found in the <paramref name="calculationElement"/>.
        /// </summary>
        /// <param name="calculationElement">The element containing values for stochast parameters.</param>
        /// <param name="stochastName">The name of the stochast to find the parameter values for.</param>
        /// <returns>The configuration, or <c>null</c> when the <paramref name="calculationElement"/>
        /// does not have stochast elements with the name <paramref name="stochastName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when the value for a parameter isn't in the correct format.</exception>
        /// <exception cref="OverflowException">Thrown when the value for a parameter represents a number less
        /// than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        public static StochastConfiguration GetStochastConfiguration(this XElement calculationElement, string stochastName)
        {
            if (calculationElement == null)
            {
                throw new ArgumentNullException(nameof(calculationElement));
            }

            if (stochastName == null)
            {
                throw new ArgumentNullException(nameof(stochastName));
            }

            XElement element = calculationElement.GetStochastElement(stochastName);

            if (element != null)
            {
                return new StochastConfiguration
                {
                    Mean = element.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.MeanElement),
                    StandardDeviation = element.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.StandardDeviationElement),
                    VariationCoefficient = element.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.VariationCoefficientElement)
                };
            }

            return null;
        }

        /// <summary>
        /// Gets a wave reduction configuration based on the values found in the <paramref name="calculationElement"/>.
        /// </summary>
        /// <param name="calculationElement">The element containing values for wave reduction parameters.</param>
        /// <returns>The configuration, or <c>null</c> when the <paramref name="calculationElement"/> does not
        /// have wave reduction elements.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationElement"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the value for break water type isn't valid.</exception>
        /// <exception cref="FormatException">Thrown when the value for break water height, use foreshore profile or
        /// use breakwater isn't in the correct format to convert to a value.</exception>
        /// <exception cref="OverflowException">Thrown when the value for a break water height represents a number less
        /// than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        public static WaveReductionConfiguration GetWaveReductionParameters(this XElement calculationElement)
        {
            XElement waveReduction = calculationElement.GetDescendantElement(ConfigurationSchemaIdentifiers.WaveReduction);
            if (waveReduction != null)
            {
                return new WaveReductionConfiguration
                {
                    BreakWaterType = (ConfigurationBreakWaterType?) calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationBreakWaterTypeConverter>(ConfigurationSchemaIdentifiers.BreakWaterType),
                    BreakWaterHeight = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.BreakWaterHeight),
                    UseBreakWater = calculationElement.GetBoolValueFromDescendantElement(ConfigurationSchemaIdentifiers.UseBreakWater),
                    UseForeshoreProfile = calculationElement.GetBoolValueFromDescendantElement(ConfigurationSchemaIdentifiers.UseForeshore)
                };
            }

            return null;
        }

        /// <summary>
        /// Gets a scenario configuration based on the values found in the <paramref name="calculationElement"/>.
        /// </summary>
        /// <param name="calculationElement">The element containing values for scenario parameters.</param>
        /// <returns>The configuration, or <c>null</c> when the <paramref name="calculationElement"/> does not
        /// have scenario elements.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationElement"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when the value for contribution or is relevant 
        /// for scenario is not in the correct format to convert to a value.</exception>
        /// <exception cref="OverflowException">Thrown when the value for a contribution represents 
        /// a number less than <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.</exception>
        public static ScenarioConfiguration GetScenarioConfiguration(this XElement calculationElement)
        {
            if (calculationElement == null)
            {
                throw new ArgumentNullException(nameof(calculationElement));
            }

            XElement scenario = calculationElement.GetDescendantElement(ConfigurationSchemaIdentifiers.ScenarioElement);
            if (scenario != null)
            {
                return new ScenarioConfiguration
                {
                    IsRelevant = calculationElement.GetBoolValueFromDescendantElement(ConfigurationSchemaIdentifiers.IsRelevantForScenario),
                    Contribution = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.ScenarioContribution)
                };
            }

            return null;
        }

        /// <summary>
        /// Gets a hydraulic boundary location name based on the values found in the <paramref name="calculationElement"/>.
        /// </summary>
        /// <param name="calculationElement">The element containing values for hydraulic boundary location parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationElement"/> is <c>null</c>.</exception>
        /// <returns>The hydraulic boundary location name, or <c>null</c> when the <paramref name="calculationElement"/> does not
        /// have hydraulic boundary location elements.</returns>
        public static string GetHydraulicBoundaryLocationName(this XElement calculationElement)
        {
            if (calculationElement == null)
            {
                throw new ArgumentNullException(nameof(calculationElement));
            }

            return calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElementNew)
                   ?? calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElementOld);
        }
    }
}