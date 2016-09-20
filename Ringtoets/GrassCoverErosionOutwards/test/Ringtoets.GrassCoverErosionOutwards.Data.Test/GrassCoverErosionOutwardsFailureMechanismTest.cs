﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.AreEqual("Dijken en dammen - Grasbekleding erosie buitentalud", failureMechanism.Name);
            Assert.AreEqual("GEBU", failureMechanism.Code);
            CollectionAssert.IsEmpty(failureMechanism.Sections);

            Assert.AreEqual("Berekeningen", failureMechanism.WaveConditionsCalculationGroup.Name);
            Assert.IsFalse(failureMechanism.WaveConditionsCalculationGroup.IsNameEditable);
            CollectionAssert.IsEmpty(failureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.HydraulicBoundaryLocations);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);

            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
            Assert.IsInstanceOf<IList<ForeshoreProfile>>(failureMechanism.ForeshoreProfiles);
            Assert.IsInstanceOf<IObservable>(failureMechanism.ForeshoreProfiles);
        }

        [Test]
        public void AddSection_WithSection_AddedGrassCoverErosionOutwardsFailureMechanismSectionResult()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            failureMechanism.AddSection(new FailureMechanismSection(string.Empty, new[]
            {
                new Point2D(2, 1)
            }));

            // Assert
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.IsInstanceOf<GrassCoverErosionOutwardsFailureMechanismSectionResult>(failureMechanism.SectionResults.ElementAt(0));
        }

        [Test]
        public void CleanAllSections_WithSection_RemoveSectionResults()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection(string.Empty, new[]
            {
                new Point2D(2, 1)
            }));

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
        }

        [Test]
        public void Calculations_MultipleChildrenAdded_ReturnGrassCoverErosionOutwardsWaveConditionsCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                WaveConditionsCalculationGroup =
                {
                    Children =
                    {
                        new CalculationGroup(),
                        new GrassCoverErosionOutwardsWaveConditionsCalculation(),
                        mocks.Stub<ICalculation>(),
                        new GrassCoverErosionOutwardsWaveConditionsCalculation()
                    }
                }
            };

            mocks.ReplayAll();

            // Call
            var calculations = failureMechanism.Calculations.ToList();

            // Assert
            Assert.AreEqual(2, calculations.Count);
            Assert.IsTrue(calculations.All(c => c is GrassCoverErosionOutwardsWaveConditionsCalculation));
            mocks.VerifyAll();
        }
    }
}