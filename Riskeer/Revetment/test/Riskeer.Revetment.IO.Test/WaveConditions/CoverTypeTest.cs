﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using NUnit.Framework;
using Riskeer.Revetment.IO.WaveConditions;

namespace Riskeer.Revetment.IO.Test.WaveConditions
{
    [TestFixture]
    public class CoverTypeTest
    {
        [Test]
        public void Name_StoneCoverBlocks_CorrectName()
        {
            // Assert
            Assert.AreEqual("Steen (blokken)", CoverType.StoneCoverBlocks.Name);
        }

        [Test]
        public void Name_StoneCoverColumns_CorrectName()
        {
            // Assert
            Assert.AreEqual("Steen (zuilen)", CoverType.StoneCoverColumns.Name);
        }

        [Test]
        public void Name_Asphalt_CorrectName()
        {
            // Assert
            Assert.AreEqual("Asfalt", CoverType.Asphalt.Name);
        }

        [Test]
        public void Name_GrassWaveRunUp_CorrectName()
        {
            // Assert
            Assert.AreEqual("Gras (golfoploop)", CoverType.GrassWaveRunUp.Name);
        }

        [Test]
        public void Name_GrassWaveImpact_CorrectName()
        {
            // Assert
            Assert.AreEqual("Gras (golfklap)", CoverType.GrassWaveImpact.Name);
        }

        [Test]
        public void Name_GrassWaveImpactWithWaveDirection_CorrectName()
        {
            // Assert
            Assert.AreEqual("Gras (golfklap met golfrichting)", CoverType.GrassWaveImpactWithWaveDirection.Name);
        }
    }
}