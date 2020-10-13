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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Forms.PropertyClasses;

namespace Riskeer.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    class ProbabilisticPipingInputContextPropertiesTest
    {
        private const int selectedHydraulicBoundaryLocationPropertyIndex = 1;
        private const int dampingFactorExitPropertyIndex = 2;
        private const int phreaticLevelExitPropertyIndex = 3;
        private const int piezometricHeadExitPropertyIndex = 4;

        private const int surfaceLinePropertyIndex = 5;
        private const int stochasticSoilModelPropertyIndex = 6;
        private const int stochasticSoilProfilePropertyIndex = 7;
        private const int entryPointLPropertyIndex = 8;
        private const int exitPointLPropertyIndex = 9;
        private const int seepageLengthPropertyIndex = 10;
        private const int thicknessCoverageLayerPropertyIndex = 11;
        private const int effectiveThicknessCoverageLayerPropertyIndex = 12;
        private const int thicknessAquiferLayerPropertyIndex = 13;
        private const int darcyPermeabilityPropertyIndex = 14;
        private const int diameter70PropertyIndex = 15;
        private const int saturatedVolumicWeightOfCoverageLayerPropertyIndex = 16;

        private const int sectionNamePropertyIndex = 17;
        private const int sectionLengthPropertyIndex = 18;

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new ProbabilisticPipingInputContextProperties(null, AssessmentSectionTestHelper.GetTestAssessmentLevel, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }
    }
}
