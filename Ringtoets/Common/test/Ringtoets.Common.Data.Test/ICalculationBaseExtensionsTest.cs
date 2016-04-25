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
using Core.Common.Base;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Common.Data.Test
{
    [TestFixture]
    public class ICalculationBaseExtensionsTest
    {
        [Test]
        public void GetPipingCalculations_FromPipingCalculation_ReturnThatCalculationInstance()
        {
            // Setup
            ICalculationBase calculationItem = new TestCalculationItem();

            // Call
            IEnumerable<ICalculationBase> result = calculationItem.GetCalculations();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                calculationItem
            }, result);
        }

        [Test]
        public void GetPipingCalculations_FromCalculationGroupWithoutChildren_ReturnEmpty()
        {
            // Setup
            ICalculationBase groupWithoutChildren = new CalculationGroup();

            // Call
            IEnumerable<ICalculationScenario> result = groupWithoutChildren.GetCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingCalculations_FromCalculationGroupWithEmptyGroups_ReturnEmpty()
        {
            // Setup
            var rootGroup = new CalculationGroup();
            rootGroup.Children.Add(new CalculationGroup());
            rootGroup.Children.Add(new CalculationGroup());
            rootGroup.Children.Add(new CalculationGroup());

            ICalculationBase groupsWithoutChildren = rootGroup;

            // Call
            IEnumerable<ICalculationScenario> result = groupsWithoutChildren.GetCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingCalculations_FromCalculationGroupWithGroupsAndCalculations_ReturnAllCalculationsRecursiveslyInAnyOrder()
        {
            // Setup
            var calculation1 = new TestCalculationItem();
            var calculation2 = new TestCalculationItem();
            var calculation3 = new TestCalculationItem();
            var calculation4 = new TestCalculationItem();

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
            IEnumerable<ICalculationScenario> result = groupWithoutChildren.GetCalculations();

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

    public class TestCalculationItem : ICalculationScenario
    {
        public TestCalculationItem()
        {

        }

        public void Attach(IObserver observer)
        {
            throw new System.NotImplementedException();
        }

        public void Detach(IObserver observer)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyObservers()
        {
            throw new System.NotImplementedException();
        }

        public string Name { get; set; }
        public string Comments { get; set; }
        public bool HasOutput { get; private set; }
        public void ClearOutput()
        {
            throw new System.NotImplementedException();
        }

        public void ClearHydraulicBoundaryLocation()
        {
            throw new System.NotImplementedException();
        }

        public ICalculationInput Input { get; private set; }
        public bool IsRelevant { get; set; }
        public RoundedDouble Contribution { get; set; }
        public RoundedDouble? Probability { get; private set; }
    }
}