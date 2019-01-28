// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Gui.Commands;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Riskeer.Integration.Data;

namespace Ringtoets.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class AssessmentSectionCompositionChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_ViewCommandsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new AssessmentSectionCompositionChangeHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("viewCommands", exception.ParamName);
        }

        [Test]
        public void Constructor_WithViewCommands_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionCompositionChangeHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void ConfirmCompositionChange_Always_ShowMessageBox()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var title = "";
            var message = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                title = tester.Title;
                message = tester.Text;

                tester.ClickOk();
            };

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            handler.ConfirmCompositionChange();

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u het trajecttype aanpast, dan worden alle rekenresultaten van alle relevante toetssporen verwijderd."
                                     + Environment.NewLine
                                     + Environment.NewLine +
                                     "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
            mocks.VerifyAll();
        }

        [Test]
        public void ConfirmCompositionChange_MessageBoxOk_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            bool result = handler.ConfirmCompositionChange();

            // Assert
            Assert.IsTrue(result);
            mocks.VerifyAll();
        }

        [Test]
        public void ConfirmCompositionChange_MessageBoxCancel_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickCancel();
            };

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            bool result = handler.ConfirmCompositionChange();

            // Assert
            Assert.IsFalse(result);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeComposition_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            TestDelegate call = () => handler.ChangeComposition(null, AssessmentSectionComposition.Dike);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeComposition_ChangeToSameValue_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

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

            GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
            IEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculationsWithOutput =
                GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.GetAllHydraulicBoundaryLocationCalculationsWithOutput(failureMechanism);

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ChangeComposition(assessmentSection, originalComposition);

            // Assert
            Assert.True(calculationsWithOutput.All(c => c.HasOutput),
                        "All calculations that had output still have them.");

            Assert.True(duneLocationCalculationsWithOutput.All(HasDuneLocationCalculationOutput));
            Assert.True(hydraulicBoundaryLocationCalculationsWithOutput.All(calc => calc.HasOutput));

            CollectionAssert.IsEmpty(affectedObjects);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void ChangeComposition_ChangeToDuneComposition_ChangeCompositionAndClearAllCalculationOutputAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                                        AssessmentSectionComposition newComposition)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations(oldComposition);

            IEnumerable<ICalculation> unaffectedObjects = GetDuneIrrelevantFailureMechanisms(assessmentSection)
                                                          .SelectMany(fm => fm.Calculations)
                                                          .Where(calc => calc.HasOutput)
                                                          .ToArray();

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            IEnumerable<IObservable> expectedAffectedObjects = GetAllAffectedDuneLocationCalculations(duneErosionFailureMechanism)
                                                               .Concat(GetAllAffectedGrassCoverErosionOutwardsCalculations(grassCoverErosionOutwardsFailureMechanism))
                                                               .Concat(new IObservable[]
                                                               {
                                                                   assessmentSection
                                                               })
                                                               .Concat(GetDuneRelevantFailureMechanisms(assessmentSection)
                                                                       .SelectMany(fm => fm.Calculations)
                                                                       .Where(calc => calc.HasOutput))
                                                               .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            string[] expectedMessage =
            {
                "De resultaten van 28 berekeningen zijn verwijderd.",
                "Alle berekende hydraulische belastingen van de relevante toetssporen zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessage, 2);

            Assert.AreEqual(newComposition, assessmentSection.Composition);
            AssertCorrectOutputClearedWhenCompositionDune(unaffectedObjects, assessmentSection);
            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);

            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.AssertHydraulicBoundaryLocationCalculationsHaveNoOutputs(grassCoverErosionOutwardsFailureMechanism);
            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(duneErosionFailureMechanism);

            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations(oldComposition);

            IEnumerable<ICalculation> expectedUnaffectedObjects = assessmentSection.GetFailureMechanisms()
                                                                                   .SelectMany(fm => fm.Calculations)
                                                                                   .Where(calc => calc.HasOutput)
                                                                                   .ToArray();

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            IEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculationsWithOutput =
                GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.GetAllHydraulicBoundaryLocationCalculationsWithOutput(grassCoverErosionOutwardsFailureMechanism);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            IEnumerable<IObservable> expectedAffectedObjects = GetAllAffectedDuneLocationCalculations(duneErosionFailureMechanism)
                                                               .Concat(new IObservable[]
                                                               {
                                                                   assessmentSection
                                                               })
                                                               .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            const string expectedMessage = "Alle berekende hydraulische belastingen van de relevante toetssporen zijn verwijderd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);

            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);
            AssertOutputNotCleared(expectedUnaffectedObjects, assessmentSection.GetFailureMechanisms());

            Assert.IsTrue(hydraulicBoundaryLocationCalculationsWithOutput.All(c => c.HasOutput));
            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(duneErosionFailureMechanism);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void ChangeComposition_ChangeToDuneAndNoCalculationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                            AssessmentSectionComposition newComposition)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput(oldComposition);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            IEnumerable<IObservable> expectedAffectedObjects =
                GetAllAffectedDuneLocationCalculations(duneErosionFailureMechanism)
                    .Concat(GetAllAffectedGrassCoverErosionOutwardsCalculations(grassCoverErosionOutwardsFailureMechanism))
                    .Concat(new IObservable[]
                    {
                        assessmentSection
                    })
                    .Concat(GetDuneRelevantFailureMechanisms(assessmentSection)
                            .SelectMany(fm => fm.Calculations)
                            .Where(calc => calc.HasOutput))
                    .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Alle berekende hydraulische belastingen van de relevante toetssporen zijn verwijderd.", 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);
            Assert.True(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).All(c => !c.HasOutput));
            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.AssertHydraulicBoundaryLocationCalculationsHaveNoOutputs(grassCoverErosionOutwardsFailureMechanism);
            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(duneErosionFailureMechanism);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(Dune,Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_ChangeToNonDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(Dune,DikeDune)")]
        public void ChangeComposition_ChangeToNonDuneAndNoCalculationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                               AssessmentSectionComposition newComposition)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutCalculationOutput(oldComposition);

            GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
            IEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculationsWithOutput =
                GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.GetAllHydraulicBoundaryLocationCalculationsWithOutput(failureMechanism);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            IEnumerable<IObservable> expectedAffectedObjects =
                GetAllAffectedDuneLocationCalculations(duneErosionFailureMechanism).Concat(new IObservable[]
                                                                                   {
                                                                                       assessmentSection
                                                                                   })
                                                                                   .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Alle berekende hydraulische belastingen van de relevante toetssporen zijn verwijderd.", 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);
            Assert.True(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).All(c => !c.HasOutput));
            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);

            Assert.IsTrue(hydraulicBoundaryLocationCalculationsWithOutput.All(calc => calc.HasOutput));
            DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(duneErosionFailureMechanism);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_ChangeToDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(Dike,Dune)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_ChangeToDuneNoLocationsWithOutput_ChangeCompositionReturnsAllAffectedObjects(DikeDune,Dune)")]
        public void ChangeComposition_ChangeToDuneAndNoHydraulicBoudaryLocationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                                                                         AssessmentSectionComposition newComposition)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

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

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "De resultaten van 28 berekeningen zijn verwijderd.", 1);
            Assert.AreEqual(newComposition, assessmentSection.Composition);

            // Assert 
            AssertCorrectOutputClearedWhenCompositionDune(notAffectedObjects, assessmentSection);
            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);
            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsWithoutHydraulicBoundaryLocationAndDuneOutput(oldComposition);

            IEnumerable<ICalculation> expectedUnaffectedObjects = assessmentSection.GetFailureMechanisms()
                                                                                   .SelectMany(fm => fm.Calculations)
                                                                                   .Where(calc => calc.HasOutput)
                                                                                   .ToArray();

            IObservable[] expectedAffectedObjects =
            {
                assessmentSection
            };

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            Assert.AreEqual(newComposition, assessmentSection.Composition);
            AssertOutputNotCleared(expectedUnaffectedObjects, assessmentSection.GetFailureMechanisms());

            CollectionAssert.IsSubsetOf(expectedAffectedObjects, affectedObjects);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dike, DikeDune)")]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dike, Dune)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(DikeDune, Dike)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(DikeDune, Dune)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dune, Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dune, DikeDune)")]
        public void ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                                          AssessmentSectionComposition newComposition)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(oldComposition);

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            IEnumerable<IObservable> expectedAffectedObjects = new List<IObservable>(assessmentSection.GetFailureMechanisms())
            {
                assessmentSection
            };

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune, 0, TestName = "ChangeComposition_RelevancyChanged_CloseViewsForIrrelevantFailureMechanisms(Dike, DikeDune)")]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune, 10, TestName = "ChangeComposition_RelevancyChanged_CloseViewsForIrrelevantFailureMechanisms(Dike, Dune)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike, 1, TestName = "ChangeComposition_RelevancyChanged_CloseViewsForIrrelevantFailureMechanisms(DikeDune, Dike)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune, 10, TestName = "ChangeComposition_RelevancyChanged_CloseViewsForIrrelevantFailureMechanisms(DikeDune, Dune)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike, 1, TestName = "ChangeComposition_RelevancyChanged_CloseViewsForIrrelevantFailureMechanisms(Dune, Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune, 0, TestName = "ChangeComposition_RelevancyChanged_CloseViewsForIrrelevantFailureMechanisms(Dune, DikeDune)")]
        public void ChangeComposition_RelevancyChanged_CloseViewsForIrrelevantFailureMechanisms(AssessmentSectionComposition oldComposition,
                                                                                                AssessmentSectionComposition newComposition,
                                                                                                int expectedNumberOfCalls)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(Arg<IFailureMechanism>.Matches(fm => !fm.IsRelevant)))
                        .Repeat.Times(expectedNumberOfCalls);
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(oldComposition);

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            mocks.VerifyAll();
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

        #region Grass Cover Erosion Outwards failure mechanism helpers

        private static IEnumerable<IObservable> GetAllAffectedGrassCoverErosionOutwardsCalculations(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.GetAllHydraulicBoundaryLocationCalculationsWithOutput(failureMechanism);
        }

        #endregion

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