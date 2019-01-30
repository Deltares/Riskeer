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
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Data.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class FaultTreeIllustrationPointTestFactoryTest
    {
        [Test]
        public void CreateTestFaultTreeIllustrationPoint_DefaultBeta_ReturnsExpectedProperties()
        {
            // Call
            FaultTreeIllustrationPoint illustrationPoint = FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPoint();

            // Assert
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(illustrationPoint);

            Assert.AreEqual("Illustration point", illustrationPoint.Name);
            Assert.AreEqual(1.23, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(CombinationType.And, illustrationPoint.CombinationType);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
        }

        [Test]
        public void CreateTestFaultTreeIllustrationPoint_WithBeta_ReturnsExpectedProperties()
        {
            // Setup
            double beta = new Random().NextDouble();

            // Call
            FaultTreeIllustrationPoint illustrationPoint = FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPoint(beta);

            // Assert
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(illustrationPoint);

            Assert.AreEqual("Illustration point", illustrationPoint.Name);
            Assert.AreEqual(beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(CombinationType.And, illustrationPoint.CombinationType);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
        }

        [Test]
        public void CreateTestFaultTreeIllustrationPointCombinationTypeAnd_ValidBetaParameter_ReturnsExpectedProperties()
        {
            // Setup
            double beta = new Random().NextDouble();

            // Call
            FaultTreeIllustrationPoint illustrationPoint = FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPointCombinationTypeAnd(beta);

            // Assert
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(illustrationPoint);

            Assert.AreEqual("Illustration point", illustrationPoint.Name);
            Assert.AreEqual(beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(CombinationType.And, illustrationPoint.CombinationType);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
        }

        [Test]
        public void CreateTestFaultTreeIllustrationPointCombinationTypeAnd_ValidNameParameter_ReturnsExpectedProperties()
        {
            // Setup
            const string name = "Random name";

            // Call
            FaultTreeIllustrationPoint illustrationPoint = FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPointCombinationTypeAnd(name);

            // Assert
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(illustrationPoint);

            Assert.AreEqual(name, illustrationPoint.Name);
            Assert.AreEqual(1.23, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(CombinationType.And, illustrationPoint.CombinationType);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
        }

        [Test]
        public void CreateTestFaultTreeIllustrationPointCombinationTypeOr_ValidParameter_ReturnsExpectedProperties()
        {
            // Setup
            double beta = new Random().NextDouble();

            // Call
            FaultTreeIllustrationPoint illustrationPoint = FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPointCombinationTypeOr(beta);

            // Assert
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(illustrationPoint);

            Assert.AreEqual("Illustration point", illustrationPoint.Name);
            Assert.AreEqual(beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(CombinationType.Or, illustrationPoint.CombinationType);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
        }
    }
}