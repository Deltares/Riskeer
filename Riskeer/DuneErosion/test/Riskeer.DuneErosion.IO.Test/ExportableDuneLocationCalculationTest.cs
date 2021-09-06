// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;

namespace Riskeer.DuneErosion.IO.Test
{
    [TestFixture]
    public class ExportableDuneLocationCalculationTest
    {
        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ExportableDuneLocationCalculation(null, new Random(39).NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidParameters_ExpectedProperties()
        {
            // Setup
            var calculation = new DuneLocationCalculation(new TestDuneLocation());
            double targetProbability = new Random(39).NextDouble();

            // Call
            var exportableCalculation = new ExportableDuneLocationCalculation(calculation, targetProbability);

            // Assert
            Assert.AreSame(calculation, exportableCalculation.Calculation);
            Assert.AreEqual(targetProbability, exportableCalculation.TargetProbability);
        }
    }
}