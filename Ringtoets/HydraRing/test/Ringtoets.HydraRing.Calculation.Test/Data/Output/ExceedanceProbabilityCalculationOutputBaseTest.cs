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
using Ringtoets.HydraRing.Calculation.Data.Output;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Output
{
    [TestFixture]
    public class ExceedanceProbabilityCalculationOutputBaseTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            int ringCombinMethod = 1;
            int presentationSectionId = 2;
            int mainMechanismId = 3;
            int mainMechanismCombinMethod = 4;
            int mechanismId = 5;
            int sectionId = 6;
            int layerId = 7;
            int alternativeId = 8;

            // Call
            ExceedanceProbabilityCalculationOutputBaseImplementation exceedanceProbabilityCalculationOutput =
                new ExceedanceProbabilityCalculationOutputBaseImplementation(ringCombinMethod, presentationSectionId,
                                                                             mainMechanismId, mainMechanismCombinMethod, mechanismId,
                                                                             sectionId, layerId, alternativeId);

            // Assert
            Assert.IsNotNull(exceedanceProbabilityCalculationOutput);
            Assert.AreEqual(ringCombinMethod, exceedanceProbabilityCalculationOutput.RingCombinMethod);
            Assert.AreEqual(presentationSectionId, exceedanceProbabilityCalculationOutput.PresentationSectionId);
            Assert.AreEqual(mainMechanismId, exceedanceProbabilityCalculationOutput.MainMechanismId);
            Assert.AreEqual(mainMechanismCombinMethod, exceedanceProbabilityCalculationOutput.MainMechanismCombinMethod);
            Assert.AreEqual(mechanismId, exceedanceProbabilityCalculationOutput.MechanismId);
            Assert.AreEqual(sectionId, exceedanceProbabilityCalculationOutput.SectionId);
            Assert.AreEqual(layerId, exceedanceProbabilityCalculationOutput.LayerId);
            Assert.AreEqual(alternativeId, exceedanceProbabilityCalculationOutput.AlternativeId);
        }
    }

    internal class ExceedanceProbabilityCalculationOutputBaseImplementation : ExceedanceProbabilityCalculationOutputBase
    {
        public ExceedanceProbabilityCalculationOutputBaseImplementation(int ringCombinMethod, int presentationSectionId, int mainMechanismId, int mainMechanismCombinMethod, int mechanismId, int sectionId, int layerId, int alternativeId)
            : base(ringCombinMethod, presentationSectionId, mainMechanismId, mainMechanismCombinMethod, mechanismId, sectionId, layerId, alternativeId) {}
    }
}