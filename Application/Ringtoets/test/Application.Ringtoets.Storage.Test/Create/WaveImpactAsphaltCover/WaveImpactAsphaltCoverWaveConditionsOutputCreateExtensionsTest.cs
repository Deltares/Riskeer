﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create.WaveImpactAsphaltCover
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsOutputCreateExtensionsTest
    {
        [Test]
        public void Create_AllOutputValuesSet_ReturnEntity()
        {
            // Setup
            var output = new TestWaveConditionsOutput();
            const int order = 22;

            // Call
            WaveImpactAsphaltCoverWaveConditionsOutputEntity entity = output.CreateWaveImpactAsphaltCoverWaveConditionsOutputEntity(order);

            // Assert
            Assert.AreEqual(order, entity.Order);
            Assert.AreEqual(output.WaterLevel, entity.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(output.WaveHeight, entity.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(output.WavePeakPeriod, entity.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(output.WaveAngle, entity.WaveAngle, output.WaveAngle.GetAccuracy());
            Assert.AreEqual(output.WaveDirection, entity.WaveDirection, output.WaveDirection.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(output.CalculationConvergence, (CalculationConvergence) entity.CalculationConvergence);

            Assert.IsNull(entity.WaveImpactAsphaltCoverWaveConditionsCalculationEntity);
        }

        [Test]
        public void Create_AllOutputValuesNaN_ReturnEntityWithNullValues()
        {
            // Setup
            var output = new WaveConditionsOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                                                  double.NaN, double.NaN, double.NaN, CalculationConvergence.NotCalculated);

            // Call
            WaveImpactAsphaltCoverWaveConditionsOutputEntity entity = output.CreateWaveImpactAsphaltCoverWaveConditionsOutputEntity(22);

            // Assert
            Assert.IsNull(entity.WaterLevel);
            Assert.IsNull(entity.WaveHeight);
            Assert.IsNull(entity.WavePeakPeriod);
            Assert.IsNull(entity.WaveAngle);
            Assert.IsNull(entity.WaveDirection);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);

            Assert.IsNull(entity.WaveImpactAsphaltCoverWaveConditionsCalculationEntity);
        }
    }
}