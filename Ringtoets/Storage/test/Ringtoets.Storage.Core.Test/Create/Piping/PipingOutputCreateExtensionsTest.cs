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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;
using Riskeer.Storage.Core.Create.Piping;
using Riskeer.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.Piping
{
    [TestFixture]
    public class PipingOutputCreateExtensionsTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((PipingOutput) null).Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Create_AllOutputValuesSet_ReturnEntity()
        {
            // Setup
            var pipingOutput = new PipingOutput(new PipingOutput.ConstructionProperties
            {
                UpliftZValue = 1.1,
                UpliftFactorOfSafety = 2.2,
                HeaveZValue = 3.3,
                HeaveFactorOfSafety = 4.4,
                SellmeijerZValue = 5.5,
                SellmeijerFactorOfSafety = 6.6,
                UpliftEffectiveStress = 7.7,
                HeaveGradient = 8.8,
                SellmeijerCreepCoefficient = 9.9,
                SellmeijerCriticalFall = 10.10,
                SellmeijerReducedFall = 11.11
            });

            // Call
            PipingCalculationOutputEntity entity = pipingOutput.Create();

            // Assert
            Assert.AreEqual(pipingOutput.HeaveFactorOfSafety, entity.HeaveFactorOfSafety);
            Assert.AreEqual(pipingOutput.HeaveZValue, entity.HeaveZValue);
            Assert.AreEqual(pipingOutput.SellmeijerFactorOfSafety, entity.SellmeijerFactorOfSafety);
            Assert.AreEqual(pipingOutput.SellmeijerZValue, entity.SellmeijerZValue);
            Assert.AreEqual(pipingOutput.UpliftFactorOfSafety, entity.UpliftFactorOfSafety);
            Assert.AreEqual(pipingOutput.UpliftZValue, entity.UpliftZValue);
            Assert.AreEqual(pipingOutput.UpliftEffectiveStress, entity.UpliftEffectiveStress, pipingOutput.UpliftEffectiveStress.GetAccuracy());
            Assert.AreEqual(pipingOutput.HeaveGradient, entity.HeaveGradient, pipingOutput.HeaveGradient.GetAccuracy());
            Assert.AreEqual(pipingOutput.SellmeijerCreepCoefficient, entity.SellmeijerCreepCoefficient, pipingOutput.SellmeijerCreepCoefficient.GetAccuracy());
            Assert.AreEqual(pipingOutput.SellmeijerCriticalFall, entity.SellmeijerCriticalFall, pipingOutput.SellmeijerCriticalFall.GetAccuracy());
            Assert.AreEqual(pipingOutput.SellmeijerReducedFall, entity.SellmeijerReducedFall, pipingOutput.SellmeijerReducedFall.GetAccuracy());
        }

        [Test]
        public void Create_AllOutputValuesNaN_ReturnEntityWithNullValues()
        {
            // Setup
            var pipingOutput = new PipingOutput(new PipingOutput.ConstructionProperties());

            // Call
            PipingCalculationOutputEntity entity = pipingOutput.Create();

            // Assert
            Assert.IsNull(entity.HeaveFactorOfSafety);
            Assert.IsNull(entity.HeaveZValue);
            Assert.IsNull(entity.SellmeijerFactorOfSafety);
            Assert.IsNull(entity.SellmeijerZValue);
            Assert.IsNull(entity.UpliftFactorOfSafety);
            Assert.IsNull(entity.UpliftZValue);
            Assert.IsNull(entity.UpliftEffectiveStress);
            Assert.IsNull(entity.HeaveGradient);
            Assert.IsNull(entity.SellmeijerCreepCoefficient);
            Assert.IsNull(entity.SellmeijerCriticalFall);
            Assert.IsNull(entity.SellmeijerReducedFall);
        }
    }
}