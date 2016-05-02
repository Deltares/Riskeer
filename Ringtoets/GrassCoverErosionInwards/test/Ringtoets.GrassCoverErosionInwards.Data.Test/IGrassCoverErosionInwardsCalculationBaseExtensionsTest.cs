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

using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class IGrassCoverErosionInwardsCalculationBaseExtensionsTest
    {
        [Test]
        public void GetGrassCoverErosionInwardsCalculations_FromGrassCoverErosionInwardsCalculation_ReturnThatCalculationInstance()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(new GeneralGrassCoverErosionInwardsInput());
            mockRepository.ReplayAll();

            // Call
            IEnumerable<GrassCoverErosionInwardsCalculation> result = calculation.GetGrassCoverErosionInwardsCalculations();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, result);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetGrassCoverErosionInwardsCalculations_FromCalculationGroupWithoutChildren_ReturnEmpty()
        {
            // Setup
            ICalculationBase groupWithoutChildren = new CalculationGroup();

            // Call
            IEnumerable<GrassCoverErosionInwardsCalculation> result = groupWithoutChildren.GetGrassCoverErosionInwardsCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetGrassCoverErosionInwardsCalculations_FromCalculationGroupWithEmptyGroups_ReturnEmpty()
        {
            // Setup
            var rootGroup = new CalculationGroup();
            rootGroup.Children.Add(new CalculationGroup());
            rootGroup.Children.Add(new CalculationGroup());
            rootGroup.Children.Add(new CalculationGroup());

            ICalculationBase groupsWithoutChildren = rootGroup;

            // Call
            IEnumerable<GrassCoverErosionInwardsCalculation> result = groupsWithoutChildren.GetGrassCoverErosionInwardsCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetGrassCoverErosionInwardsCalculations_FromCalculationGroupWithGroupsAndCalculations_ReturnAllCalculationsRecursiveslyInAnyOrder()
        {
            // Setup
            var mockRepository = new MockRepository();
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var calculation1 = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(generalInput);
            var calculation2 = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(generalInput);
            var calculation3 = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(generalInput);
            var calculation4 = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(generalInput);
            mockRepository.ReplayAll();

            var subsubGroup = new CalculationGroup();
            subsubGroup.Children.Add(calculation4);

            var subgroup1 = new CalculationGroup();
            subgroup1.Children.Add(calculation2);
            subgroup1.Children.Add(subsubGroup);

            var subgroup2 = new CalculationGroup();
            subgroup2.Children.Add(calculation3);

            var rootGroup = new CalculationGroup();
            rootGroup.Children.Add(subgroup1);
            rootGroup.Children.Add(calculation1);
            rootGroup.Children.Add(subgroup2);

            ICalculationBase groupWithoutChildren = rootGroup;

            // Call
            IEnumerable<GrassCoverErosionInwardsCalculation> result = groupWithoutChildren.GetGrassCoverErosionInwardsCalculations();

            // Assert
            var itemsThatShouldBeFound = new[]
            {
                calculation1,
                calculation2,
                calculation3,
                calculation4
            };
            CollectionAssert.AreEquivalent(itemsThatShouldBeFound, result);
        }
    }
}