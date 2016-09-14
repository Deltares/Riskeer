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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Properties;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Data.Test.StandAlone.SectionResults
{
    [TestFixture]
    public class MacrostabilityOutwardsFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_WithoutSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacrostabilityOutwardsFailureMechanismSectionResult(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void Constructor_WithSection_ResultCreatedForSection()
        {
            // Setup
            var section = new FailureMechanismSection("Section", new[]
            {
                new Point2D(0, 0)
            });

            // Call
            var result = new MacrostabilityOutwardsFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResult>(result);
            Assert.AreSame(section, result.Section);
            Assert.IsFalse(result.AssessmentLayerOne);
            Assert.IsNaN(result.AssessmentLayerTwoA);
            Assert.IsNaN(result.AssessmentLayerThree);
        }

        [Test]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void AssessmentLayerTwoA_ForInvalidValues_ThrowsException(double a)
        {
            // Setup
            var section = new FailureMechanismSection("Section", new[]
            {
                new Point2D(0, 0)
            });
            var result = new MacrostabilityOutwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => result.AssessmentLayerTwoA = a;

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual(
                Resources.ArbitraryProbabilityFailureMechanismSectionResult_AssessmentLayerTwoA_Value_needs_to_be_between_0_and_1,
                message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1e-6)]
        [TestCase(0.5)]
        [TestCase(1 - 1e-6)]
        [TestCase(1)]
        public void AssessmentLayerTwoA_ForValidValues_NewValueSet(double a)
        {
            // Setup
            var section = new FailureMechanismSection("Section", new[]
            {
                new Point2D(0, 0)
            });
            var result = new MacrostabilityOutwardsFailureMechanismSectionResult(section);

            // Call
            result.AssessmentLayerTwoA = a;

            // Assert
            Assert.AreEqual(a, result.AssessmentLayerTwoA);
        }
    }
}