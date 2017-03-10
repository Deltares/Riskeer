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
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Readers;

namespace Ringtoets.Common.IO.Test.Readers
{
    [TestFixture]
    public class CombinedXmlSchemaDefinitionTest
    {
        private readonly string validMainSchemaDefinition;

        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                               "CombinedXmlSchemaDefinition");

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_EmptyMainSchemaDefinition_ThrowArgumentException(string emptyMainSchemaDefinition)
        {
            // Call
            TestDelegate call = () => new CombinedXmlSchemaDefinition(emptyMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("'mainSchemaDefinition' null, empty or only containing white spaces.", exception.Message);
        }

        [Test]
        public void Constructor_NestedSchemaDefinitionsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new CombinedXmlSchemaDefinition(validMainSchemaDefinition, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("nestedSchemaDefinitions", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_EmptyNestedSchemaDefinition_ThrowArgumentException(string emptyNestedSchemaDefinition)
        {
            // Call
            TestDelegate call = () => new CombinedXmlSchemaDefinition(validMainSchemaDefinition, new Dictionary<string, string>
            {
                {
                    "Test", emptyNestedSchemaDefinition
                }
            });

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("'nestedSchemaDefinitions' holds a nested schema definition value that equals null, is empty or only contains white spaces.", exception.Message);
        }

        [Test]
        [TestCase("textContent.xsd",
            "'mainSchemaDefinition' containing invalid schema definition: Data at the root level is invalid. Line 1, position 1.",
            typeof(XmlException))]
        [TestCase("invalidXsdContent.xsd",
            "'mainSchemaDefinition' containing invalid schema definition: The 'http://www.w3.org/2001/XMLSchema:redefine' element is not supported in this context.",
            typeof(XmlSchemaException))]
        public void Constructor_InvalidMainSchemaDefinition_ThrowArgumentException(string invalidMainSchemaDefinition,
                                                                                   string expectedMessage,
                                                                                   Type expectedInnerExceptionType)
        {
            // Setup
            string xsdPath = Path.Combine(testDirectoryPath, invalidMainSchemaDefinition);

            // Call
            TestDelegate call = () => new CombinedXmlSchemaDefinition(File.ReadAllText(xsdPath), new Dictionary<string, string>());

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf(expectedInnerExceptionType, exception.InnerException);
        }

        public CombinedXmlSchemaDefinitionTest()
        {
            validMainSchemaDefinition = File.ReadAllText(Path.Combine(testDirectoryPath, "validMainSchemaDefinition.xsd"));
        }
    }
}