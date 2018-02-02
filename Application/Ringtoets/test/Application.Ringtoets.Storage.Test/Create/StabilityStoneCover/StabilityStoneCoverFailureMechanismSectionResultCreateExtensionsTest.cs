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
using Application.Ringtoets.Storage.Create.StabilityStoneCover;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.StabilityStoneCover.Data;

namespace Application.Ringtoets.Storage.Test.Create.StabilityStoneCover
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismSectionResultCreateExtensionsTest
    {
        [Test]
        public void Create_ValidData_ReturnsEntityEqualData()
        {
            // Setup
            var random = new Random(21);

            var sectionResult = new StabilityStoneCoverFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerOne = random.NextEnumValue<AssessmentLayerOneState>(),
                AssessmentLayerTwoA = random.NextEnumValue<AssessmentLayerTwoAResult>(),
                AssessmentLayerThree = random.NextRoundedDouble()
            };

            // Call
            StabilityStoneCoverSectionResultEntity result = sectionResult.Create();

            // Assert
            Assert.AreEqual(Convert.ToByte(sectionResult.AssessmentLayerOne), result.LayerOne);
            Assert.AreEqual(Convert.ToByte(sectionResult.AssessmentLayerTwoA), result.LayerTwoA);
            Assert.AreEqual(sectionResult.AssessmentLayerThree, result.LayerThree,
                            sectionResult.AssessmentLayerThree.GetAccuracy());
        }

        [Test]
        public void Create_WithNaNLevel3Result_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var sectionResult = new StabilityStoneCoverFailureMechanismSectionResult(new TestFailureMechanismSection())
            {
                AssessmentLayerThree = RoundedDouble.NaN
            };

            // Call
            StabilityStoneCoverSectionResultEntity result = sectionResult.Create();

            // Assert
            Assert.IsNull(result.LayerThree);
        }
    }
}