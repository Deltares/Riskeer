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
using NUnit.Framework;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.Test.Structures
{
    [TestFixture]
    public class StructuresOutputTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ProbabilityAssessmentNull_ThrowsArgumentNullException(bool withIllustrationPoints)
        {
            // Setup
            TestGeneralResultFaultTreeIllustrationPoint generalResult = withIllustrationPoints
                                                                            ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                                            : null;

            // Call
            TestDelegate call = () => new StructuresOutput(null, generalResult);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("probabilityAssessmentOutput", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidProbabilityAssessmentOutputAndGeneralResultNulls_ReturnsExpectedProperties()
        {
            // Setup
            var output = new TestProbabilityAssessmentOutput();

            // Call
            var structuresOutput = new StructuresOutput(output, null);

            // Assert
            Assert.AreSame(output, structuresOutput.ProbabilityAssessmentOutput);
            Assert.IsFalse(structuresOutput.HasGeneralResult);
            Assert.IsNull(structuresOutput.GeneralResult);
        }

        [Test]
        public void Constructor_ValidProbabilityAssessmentOutputAndGeneralResult_SetExpectedProperties()
        {
            // Setup
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();
            var probabilityAssessmentOutput = new TestProbabilityAssessmentOutput();

            // Call
            var structuresOutput = new StructuresOutput(probabilityAssessmentOutput, generalResult);

            // Assert
            Assert.AreSame(probabilityAssessmentOutput, structuresOutput.ProbabilityAssessmentOutput);
            Assert.AreSame(generalResult, structuresOutput.GeneralResult);
            Assert.IsTrue(structuresOutput.HasGeneralResult);
        }
    }
}