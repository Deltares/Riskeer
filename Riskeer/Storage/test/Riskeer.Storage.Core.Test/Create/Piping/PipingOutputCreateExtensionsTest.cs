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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Storage.Core.Create.Piping;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.Piping
{
    [TestFixture]
    public class PipingOutputCreateExtensionsTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((PipingOutput) null).Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Create_AllOutputValuesSet_ReturnEntity()
        {
            // Setup
            var pipingOutput = new PipingOutput(new PipingOutput.ConstructionProperties
            {
                UpliftFactorOfSafety = 2.2,
                HeaveFactorOfSafety = 4.4,
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
            Assert.AreEqual(pipingOutput.SellmeijerFactorOfSafety, entity.SellmeijerFactorOfSafety);
            Assert.AreEqual(pipingOutput.UpliftFactorOfSafety, entity.UpliftFactorOfSafety);
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
            Assert.IsNull(entity.SellmeijerFactorOfSafety);
            Assert.IsNull(entity.UpliftFactorOfSafety);
            Assert.IsNull(entity.UpliftEffectiveStress);
            Assert.IsNull(entity.HeaveGradient);
            Assert.IsNull(entity.SellmeijerCreepCoefficient);
            Assert.IsNull(entity.SellmeijerCriticalFall);
            Assert.IsNull(entity.SellmeijerReducedFall);
        }
    }
}