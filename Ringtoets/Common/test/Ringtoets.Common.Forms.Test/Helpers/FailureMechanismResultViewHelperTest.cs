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

using NUnit.Framework;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailureMechanismResultViewHelperTest
    {
        [Test]
        [TestCase(SimpleAssessmentResultType.NotApplicable)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible)]
        public void SimpleAssessmentIsSufficient_WithPassingSimpleAssessmentResultType_ReturnsTrue(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Call
            bool isSufficient = FailureMechanismResultViewHelper.SimpleAssessmentIsSufficient(simpleAssessmentResult);

            // Assert
            Assert.IsTrue(isSufficient);
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void SimpleAssessmentIsSufficient_WithNotPassingSimpleAssessmentResultType_ReturnsFalse(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Call
            bool isSufficient = FailureMechanismResultViewHelper.SimpleAssessmentIsSufficient(simpleAssessmentResult);

            // Assert
            Assert.IsFalse(isSufficient);
        }

        [Test]
        [TestCase(SimpleAssessmentResultValidityOnlyType.NotApplicable)]
        public void SimpleAssessmentIsSufficient_WithPassingSimpleAssessmentResultValidityOnlyType_ReturnsTrue(
            SimpleAssessmentResultValidityOnlyType simpleAssessmentResult)
        {
            // Call
            bool isSufficient = FailureMechanismResultViewHelper.SimpleAssessmentIsSufficient(simpleAssessmentResult);

            // Assert
            Assert.IsTrue(isSufficient);
        }

        [Test]
        [TestCase(SimpleAssessmentResultValidityOnlyType.None)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.Applicable)]
        public void SimpleAssessmentIsSufficient_WithNotPassingSimpleAssessmentResultValidityOnlyType_ReturnsFalse(
            SimpleAssessmentResultValidityOnlyType simpleAssessmentResult)
        {
            // Call
            bool isSufficient = FailureMechanismResultViewHelper.SimpleAssessmentIsSufficient(simpleAssessmentResult);

            // Assert
            Assert.IsFalse(isSufficient);
        }
    }
}