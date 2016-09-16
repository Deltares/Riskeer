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

using System;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Create.Piping
{
    [TestFixture]
    public class PipingOutputCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var pipingOutput = new PipingOutput(1.1, 2.2, 3.3, 4.4, 5.5, 6.6);

            // Call
            TestDelegate call = () => pipingOutput.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_AllOutputValuesSet_ReturnEntity()
        {
            // Setup
            var pipingOutput = new PipingOutput(1.1, 2.2, 3.3, 4.4, 5.5, 6.6);

            var registry = new PersistenceRegistry();

            // Call
            PipingCalculationOutputEntity entity = pipingOutput.Create(registry);

            // Assert
            Assert.AreEqual(pipingOutput.HeaveFactorOfSafety, entity.HeaveFactorOfSafety);
            Assert.AreEqual(pipingOutput.HeaveZValue, entity.HeaveZValue);
            Assert.AreEqual(pipingOutput.SellmeijerFactorOfSafety, entity.SellmeijerFactorOfSafety);
            Assert.AreEqual(pipingOutput.SellmeijerZValue, entity.SellmeijerZValue);
            Assert.AreEqual(pipingOutput.UpliftFactorOfSafety, entity.UpliftFactorOfSafety);
            Assert.AreEqual(pipingOutput.UpliftZValue, entity.UpliftZValue);

            Assert.AreEqual(0, entity.PipingCalculationOutputEntityId);
            Assert.AreEqual(0, entity.PipingCalculationEntityId);
        }

        [Test]
        public void Create_AllOutputValuesNaN_ReturnEntityWithNullValues()
        {
            // Setup
            var pipingOutput = new PipingOutput(double.NaN, double.NaN, double.NaN,
                                                double.NaN, double.NaN, double.NaN);

            var registry = new PersistenceRegistry();

            // Call
            PipingCalculationOutputEntity entity = pipingOutput.Create(registry);

            // Assert
            Assert.IsNull(entity.HeaveFactorOfSafety);
            Assert.IsNull(entity.HeaveZValue);
            Assert.IsNull(entity.SellmeijerFactorOfSafety);
            Assert.IsNull(entity.SellmeijerZValue);
            Assert.IsNull(entity.UpliftFactorOfSafety);
            Assert.IsNull(entity.UpliftZValue);

            Assert.AreEqual(0, entity.PipingCalculationOutputEntityId);
            Assert.AreEqual(0, entity.PipingCalculationEntityId);
        }
    }
}