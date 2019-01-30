// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Data.Test.Structures
{
    [TestFixture]
    public class StructuresOutputTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ExpectedValues(
            bool withIllustrationPoints)
        {
            // Setup
            var random = new Random(39);
            double reliability = random.NextDouble();
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = withIllustrationPoints
                                                                                  ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                                                  : null;

            // Call
            var structuresOutput = new StructuresOutput(reliability, generalResult);

            // Assert
            Assert.IsInstanceOf<ICloneable>(structuresOutput);
            Assert.AreEqual(reliability, structuresOutput.Reliability);
            Assert.AreEqual(withIllustrationPoints, structuresOutput.HasGeneralResult);
            Assert.AreSame(generalResult, structuresOutput.GeneralResult);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            StructuresOutput original = CommonTestDataGenerator.GetRandomStructuresOutput(null);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            StructuresOutput original = CommonTestDataGenerator.GetRandomStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }
    }
}