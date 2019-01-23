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
using Ringtoets.Piping.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.Piping;

namespace Ringtoets.Storage.Core.Test.Read.Piping
{
    [TestFixture]
    public class PipingCalculationOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((PipingCalculationOutputEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidEntity_ReturnPipingOutput()
        {
            // Setup
            var entity = new PipingCalculationOutputEntity
            {
                HeaveFactorOfSafety = 9.8,
                HeaveZValue = 7.6,
                UpliftZValue = 5.4,
                UpliftFactorOfSafety = 3.2,
                SellmeijerZValue = 1.9,
                SellmeijerFactorOfSafety = 8.7,
                UpliftEffectiveStress = 15.2,
                HeaveGradient = 12.2,
                SellmeijerCreepCoefficient = 1.4,
                SellmeijerCriticalFall = 6.2,
                SellmeijerReducedFall = 8.1
            };

            // Call
            PipingOutput output = entity.Read();

            // Assert
            Assert.AreEqual(entity.HeaveFactorOfSafety, output.HeaveFactorOfSafety);
            Assert.AreEqual(entity.HeaveZValue, output.HeaveZValue);
            Assert.AreEqual(entity.SellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety);
            Assert.AreEqual(entity.SellmeijerZValue, output.SellmeijerZValue);
            Assert.AreEqual(entity.UpliftZValue, output.UpliftZValue);
            Assert.AreEqual(entity.UpliftFactorOfSafety, output.UpliftFactorOfSafety);
            Assert.AreEqual(entity.UpliftEffectiveStress, output.UpliftEffectiveStress.Value);
            Assert.AreEqual(entity.HeaveGradient, output.HeaveGradient.Value);
            Assert.AreEqual(entity.SellmeijerCreepCoefficient, output.SellmeijerCreepCoefficient.Value);
            Assert.AreEqual(entity.SellmeijerCriticalFall, output.SellmeijerCriticalFall.Value);
            Assert.AreEqual(entity.SellmeijerReducedFall, output.SellmeijerReducedFall.Value);
        }

        [Test]
        public void Read_ValidEntityWithNullParameterValues_ReturnPipingOutput()
        {
            // Setup
            var entity = new PipingCalculationOutputEntity
            {
                HeaveFactorOfSafety = null,
                HeaveZValue = null,
                UpliftZValue = null,
                UpliftFactorOfSafety = null,
                SellmeijerZValue = null,
                SellmeijerFactorOfSafety = null,
                UpliftEffectiveStress = null,
                HeaveGradient = null,
                SellmeijerCreepCoefficient = null,
                SellmeijerCriticalFall = null,
                SellmeijerReducedFall = null
            };

            // Call
            PipingOutput output = entity.Read();

            // Assert
            Assert.IsNaN(output.HeaveFactorOfSafety);
            Assert.IsNaN(output.HeaveZValue);
            Assert.IsNaN(output.SellmeijerFactorOfSafety);
            Assert.IsNaN(output.SellmeijerZValue);
            Assert.IsNaN(output.UpliftZValue);
            Assert.IsNaN(output.UpliftFactorOfSafety);
            Assert.IsNaN(output.UpliftEffectiveStress);
            Assert.IsNaN(output.HeaveGradient);
            Assert.IsNaN(output.SellmeijerCreepCoefficient);
            Assert.IsNaN(output.SellmeijerCriticalFall);
            Assert.IsNaN(output.SellmeijerReducedFall);
        }
    }
}