using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class IPipingCalculationItemExtensionsTest
    {
        [Test]
        public void GetPipingCalculations_FromPipingCalculation_ReturnThatCalculationInstance()
        {
            // Setup
            IPipingCalculationItem calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            // Call
            IEnumerable<PipingCalculation> result = calculation.GetPipingCalculations();

            // Assert
            CollectionAssert.AreEqual(new[]{calculation}, result);
        }

        [Test]
        public void GetPipingCalculations_FromPipingCalculationGroupWithoutChildren_ReturnEmpty()
        {
            // Setup
            IPipingCalculationItem groupWithoutChildren = new PipingCalculationGroup();

            // Call
            IEnumerable<PipingCalculation> result = groupWithoutChildren.GetPipingCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingCalculations_FromPipingCalculationGroupWithEmptyGroups_ReturnEmpty()
        {
            // Setup
            var rootGroup = new PipingCalculationGroup();
            rootGroup.Children.Add(new PipingCalculationGroup());
            rootGroup.Children.Add(new PipingCalculationGroup());
            rootGroup.Children.Add(new PipingCalculationGroup());

            IPipingCalculationItem groupsWithoutChildren = rootGroup;

            // Call
            IEnumerable<PipingCalculation> result = groupsWithoutChildren.GetPipingCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingCalculations_FromPipingCalculationGroupWithGroupsAndCalculations_ReturnAllCalculationsRecursiveslyInAnyOrder()
        {
            // Setup
            var generalPipingInput = new GeneralPipingInput();
            var semiProbabilisticInput = new SemiProbabilisticPipingInput();
            var calculation1 = new PipingCalculation(generalPipingInput, semiProbabilisticInput);
            var calculation2 = new PipingCalculation(generalPipingInput, semiProbabilisticInput);
            var calculation3 = new PipingCalculation(generalPipingInput, semiProbabilisticInput);
            var calculation4 = new PipingCalculation(generalPipingInput, semiProbabilisticInput);

            var subsubGroup = new PipingCalculationGroup();
            subsubGroup.Children.Add(calculation4);

            var subgroup1 = new PipingCalculationGroup();
            subgroup1.Children.Add(calculation2);
            subgroup1.Children.Add(subsubGroup);

            var subgroup2 = new PipingCalculationGroup();
            subgroup2.Children.Add(calculation3);

            var rootGroup = new PipingCalculationGroup();
            rootGroup.Children.Add(subgroup1);
            rootGroup.Children.Add(calculation1);
            rootGroup.Children.Add(subgroup2);

            IPipingCalculationItem groupWithoutChildren = rootGroup;

            // Call
            IEnumerable<PipingCalculation> result = groupWithoutChildren.GetPipingCalculations();

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

        [Test]
        public void GetPipingCalculations_FromEmptyEnummerable_ReturnEmpty()
        {
            // Setup
            IEnumerable<IPipingCalculationItem> emptyEnumerable = Enumerable.Empty<IPipingCalculationItem>();

            // Call
            IEnumerable<PipingCalculation> result = emptyEnumerable.GetPipingCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingCalculations_FromArrayWithCalculations_ReturnAllThoseCalculationsInAnyOrder()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();
            var semiProbabilisticInput = new SemiProbabilisticPipingInput();
            var calculation1 = new PipingCalculation(generalInputParameters, semiProbabilisticInput);
            var calculation2 = new PipingCalculation(generalInputParameters, semiProbabilisticInput);
            IEnumerable<IPipingCalculationItem> calculationArray = new[] { calculation1, calculation2 };

            // Call
            IEnumerable<PipingCalculation> result = calculationArray.GetPipingCalculations();

            // Assert
            CollectionAssert.AreEquivalent(calculationArray, result);
        }

        [Test]
        public void GetPipingCalculations_FromArrayWithEmptyGroups_ReturnEmpty()
        {
            // Setup
            var emptyGroup1 = new PipingCalculationGroup();
            var emptyGroup2 = new PipingCalculationGroup();
            IEnumerable<IPipingCalculationItem> emptyEnumerable = new[] { emptyGroup1, emptyGroup2 };

            // Call
            IEnumerable<PipingCalculation> result = emptyEnumerable.GetPipingCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingCalculations_FromArrayWithMixedGroupsAndCalculations_ReturnAllCalculationsInAnyOrder()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();
            var semiProbabilisticInput = new SemiProbabilisticPipingInput();
            var rootcalculation = new PipingCalculation(generalInputParameters, semiProbabilisticInput);
            var calculation1 = new PipingCalculation(generalInputParameters, semiProbabilisticInput);
            var calculation2 = new PipingCalculation(generalInputParameters, semiProbabilisticInput);
            var calculation3 = new PipingCalculation(generalInputParameters, semiProbabilisticInput);
            var calculation4 = new PipingCalculation(generalInputParameters, semiProbabilisticInput);

            var subsubGroup = new PipingCalculationGroup();
            subsubGroup.Children.Add(calculation4);

            var subgroup1 = new PipingCalculationGroup();
            subgroup1.Children.Add(calculation2);
            subgroup1.Children.Add(subsubGroup);

            var subgroup2 = new PipingCalculationGroup();
            subgroup2.Children.Add(calculation3);

            var rootGroup = new PipingCalculationGroup();
            rootGroup.Children.Add(subgroup1);
            rootGroup.Children.Add(calculation1);
            rootGroup.Children.Add(subgroup2);

            var emptyRootGroup = new PipingCalculationGroup();

            IEnumerable<IPipingCalculationItem> mixedArray = new IPipingCalculationItem[] { emptyRootGroup, rootGroup, rootcalculation };

            // Call
            IEnumerable<PipingCalculation> result = mixedArray.GetPipingCalculations();

            // Assert
            var expectedCalculationItems = new[]
            {
                rootcalculation,
                calculation1,
                calculation2,
                calculation3,
                calculation4
            };
            CollectionAssert.AreEquivalent(expectedCalculationItems, result);
        }
    }
}