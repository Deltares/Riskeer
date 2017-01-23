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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.Service;
using Ringtoets.Integration.TestUtils;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class AssessmentSectionCompositionChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void ConfirmCompositionChange_Always_ShowMessageBox()
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

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            handler.ConfirmCompositionChange();

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Na het aanpassen van het trajecttype zullen alle rekenresultaten van alle toetssporen gewist worden." + Environment.NewLine +
                                     Environment.NewLine +
                                     "Wilt u doorgaan?";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ConfirmCompositionChange_MessageBoxOk_ReturnTrue()
        {
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            bool result = handler.ConfirmCompositionChange();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ConfirmCompositionChange_MessageBoxCancel_ReturnFalse()
        {
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickCancel();
            };

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            bool result = handler.ConfirmCompositionChange();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ChangeComposition_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            TestDelegate call = () => handler.ChangeComposition(null, AssessmentSectionComposition.Dike);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void ChangeComposition_ChangeToSameValue_DoNothing()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();
            AssessmentSectionComposition originalComposition = assessmentSection.Composition;
            ICalculation[] calculationsWithOutput = assessmentSection.GetFailureMechanisms()
                                                                     .SelectMany(fm => fm.Calculations)
                                                                     .Where(c => c.HasOutput)
                                                                     .ToArray();

            DuneLocation[] duneLocationWithOutput = assessmentSection.GetFailureMechanisms().OfType<DuneErosionFailureMechanism>()
                                                                     .SelectMany(f => f.DuneLocations)
                                                                     .Where(loc => loc.Output != null)
                                                                     .ToArray();

            HydraulicBoundaryLocation[] hblWithOutput = assessmentSection.GetFailureMechanisms().OfType<GrassCoverErosionOutwardsFailureMechanism>()
                                                                         .SelectMany(f => f.HydraulicBoundaryLocations)
                                                                         .Where(loc => loc.DesignWaterLevelOutput != null || loc.WaveHeightOutput != null)
                                                                         .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ChangeComposition(assessmentSection, originalComposition);

            // Assert
            Assert.True(calculationsWithOutput.All(c => c.HasOutput),
                        "All calculations that had output still have them.");

            Assert.True(duneLocationWithOutput.All(loc => loc.Output != null));
            Assert.True(hblWithOutput.All(loc => loc.DesignWaterLevelOutput != null || loc.WaveHeightOutput != null));

            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void ChangeComposition_ChangeToDuneComposition_ChangeCompositionAndClearAllCalculationOutputAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                                        AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection(oldComposition);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                                                   .OfType<GrassCoverErosionOutwardsFailureMechanism>()
                                                                                                                   .First();

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .First();

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, newComposition);

            IEnumerable<ICalculation> notAffectedObjects = GetDuneIrrelevantFailureMechanisms(assessmentSection)
                .SelectMany(fm => fm.Calculations)
                .Where(calc => calc.HasOutput);

            IObservable[] expectedAffectedObjects = new IObservable[]
            {
                grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations,
                duneErosionFailureMechanism.DuneLocations,
                assessmentSection
            }.Concat(GetDuneRelevantFailureMechanisms(assessmentSection)
                         .SelectMany(fm => fm.Calculations)
                         .Where(calc => calc.HasOutput)).ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            string[] expectedMessage =
            {
                "De resultaten van 24 berekeningen zijn verwijderd.",
                "Alle berekende hydraulische randvoorwaarden van de relevante toetssporen zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessage, 2);

            Assert.AreEqual(newComposition, assessmentSection.Composition);

            // Assert 
            // What should NOT be deleted when changing:
            // - The Hydraulic Boundary location output defined at assessment section level
            // - Calculation output related to wave impact asphalt cover 
            // - Calculation output related stability stone cover
            Assert.IsTrue(GetDuneRelevantFailureMechanisms(assessmentSection).SelectMany(fm => fm.Calculations)
                                                                             .All(calc => !calc.HasOutput));

            CollectionAssert.AreEquivalent(notAffectedObjects, GetDuneIrrelevantFailureMechanisms(assessmentSection)
                                               .SelectMany(fm => fm.Calculations)
                                               .Where(calc => calc.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);

            foreach (HydraulicBoundaryLocation location in assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations)
            {
                // Sufficient to evaluate if the output is null or not
                Assert.IsNull(location.WaveHeightOutput);
                Assert.IsNull(location.DesignWaterLevelOutput);
            }
            foreach (DuneLocation duneLocation in assessmentSection.DuneErosion.DuneLocations)
            {
                Assert.IsNull(duneLocation.Output);
            }
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        public void ChangeComposition_ChangeToNonDuneComposition_ChangeCompositionAndClearAllCalculationOutputAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                                           AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection(oldComposition);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .First();

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, newComposition);

            var expectedNotAffectedObjects = assessmentSection.GetFailureMechanisms()
                                                              .SelectMany(fm => fm.Calculations)
                                                              .Where(calc => calc.HasOutput);

            IObservable[] expectedAffectedObjects = new IObservable[]
            {
                assessmentSection,
                duneErosionFailureMechanism.DuneLocations
            }.ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            string[] expectedMessage =
            {
                "Alle berekende hydraulische randvoorwaarden van de relevante toetssporen zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessage, 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);

            // The calculations within the failure mechanisms should not have been deleted in this scenario,
            // except for the calculated hydraulic boundary location output for the dune erosion failure mechanism. 
            CollectionAssert.AreEquivalent(expectedNotAffectedObjects, assessmentSection.GetFailureMechanisms()
                                                                                        .SelectMany(fm => fm.Calculations)
                                                                                        .Where(calc => calc.HasOutput));

            foreach (HydraulicBoundaryLocation location in assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations)
            {
                // Sufficient to evaluate if the output is null or not.
                // Note: could also be replaced with a LINQ query
                Assert.IsNotNull(location.WaveHeightOutput);
                Assert.IsNotNull(location.DesignWaterLevelOutput);
            }

            foreach (DuneLocation duneLocation in assessmentSection.DuneErosion.DuneLocations)
            {
                Assert.IsNull(duneLocation.Output);
            }
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void ChangeComposition_ChangeToDuneAndNoCalculationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                            AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection(oldComposition);
            RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, newComposition);
            CollectionAssert.IsEmpty(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(c => c.HasOutput));

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                                                   .OfType<GrassCoverErosionOutwardsFailureMechanism>()
                                                                                                                   .First();

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .First();

            IObservable[] expectedAffectedObjects = new IObservable[]
            {
                assessmentSection,
                grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations,
                duneErosionFailureMechanism.DuneLocations
            }.ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Alle berekende hydraulische randvoorwaarden van de relevante toetssporen zijn verwijderd.", 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);
            Assert.True(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).All(c => !c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);

            foreach (HydraulicBoundaryLocation location in assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations)
            {
                Assert.IsNull(location.WaveHeightOutput);
                Assert.IsNull(location.DesignWaterLevelOutput);
            }
            foreach (DuneLocation duneLocation in assessmentSection.DuneErosion.DuneLocations)
            {
                Assert.IsNull(duneLocation.Output);
            }
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        public void ChangeComposition_ChangeToNonDuneAndNoCalculationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                               AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection(oldComposition);
            RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, newComposition);
            CollectionAssert.IsEmpty(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(c => c.HasOutput));

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .First();

            IObservable[] expectedAffectedObjects = new IObservable[]
            {
                assessmentSection,
                duneErosionFailureMechanism.DuneLocations
            }.ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Alle berekende hydraulische randvoorwaarden van de relevante toetssporen zijn verwijderd.", 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);
            Assert.True(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).All(c => !c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);

            foreach (HydraulicBoundaryLocation location in assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations)
            {
                Assert.IsNotNull(location.WaveHeightOutput);
                Assert.IsNotNull(location.DesignWaterLevelOutput);
            }
            foreach (DuneLocation duneLocation in assessmentSection.DuneErosion.DuneLocations)
            {
                Assert.IsNull(duneLocation.Output);
            }
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void ChangeComposition_ChangeToDuneAndNoHydraulicBoudaryLocationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                                         AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection(oldComposition);
            RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutputOfFailureMechanisms(assessmentSection);

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, newComposition);
            CollectionAssert.IsEmpty(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Where(loc => loc.DesignWaterLevelOutput != null
                                                                                                                         || loc.WaveHeightOutput != null));
            CollectionAssert.IsEmpty(assessmentSection.DuneErosion.DuneLocations.Where(dl => dl.Output != null));

            IEnumerable<ICalculation> notAffectedObjects = GetDuneIrrelevantFailureMechanisms(assessmentSection)
                .SelectMany(fm => fm.Calculations)
                .Where(calc => calc.HasOutput);

            IObservable[] expectedAffectedObjects = new IObservable[]
            {
                assessmentSection
            }.Concat(GetDuneRelevantFailureMechanisms(assessmentSection)
                         .SelectMany(fm => fm.Calculations)
                         .Where(c => c.HasOutput)).ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "De resultaten van 24 berekeningen zijn verwijderd.", 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);

            // Assert 
            // What should NOT be deleted when changing:
            // - The Hydraulic Boundary location output defined at assessment section level
            // - Calculation output related to wave impact asphalt cover 
            // - Calculation output related stability stone cover
            Assert.IsTrue(GetDuneRelevantFailureMechanisms(assessmentSection).SelectMany(fm => fm.Calculations)
                                                                             .All(calc => !calc.HasOutput));
            CollectionAssert.AreEquivalent(notAffectedObjects, GetDuneIrrelevantFailureMechanisms(assessmentSection).SelectMany(fm => fm.Calculations)
                                                                                                                    .Where(calc => calc.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        public void ChangeComposition_ChangeToNonDuneAndNoHydraulicBoudaryLocationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                                            AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection(oldComposition);
            RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutputOfFailureMechanisms(assessmentSection);

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, newComposition);
            CollectionAssert.IsEmpty(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Where(loc => loc.DesignWaterLevelOutput != null
                                                                                                                         || loc.WaveHeightOutput != null));
            CollectionAssert.IsEmpty(assessmentSection.DuneErosion.DuneLocations.Where(dl => dl.Output != null));

            var expectedNotAffectedObjects = assessmentSection.GetFailureMechanisms()
                                                              .SelectMany(fm => fm.Calculations)
                                                              .Where(calc => calc.HasOutput);

            IObservable[] expectedAffectedObjects = new[]
            {
                assessmentSection
            }.ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            Assert.AreEqual(newComposition, assessmentSection.Composition);

            // The calculations within the failure mechanisms should not have been deleted in this scenario,
            CollectionAssert.AreEquivalent(expectedNotAffectedObjects, assessmentSection.GetFailureMechanisms()
                                                                                        .SelectMany(fm => fm.Calculations)
                                                                                        .Where(calc => calc.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        #region LinQ queries for expectancies

        /// <summary>
        /// Function retrieves the failure mechanisms that should be cleared when the
        /// AssessmentSectionComposition changes to Dunes.
        /// </summary>
        /// <param name="assessmentSection"></param>
        /// <returns>A collection of objects that are cleared when changing the assessmentsectioncomposition to dunes.</returns>
        private static IEnumerable<IFailureMechanism> GetDuneRelevantFailureMechanisms(IAssessmentSection assessmentSection)
        {
            var affectedObjects = new List<IFailureMechanism>();

            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                var pipingFailureMechanism = failureMechanism as PipingFailureMechanism;
                var grassCoverErosionInwardsFailureMechanism = failureMechanism as GrassCoverErosionInwardsFailureMechanism;
                var grassCoverErosionOutwardsFailureMechanism = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
                var heightStructuresFailureMechanism = failureMechanism as HeightStructuresFailureMechanism;
                var closingStructuresFailureMechanism = failureMechanism as ClosingStructuresFailureMechanism;
                var stabilityPointStructuresFailureMechanism = failureMechanism as StabilityPointStructuresFailureMechanism;

                if (pipingFailureMechanism != null)
                {
                    affectedObjects.Add(pipingFailureMechanism);
                }

                if (grassCoverErosionInwardsFailureMechanism != null)
                {
                    affectedObjects.Add(grassCoverErosionInwardsFailureMechanism);
                }

                if (grassCoverErosionOutwardsFailureMechanism != null)
                {
                    affectedObjects.Add(grassCoverErosionOutwardsFailureMechanism);
                }

                if (heightStructuresFailureMechanism != null)
                {
                    affectedObjects.Add(heightStructuresFailureMechanism);
                }

                if (closingStructuresFailureMechanism != null)
                {
                    affectedObjects.Add(closingStructuresFailureMechanism);
                }

                if (stabilityPointStructuresFailureMechanism != null)
                {
                    affectedObjects.Add(stabilityPointStructuresFailureMechanism);
                }
            }

            return affectedObjects;
        }

        /// <summary>
        /// Function retrieves all failure mechanisms that should be untouched when the
        /// AssessmentSectionComposition changes to Dunes.
        /// </summary>
        /// <param name="assessmentSection"></param>
        /// <returns>A collection of objects that are untouched when changing the assessmentsectioncomposition to dunes.</returns>
        private static IEnumerable<IFailureMechanism> GetDuneIrrelevantFailureMechanisms(IAssessmentSection assessmentSection)
        {
            var failureMechanisms = new List<IFailureMechanism>();

            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                var waveImpactAsphaltCoverFailureMechanism = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
                var stabilityStoneCoverFailureMechanism = failureMechanism as StabilityStoneCoverFailureMechanism;

                if (waveImpactAsphaltCoverFailureMechanism != null)
                {
                    failureMechanisms.Add(waveImpactAsphaltCoverFailureMechanism);
                }

                if (stabilityStoneCoverFailureMechanism != null)
                {
                    failureMechanisms.Add(stabilityStoneCoverFailureMechanism);
                }
            }

            return failureMechanisms;
        }

        #endregion
    }
}