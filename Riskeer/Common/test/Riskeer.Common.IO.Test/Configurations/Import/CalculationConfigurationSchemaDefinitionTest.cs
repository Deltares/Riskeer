﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Collections.Generic;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations.Import;

namespace Riskeer.Common.IO.Test.Configurations.Import
{
    [TestFixture]
    public class CalculationConfigurationSchemaDefinitionTest
    {
        [Test]
        public void Constructor_MainSchemaDefinitionNull_ThrowsArgumentNullException()
        {
            // Setup
            int versionNumber = new Random(21).Next();
            var nestedSchemaDefinitions = new Dictionary<string, string>();
            const string migrationScript = "migrationScript";

            // Call
            void Call() => new CalculationConfigurationSchemaDefinition(versionNumber, null, nestedSchemaDefinitions, migrationScript);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("mainSchemaDefinition", exception.ParamName);
        }

        [Test]
        public void Constructor_NestedSchemaDefinitionsNull_ThrowsArgumentNullException()
        {
            // Setup
            int versionNumber = new Random(21).Next();
            const string mainSchemaDefinition = "mainSchemaDefinition";
            const string migrationScript = "migrationScript";

            // Call
            void Call() => new CalculationConfigurationSchemaDefinition(versionNumber, mainSchemaDefinition, null, migrationScript);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("nestedSchemaDefinitions", exception.ParamName);
        }

        [Test]
        public void Constructor_MigrationScriptNull_ThrowsArgumentNullException()
        {
            // Setup
            int versionNumber = new Random(21).Next();
            const string mainSchemaDefinition = "mainSchemaDefinition";
            var nestedSchemaDefinitions = new Dictionary<string, string>();

            // Call
            void Call() => new CalculationConfigurationSchemaDefinition(versionNumber, mainSchemaDefinition, nestedSchemaDefinitions, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("migrationScript", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            int versionNumber = new Random(21).Next();
            const string mainSchemaDefinition = "mainSchemaDefinition";
            var nestedSchemaDefinitions = new Dictionary<string, string>();
            const string migrationScript = "migrationScript";

            // Call
            var schemaDefinition = new CalculationConfigurationSchemaDefinition(versionNumber, mainSchemaDefinition, nestedSchemaDefinitions, migrationScript);

            // Assert
            Assert.AreEqual(versionNumber, schemaDefinition.VersionNumber);
            Assert.AreEqual(mainSchemaDefinition, schemaDefinition.MainSchemaDefinition);
            Assert.AreSame(nestedSchemaDefinitions, schemaDefinition.NestedSchemaDefinitions);
            Assert.AreEqual(migrationScript, schemaDefinition.MigrationScript);
        }
    }
}