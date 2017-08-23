﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new GrassCoverErosionOutwardsFailureMechanismSectionResult(section);

            // Call
            var row = new GrassCoverErosionOutwardsFailureMechanismSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<GrassCoverErosionOutwardsFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);

            TestHelper.AssertTypeConverter<GrassCoverErosionOutwardsFailureMechanismSectionResultRow, NoValueRoundedDoubleConverter>(
                nameof(GrassCoverErosionOutwardsFailureMechanismSectionResultRow.AssessmentLayerThree));
        }

        [Test]
        public void AssessmentLayerTwoA_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            const AssessmentLayerTwoAResult newValue = AssessmentLayerTwoAResult.Successful;
            FailureMechanismSection section = CreateSection();
            var result = new GrassCoverErosionOutwardsFailureMechanismSectionResult(section);
            var row = new GrassCoverErosionOutwardsFailureMechanismSectionResultRow(result);

            // Call
            row.AssessmentLayerTwoA = newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerThree_ValueSet_ReturnExpectedValue()
        {
            // Setup
            var random = new Random(21);
            RoundedDouble assessmentLayerThree = random.NextRoundedDouble();

            var sectionResult = new GrassCoverErosionOutwardsFailureMechanismSectionResult(CreateSection());
            var row = new GrassCoverErosionOutwardsFailureMechanismSectionResultRow(sectionResult);

            // Call
            row.AssessmentLayerThree = assessmentLayerThree;

            // Assert
            Assert.AreEqual(assessmentLayerThree, sectionResult.AssessmentLayerThree);
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