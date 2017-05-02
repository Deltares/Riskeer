// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Revetment.TestUtil.Test
{
    [TestFixture]
    public class TestWaveConditionsCalculationTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var calculation = new TestWaveConditionsCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsNull(calculation.Output);
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsNotNull(calculation.InputParameters.ForeshoreProfile);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0),
                new Point2D(2.0, 2.0)
            }, calculation.InputParameters.ForeshoreGeometry);
        }
    }
}