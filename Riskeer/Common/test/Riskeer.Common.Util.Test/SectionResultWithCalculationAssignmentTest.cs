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
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Riskeer.Common.Util.Test
{
    [TestFixture]
    public class SectionResultWithCalculationAssignmentTest
    {
        [Test]
        public void Constructor_FailureMechanismSectionResultIsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SectionResultWithCalculationAssignment(null, result => null, (result, calculation) => {});

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("result", paramName);
        }

        [Test]
        public void Constructor_GetCalculationActionIsNull_ThrowsArgumentNullException()
        {
            // Setup
            TestFailureMechanismSectionResult failureMechanismSectionResult =
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            // Call
            TestDelegate test = () => new SectionResultWithCalculationAssignment(failureMechanismSectionResult,
                                                                                 null, (result, calculation) => {});

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("getCalculationAction", paramName);
        }

        [Test]
        public void Constructor_SetCalculationActionIsNull_ThrowsArgumentNullException()
        {
            // Setup
            TestFailureMechanismSectionResult failureMechanismSectionResult =
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            // Call
            TestDelegate test = () => new SectionResultWithCalculationAssignment(failureMechanismSectionResult,
                                                                                 result => null, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("setCalculationAction", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            TestFailureMechanismSectionResult failureMechanismSectionResult =
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            // Call
            var sectionResultWithCalculationAssignment = new SectionResultWithCalculationAssignment(failureMechanismSectionResult,
                                                                                                    result => null, (result, calculation) => {});

            // Assert
            Assert.AreSame(failureMechanismSectionResult, sectionResultWithCalculationAssignment.Result);
        }
    }
}