﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.StabilityStoneCover
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsOutputCreateExtensionsTest
    {
        [Test]
        [TestCase(StabilityStoneCoverWaveConditionsOutputType.Blocks)]
        [TestCase(StabilityStoneCoverWaveConditionsOutputType.Columns)]
        public void Create_AllOutputValuesSet_ReturnEntity(StabilityStoneCoverWaveConditionsOutputType outputType)
        {
            // Setup
            var output = new TestWaveConditionsOutput();
            const int order = 22;

            // Call
            StabilityStoneCoverWaveConditionsOutputEntity entity = output.CreateStabilityStoneCoverWaveConditionsOutputEntity(outputType, order);

            // Assert
            Assert.AreEqual(order, entity.Order);
            Assert.AreEqual(output.WaterLevel, entity.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(output.WaveHeight, entity.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(output.WavePeakPeriod, entity.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(output.WaveAngle, entity.WaveAngle, output.WaveAngle.GetAccuracy());
            Assert.AreEqual(Convert.ToByte(outputType), entity.OutputType);
            Assert.AreEqual(output.WaveDirection, entity.WaveDirection, output.WaveDirection.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(output.CalculationConvergence, (CalculationConvergence) entity.CalculationConvergence);

            Assert.AreEqual(0, entity.StabilityStoneCoverWaveConditionsOutputEntityId);
            Assert.AreEqual(0, entity.StabilityStoneCoverWaveConditionsCalculationEntityId);
        }

        [Test]
        [TestCase(StabilityStoneCoverWaveConditionsOutputType.Blocks)]
        [TestCase(StabilityStoneCoverWaveConditionsOutputType.Columns)]
        public void Create_AllOutputValuesNaN_ReturnEntityWithNullValues(StabilityStoneCoverWaveConditionsOutputType outputType)
        {
            // Setup
            var output = new WaveConditionsOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                                                  double.NaN, double.NaN, double.NaN, CalculationConvergence.NotCalculated);

            // Call
            StabilityStoneCoverWaveConditionsOutputEntity entity = output.CreateStabilityStoneCoverWaveConditionsOutputEntity(outputType, 22);

            // Assert
            Assert.IsNull(entity.WaterLevel);
            Assert.IsNull(entity.WaveHeight);
            Assert.IsNull(entity.WavePeakPeriod);
            Assert.IsNull(entity.WaveAngle);
            Assert.AreEqual(Convert.ToByte(outputType), entity.OutputType);
            Assert.IsNull(entity.WaveDirection);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);

            Assert.AreEqual(0, entity.StabilityStoneCoverWaveConditionsOutputEntityId);
            Assert.AreEqual(0, entity.StabilityStoneCoverWaveConditionsCalculationEntityId);
        }
    }
}