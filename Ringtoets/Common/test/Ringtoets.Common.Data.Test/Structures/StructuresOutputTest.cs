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
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using CoreCloneAssert = Core.Common.Data.TestUtil.CloneAssert;
using CommonCloneAssert = Ringtoets.Common.Data.TestUtil.CloneAssert;

namespace Ringtoets.Common.Data.Test.Structures
{
    [TestFixture]
    public class StructuresOutputTest
    {
        [Test]
        public void Constructor_ProbabilityAssessmentNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StructuresOutput(null, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("probabilityAssessmentOutput", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ValidProbabilityAssessmentOutputAndGeneralResult_ReturnsExpectedProperties(
            bool withIllustrationPoints)
        {
            // Setup
            var output = new TestProbabilityAssessmentOutput();
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = withIllustrationPoints
                                                                                  ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                                                  : null;

            // Call
            var structuresOutput = new StructuresOutput(output, generalResult);

            // Assert
            Assert.IsInstanceOf<ICloneable>(structuresOutput);
            Assert.AreSame(output, structuresOutput.ProbabilityAssessmentOutput);
            Assert.AreEqual(withIllustrationPoints, structuresOutput.HasGeneralResult);
            Assert.AreSame(generalResult, structuresOutput.GeneralResult);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new StructuresOutput(new TestProbabilityAssessmentOutput(), null);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new StructuresOutput(new TestProbabilityAssessmentOutput(), new TestGeneralResultFaultTreeIllustrationPoint());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }
    }
}