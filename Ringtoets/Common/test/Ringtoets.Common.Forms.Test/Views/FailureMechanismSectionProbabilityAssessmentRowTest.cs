// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.Probability;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismSectionProbabilityAssessmentRowTest
    {
        [Test]
        public void Constructor_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = GetTestFailureMechanismSection();

            // Call
            TestDelegate test = () => new FailureMechanismSectionProbabilityAssessmentRow(section, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("probabilityAssessmentInput", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = GetTestFailureMechanismSection();
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(1, 2);

            // Call
            var sectionRow = new FailureMechanismSectionProbabilityAssessmentRow(section, probabilityAssessmentInput);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionRow>(sectionRow);
            Assert.AreEqual(section.Name, sectionRow.Name);
            Assert.AreEqual(2, sectionRow.Length.NumberOfDecimalPlaces);
            Assert.AreEqual(section.Length, sectionRow.Length, sectionRow.Length.GetAccuracy());
            Assert.AreEqual(2, sectionRow.N.NumberOfDecimalPlaces);
            Assert.AreEqual(probabilityAssessmentInput.GetN(section.Length), sectionRow.N, sectionRow.N.GetAccuracy());
        }

        private static FailureMechanismSection GetTestFailureMechanismSection()
        {
            return new FailureMechanismSection("test", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(0.0, 10.0)
            });
        }
    }
}