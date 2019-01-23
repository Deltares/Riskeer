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
using NUnit.Framework;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Storage.Core.TestUtil.IllustrationPoints;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    internal class StructuresOutputCreateExtensionsTest
    {
        [Test]
        public void Constructor_StructuresOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((StructuresOutput) null).Create<TestStructureOutputEntity>();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("structuresOutput", exception.ParamName);
        }

        [Test]
        public void Create_CalculationWithOutput_ReturnEntityWithOutput()
        {
            // Setup
            var random = new Random(567);
            var output = new StructuresOutput(
                random.NextDouble(), null);

            // Call
            var entity = output.Create<TestStructureOutputEntity>();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(output.Reliability, entity.Reliability);
            Assert.IsNull(entity.GeneralResultFaultTreeIllustrationPointEntity);
        }

        [Test]
        public void Create_CalculationWithNaNOutput_ReturnEntityWithNullOutput()
        {
            // Setup
            var output = new StructuresOutput(
                double.NaN, null);

            // Call
            var entity = output.Create<TestStructureOutputEntity>();

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.Reliability);
            Assert.IsNull(entity.GeneralResultFaultTreeIllustrationPointEntity);
        }

        [Test]
        public void Create_CalculationWithOutputAndGeneralResult_ReturnEntityWithOutputAndGeneralResult()
        {
            // Setup
            var random = new Random(567);
            var output = new StructuresOutput(random.NextDouble(),
                                              new TestGeneralResultFaultTreeIllustrationPoint());

            // Call
            var entity = output.Create<TestStructureOutputEntity>();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(output.Reliability, entity.Reliability);
            GeneralResultEntityTestHelper.AssertGeneralResultPropertyValues(output.GeneralResult, entity.GeneralResultFaultTreeIllustrationPointEntity);
        }

        private class TestStructureOutputEntity : IStructuresOutputEntity,
                                                  IHasGeneralResultFaultTreeIllustrationPointEntity
        {
            public GeneralResultFaultTreeIllustrationPointEntity GeneralResultFaultTreeIllustrationPointEntity { get; set; }
            public double? Reliability { get; set; }
        }
    }
}