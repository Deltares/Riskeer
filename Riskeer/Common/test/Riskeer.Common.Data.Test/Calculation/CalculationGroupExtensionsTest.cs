// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Common.Data.Test.Calculation
{
    [TestFixture]
    public class CalculationGroupExtensionsTest
    {
        [Test]
        public void GetCalculations_FromCalculationGroupWithoutChildren_ReturnEmpty()
        {
            // Setup
            var groupWithoutChildren = new CalculationGroup();

            // Call
            IEnumerable<ICalculation> result = groupWithoutChildren.GetCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetCalculations_FromCalculationGroupWithEmptyGroups_ReturnEmpty()
        {
            // Setup
            var rootGroup = new CalculationGroup();
            rootGroup.Children.Add(new CalculationGroup());
            rootGroup.Children.Add(new CalculationGroup());
            rootGroup.Children.Add(new CalculationGroup());

            // Call
            IEnumerable<ICalculation> result = rootGroup.GetCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetCalculations_FromCalculationGroupWithGroupsAndCalculations_ReturnAllCalculationsRecursiveslyInAnyOrder()
        {
            // Setup
            var calculation1 = Substitute.For<ICalculation>();
            var calculation2 = Substitute.For<ICalculation>();
            var calculation3 = Substitute.For<ICalculation>();
            var calculation4 = Substitute.For<ICalculation>();
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

            // Call
            IEnumerable<ICalculation> result = rootGroup.GetCalculations();

            // Assert
            ICalculation[] itemsThatShouldBeFound =
            {
                calculation1,
                calculation2,
                calculation3,
                calculation4
            };
            CollectionAssert.AreEquivalent(itemsThatShouldBeFound, result);
        }

        [Test]
        public void ClearCalculationOutput_ForCalculationGroupWithGroupsAndCalculations_OutputOfRelevantCalculationsIsClearedAndObserversAreNotified()
        {
            // Setup
            var calculation1 = Substitute.For<ICalculation>();
            var calculation2 = Substitute.For<ICalculation>();
            var calculation3 = Substitute.For<ICalculation>();
            var calculation4 = Substitute.For<ICalculation>();

            calculation1.HasOutput.Returns(true);
            calculation2.HasOutput.Returns(true);
            calculation3.HasOutput.Returns(false);
            calculation4.HasOutput.Returns(false);

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

            // Call
            rootGroup.ClearCalculationOutput();

            // Assert
            calculation1.Received().ClearOutput();
            calculation2.Received().ClearOutput();
            calculation1.Received().NotifyObservers();
            calculation2.Received().NotifyObservers();
        }

        [Test]
        public void HasOutput_ForCalculationGroupWithGroupsAndCalculationsWithoutOutput_ReturnsFalse()
        {
            // Setup
            var calculation1 = Substitute.For<ICalculation>();
            var calculation2 = Substitute.For<ICalculation>();
            var calculation3 = Substitute.For<ICalculation>();
            var calculation4 = Substitute.For<ICalculation>();

            calculation1.HasOutput.Returns(false);
            calculation2.HasOutput.Returns(false);
            calculation3.HasOutput.Returns(false);
            calculation4.HasOutput.Returns(false);
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

            // Call
            bool hasOutput = rootGroup.HasOutput();

            // Assert
            Assert.IsFalse(hasOutput);
        }

        [Test]
        public void HasOutput_ForCalculationGroupWithGroupsAndOneCalculationWithOutput_ReturnsTrue()
        {
            // Setup
            var calculation1 = Substitute.For<ICalculation>();
            var calculation2 = Substitute.For<ICalculation>();
            var calculation3 = Substitute.For<ICalculation>();
            var calculation4 = Substitute.For<ICalculation>();

            calculation1.HasOutput.Returns(false);
            calculation2.HasOutput.Returns(false);
            calculation3.HasOutput.Returns(false);
            calculation4.HasOutput.Returns(true);
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

            // Call
            bool hasOutput = rootGroup.HasOutput();

            // Assert
            Assert.IsTrue(hasOutput);
        }

        [Test]
        public void GetAllChildrenRecursive_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var group = new CalculationGroup();

            // Call
            IEnumerable<ICalculationBase> children = group.GetAllChildrenRecursive();

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void GetAllChildrenRecursive_GroupWithNestedGroupsWithCalculations_ReturnAllNestedGroupsAndCalculations()
        {
            // Setup
            var calculation1 = Substitute.For<ICalculation>();
            var calculation2 = Substitute.For<ICalculation>();
            var calculation3 = Substitute.For<ICalculation>();
            var calculation4 = Substitute.For<ICalculation>();
            var nestedChildGroup = new CalculationGroup
            {
                Children =
                {
                    calculation3
                }
            };

            var childGroup1 = new CalculationGroup
            {
                Children =
                {
                    calculation2
                }
            };
            var childGroup2 = new CalculationGroup
            {
                Children =
                {
                    nestedChildGroup,
                    calculation4
                }
            };

            var rootGroup = new CalculationGroup
            {
                Children =
                {
                    calculation1,
                    childGroup1,
                    childGroup2
                }
            };

            var expectedChildren = new ICalculationBase[]
            {
                calculation1,
                calculation2,
                calculation3,
                calculation4,
                childGroup1,
                childGroup2,
                nestedChildGroup
            };

            // Call
            IEnumerable<ICalculationBase> children = rootGroup.GetAllChildrenRecursive();

            // Assert
            CollectionAssert.AreEquivalent(expectedChildren, children);
        }
    }
}