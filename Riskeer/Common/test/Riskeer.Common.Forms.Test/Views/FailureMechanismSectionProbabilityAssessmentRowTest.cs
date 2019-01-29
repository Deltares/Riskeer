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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.Probability;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
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
            TestDelegate test = () => new FailureMechanismSectionProbabilityAssessmentRow(section, double.NaN, double.NaN, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("probabilityAssessmentInput", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            FailureMechanismSection section = GetTestFailureMechanismSection();
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(random.NextDouble(), random.NextDouble());

            // Call
            var sectionRow = new FailureMechanismSectionProbabilityAssessmentRow(section, sectionStart, sectionEnd, probabilityAssessmentInput);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionRow>(sectionRow);

            Assert.AreEqual(section.Name, sectionRow.Name);

            Assert.AreEqual(sectionStart, sectionRow.SectionStart, sectionRow.SectionStart.GetAccuracy());
            Assert.AreEqual(sectionEnd, sectionRow.SectionEnd, sectionRow.SectionEnd.GetAccuracy());

            Assert.AreEqual(section.Length, sectionRow.Length, sectionRow.Length.GetAccuracy());

            Assert.AreEqual(2, sectionRow.N.NumberOfDecimalPlaces);
            AssertLengthEffect(probabilityAssessmentInput, section, sectionRow);
        }

        [Test]
        public void GivenRow_WhenProbabilityAssessmentInputChanged_ThenLengthEffectChangedAccordingly()
        {
            // Given
            var random = new Random(39);
            FailureMechanismSection section = GetTestFailureMechanismSection();
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(random.NextDouble(), random.NextDouble());
            var sectionRow = new FailureMechanismSectionProbabilityAssessmentRow(section, double.NaN, double.NaN, probabilityAssessmentInput);

            // Precondition
            AssertLengthEffect(probabilityAssessmentInput, section, sectionRow);

            // When
            probabilityAssessmentInput.A = random.NextDouble();

            // Then
            AssertLengthEffect(probabilityAssessmentInput, section, sectionRow);
        }

        private static FailureMechanismSection GetTestFailureMechanismSection()
        {
            var random = new Random();

            return new FailureMechanismSection("test", new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            });
        }

        private static void AssertLengthEffect(ProbabilityAssessmentInput probabilityAssessmentInput, FailureMechanismSection section, FailureMechanismSectionProbabilityAssessmentRow sectionRow)
        {
            Assert.AreEqual(probabilityAssessmentInput.GetN(section.Length), sectionRow.N, sectionRow.N.GetAccuracy());
        }
    }
}