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
using System.Xml.Linq;
using NUnit.Framework;
using Ringtoets.Common.IO.Readers;

namespace Ringtoets.Common.IO.Test.Readers
{
    [TestFixture]
    public class ConfigurationReaderHelperTest
    {
        [Test]
        public void GetDoubleValueFromChildElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ConfigurationReaderHelper.GetDoubleValueFromChildElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetDoubleValueFromChildElement_ChildElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ConfigurationReaderHelper.GetDoubleValueFromChildElement(new XElement("Root"), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("childElementName", exception.ParamName);
        }

        [Test]
        public void GetDoubleValueFromChildElement_ValidChildElement_ReturnValue()
        {
            // Setup
            const string childElementName = "number";
            const double childElementValue = 3;

            var element = new XElement("Root", new XElement(childElementName, childElementValue));

            // Call
            double? readValue = ConfigurationReaderHelper.GetDoubleValueFromChildElement(element, childElementName);

            // Assert
            Assert.AreEqual(childElementValue, readValue.Value);
        }

        [Test]
        public void GetDoubleValueFromChildElement_InvalidChildElement_ReturnNull()
        {
            // Setup
            var element = new XElement("Root", new XElement("number", (double) 3));

            // Call
            double? readValue = ConfigurationReaderHelper.GetDoubleValueFromChildElement(element, "invalidName");

            // Assert
            Assert.IsNull(readValue);
        }

        [Test]
        public void GetStringValueFromChildElement_ParentElementNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ConfigurationReaderHelper.GetStringValueFromChildElement(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentElement", exception.ParamName);
        }

        [Test]
        public void GetStringValueFromChildElement_ChildElementNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ConfigurationReaderHelper.GetStringValueFromChildElement(new XElement("Test"), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("childElementName", exception.ParamName);
        }

        [Test]
        public void GetStringValueFromChildElement_ValidChildElement_ReturnValue()
        {
            // Setup
            const string childElementName = "text";
            const string childElementValue = "valueText";

            var element = new XElement("Root", new XElement(childElementName, childElementValue));

            // Call
            string readValue = ConfigurationReaderHelper.GetStringValueFromChildElement(element, childElementName);

            // Assert
            Assert.AreEqual(childElementValue, readValue);
        }

        [Test]
        public void GetStringValueFromChildElement_InvalidChildElement_ReturnNull()
        {
            // Setup
            var element = new XElement("Root", new XElement("number", "valueText"));

            // Call
            string readValue = ConfigurationReaderHelper.GetStringValueFromChildElement(element, "invalidName");

            // Assert
            Assert.IsNull(readValue);
        }
    }
}