using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void ConfirmPropertyChange_Always_ShowMessageBox()
        {
            // Setup
            string title = "";
            string message = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                title = tester.Title;
                message = tester.Text;

                tester.ClickOk();
            };

            var handler = new GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler();

            // Call
            handler.ConfirmPropertyChange();

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u een parameter in een toetsspoor wijzigt, zal de uitvoer van alle randvoorwaarden locaties en berekeningen in dit toetsspoor verwijderd worden." + Environment.NewLine +
                                     Environment.NewLine +
                                     "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ConfirmPropertyChange_MessageBoxOk_ReturnTrue()
        {
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            var handler = new GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler();

            // Call
            bool result = handler.ConfirmPropertyChange();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ConfirmPropertyChange_MessageBoxCancel_ReturnFalse()
        {
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickCancel();
            };

            var handler = new GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler();

            // Call
            bool result = handler.ConfirmPropertyChange();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ChangeComposition_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler();

            // Call
            TestDelegate test = () => handler.PropertyChanged(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        [TestCaseSource("ChangePropertyTestCases")]
        public void ChangeComposition_FailureMechanismWithDifferentCalculationCollections_ReturnsCalculationsWhichHadOutput(ChangePropertyTestCase testCase)
        {
            // Setup
            var handler = new GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            foreach (var calculation in testCase.Calculations)
            {
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            }
            failureMechanism.HydraulicBoundaryLocations.AddRange(testCase.Locations);

            // Call
            IEnumerable<IObservable> result = handler.PropertyChanged(failureMechanism);

            // Assert
            CollectionAssert.AreEquivalent(testCase.ExpectedAffectedCalculations.Concat(testCase.ExpectedAffectedLocations), result);
        }

        public class ChangePropertyTestCase
        {
            public ChangePropertyTestCase(ICollection<HydraulicBoundaryLocation> locations, ICollection<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations)
            {
                Calculations = calculations;
                ExpectedAffectedCalculations = calculations.Where(c => c.HasOutput).ToArray();
                Locations = locations;
                ExpectedAffectedLocations = locations.Where(c => c.DesignWaterLevelOutput != null || c.WaveHeightOutput != null).ToArray();
            }

            public ICollection<HydraulicBoundaryLocation> Locations { get; private set; }
            public ICollection<IObservable> ExpectedAffectedLocations { get; private set; }

            public ICollection<GrassCoverErosionOutwardsWaveConditionsCalculation> Calculations { get; private set; }

            public ICollection<IObservable> ExpectedAffectedCalculations { get; private set; }
        }

        static IEnumerable ChangePropertyTestCases()
        {
            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                                           {
                                               CreateHydraulicBoundaryLocationWithoutOutput()
                                           }, new[]
                                           {
                                               CreateCalculationWithOutput()
                                           })
            ).SetName("ChangeComposition Single location without output, calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                                           {
                                               CreateHydraulicBoundaryLocationWithOutput()
                                           },
                                           new[]
                                           {
                                               CreateCalculationWithoutOutput()
                                           })
            ).SetName("ChangeComposition Single location with output, single calculation without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                                           {
                                               CreateHydraulicBoundaryLocationWithoutOutput(),
                                               CreateHydraulicBoundaryLocationWithoutOutput()
                                           },
                                           new[]
                                           {
                                               CreateCalculationWithoutOutput(),
                                               CreateCalculationWithoutOutput()
                                           })
            ).SetName("ChangeComposition Two locations without output, calculations without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                                           {
                                               CreateHydraulicBoundaryLocationWithOutput(),
                                               CreateHydraulicBoundaryLocationWithoutOutput()
                                           },
                                           new[]
                                           {
                                               CreateCalculationWithOutput(),
                                               CreateCalculationWithoutOutput()
                                           })
            ).SetName("ChangeComposition Location with, location without, calculation without and calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                                           {
                                               CreateHydraulicBoundaryLocationWithOutput(),
                                               CreateHydraulicBoundaryLocationWithOutput()
                                           },
                                           new[]
                                           {
                                               CreateCalculationWithOutput(),
                                               CreateCalculationWithOutput()
                                           })
            ).SetName("ChangeComposition Two locations with output, two calculations with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                                           {
                                               CreateHydraulicBoundaryLocationWithOutput(),
                                               CreateHydraulicBoundaryLocationWithoutOutput(),
                                               CreateHydraulicBoundaryLocationWithOutput()
                                           },
                                           new[]
                                           {
                                               CreateCalculationWithOutput(),
                                               CreateCalculationWithOutput(),
                                               CreateCalculationWithoutOutput()
                                           })
            ).SetName("ChangeComposition Two locations with, one location without, two calculations with and one calculation without output");
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation CreateCalculationWithoutOutput()
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation();
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation CreateCalculationWithOutput()
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
        }

        private static TestHydraulicBoundaryLocation CreateHydraulicBoundaryLocationWithoutOutput()
        {
            return new TestHydraulicBoundaryLocation();
        }

        private static TestHydraulicBoundaryLocation CreateHydraulicBoundaryLocationWithOutput()
        {
            return new TestHydraulicBoundaryLocation
            {
                WaveHeightOutput = new TestHydraulicBoundaryLocationOutput(0.5),
                DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(2.3)
            };
        }
    }
}