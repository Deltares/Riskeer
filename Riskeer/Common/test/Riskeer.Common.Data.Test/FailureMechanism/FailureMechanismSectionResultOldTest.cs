﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismSectionResultOldTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var result = new TestFailureMechanismSectionResultOld(section);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionResult>(result);
            Assert.IsInstanceOf<Observable>(result);
            Assert.AreSame(section, result.Section);
        }

        [Test]
        public void Constructor_WithoutSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestFailureMechanismSectionResultOld(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        private class TestFailureMechanismSectionResultOld : FailureMechanismSectionResultOld
        {
            public TestFailureMechanismSectionResultOld(FailureMechanismSection section) : base(section) {}
        }
    }
}