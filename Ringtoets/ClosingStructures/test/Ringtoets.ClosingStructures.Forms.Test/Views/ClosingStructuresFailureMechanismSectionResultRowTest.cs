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
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.Test.Views
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new ClosingStructuresFailureMechanismSectionResult(section);

            // Call
            var row = new ClosingStructuresFailureMechanismSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<ClosingStructuresFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            Assert.IsTrue(TypeUtils.HasTypeConverter<ClosingStructuresFailureMechanismSectionResultRow,
                              FailureMechanismSectionResultNoProbabilityValueDoubleConverter>(
                                  r => r.AssessmentLayerTwoA));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        public void AssessmentLayerTwoA_ForValidValues_ResultPropertyChanged(double value)
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new ClosingStructuresFailureMechanismSectionResult(section);
            var row = new ClosingStructuresFailureMechanismSectionResultRow(result);

            // Call
            row.AssessmentLayerTwoA = value;

            // Assert
            Assert.AreEqual(value, row.AssessmentLayerTwoA);
        }

        [Test]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void AssessmentLayerTwoA_ForInvalidValues_ThrowsArgumentException(double value)
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new ClosingStructuresFailureMechanismSectionResult(section);
            var row = new ClosingStructuresFailureMechanismSectionResultRow(result);

            // Call
            TestDelegate test = () => row.AssessmentLayerTwoA = value;

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual(
                RingtoetsCommonDataResources.ArbitraryProbabilityFailureMechanismSectionResult_AssessmentLayerTwoA_Value_needs_to_be_between_0_and_1,
                message);
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("name", new[]
            {
                new Point2D(0, 0)
            });
        }
    }
}