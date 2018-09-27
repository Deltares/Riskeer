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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read;

namespace Ringtoets.Storage.Core.Test.Read
{
    [TestFixture]
    public class StructureCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_InputToUpdateNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var entity = mocks.Stub<IStructuresCalculationEntity>();
            mocks.ReplayAll();

            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => entity.Read<StructureBase>(null, collector);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("inputToUpdate", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Read_ReadConversionCollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var entity = mocks.Stub<IStructuresCalculationEntity>();
            mocks.ReplayAll();

            var inputToUpdate = new SimpleStructuresInput();

            // Call
            TestDelegate call = () => entity.Read(inputToUpdate, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Read_ValidEntity_InputObjectUpdated()
        {
            // Setup
            var random = new Random(78);

            var mocks = new MockRepository();
            var entity = mocks.Stub<IStructuresCalculationEntity>();
            entity.StructureNormalOrientation = random.NextDouble(0, 360);
            entity.AllowedLevelIncreaseStorageMean = random.NextDouble(1e-6, 9999.9999);
            entity.AllowedLevelIncreaseStorageStandardDeviation = random.NextDouble(1e-6, 9999.9999);
            entity.FlowWidthAtBottomProtectionMean = random.NextDouble(1e-6, 9999.9999);
            entity.FlowWidthAtBottomProtectionStandardDeviation = random.NextDouble(1e-6, 9999.9999);
            entity.CriticalOvertoppingDischargeMean = random.NextDouble(1e-6, 9999.9999);
            entity.CriticalOvertoppingDischargeCoefficientOfVariation = random.NextDouble(1e-6, 9999.9999);
            entity.FailureProbabilityStructureWithErosion = random.NextDouble();
            entity.WidthFlowAperturesMean = random.NextDouble(1e-6, 9999.9999);
            entity.WidthFlowAperturesStandardDeviation = random.NextDouble(1e-6, 9999.9999);
            entity.StormDurationMean = random.NextDouble(1e-6, 9999.9999);
            entity.UseForeshore = Convert.ToByte(random.NextBoolean());
            entity.UseBreakWater = Convert.ToByte(random.NextBoolean());
            entity.ShouldIllustrationPointsBeCalculated = Convert.ToByte(random.NextBoolean());
            mocks.ReplayAll();

            var inputToUpdate = new SimpleStructuresInput();
            var collector = new ReadConversionCollector();

            // Call
            entity.Read(inputToUpdate, collector);

            // Assert
            AssertBoolean(entity.UseForeshore, inputToUpdate.UseForeshore);
            AssertBoolean(entity.UseBreakWater, inputToUpdate.UseBreakWater);
            AssertBoolean(entity.ShouldIllustrationPointsBeCalculated, inputToUpdate.ShouldIllustrationPointsBeCalculated);

            AssertRoundedDouble(entity.StructureNormalOrientation, inputToUpdate.StructureNormalOrientation);
            AssertRoundedDouble(entity.AllowedLevelIncreaseStorageMean, inputToUpdate.AllowedLevelIncreaseStorage.Mean);
            AssertRoundedDouble(entity.AllowedLevelIncreaseStorageStandardDeviation, inputToUpdate.AllowedLevelIncreaseStorage.StandardDeviation);
            AssertRoundedDouble(entity.FlowWidthAtBottomProtectionMean, inputToUpdate.FlowWidthAtBottomProtection.Mean);
            AssertRoundedDouble(entity.FlowWidthAtBottomProtectionStandardDeviation, inputToUpdate.FlowWidthAtBottomProtection.StandardDeviation);
            AssertRoundedDouble(entity.CriticalOvertoppingDischargeMean, inputToUpdate.CriticalOvertoppingDischarge.Mean);
            AssertRoundedDouble(entity.CriticalOvertoppingDischargeCoefficientOfVariation, inputToUpdate.CriticalOvertoppingDischarge.CoefficientOfVariation);
            Assert.AreEqual(entity.FailureProbabilityStructureWithErosion, inputToUpdate.FailureProbabilityStructureWithErosion);
            AssertRoundedDouble(entity.WidthFlowAperturesMean, inputToUpdate.WidthFlowApertures.Mean);
            AssertRoundedDouble(entity.WidthFlowAperturesStandardDeviation, inputToUpdate.WidthFlowApertures.StandardDeviation);
            AssertRoundedDouble(entity.StormDurationMean, inputToUpdate.StormDuration.Mean);

            CollectionAssert.IsEmpty(inputToUpdate.ForeshoreGeometry);
            Assert.IsNull(inputToUpdate.ForeshoreProfile);
            Assert.IsNull(inputToUpdate.HydraulicBoundaryLocation);
            Assert.IsNull(inputToUpdate.Structure);
            mocks.VerifyAll();
        }

        [Test]
        public void Read_EntityWithParametersNull_InputObjectUpdatedWithNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var entity = mocks.Stub<IStructuresCalculationEntity>();
            entity.StructureNormalOrientation = null;
            entity.AllowedLevelIncreaseStorageMean = null;
            entity.AllowedLevelIncreaseStorageStandardDeviation = null;
            entity.FlowWidthAtBottomProtectionMean = null;
            entity.FlowWidthAtBottomProtectionStandardDeviation = null;
            entity.CriticalOvertoppingDischargeMean = null;
            entity.CriticalOvertoppingDischargeCoefficientOfVariation = null;
            entity.WidthFlowAperturesMean = null;
            entity.WidthFlowAperturesStandardDeviation = null;
            entity.StormDurationMean = null;
            mocks.ReplayAll();

            var inputToUpdate = new SimpleStructuresInput();
            var collector = new ReadConversionCollector();

            // Call
            entity.Read(inputToUpdate, collector);

            // Assert
            Assert.IsNaN(inputToUpdate.StructureNormalOrientation);
            Assert.IsNaN(inputToUpdate.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNaN(inputToUpdate.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNaN(inputToUpdate.FlowWidthAtBottomProtection.Mean);
            Assert.IsNaN(inputToUpdate.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNaN(inputToUpdate.CriticalOvertoppingDischarge.Mean);
            Assert.IsNaN(inputToUpdate.CriticalOvertoppingDischarge.CoefficientOfVariation);
            Assert.IsNaN(inputToUpdate.WidthFlowApertures.Mean);
            Assert.IsNaN(inputToUpdate.WidthFlowApertures.StandardDeviation);
            Assert.IsNaN(inputToUpdate.StormDuration.Mean);
            mocks.VerifyAll();
        }

        [Test]
        public void Read_EntityWithForeshoreProfileEntity_InputObjectUpdatedWithForeshoreProfile()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var foreshoreEntity = new ForeshoreProfileEntity();

            var mocks = new MockRepository();
            var entity = mocks.Stub<IStructuresCalculationEntity>();
            entity.ForeshoreProfileEntity = foreshoreEntity;
            mocks.ReplayAll();

            var inputToUpdate = new SimpleStructuresInput();
            var collector = new ReadConversionCollector();
            collector.Read(foreshoreEntity, foreshoreProfile);

            // Call
            entity.Read(inputToUpdate, collector);

            // Assert
            Assert.AreSame(foreshoreProfile, inputToUpdate.ForeshoreProfile);
            mocks.VerifyAll();
        }

        [Test]
        public void Read_EntityWithHydraulicLocationEntity_InputObjectUpdatedWithHydraulicBoundaryLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "A", 0, 0);
            var hydraulicLocationEntity = new HydraulicLocationEntity();

            var mocks = new MockRepository();
            var entity = mocks.Stub<IStructuresCalculationEntity>();
            entity.HydraulicLocationEntity = hydraulicLocationEntity;
            mocks.ReplayAll();

            var inputToUpdate = new SimpleStructuresInput();
            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            entity.Read(inputToUpdate, collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, inputToUpdate.HydraulicBoundaryLocation);
            mocks.VerifyAll();
        }

        private static void AssertBoolean(byte expectedByte, bool actual)
        {
            Assert.AreEqual(Convert.ToBoolean(expectedByte), actual);
        }

        private static void AssertRoundedDouble(double? entityValue, RoundedDouble roundedDouble)
        {
            Assert.AreEqual((RoundedDouble) entityValue.ToNullAsNaN(), roundedDouble, roundedDouble.GetAccuracy());
        }

        private class SimpleStructuresInput : StructuresInputBase<StructureBase>
        {
            public override bool IsStructureInputSynchronized
            {
                get
                {
                    return false;
                }
            }

            public override void SynchronizeStructureInput() {}
        }
    }
}