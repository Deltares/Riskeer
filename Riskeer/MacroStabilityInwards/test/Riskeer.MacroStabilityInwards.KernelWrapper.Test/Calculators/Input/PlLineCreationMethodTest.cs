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
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Calculators.Input
{
    [TestFixture]
    public class PlLineCreationMethodTest
    {
        [Test]
        public void Values_ExpectedValues()
        {
            // Assert
            Assert.AreEqual(7, Enum.GetValues(typeof(PlLineCreationMethod)).Length);
            Assert.AreEqual(1, (int) PlLineCreationMethod.ExpertKnowledgeRrd);
            Assert.AreEqual(2, (int) PlLineCreationMethod.ExpertKnowledgeLinearInDike);
            Assert.AreEqual(3, (int) PlLineCreationMethod.RingtoetsWti2017);
            Assert.AreEqual(4, (int) PlLineCreationMethod.DupuitStatic);
            Assert.AreEqual(5, (int) PlLineCreationMethod.DupuitDynamic);
            Assert.AreEqual(6, (int) PlLineCreationMethod.Sensors);
            Assert.AreEqual(7, (int) PlLineCreationMethod.None);
        }
    }
}