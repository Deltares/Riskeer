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

using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class GeneralPipingInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var inputParameters = new GeneralPipingInput();

            // Assert
            Assert.AreEqual(1.0, inputParameters.UpliftModelFactor);
            Assert.AreEqual(1.0, inputParameters.SellmeijerModelFactor);

            Assert.AreEqual(9.81, inputParameters.WaterVolumetricWeight);

            Assert.AreEqual(0.3, inputParameters.CriticalHeaveGradient);

            Assert.AreEqual(16.2, inputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(0.25, inputParameters.WhitesDragCoefficient);
            Assert.AreEqual(37, inputParameters.BeddingAngle);
            Assert.AreEqual(1.33e-6, inputParameters.WaterKinematicViscosity);
            Assert.AreEqual(9.81, inputParameters.Gravity);
            Assert.AreEqual(2.08e-4, inputParameters.MeanDiameter70);
            Assert.AreEqual(0.3, inputParameters.SellmeijerReductionFactor);
        }
    }
}