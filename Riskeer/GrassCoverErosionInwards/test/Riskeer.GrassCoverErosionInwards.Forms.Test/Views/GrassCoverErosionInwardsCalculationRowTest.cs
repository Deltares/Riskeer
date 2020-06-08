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
using Rhino.Mocks;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationRowTest
    {
        [Test]
        public void Constructor_WithoutCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new GrassCoverErosionInwardsCalculationRow(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("grassCoverErosionInwardsCalculationScenario", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutHandler_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionInwardsCalculationRow(new GrassCoverErosionInwardsCalculationScenario(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("handler", paramName);
        }

        [Test]
        public void Constructor_WithCalculation_PropertiesCorrectlySet()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();
            var grassCoverErosionInwardsCalculationScenario = new GrassCoverErosionInwardsCalculationScenario();

            // Call
            var grassCoverErosionInwardsCalculationRow = new GrassCoverErosionInwardsCalculationRow(grassCoverErosionInwardsCalculationScenario, handler);

            // Asserts
            Assert.AreSame(grassCoverErosionInwardsCalculationScenario, grassCoverErosionInwardsCalculationRow.GrassCoverErosionInwardsCalculationScenario);

            mocks.VerifyAll();
        }
    }
}