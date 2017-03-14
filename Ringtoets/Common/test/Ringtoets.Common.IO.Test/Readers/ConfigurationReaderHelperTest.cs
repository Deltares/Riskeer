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
using System.Xml.Linq;
using NUnit.Framework;
using Ringtoets.Common.IO.Readers;

namespace Ringtoets.Common.IO.Test.Readers
{
    [TestFixture]
    public class ConfigurationReaderHelperTest
    {
        [Test]
        public void GetDoubleValueFromDescendantElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ConfigurationReaderHelper.GetDoubleValueFromDescendantElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetDoubleValueFromDescendantElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ConfigurationReaderHelper.GetDoubleValueFromDescendantElement(new XElement("Root"), null);

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
            double? readValue = ConfigurationReaderHelper.GetDoubleValueFromDescendantElement(element, descendantElementName);

            // Assert
            Assert.AreEqual(descendantElementValue, readValue.Value);
        }

        [Test]
        public void GetDoubleValueFromDescendantElement_InvalidDescendantElement_ReturnNull()
        {
            // Setup
            var element = new XElement("Root", new XElement("number", (double) 3));

            // Call
            double? readValue = ConfigurationReaderHelper.GetDoubleValueFromDescendantElement(element, "invalidName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetStringValueFromDescendantElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ConfigurationReaderHelper.GetStringValueFromDescendantElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetStringValueFromDescendantElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ConfigurationReaderHelper.GetStringValueFromDescendantElement(new XElement("Test"), null);

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
            string readValue = ConfigurationReaderHelper.GetStringValueFromDescendantElement(element, descendantElementName);

            // Assert
            Assert.AreEqual(descendantElementValue, readValue);
        }

        [Test]
        public void GetStringValueFromDescendantElement_InvalidDescendantElement_ReturnNull()
        {
            // Setup
            var element = new XElement("Root", new XElement("number", "valueText"));

            // Call
            string readValue = ConfigurationReaderHelper.GetStringValueFromDescendantElement(element, "invalidName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetDescendantElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ConfigurationReaderHelper.GetDescendantElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetDescendantElement_DescendantElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ConfigurationReaderHelper.GetDescendantElement(new XElement("Test"), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("descendantElementName", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(XElements))]
        public void GetDescendantElement_ValidDescendantName_ReturnElement(XElement parentElement)
        {
            // Call
            XElement element = ConfigurationReaderHelper.GetDescendantElement(parentElement, "descendant");

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
            XElement element = ConfigurationReaderHelper.GetDescendantElement(parentElement, "something_else");

            // Assert
            Assert.IsNull(element);
        }

        private static IEnumerable<TestCaseData> XElements
        {
            get
            {
                yield return new TestCaseData(new XElement("Root", new XElement("descendant")));
                yield return new TestCaseData(new XElement("Root", new XElement("Child", new XElement("descendant"))));
            }
        }
    }
}