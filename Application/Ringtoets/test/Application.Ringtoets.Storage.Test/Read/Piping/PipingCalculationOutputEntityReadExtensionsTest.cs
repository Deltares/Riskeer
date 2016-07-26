// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.Piping;

using NUnit.Framework;

using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Read.Piping
{
    [TestFixture]
    public class PipingCalculationOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_ValidEntity_ReturnPipingOutput()
        {
            // Setup
            var entity = new PipingCalculationOutputEntity
            {
                PipingCalculationOutputEntityId = 123,
                HeaveFactorOfSafety = 9.8,
                HeaveZValue = 7.6,
                UpliftZValue = 5.4,
                UpliftFactorOfSafety = 3.2,
                SellmeijerZValue = 1.9,
                SellmeijerFactorOfSafety = 8.7
            };

            // Call
            PipingOutput output = entity.Read();

            // Assert
            Assert.AreEqual(entity.PipingCalculationOutputEntityId, output.StorageId);
            Assert.AreEqual(entity.HeaveFactorOfSafety, output.HeaveFactorOfSafety);
            Assert.AreEqual(entity.HeaveZValue, output.HeaveZValue);
            Assert.AreEqual(entity.SellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety);
            Assert.AreEqual(entity.SellmeijerZValue, output.SellmeijerZValue);
            Assert.AreEqual(entity.UpliftZValue, output.UpliftZValue);
            Assert.AreEqual(entity.UpliftFactorOfSafety, output.UpliftFactorOfSafety);
        }

        [Test]
        public void Read_ValidEntityWithNullParameterValues_ReturnPipingOutput()
        {
            // Setup
            var entity = new PipingCalculationOutputEntity
            {
                PipingCalculationOutputEntityId = 645,
                HeaveFactorOfSafety = null,
                HeaveZValue = null,
                UpliftZValue = null,
                UpliftFactorOfSafety = null,
                SellmeijerZValue = null,
                SellmeijerFactorOfSafety =  null
            };

            // Call
            PipingOutput output = entity.Read();

            // Assert
            Assert.AreEqual(entity.PipingCalculationOutputEntityId, output.StorageId);
            Assert.IsNaN(output.HeaveFactorOfSafety);
            Assert.IsNaN(output.HeaveZValue);
            Assert.IsNaN(output.SellmeijerFactorOfSafety);
            Assert.IsNaN(output.SellmeijerZValue);
            Assert.IsNaN(output.UpliftZValue);
            Assert.IsNaN(output.UpliftFactorOfSafety);
        }
    }
}