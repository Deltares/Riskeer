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
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.StandAlone;

namespace Ringtoets.Integration.Data.Test.Placeholders
{
    [TestFixture]
    public class StandAloneFailureMechanismTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_NullOrEmptyName_ThrowsArgumentException(string name)
        {
            // Call
            TestDelegate test = () => new StandAloneFailureMechanism(name, "testCode");

            // Assert
            var paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("failureMechanismName", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_NullOrEmptyCode_ThrowsArgumentException(string code)
        {
            // Call
            TestDelegate test = () => new StandAloneFailureMechanism("testName", code);

            // Assert
            var paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("failureMechanismCode", paramName);
        }

        [Test]
        public void Constructor_WithNameAndCode_PropertiesSet()
        {
            // Setup
            const string expectedName = "testName";
            const string expectedCode = "testCode";

            // Call
            var failureMechanism = new StandAloneFailureMechanism(expectedName, expectedCode);

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase<FailureMechanismSectionResult>>(failureMechanism);
            Assert.AreEqual(expectedName, failureMechanism.Name);
            Assert.AreEqual(expectedCode, failureMechanism.Code);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }
    }
}