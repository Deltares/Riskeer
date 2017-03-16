// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Test.Readers
{
    [TestFixture]
    public class CalculationConfigurationReaderHelperTest
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
            TestDelegate test = () => CalculationConfigurationReaderHelper.GetDoubleValueFromDescendantElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetDoubleValueFromDescendantElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CalculationConfigurationReaderHelper.GetDoubleValueFromDescendantElement(new XElement("Root"), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
        }

        [Test]
        public void GetDoubleValueFromDescendantElement_ValidDescendantElement_ReturnValue()
        {
            // Setup
            const string descendantElementName = "number";
            const double descendantElementValue = 3;

            var element = new XElement("Root", new XElement(descendantElementName, descendantElementValue));

            // Call
            double? readValue = CalculationConfigurationReaderHelper.GetDoubleValueFromDescendantElement(element, descendantElementName);

            // Assert
            Assert.AreEqual(descendantElementValue, readValue.Value);
        }

        [Test]
        public void GetDoubleValueFromDescendantElement_InvalidDescendantElement_ReturnNull()
        {
            // Setup
            var element = new XElement("Root", new XElement("number", (double) 3));

            // Call
            double? readValue = CalculationConfigurationReaderHelper.GetDoubleValueFromDescendantElement(element, "invalidName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetStringValueFromDescendantElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CalculationConfigurationReaderHelper.GetStringValueFromDescendantElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetStringValueFromDescendantElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CalculationConfigurationReaderHelper.GetStringValueFromDescendantElement(new XElement("Test"), null);

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
            string readValue = CalculationConfigurationReaderHelper.GetStringValueFromDescendantElement(element, descendantElementName);

            // Assert
            Assert.AreEqual(descendantElementValue, readValue);
        }

        [Test]
        public void GetStringValueFromDescendantElement_InvalidDescendantElement_ReturnNull()
        {
            // Setup
            var element = new XElement("Root", new XElement("number", "valueText"));

            // Call
            string readValue = CalculationConfigurationReaderHelper.GetStringValueFromDescendantElement(element, "invalidName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetBoolValueFromDescendantElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CalculationConfigurationReaderHelper.GetBoolValueFromDescendantElement(null, "");

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
            TestDelegate test = () => CalculationConfigurationReaderHelper.GetBoolValueFromDescendantElement(element, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
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
            bool? readValue = CalculationConfigurationReaderHelper.GetBoolValueFromDescendantElement(element, descendantElementName);

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
            bool? readValue = CalculationConfigurationReaderHelper.GetBoolValueFromDescendantElement(element, "unmatchingChildElementName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetStochastElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CalculationConfigurationReaderHelper.GetStochastElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetStochastElement_StochastNameNull_ThrownArgumentNullException()
        {
            // Setup
            var element = new XElement("Root");

            // Call
            TestDelegate test = () => CalculationConfigurationReaderHelper.GetStochastElement(element, null);

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
            XElement stochastElement = CalculationConfigurationReaderHelper.GetStochastElement(element, "stochast_name");

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
            XElement stochastElement = CalculationConfigurationReaderHelper.GetStochastElement(element, "stochast_name");

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
            XElement stochastElement = CalculationConfigurationReaderHelper.GetStochastElement(element, "stochast_name");

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
            XElement stochastElement = CalculationConfigurationReaderHelper.GetStochastElement(element, stochastName);

            // Assert
            Assert.AreSame(stochastElementToMatch, stochastElement);
        }

        [Test]
        public void GetDescendantElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CalculationConfigurationReaderHelper.GetDescendantElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetDescendantElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CalculationConfigurationReaderHelper.GetDescendantElement(new XElement("Test"), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(XElements))]
        public void GetDescendantElement_ValidDescendantName_ReturnElement(XElement parentElement)
        {
            // Call
            XElement element = CalculationConfigurationReaderHelper.GetDescendantElement(parentElement, "descendant");

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
            XElement element = CalculationConfigurationReaderHelper.GetDescendantElement(parentElement, "something_else");

            // Assert
            Assert.IsNull(element);
        }
    }
}