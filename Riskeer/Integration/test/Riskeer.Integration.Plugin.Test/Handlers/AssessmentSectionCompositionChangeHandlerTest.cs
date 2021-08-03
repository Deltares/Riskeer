// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class AssessmentSectionCompositionChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var handler = new AssessmentSectionCompositionChangeHandler();

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionCompositionChangeHandler>(handler);
        }

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
            // Setup
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
            // Setup
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
            void Call() => handler.ChangeComposition(null, AssessmentSectionComposition.Dike);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
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

            DuneLocationCalculation[] duneLocationCalculationsWithOutput = assessmentSection.DuneErosion.CalculationsForMechanismSpecificFactorizedSignalingNorm.Where(HasDuneLocationCalculationOutput)
                                                                                            .Concat(assessmentSection.DuneErosion.CalculationsForMechanismSpecificSignalingNorm.Where(HasDuneLocationCalculationOutput))
                                                                                            .Concat(assessmentSection.DuneErosion.CalculationsForMechanismSpecificLowerLimitNorm.Where(HasDuneLocationCalculationOutput))
                                                                                            .Concat(assessmentSection.DuneErosion.CalculationsForLowerLimitNorm.Where(HasDuneLocationCalculationOutput))
                                                                                            .Concat(assessmentSection.DuneErosion.CalculationsForFactorizedLowerLimitNorm.Where(HasDuneLocationCalculationOutput))
                                                                                            .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ChangeComposition(assessmentSection, originalComposition);

            // Assert
            Assert.True(calculationsWithOutput.All(c => c.HasOutput),
                        "All calculations that had output still have them.");

            Assert.True(duneLocationCalculationsWithOutput.All(HasDuneLocationCalculationOutput));

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

            IEnumerable<ICalculation> unaffectedObjects = GetDuneIrrelevantFailureMechanisms(assessmentSection)
                                                          .SelectMany(fm => fm.Calculations)
                                                          .Where(calc => calc.HasOutput)
                                                          .ToArray();

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            IEnumerable<IObservable> expectedAffectedObjects = GetAllAffectedDuneLocationCalculations(duneErosionFailureMechanism)
                                                               .Concat(new IObservable[]
                                                               {
                                                                   assessmentSection
                                                               })
                                                               .Concat(GetDuneRelevantFailureMechanisms(assessmentSection)
                                                                       .SelectMany(fm => fm.Calculations)
                                                                       .Where(calc => calc.HasOutput))
                                                               .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            void Call() => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            string[] expectedMessage =
            {
                "De resultaten van 28 berekeningen zijn verwijderd.",
                "Alle berekende hydraulische belastingen van de relevante toetssporen zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(Call, expectedMessage, 2);

            Assert.AreEqual(newComposition, assessmentSection.Composition);
            AssertCorrectOutputClearedWhenCompositionDune(unaffectedObjects, assessmentSection);
            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);

            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(duneErosionFailureMechanism);
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

            IEnumerable<ICalculation> expectedUnaffectedObjects = assessmentSection.GetFailureMechanisms()
                                                                                   .SelectMany(fm => fm.Calculations)
                                                                                   .Where(calc => calc.HasOutput)
                                                                                   .ToArray();

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            IEnumerable<IObservable> expectedAffectedObjects = GetAllAffectedDuneLocationCalculations(duneErosionFailureMechanism)
                                                               .Concat(new IObservable[]
                                                               {
                                                                   assessmentSection
                                                               })
                                                               .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            void Call() => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            const string expectedMessage = "Alle berekende hydraulische belastingen van de relevante toetssporen zijn verwijderd.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);

            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);
            AssertOutputNotCleared(expectedUnaffectedObjects, assessmentSection.GetFailureMechanisms());

            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(duneErosionFailureMechanism);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void ChangeComposition_ChangeToDuneAndNoCalculationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                            AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput(oldComposition);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            IEnumerable<IObservable> expectedAffectedObjects =
                GetAllAffectedDuneLocationCalculations(duneErosionFailureMechanism)
                    .Concat(new IObservable[]
                    {
                        assessmentSection
                    })
                    .Concat(GetDuneRelevantFailureMechanisms(assessmentSection)
                            .SelectMany(fm => fm.Calculations)
                            .Where(calc => calc.HasOutput))
                    .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            void Call() => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, "Alle berekende hydraulische belastingen van de relevante toetssporen zijn verwijderd.", 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);
            Assert.True(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).All(c => !c.HasOutput));
            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);
            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(duneErosionFailureMechanism);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(Dune,Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(Dune,DikeDune)")]
        public void ChangeComposition_ChangeToNonDuneAndNoCalculationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                               AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput(oldComposition);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            IEnumerable<IObservable> expectedAffectedObjects =
                GetAllAffectedDuneLocationCalculations(duneErosionFailureMechanism).Concat(new IObservable[]
                                                                                   {
                                                                                       assessmentSection
                                                                                   })
                                                                                   .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            void Call() => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, "Alle berekende hydraulische belastingen van de relevante toetssporen zijn verwijderd.", 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);
            Assert.True(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).All(c => !c.HasOutput));
            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);

            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(duneErosionFailureMechanism);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_ChangeToDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(Dike,Dune)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_ChangeToDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(DikeDune,Dune)")]
        public void ChangeComposition_ChangeToDuneAndNoHydraulicBoundaryLocationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                                          AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput(oldComposition);

            IEnumerable<ICalculation> notAffectedObjects = GetDuneIrrelevantFailureMechanisms(assessmentSection)
                                                           .SelectMany(fm => fm.Calculations)
                                                           .Where(calc => calc.HasOutput)
                                                           .ToArray();

            IObservable[] expectedAffectedObjects = new IObservable[]
            {
                assessmentSection
            }.Concat(GetDuneRelevantFailureMechanisms(assessmentSection)
                     .SelectMany(fm => fm.Calculations)
                     .Where(c => c.HasOutput)).ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            void Call() => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, "De resultaten van 28 berekeningen zijn verwijderd.", 1);
            Assert.AreEqual(newComposition, assessmentSection.Composition);

            // Assert 
            AssertCorrectOutputClearedWhenCompositionDune(notAffectedObjects, assessmentSection);
            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAffectedObjects(Dike,DikeDune)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAffectedObjects(DikeDune,Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAffectedObjects(Dune,Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAffectedObjects(Dune,DikeDune)")]
        public void ChangeComposition_ChangeToNonDuneAndNoHydraulicBoundaryLocationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                                             AssessmentSectionComposition newComposition)
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput(oldComposition);

            IEnumerable<ICalculation> expectedUnaffectedObjects = assessmentSection.GetFailureMechanisms()
                                                                                   .SelectMany(fm => fm.Calculations)
                                                                                   .Where(calc => calc.HasOutput)
                                                                                   .ToArray();

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

            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dike, DikeDune)")]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dike, Dune)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(DikeDune, Dike)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(DikeDune, Dune)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dune, Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dune, DikeDune)")]
        public void ChangeComposition_SetNewValue_ReturnAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                        AssessmentSectionComposition newComposition)
        {
            // Setup
            var handler = new AssessmentSectionCompositionChangeHandler();
            var assessmentSection = new AssessmentSection(oldComposition);

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            IEnumerable<IObservable> expectedAffectedObjects = new List<IObservable>(assessmentSection.GetFailureMechanisms())
            {
                assessmentSection
            };

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

        #region Dune Erosion failure mechanism helpers

        private static IEnumerable<IObservable> GetAllAffectedDuneLocationCalculations(DuneErosionFailureMechanism failureMechanism)
        {
            return DuneLocationsTestHelper.GetAllDuneLocationCalculationsWithOutput(failureMechanism);
        }

        private static bool HasDuneLocationCalculationOutput(DuneLocationCalculation calculation)
        {
            return calculation.Output != null;
        }

        #endregion

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