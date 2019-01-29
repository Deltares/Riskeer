// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using NUnit.Framework;

namespace Migration.Scripts.Data.TestUtil.Test
{
    [TestFixture]
    public class TestCreateScriptTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_VersionEmptyOrNull_ThrowsException(string version)
        {
            // Call
            TestDelegate call = () => new TestCreateScript(version);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("version", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedProperties()
        {
            // Setup
            const string version = "version";

            // Call
            var createScript = new TestCreateScript(version);

            // Assert
            Assert.IsInstanceOf<CreateScript>(createScript);
            Assert.AreEqual(version, createScript.GetVersion());
        }
    }
}