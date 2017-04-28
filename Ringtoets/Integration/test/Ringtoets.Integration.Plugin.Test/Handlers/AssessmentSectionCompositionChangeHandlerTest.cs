// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
            var title = "";
            var message = "";
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
            string expectedMessage = "Als u het trajecttype aanpast, dan worden alle rekenresultaten van alle relevante toetssporen verwijderd."
                                     + Environment.NewLine
                                     + Environment.NewLine +
                                     "Weet u zeker dat u wilt doorgaan?";
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
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
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
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations(oldComposition);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                                                   .OfType<GrassCoverErosionOutwardsFailureMechanism>()
                                                                                                                   .Single();

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .Single();

            IEnumerable<ICalculation> unaffectedObjects = GetDuneIrrelevantFailureMechanisms(assessmentSection)
                .SelectMany(fm => fm.Calculations)
                .Where(calc => calc.HasOutput)
                .ToList();

            IEnumerable<IObservable> expectedAffectedObjects =
                duneErosionFailureMechanism.DuneLocations.Where(dl => dl.Output != null).Cast<IObservable>()
                                           .Concat(grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations)
                                           .Concat(new IObservable[]
                                           {
                                               grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations,
                                               duneErosionFailureMechanism.DuneLocations,
                                               assessmentSection
                                           })
                                           .Concat(GetDuneRelevantFailureMechanisms(assessmentSection)
                                                       .SelectMany(fm => fm.Calculations)
                                                       .Where(calc => calc.HasOutput))
                                           .ToList();

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
            AssertCorrectOutputClearedWhenCompositionDune(unaffectedObjects, assessmentSection);
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
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_ChangeToNonDuneComposition_ChangeCompositionClearRelevantOutputReturnsAllAffectedObjects(Dike,DikeDune)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_ChangeToNonDuneComposition_ChangeCompositionClearRelevantOutputReturnsAllAffectedObjects(DikeDune,Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_ChangeToNonDuneComposition_ChangeCompositionClearRelevantOutputReturnsAllAffectedObjects(Dune,Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_ChangeToNonDuneComposition_ChangeCompositionClearRelevantOutputReturnsAllAffectedObjects(Dune,DikeDune)")]
        public void ChangeComposition_ChangeToNonDuneComposition_ChangeCompositionAndClearAllLocationOutputAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                                        AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations(oldComposition);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .Single();

            IEnumerable<ICalculation> expectedUnaffectedObjects = assessmentSection.GetFailureMechanisms()
                                                                                   .SelectMany(fm => fm.Calculations)
                                                                                   .Where(calc => calc.HasOutput)
                                                                                   .ToList();

            IEnumerable<IObservable> expectedAffectedObjects = duneErosionFailureMechanism.DuneLocations.Where(dl => dl.Output != null)
                                                                                          .Concat(new IObservable[]
                                                                                          {
                                                                                              assessmentSection,
                                                                                              duneErosionFailureMechanism.DuneLocations
                                                                                          })
                                                                                          .ToList();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            const string expectedMessage = "Alle berekende hydraulische randvoorwaarden van de relevante toetssporen zijn verwijderd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
            AssertOutputNotCleared(expectedUnaffectedObjects, assessmentSection.GetFailureMechanisms());

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
        public void ChangeComposition_ChangeToDuneAndNoCalculationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                            AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput(oldComposition);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                                                   .OfType<GrassCoverErosionOutwardsFailureMechanism>()
                                                                                                                   .Single();

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .Single();

            IEnumerable<IObservable> expectedAffectedObjects =
                duneErosionFailureMechanism.DuneLocations.Where(dl => dl.Output != null).Cast<IObservable>()
                                           .Concat(grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations)
                                           .Concat(new IObservable[]
                                           {
                                               grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations,
                                               duneErosionFailureMechanism.DuneLocations,
                                               assessmentSection
                                           })
                                           .Concat(GetDuneRelevantFailureMechanisms(assessmentSection)
                                                       .SelectMany(fm => fm.Calculations)
                                                       .Where(calc => calc.HasOutput))
                                           .ToList();

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
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(Dune,Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(Dune,DikeDune)")]
        public void ChangeComposition_ChangeToNonDuneAndNoCalculationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                               AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput(oldComposition);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .Single();

            IEnumerable<IObservable> expectedAffectedObjects = duneErosionFailureMechanism.DuneLocations.Where(dl => dl.Output != null)
                                                                                          .Concat(new IObservable[]
                                                                                          {
                                                                                              assessmentSection,
                                                                                              duneErosionFailureMechanism.DuneLocations
                                                                                          })
                                                                                          .ToList();

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
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_ChangeToDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(Dike,Dune)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_ChangeToDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(DikeDune,Dune)")]
        public void ChangeComposition_ChangeToDuneAndNoHydraulicBoudaryLocationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                                         AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput(oldComposition);

            IEnumerable<ICalculation> notAffectedObjects = GetDuneIrrelevantFailureMechanisms(assessmentSection)
                .SelectMany(fm => fm.Calculations)
                .Where(calc => calc.HasOutput)
                .ToList();

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
            AssertCorrectOutputClearedWhenCompositionDune(notAffectedObjects, assessmentSection);
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAffectedObjects(Dike,DikeDune)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAffectedObjects(DikeDune,Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAffectedObjects(Dune,Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAffectedObjects(Dune,DikeDune)")]
        public void ChangeComposition_ChangeToNonDuneAndNoHydraulicBoudaryLocationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                                            AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput(oldComposition);

            List<ICalculation> expectedUnaffectedObjects = assessmentSection.GetFailureMechanisms()
                                                                            .SelectMany(fm => fm.Calculations)
                                                                            .Where(calc => calc.HasOutput)
                                                                            .ToList();

            IObservable[] expectedAffectedObjects =
            {
                assessmentSection
            };

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            Assert.AreEqual(newComposition, assessmentSection.Composition);
            AssertOutputNotCleared(expectedUnaffectedObjects, assessmentSection.GetFailureMechanisms());

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        /// <summary>
        /// Asserts whether the expected unaffected objects retain their outputs when cleared.
        /// </summary>
        /// <param name="expectedUnaffectedObjects">The list of objects that should not have been affected.</param>
        /// <param name="failureMechanisms">The failure mechanisms to assert.</param>
        private static void AssertOutputNotCleared(IEnumerable<ICalculation> expectedUnaffectedObjects, IEnumerable<IFailureMechanism> failureMechanisms)
        {
            CollectionAssert.AreEquivalent(expectedUnaffectedObjects,
                                           failureMechanisms.SelectMany(fm => fm.Calculations)
                                                            .Where(calc => calc.HasOutput),
                                           "The calculation output within the failure mechanisms should not have been deleted in this scenario");
        }

        /// <summary>
        /// Asserts whether the correct failure mechanism outputs are cleared when the <see cref="IAssessmentSection.Composition"/>
        /// changes to <see cref="AssessmentSectionComposition.Dune"/>.
        /// </summary>
        /// <param name="unaffectedObjects">The objects that should not have been affected.</param>
        /// <param name="assessmentSection">The assessment section to assert.</param>
        private static void AssertCorrectOutputClearedWhenCompositionDune(IEnumerable<ICalculation> unaffectedObjects, IAssessmentSection assessmentSection)
        {
            Assert.IsTrue(GetDuneRelevantFailureMechanisms(assessmentSection).SelectMany(fm => fm.Calculations)
                                                                             .All(calc => !calc.HasOutput));

            AssertOutputNotCleared(unaffectedObjects, GetDuneIrrelevantFailureMechanisms(assessmentSection));
        }

        #region LinQ queries for expectancies

        /// <summary>
        /// Retrieves the failure mechanisms that should be cleared when the
        /// <see cref="AssessmentSectionComposition"/> changes to Dunes.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to retrieve the failure mechanisms from.</param>
        /// <returns>A collection of affected failure mechanisms when changing <see cref="IAssessmentSection.Composition"/> to 
        /// <see cref="AssessmentSectionComposition.Dune"/>.</returns>
        private static IEnumerable<IFailureMechanism> GetDuneRelevantFailureMechanisms(IAssessmentSection assessmentSection)
        {
            var relevantFailureMechanisms = new List<IFailureMechanism>();

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
                    relevantFailureMechanisms.Add(pipingFailureMechanism);
                }

                if (grassCoverErosionInwardsFailureMechanism != null)
                {
                    relevantFailureMechanisms.Add(grassCoverErosionInwardsFailureMechanism);
                }

                if (grassCoverErosionOutwardsFailureMechanism != null)
                {
                    relevantFailureMechanisms.Add(grassCoverErosionOutwardsFailureMechanism);
                }

                if (heightStructuresFailureMechanism != null)
                {
                    relevantFailureMechanisms.Add(heightStructuresFailureMechanism);
                }

                if (closingStructuresFailureMechanism != null)
                {
                    relevantFailureMechanisms.Add(closingStructuresFailureMechanism);
                }

                if (stabilityPointStructuresFailureMechanism != null)
                {
                    relevantFailureMechanisms.Add(stabilityPointStructuresFailureMechanism);
                }
            }

            return relevantFailureMechanisms;
        }

        /// <summary>
        /// Retrieves all failure mechanisms that should be untouched when the
        /// <see cref="AssessmentSectionComposition"/> changes to Dunes.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to retrieve the failure mechanisms from.</param>
        /// <returns>A collection of irrelevant (unaffected) failure mechanisms when changing the <see cref="IAssessmentSection.Composition"/>
        /// to <see cref="AssessmentSectionComposition.Dune"/>.</returns>
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