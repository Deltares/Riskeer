// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.IO;
using System.Xml.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations.Import;

namespace Riskeer.Common.IO.Test.Configurations.Import
{
    [TestFixture]
    public class CalculationConfigurationMigratorTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                               nameof(CalculationConfigurationMigrator));
        [Test]
        public void Migrate_XmlDocumentNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => CalculationConfigurationMigrator.Migrate(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("xmlDocument", exception.ParamName);
        }

        [Test]
        public void Migrate_MigrationScriptDefinitionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => CalculationConfigurationMigrator.Migrate(new XDocument(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("migrationScriptDefinition", exception.ParamName);
        }

        [Test]
        public void Migrate_EmptyMigrationScript_MigratesXmlDocument()
        {
            // Setup
            string xslt = File.ReadAllText(Path.Combine(testDirectoryPath, "EmptyMigrationScript.xslt"));
            XDocument xmlDocument = GetDefaultXDocument();

            // Call
            XDocument migratedXDocument = CalculationConfigurationMigrator.Migrate(xmlDocument, xslt);

            // Assert
            Assert.IsTrue(XNode.DeepEquals(xmlDocument, migratedXDocument));
        }

        [Test]
        public void Migrate_WithMigrationScript_MigratesXmlDocument()
        {
            // Setup
            string xslt = File.ReadAllText(Path.Combine(testDirectoryPath, "MigrationScript.xslt"));
            XDocument xmlDocument = GetDefaultXDocument();

            // Call
            XDocument migratedXDocument = CalculationConfigurationMigrator.Migrate(xmlDocument, xslt);

            // Assert
            XDocument expectedXmlDocument = GetDefaultXDocument();
            var attribute = new XAttribute("test", true);
            expectedXmlDocument.Root.Add(attribute);
            var newNode = new XElement("newNode");
            expectedXmlDocument.Root.Add(newNode);

            Assert.IsTrue(XNode.DeepEquals(expectedXmlDocument, migratedXDocument));
        }

        [Test]
        public void Migrate_InvalidMigrationScript_ThrowsCalculationConfigurationMigrationException()
        {
            // Setup
            string xslt = File.ReadAllText(Path.Combine(testDirectoryPath, "InvalidMigrationScript.xslt"));
            XDocument xmlDocument = GetDefaultXDocument();

            // Call
            void Call() => CalculationConfigurationMigrator.Migrate(xmlDocument, xslt);

            // Assert
            var exception = Assert.Throws<CalculationConfigurationMigrationException>(Call);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        private static XDocument GetDefaultXDocument()
        {
            var xDocument = new XDocument();
            var root = new XElement("root");
            xDocument.Add(root);
            return xDocument;
        }
    }
}