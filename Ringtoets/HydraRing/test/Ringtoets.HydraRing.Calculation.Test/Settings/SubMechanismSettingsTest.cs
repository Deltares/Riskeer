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
using Ringtoets.HydraRing.Calculation.Settings;

namespace Ringtoets.HydraRing.Calculation.Test.Settings
{
    [TestFixture]
    public class SubMechanismSettingsTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var subMechanismSettings = new SubMechanismSettings(1, 2, 3, 4.4, 5.5, 6.6, 7.7, 8, 9, 10, 11.11, 12.12, 13.13, 14);

            // Assert
            Assert.AreEqual(1, subMechanismSettings.CalculationTechniqueId);
            Assert.AreEqual(2, subMechanismSettings.FormStartMethod);
            Assert.AreEqual(3, subMechanismSettings.FormNumberOfIterations);
            Assert.AreEqual(4.4, subMechanismSettings.FormRelaxationFactor);
            Assert.AreEqual(5.5, subMechanismSettings.FormEpsBeta);
            Assert.AreEqual(6.6, subMechanismSettings.FormEpsHoh);
            Assert.AreEqual(7.7, subMechanismSettings.FormEpsZFunc);
            Assert.AreEqual(8, subMechanismSettings.DsStartMethod);
            Assert.AreEqual(9, subMechanismSettings.DsMinNumberOfIterations);
            Assert.AreEqual(10, subMechanismSettings.DsMaxNumberOfIterations);
            Assert.AreEqual(11.11, subMechanismSettings.DsVarCoefficient);
            Assert.AreEqual(12.12, subMechanismSettings.NiUMin);
            Assert.AreEqual(13.13, subMechanismSettings.NiUMax);
            Assert.AreEqual(14, subMechanismSettings.NiNumberSteps);
        }
    }
}