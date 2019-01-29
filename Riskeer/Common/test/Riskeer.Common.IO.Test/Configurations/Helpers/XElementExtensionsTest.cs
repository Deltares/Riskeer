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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Helpers;

namespace Riskeer.Common.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class XElementExtensionsTest
    {
        private static IEnumerable<TestCaseData> XElements
        {
            get
            {
                yield return new TestCaseData(new XElement("Root", new XElement("descendant")));
                yield return new TestCaseData(new XElement("Root", new XElement("Child", new XElement("descendant"))));
            }
        }

        [Test]
        public void GetDoubleValueFromDescendantElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => XElementExtensions.GetDoubleValueFromDescendantElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetDoubleValueFromDescendantElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Setup
            var rootElement = new XElement("Root");

            // Call
            TestDelegate test = () => rootElement.GetDoubleValueFromDescendantElement(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
        }

        [Test]
        public void GetDoubleValueFromDescendantElement_DescendantElementInvalidFormat_ThrowFormatException()
        {
            // Setup
            const string descendantElementName = "number";
            const string descendantElementValue = "drie";

            var element = new XElement("Root", new XElement(descendantElementName, descendantElementValue));

            // Call
            TestDelegate test = () => element.GetDoubleValueFromDescendantElement(descendantElementName);

            // Assert
            Assert.Throws<FormatException>(test);
        }

        [Test]
        public void GetDoubleValueFromDescendantElement_DescendantElementOverflows_ThrowOverflowException()
        {
            // Setup
            const string descendantElementName = "number";
            string descendantElementValue = string.Format(CultureInfo.InvariantCulture, "1{0}", double.MaxValue);

            var element = new XElement("Root", new XElement(descendantElementName, descendantElementValue));

            // Call
            TestDelegate test = () => element.GetDoubleValueFromDescendantElement(descendantElementName);

            // Assert
            Assert.Throws<OverflowException>(test);
        }

        [Test]
        [TestCase(3)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void GetDoubleValueFromDescendantElement_ValidDescendantElement_ReturnValue(double descendantElementValue)
        {
            // Setup
            const string descendantElementName = "number";
            var element = new XElement("Root", new XElement(descendantElementName, descendantElementValue));

            // Call
            double? readValue = element.GetDoubleValueFromDescendantElement(descendantElementName);

            // Assert
            Assert.AreEqual(descendantElementValue, readValue.Value);
        }

        [Test]
        public void GetDoubleValueFromDescendantElement_InvalidDescendantElement_ReturnNull()
        {
            // Setup
            var element = new XElement("Root", new XElement("number", (double) 3));

            // Call
            double? readValue = element.GetDoubleValueFromDescendantElement("invalidName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetIntegerValueFromDescendantElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => XElementExtensions.GetIntegerValueFromDescendantElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetIntegerValueFromDescendantElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Setup
            var rootElement = new XElement("Root");

            // Call
            TestDelegate test = () => rootElement.GetIntegerValueFromDescendantElement(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
        }

        [Test]
        public void GetIntegerValueFromDescendantElement_DescendantElementInvalidFormat_ThrowFormatException()
        {
            // Setup
            const string descendantElementName = "number";
            const string descendantElementValue = "drie";

            var element = new XElement("Root", new XElement(descendantElementName, descendantElementValue));

            // Call
            TestDelegate test = () => element.GetIntegerValueFromDescendantElement(descendantElementName);

            // Assert
            Assert.Throws<FormatException>(test);
        }

        [Test]
        public void GetIntegerValueFromDescendantElement_DescendantElementOverflows_ThrowOverflowException()
        {
            // Setup
            const string descendantElementName = "number";
            string descendantElementValue = string.Format(CultureInfo.InvariantCulture, "1{0}", int.MaxValue);

            var element = new XElement("Root", new XElement(descendantElementName, descendantElementValue));

            // Call
            TestDelegate test = () => element.GetIntegerValueFromDescendantElement(descendantElementName);

            // Assert
            Assert.Throws<OverflowException>(test);
        }

        [Test]
        public void GetIntegerValueFromDescendantElement_ValidDescendantElement_ReturnValue()
        {
            // Setup
            const string descendantElementName = "number";
            const int descendantElementValue = 3;

            var element = new XElement("Root", new XElement(descendantElementName, descendantElementValue));

            // Call
            double? readValue = element.GetIntegerValueFromDescendantElement(descendantElementName);

            // Assert
            Assert.AreEqual(descendantElementValue, readValue.Value);
        }

        [Test]
        public void GetIntegerValueFromDescendantElement_InvalidDescendantElement_ReturnNull()
        {
            // Setup
            var element = new XElement("Root", new XElement("number", (double) 3));

            // Call
            double? readValue = element.GetIntegerValueFromDescendantElement("invalidName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetStringValueFromDescendantElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => XElementExtensions.GetStringValueFromDescendantElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetStringValueFromDescendantElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new XElement("Test").GetStringValueFromDescendantElement(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
        }

        [Test]
        public void GetStringValueFromDescendantElement_ValidDescendantElement_ReturnValue()
        {
            // Setup
            const string descendantElementName = "text";
            const string descendantElementValue = "valueText";

            var element = new XElement("Root", new XElement(descendantElementName, descendantElementValue));

            // Call
            string readValue = element.GetStringValueFromDescendantElement(descendantElementName);

            // Assert
            Assert.AreEqual(descendantElementValue, readValue);
        }

        [Test]
        public void GetStringValueFromDescendantElement_InvalidDescendantElement_ReturnNull()
        {
            // Setup
            var element = new XElement("Root", new XElement("number", "valueText"));

            // Call
            string readValue = element.GetStringValueFromDescendantElement("invalidName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetBoolValueFromDescendantElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => XElementExtensions.GetBoolValueFromDescendantElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetBoolValueFromDescendantElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Setup
            var element = new XElement("Root");

            // Call
            TestDelegate test = () => element.GetBoolValueFromDescendantElement(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
        }

        [Test]
        public void GetBoolValueFromDescendantElement_DescendantElementIncorrectFormat_ThrowFormatException()
        {
            // Setup
            const string descendantElementName = "booleanValue";
            const string elementValue = "nope";

            var element = new XElement("Root", new XElement(descendantElementName, elementValue));

            // Call
            TestDelegate test = () => element.GetBoolValueFromDescendantElement(descendantElementName);

            // Assert
            Assert.Throws<FormatException>(test);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetBoolValueFromDescendantElement_ValidDescendantElement_ReturnValue(bool booleanValue)
        {
            // Setup
            const string descendantElementName = "booleanValue";
            string elementValue = XmlConvert.ToString(booleanValue);

            var element = new XElement("Root", new XElement(descendantElementName, elementValue));

            // Call
            bool? readValue = element.GetBoolValueFromDescendantElement(descendantElementName);

            // Assert
            Assert.AreEqual(booleanValue, readValue);
        }

        [Test]
        public void GetBoolValueFromDescendantElement_UnmatchedDescendantElement_ReturnNull()
        {
            // Setup
            string elementValue = XmlConvert.ToString(true);

            var element = new XElement("Root", new XElement("booleanValue", elementValue));

            // Call
            bool? readValue = element.GetBoolValueFromDescendantElement("unmatchingChildElementName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetConvertedValueFromDescendantStringElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => XElementExtensions.GetConvertedValueFromDescendantStringElement<TypeConverter>(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetConvertedValueFromDescendantStringElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Setup
            var element = new XElement("Root");

            // Call
            TestDelegate test = () => element.GetConvertedValueFromDescendantStringElement<TypeConverter>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
        }

        [Test]
        public void GetConvertedValueFromDescendantStringElement_DescendantElementInvalidToConvert_ThrowException()
        {
            // Setup
            const string descendantElementName = "value";
            const string elementValue = "three";

            var element = new XElement("Root", new XElement(descendantElementName, elementValue));

            // Call
            TestDelegate call = () => element.GetConvertedValueFromDescendantStringElement<DoubleConverter>(descendantElementName);

            // Assert
            var exception = Assert.Throws<Exception>(call);
            Assert.AreEqual($"{elementValue} is not a valid value for Double.", exception.Message);
        }

        [Test]
        public void GetConvertedValueFromDescendantStringElement_DescendantElementNotSupported_ThrowNotSupportedException()
        {
            // Setup
            const string descendantElementName = "value";
            const string elementValue = "3";

            var element = new XElement("Root", new XElement(descendantElementName, elementValue));

            // Call
            TestDelegate call = () => element.GetConvertedValueFromDescendantStringElement<TypeConverter>(descendantElementName);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetConvertedValueFromDescendantStringElement_ValidDescendantElement_ReturnValue(bool value)
        {
            // Setup
            const string descendantElementName = "value";
            string elementValue = XmlConvert.ToString(value);

            var element = new XElement("Root", new XElement(descendantElementName, elementValue));

            // Call
            object readValue = element.GetConvertedValueFromDescendantStringElement<BooleanConverter>(descendantElementName);

            // Assert
            Assert.AreEqual(value, readValue);
        }

        [Test]
        public void GetConvertedValueFromDescendantStringElement_UnmatchedDescendantElement_ReturnNull()
        {
            // Setup
            string elementValue = XmlConvert.ToString(true);

            var element = new XElement("Root", new XElement("value", elementValue));

            // Call
            object readValue = element.GetConvertedValueFromDescendantStringElement<TypeConverter>("unmatchingChildElementName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetConvertedValueFromDescendantDoubleElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => XElementExtensions.GetConvertedValueFromDescendantDoubleElement<TypeConverter>(null, "0");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetConvertedValueFromDescendantDoubleElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Setup
            var element = new XElement("Root");

            // Call
            TestDelegate test = () => element.GetConvertedValueFromDescendantDoubleElement<TypeConverter>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
        }

        [Test]
        public void GetConvertedValueFromDescendantDoubleElement_ConvertFromThrowsException_ThrowException()
        {
            // Setup
            const string descendantElementName = "value";
            const double elementValue = 1;

            var element = new XElement("Root", new XElement(descendantElementName, elementValue));

            // Call
            TestDelegate call = () => element.GetConvertedValueFromDescendantDoubleElement<ConverterThrowsExceptionOnConvertFrom>(
                descendantElementName);

            // Assert
            var exception = Assert.Throws<Exception>(call);
            Assert.AreEqual($"{elementValue} is not a valid value for the target type.", exception.Message);
        }

        [Test]
        public void GetConvertedValueFromDescendantDoubleElement_DescendantElementInvalidFormat_ThrowFormatException()
        {
            // Setup
            const string descendantElementName = "number";
            const string descendantElementValue = "drie";

            var element = new XElement("Root", new XElement(descendantElementName, descendantElementValue));

            // Call
            TestDelegate test = () => element.GetConvertedValueFromDescendantDoubleElement<TypeConverter>(descendantElementName);

            // Assert
            Assert.Throws<FormatException>(test);
        }

        [Test]
        public void GetConvertedValueFromDescendantDoubleElement_DescendantElementOverflows_ThrowOverflowException()
        {
            // Setup
            const string descendantElementName = "number";
            string descendantElementValue = string.Format(CultureInfo.InvariantCulture, "1{0}", double.MaxValue);

            var element = new XElement("Root", new XElement(descendantElementName, descendantElementValue));

            // Call
            TestDelegate test = () => element.GetConvertedValueFromDescendantDoubleElement<TypeConverter>(descendantElementName);

            // Assert
            Assert.Throws<OverflowException>(test);
        }

        [Test]
        public void GetConvertedValueFromDescendantDoubleElement_DescendantElementNotSupportedByConverter_ThrowNotSupportedException()
        {
            // Setup
            const string descendantElementName = "number";
            const string descendantElementValue = "4";

            var element = new XElement("Root", new XElement(descendantElementName, descendantElementValue));

            // Call
            TestDelegate test = () => element.GetConvertedValueFromDescendantDoubleElement<TypeConverter>(descendantElementName);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
        [TestCase(0, false)]
        [TestCase(-1, true)]
        [TestCase(1, true)]
        public void GetConvertedValueFromDescendantDoubleElement_ValidDescendantElement_ReturnValue(double value, bool expectedConvertedValue)
        {
            // Setup
            const string descendantElementName = "value";
            string elementValue = XmlConvert.ToString(value);

            var element = new XElement("Root", new XElement(descendantElementName, elementValue));

            // Call
            object readValue = element.GetConvertedValueFromDescendantDoubleElement<DoubleToBooleanConverter>(descendantElementName);

            // Assert
            Assert.AreEqual(expectedConvertedValue, readValue);
        }

        [Test]
        public void GetConvertedValueFromDescendantDoubleElement_UnmatchedDescendantElement_ReturnNull()
        {
            // Setup
            string elementValue = XmlConvert.ToString(true);

            var element = new XElement("Root", new XElement("value", elementValue));

            // Call
            object readValue = element.GetConvertedValueFromDescendantDoubleElement<TypeConverter>("unmatchingChildElementName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetStochastElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => XElementExtensions.GetStochastElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetStochastElement_StochastNameNull_ThrowArgumentNullException()
        {
            // Setup
            var element = new XElement("Root");

            // Call
            TestDelegate test = () => element.GetStochastElement(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochastName", exception.ParamName);
        }

        [Test]
        public void GetStochastElement_RootElementWithoutStochastsElement_ReturnNull()
        {
            // Setup
            var element = new XElement("Root");

            // Call
            XElement stochastElement = element.GetStochastElement("stochast_name");

            // Assert
            Assert.IsNull(stochastElement);
        }

        [Test]
        public void GetStochastElement_RootElementWithEmptyStochastsElement_ReturnNull()
        {
            // Setup
            var stochastsElements = new XElement(ConfigurationSchemaIdentifiers.StochastsElement);
            var element = new XElement("Root", stochastsElements);

            // Call
            XElement stochastElement = element.GetStochastElement("stochast_name");

            // Assert
            Assert.IsNull(stochastElement);
        }

        [Test]
        public void GetStochastElement_RootElementWithStochastsAndUnmatchedStochastElement_ReturnNull()
        {
            // Setup
            var stochastA = new XElement(ConfigurationSchemaIdentifiers.StochastElement);
            stochastA.SetAttributeValue(ConfigurationSchemaIdentifiers.NameAttribute, "A");

            var stochastB = new XElement(ConfigurationSchemaIdentifiers.StochastElement);
            stochastB.SetAttributeValue(ConfigurationSchemaIdentifiers.NameAttribute, "B");

            var stochastsElements = new XElement(ConfigurationSchemaIdentifiers.StochastsElement);
            stochastsElements.Add(stochastA);
            stochastsElements.Add(stochastB);

            var element = new XElement("Root", stochastsElements);

            // Call
            XElement stochastElement = element.GetStochastElement("stochast_name");

            // Assert
            Assert.IsNull(stochastElement);
        }

        [Test]
        public void GetStochastElement_RootElementWithStochastsAndMatchedStochastElement_ReturnMatchedElement()
        {
            // Setup
            const string stochastName = "stochast_name";
            var stochastA = new XElement(ConfigurationSchemaIdentifiers.StochastElement);
            stochastA.SetAttributeValue(ConfigurationSchemaIdentifiers.NameAttribute, "A");

            var stochastElementToMatch = new XElement(ConfigurationSchemaIdentifiers.StochastElement);
            stochastElementToMatch.SetAttributeValue(ConfigurationSchemaIdentifiers.NameAttribute, stochastName);

            var stochastB = new XElement(ConfigurationSchemaIdentifiers.StochastElement);
            stochastB.SetAttributeValue(ConfigurationSchemaIdentifiers.NameAttribute, "B");

            var stochastsElements = new XElement(ConfigurationSchemaIdentifiers.StochastsElement);
            stochastsElements.Add(stochastA);
            stochastsElements.Add(stochastElementToMatch);
            stochastsElements.Add(stochastB);

            var element = new XElement("Root", stochastsElements);

            // Call
            XElement stochastElement = element.GetStochastElement(stochastName);

            // Assert
            Assert.AreSame(stochastElementToMatch, stochastElement);
        }

        [Test]
        public void GetDescendantElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => XElementExtensions.GetDescendantElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetDescendantElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new XElement("Test").GetDescendantElement(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(XElements))]
        public void GetDescendantElement_ValidDescendantName_ReturnElement(XElement parentElement)
        {
            // Call
            XElement element = parentElement.GetDescendantElement("descendant");

            // Assert
            Assert.IsNotNull(element);
            Assert.AreEqual("descendant", element.Name.LocalName);
        }

        [Test]
        public void GetDescendantElement_InvalidDescendantName_ReturnNull()
        {
            // Setup
            var parentElement = new XElement("Root", new XElement("Child", new XElement("descendant")));

            // Call
            XElement element = parentElement.GetDescendantElement("something_else");

            // Assert
            Assert.IsNull(element);
        }

        [Test]
        public void GetStochastConfiguration_CalculationElementNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((XElement) null).GetStochastConfiguration("name");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationElement", exception.ParamName);
        }

        [Test]
        public void GetStochastConfiguration_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new XElement("root").GetStochastConfiguration(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochastName", exception.ParamName);
        }

        [Test]
        public void GetStochastConfiguration_RootWithoutStochastsElement_ReturnsNull()
        {
            // Setup
            var xElement = new XElement("root");

            // Call
            StochastConfiguration stochast = xElement.GetStochastConfiguration("some name");

            // Assert
            Assert.IsNull(stochast);
        }

        [Test]
        public void GetStochastConfiguration_EmptyStochastsElement_ReturnsNull()
        {
            // Setup
            var xElement = new XElement("root", new XElement("stochasten"));

            // Call
            StochastConfiguration stochast = xElement.GetStochastConfiguration("some name");

            // Assert
            Assert.IsNull(stochast);
        }

        [Test]
        public void GetStochastConfiguration_StochastWithDifferentName_ReturnsNull()
        {
            // Setup
            var stochastElement = new XElement("stochast");
            stochastElement.SetAttributeValue("naam", "stochastA");
            var xElement = new XElement("root", new XElement("stochasten", stochastElement));

            // Call
            StochastConfiguration stochast = xElement.GetStochastConfiguration("stochastB");

            // Assert
            Assert.IsNull(stochast);
        }

        [Test]
        public void GetStochastConfiguration_StochastWithSameName_ReturnsNewStochast()
        {
            // Setup
            const string stochastName = "stochastA";

            var stochastElement = new XElement("stochast");
            stochastElement.SetAttributeValue("naam", stochastName);
            var xElement = new XElement("root", new XElement("stochasten", stochastElement));

            // Call
            StochastConfiguration stochast = xElement.GetStochastConfiguration(stochastName);

            // Assert
            Assert.IsNull(stochast.Mean);
            Assert.IsNull(stochast.StandardDeviation);
        }

        [Test]
        public void GetStochastConfiguration_StochastWithParameters_ReturnsNewStochastWithParametersSet()
        {
            // Setup
            const string stochastName = "stochastA";
            const double mean = 1.2;
            const double standardDeviation = 3.5;

            var stochastElement = new XElement("stochast");
            stochastElement.SetAttributeValue("naam", stochastName);
            stochastElement.Add(new XElement("verwachtingswaarde", mean));
            stochastElement.Add(new XElement("standaardafwijking", standardDeviation));
            stochastElement.Add(new XElement("variatiecoefficient", standardDeviation));
            var xElement = new XElement("root", new XElement("stochasten", stochastElement));

            // Call
            StochastConfiguration stochast = xElement.GetStochastConfiguration(stochastName);

            // Assert
            Assert.AreEqual(mean, stochast.Mean);
            Assert.AreEqual(standardDeviation, stochast.StandardDeviation);
            Assert.AreEqual(standardDeviation, stochast.VariationCoefficient);
        }

        [Test]
        public void GetWaveReductionParameters_CalculationElementNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((XElement) null).GetWaveReductionParameters();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetWaveReductionParameters_OtherDescendantElement_ReturnsNull()
        {
            // Setup
            var xElement = new XElement("root", new XElement("root", new XElement("OtherDescendantElement")));

            // Call
            WaveReductionConfiguration waveReduction = xElement.GetWaveReductionParameters();

            // Assert
            Assert.IsNull(waveReduction);
        }

        [Test]
        public void GetWaveReductionParameters_WithBreakWaterHeight_ReturnsConfiguration()
        {
            // Setup
            const double height = 2.1;

            var breakWaterHeightElement = new XElement("damhoogte", height);
            var waveReductionElement = new XElement("golfreductie", breakWaterHeightElement);
            var xElement = new XElement("root", new XElement("root", waveReductionElement));

            // Call
            WaveReductionConfiguration waveReduction = xElement.GetWaveReductionParameters();

            // Assert
            Assert.AreEqual(height, waveReduction.BreakWaterHeight);
            Assert.IsNull(waveReduction.BreakWaterType);
            Assert.IsNull(waveReduction.UseForeshoreProfile);
            Assert.IsNull(waveReduction.UseBreakWater);
        }

        [Test]
        [TestCase(double.MaxValue)]
        [TestCase(double.MinValue)]
        public void GetWaveReductionParameters_WithTooLargeBreakWaterHeight_ThrowsOverflowException(double heightValue)
        {
            // Setup
            string height = string.Format(CultureInfo.InvariantCulture, "{0}9", heightValue);

            var breakWaterHeightElement = new XElement("damhoogte", height);
            var waveReductionElement = new XElement("golfreductie", breakWaterHeightElement);
            var xElement = new XElement("root", new XElement("root", waveReductionElement));

            // Call
            TestDelegate test = () => xElement.GetWaveReductionParameters();

            // Assert
            Assert.Throws<OverflowException>(test);
        }

        [Test]
        public void GetWaveReductionParameters_WithInvalidBreakWaterHeight_ThrowsFormatException()
        {
            // Setup
            const string height = "very tall";

            var breakWaterHeightElement = new XElement("damhoogte", height);
            var waveReductionElement = new XElement("golfreductie", breakWaterHeightElement);
            var xElement = new XElement("root", new XElement("root", waveReductionElement));

            // Call
            TestDelegate test = () => xElement.GetWaveReductionParameters();

            // Assert
            Assert.Throws<FormatException>(test);
        }

        [Test]
        public void GetWaveReductionParameters_WithBreakWaterType_ReturnsConfiguration()
        {
            // Setup
            const string type = "havendam";

            var breakWaterTypeElement = new XElement("damtype", type);
            var waveReductionElement = new XElement("golfreductie", breakWaterTypeElement);
            var xElement = new XElement("root", new XElement("root", waveReductionElement));

            // Call
            WaveReductionConfiguration waveReduction = xElement.GetWaveReductionParameters();

            // Assert
            Assert.AreEqual(ConfigurationBreakWaterType.Dam, waveReduction.BreakWaterType);
            Assert.IsNull(waveReduction.BreakWaterHeight);
            Assert.IsNull(waveReduction.UseForeshoreProfile);
            Assert.IsNull(waveReduction.UseBreakWater);
        }

        [Test]
        public void GetWaveReductionParameters_WithInvalidBreakWaterType_ThrowsNotSupportedException()
        {
            // Setup
            const string type = "not really a type";

            var breakWaterTypeElement = new XElement("damtype", type);
            var waveReductionElement = new XElement("golfreductie", breakWaterTypeElement);
            var xElement = new XElement("root", new XElement("root", waveReductionElement));

            // Call
            TestDelegate test = () => xElement.GetWaveReductionParameters();

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
        public void GetWaveReductionParameters_WithUseForeshoreProfile_ReturnsConfiguration()
        {
            // Setup
            const string useForeshoreProfile = "true";

            var useForeshoreProfileElement = new XElement("voorlandgebruiken", useForeshoreProfile);
            var waveReductionElement = new XElement("golfreductie", useForeshoreProfileElement);
            var xElement = new XElement("root", new XElement("root", waveReductionElement));

            // Call
            WaveReductionConfiguration waveReduction = xElement.GetWaveReductionParameters();

            // Assert
            Assert.IsTrue(waveReduction.UseForeshoreProfile);
            Assert.IsNull(waveReduction.BreakWaterHeight);
            Assert.IsNull(waveReduction.BreakWaterType);
            Assert.IsNull(waveReduction.UseBreakWater);
        }

        [Test]
        public void GetWaveReductionParameters_WithInvalidUseForeshoreProfile_ThrowsFormatException()
        {
            // Setup
            const string useForeshoreProfile = "only half true";

            var useForeshoreProfileElement = new XElement("voorlandgebruiken", useForeshoreProfile);
            var waveReductionElement = new XElement("golfreductie", useForeshoreProfileElement);
            var xElement = new XElement("root", new XElement("root", waveReductionElement));

            // Call
            TestDelegate test = () => xElement.GetWaveReductionParameters();

            // Assert
            Assert.Throws<FormatException>(test);
        }

        [Test]
        public void GetWaveReductionParameters_WithUseBreakWater_ReturnsConfiguration()
        {
            // Setup
            const string useBreakWater = "true";

            var useBreakWaterElement = new XElement("damgebruiken", useBreakWater);
            var waveReductionElement = new XElement("golfreductie", useBreakWaterElement);
            var xElement = new XElement("root", new XElement("root", waveReductionElement));

            // Call
            WaveReductionConfiguration waveReduction = xElement.GetWaveReductionParameters();

            // Assert
            Assert.IsTrue(waveReduction.UseBreakWater);
            Assert.IsNull(waveReduction.BreakWaterHeight);
            Assert.IsNull(waveReduction.BreakWaterType);
            Assert.IsNull(waveReduction.UseForeshoreProfile);
        }

        [Test]
        public void GetWaveReductionParameters_WithInvalidUseBreakWater_ThrowsFormatException()
        {
            // Setup
            const string useBreakWater = "partially true";

            var useBreakWaterElement = new XElement("damgebruiken", useBreakWater);
            var waveReductionElement = new XElement("golfreductie", useBreakWaterElement);
            var xElement = new XElement("root", new XElement("root", waveReductionElement));

            // Call
            TestDelegate test = () => xElement.GetWaveReductionParameters();

            // Assert
            Assert.Throws<FormatException>(test);
        }

        [Test]
        public void GetWaveReductionParameters_WithAllParameters_ReturnsConfiguration()
        {
            // Setup
            const double height = 2.1;
            const string type = "havendam";
            const string useForeshoreProfile = "true";
            const string useBreakWater = "true";

            var breakWaterHeightElement = new XElement("damhoogte", height);
            var breakWaterTypeElement = new XElement("damtype", type);
            var useForeshoreProfileElement = new XElement("voorlandgebruiken", useForeshoreProfile);
            var useBreakWaterElement = new XElement("damgebruiken", useBreakWater);

            var waveReductionElement = new XElement("golfreductie",
                                                    breakWaterHeightElement,
                                                    breakWaterTypeElement,
                                                    useForeshoreProfileElement,
                                                    useBreakWaterElement);
            var xElement = new XElement("root", new XElement("root", waveReductionElement));

            // Call
            WaveReductionConfiguration waveReduction = xElement.GetWaveReductionParameters();

            // Assert
            Assert.AreEqual(height, waveReduction.BreakWaterHeight);
            Assert.AreEqual(ConfigurationBreakWaterType.Dam, waveReduction.BreakWaterType);
            Assert.IsTrue(waveReduction.UseForeshoreProfile);
            Assert.IsTrue(waveReduction.UseBreakWater);
        }

        [Test]
        public void GetScenarioConfiguration_CalculationElementNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((XElement) null).GetScenarioConfiguration();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationElement", exception.ParamName);
        }

        [Test]
        public void GetScenarioConfiguration_OtherDescendantElement_ReturnsNull()
        {
            // Setup
            var xElement = new XElement("root", new XElement("OtherDescendantElement"));

            // Call
            ScenarioConfiguration configuration = xElement.GetScenarioConfiguration();

            // Assert
            Assert.IsNull(configuration);
        }

        [Test]
        public void GetScenarioConfiguration_WithContribution_ReturnsConfiguration()
        {
            // Setup
            const double contribution = 2.1;

            var contributionElement = new XElement("bijdrage", contribution);
            var configurationElement = new XElement("scenario", contributionElement);
            var xElement = new XElement("root", configurationElement);

            // Call
            ScenarioConfiguration configuration = xElement.GetScenarioConfiguration();

            // Assert
            Assert.AreEqual(contribution, configuration.Contribution);
            Assert.IsNull(configuration.IsRelevant);
        }

        [Test]
        [TestCase(double.MaxValue)]
        [TestCase(double.MinValue)]
        public void GetScenarioConfiguration_WithTooLargeContribution_ThrowsOverflowException(double contributionValue)
        {
            // Setup
            string contribution = string.Format(CultureInfo.InvariantCulture, "{0}9", contributionValue);

            var contributionElement = new XElement("bijdrage", contribution);
            var configurationElement = new XElement("scenario", contributionElement);
            var xElement = new XElement("root", configurationElement);

            // Call
            TestDelegate test = () => xElement.GetScenarioConfiguration();

            // Assert
            Assert.Throws<OverflowException>(test);
        }

        [Test]
        public void GetScenarioConfiguration_WithInvalidContribution_ThrowsFormatException()
        {
            // Setup
            const string contribution = "very much";

            var contributionElement = new XElement("bijdrage", contribution);
            var configurationElement = new XElement("scenario", contributionElement);
            var xElement = new XElement("root", configurationElement);

            // Call
            TestDelegate test = () => xElement.GetScenarioConfiguration();

            // Assert
            Assert.Throws<FormatException>(test);
        }

        [Test]
        public void GetScenarioConfiguration_WithIsRelevant_ReturnsConfiguration()
        {
            // Setup
            const string isRelevant = "true";

            var isRelevantElement = new XElement("gebruik", isRelevant);
            var configurationElement = new XElement("scenario", isRelevantElement);
            var xElement = new XElement("root", configurationElement);

            // Call
            ScenarioConfiguration configuration = xElement.GetScenarioConfiguration();

            // Assert
            Assert.IsTrue(configuration.IsRelevant);
            Assert.IsNull(configuration.Contribution);
        }

        [Test]
        public void GetScenarioConfiguration_WithInvalidIsRelevant_ThrowsFormatException()
        {
            // Setup
            const string isRelevant = "partially true";

            var isRelevantElement = new XElement("gebruik", isRelevant);
            var configurationElement = new XElement("scenario", isRelevantElement);
            var xElement = new XElement("root", configurationElement);

            // Call
            TestDelegate test = () => xElement.GetScenarioConfiguration();

            // Assert
            Assert.Throws<FormatException>(test);
        }

        [Test]
        public void GetScenarioConfiguration_WithAllParameters_ReturnsConfiguration()
        {
            // Setup
            var random = new Random(123);
            double contribution = random.NextDouble();
            bool isRelevant = random.NextBoolean();

            var contributionElement = new XElement("bijdrage", contribution);
            var isRelevantElement = new XElement("gebruik", isRelevant
                                                                ? "true"
                                                                : "false");

            var configurationElement = new XElement("scenario",
                                                    contributionElement,
                                                    isRelevantElement);
            var xElement = new XElement("root", configurationElement);

            // Call
            ScenarioConfiguration configuration = xElement.GetScenarioConfiguration();

            // Assert
            Assert.AreEqual(contribution, configuration.Contribution);
            Assert.AreEqual(isRelevant, configuration.IsRelevant);
        }

        [Test]
        public void GetHydraulicBoundaryLocationName_CalculationElementNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((XElement) null).GetHydraulicBoundaryLocationName();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationElement", exception.ParamName);
        }

        [Test]
        [TestCase("hrlocatie")]
        [TestCase("hblocatie")]
        public void GetHydraulicBoundaryLocationName_WithHydraulicBoundaryLocation_ReturnHydraulicBoundaryLocationName(string tagName)
        {
            // Setup
            const string locationName = "Location1";

            var locationElement = new XElement(tagName, locationName);
            var configurationElement = new XElement("configuratie", locationElement);
            var xElement = new XElement("root", configurationElement);

            // Call
            string hydraulicBoundaryLocationName = xElement.GetHydraulicBoundaryLocationName();

            // Assert
            Assert.AreEqual(locationName, hydraulicBoundaryLocationName);
        }

        [Test]
        public void GetHydraulicBoundaryLocationName_WithBothHrLocationAndHbLocationElement_ReturnHbLocationName()
        {
            // Setup
            const string hrLocationName = "HRlocatie";
            const string hbLocationName = "HBlocatie";

            var hrLocationElement = new XElement("hrlocatie", hrLocationName);
            var hbLocationElement = new XElement("hblocatie", hbLocationName);
            var configurationElement = new XElement("configuratie", hrLocationElement, hbLocationElement);
            var xElement = new XElement("root", configurationElement);

            // Call
            string hydraulicBoundaryLocationName = xElement.GetHydraulicBoundaryLocationName();

            // Assert
            Assert.AreEqual(hbLocationName, hydraulicBoundaryLocationName);
        }

        [Test]
        public void GetHydraulicBoundaryLocationName_OtherDescendantElement_ReturnsNull()
        {
            // Setup
            var xElement = new XElement("root", new XElement("OtherDescendantElement"));

            // Call
            string hydraulicBoundaryLocationName = xElement.GetHydraulicBoundaryLocationName();

            // Assert
            Assert.IsNull(hydraulicBoundaryLocationName);
        }

        private class DoubleToBooleanConverter : TypeConverter
        {
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                var doubleValue = value as double?;
                if (doubleValue != null)
                {
                    if (Math.Abs(doubleValue.Value) < double.Epsilon)
                    {
                        return false;
                    }

                    return true;
                }

                return base.ConvertFrom(context, culture, value);
            }
        }

        private class ConverterThrowsExceptionOnConvertFrom : TypeConverter
        {
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                throw new Exception($"{value} is not a valid value for the target type.");
            }
        }
    }
}