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
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Commands;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.StandAlone;

namespace Ringtoets.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class ReferenceLineUpdateHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ReferenceLineUpdateHandler(null, viewCommands);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ViewCommandsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ReferenceLineUpdateHandler(assessmentSection, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("viewCommands", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            var handler = new ReferenceLineUpdateHandler(assessmentSection, viewCommands);

            // Assert
            Assert.IsInstanceOf<IReferenceLineUpdateHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ConfirmUpdate_ClickDialog_ReturnTrueIfOkAndFalseIfCancel(bool clickOk)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            string dialogTitle = null, dialogMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                dialogTitle = tester.Title;
                dialogMessage = tester.Text;
                if (clickOk)
                {
                    tester.ClickOk();
                }
                else
                {
                    tester.ClickCancel();
                }
            };

            var handler = new ReferenceLineUpdateHandler(assessmentSection, viewCommands);

            // Call
            bool result = handler.ConfirmUpdate();

            // Assert
            Assert.AreEqual(clickOk, result);

            Assert.AreEqual("Bevestigen", dialogTitle);
            Assert.AreEqual("Na het importeren van een aangepaste ligging van de referentielijn zullen alle geïmporteerde en berekende gegevens van alle toetssporen worden gewist." + Environment.NewLine +
                            Environment.NewLine +
                            "Wilt u doorgaan?",
                            dialogMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_OriginalReferenceLineNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var handler = new ReferenceLineUpdateHandler(assessmentSection, viewCommands);

            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () => handler.Update(null, referenceLine);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("originalReferenceLine", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_NewReferenceLineNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var handler = new ReferenceLineUpdateHandler(assessmentSection, viewCommands);

            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () => handler.Update(referenceLine, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("newReferenceLine", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_FullyConfiguredAssessmentSection_AllReferenceLineDependentDataCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            var handler = new ReferenceLineUpdateHandler(assessmentSection, viewCommands);

            ReferenceLine referenceLine = ReferenceLineTestFactory.CreateReferenceLineWithGeometry();

            // Call
            IObservable[] observables = handler.Update(assessmentSection.ReferenceLine, referenceLine).ToArray();

            // Assert
            Assert.AreEqual(59, observables.Length);

            PipingFailureMechanism pipingFailureMechanism = assessmentSection.Piping;
            CollectionAssert.IsEmpty(pipingFailureMechanism.Sections);
            CollectionAssert.IsEmpty(pipingFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, pipingFailureMechanism);
            CollectionAssert.Contains(observables, pipingFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(pipingFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, pipingFailureMechanism.CalculationsGroup);
            CollectionAssert.IsEmpty(pipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.Contains(observables, pipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(pipingFailureMechanism.SurfaceLines);
            CollectionAssert.Contains(observables, pipingFailureMechanism.SurfaceLines);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, grassCoverErosionInwardsFailureMechanism);
            CollectionAssert.Contains(observables, grassCoverErosionInwardsFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, grassCoverErosionInwardsFailureMechanism.CalculationsGroup);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.DikeProfiles);
            CollectionAssert.Contains(observables, grassCoverErosionInwardsFailureMechanism.DikeProfiles);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, grassCoverErosionOutwardsFailureMechanism);
            CollectionAssert.Contains(observables, grassCoverErosionOutwardsFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.Contains(observables, grassCoverErosionOutwardsFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.IsEmpty(grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, waveImpactAsphaltCoverFailureMechanism);
            CollectionAssert.Contains(observables, waveImpactAsphaltCoverFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.Contains(observables, waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.IsEmpty(waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles);

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, stabilityStoneCoverFailureMechanism);
            CollectionAssert.Contains(observables, stabilityStoneCoverFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.Contains(observables, stabilityStoneCoverFailureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.IsEmpty(stabilityStoneCoverFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, stabilityStoneCoverFailureMechanism.ForeshoreProfiles);

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(closingStructuresFailureMechanism.ClosingStructures);
            CollectionAssert.Contains(observables, closingStructuresFailureMechanism.ClosingStructures);

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(heightStructuresFailureMechanism.HeightStructures);
            CollectionAssert.Contains(observables, heightStructuresFailureMechanism.HeightStructures);

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism.CalculationsGroup);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(stabilityPointStructuresFailureMechanism.StabilityPointStructures);
            CollectionAssert.Contains(observables, stabilityPointStructuresFailureMechanism.StabilityPointStructures);

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            CollectionAssert.IsEmpty(duneErosionFailureMechanism.Sections);
            CollectionAssert.IsEmpty(duneErosionFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, duneErosionFailureMechanism);
            CollectionAssert.Contains(observables, duneErosionFailureMechanism.SectionResults);

            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = assessmentSection.MacroStabilityInwards;
            CollectionAssert.IsEmpty(macroStabilityInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(macroStabilityInwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, macroStabilityInwardsFailureMechanism);
            CollectionAssert.Contains(observables, macroStabilityInwardsFailureMechanism.SectionResults);
            CollectionAssert.IsEmpty(macroStabilityInwardsFailureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, macroStabilityInwardsFailureMechanism.CalculationsGroup);
            CollectionAssert.IsEmpty(macroStabilityInwardsFailureMechanism.StochasticSoilModels);
            CollectionAssert.Contains(observables, macroStabilityInwardsFailureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(macroStabilityInwardsFailureMechanism.SurfaceLines);
            CollectionAssert.Contains(observables, macroStabilityInwardsFailureMechanism.SurfaceLines);

            MacroStabilityOutwardsFailureMechanism macroStabilityOutwardsFailureMechanism = assessmentSection.MacroStabilityOutwards;
            CollectionAssert.IsEmpty(macroStabilityOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(macroStabilityOutwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, macroStabilityOutwardsFailureMechanism);
            CollectionAssert.Contains(observables, macroStabilityOutwardsFailureMechanism.SectionResults);

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            CollectionAssert.IsEmpty(microstabilityFailureMechanism.Sections);
            CollectionAssert.IsEmpty(microstabilityFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, microstabilityFailureMechanism);
            CollectionAssert.Contains(observables, microstabilityFailureMechanism.SectionResults);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            CollectionAssert.IsEmpty(waterPressureAsphaltCoverFailureMechanism.Sections);
            CollectionAssert.IsEmpty(waterPressureAsphaltCoverFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, waterPressureAsphaltCoverFailureMechanism);
            CollectionAssert.Contains(observables, waterPressureAsphaltCoverFailureMechanism.SectionResults);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            CollectionAssert.IsEmpty(grassCoverSlipOffOutwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverSlipOffOutwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, grassCoverSlipOffOutwardsFailureMechanism);
            CollectionAssert.Contains(observables, grassCoverSlipOffOutwardsFailureMechanism.SectionResults);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            CollectionAssert.IsEmpty(grassCoverSlipOffInwardsFailureMechanism.Sections);
            CollectionAssert.IsEmpty(grassCoverSlipOffInwardsFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, grassCoverSlipOffInwardsFailureMechanism);
            CollectionAssert.Contains(observables, grassCoverSlipOffInwardsFailureMechanism.SectionResults);

            StrengthStabilityLengthwiseConstructionFailureMechanism stabilityLengthwiseConstructionFailureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            CollectionAssert.IsEmpty(stabilityLengthwiseConstructionFailureMechanism.Sections);
            CollectionAssert.IsEmpty(stabilityLengthwiseConstructionFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, stabilityLengthwiseConstructionFailureMechanism);
            CollectionAssert.Contains(observables, stabilityLengthwiseConstructionFailureMechanism.SectionResults);

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            CollectionAssert.IsEmpty(pipingStructureFailureMechanism.Sections);
            CollectionAssert.IsEmpty(pipingStructureFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, pipingStructureFailureMechanism);
            CollectionAssert.Contains(observables, pipingStructureFailureMechanism.SectionResults);

            TechnicalInnovationFailureMechanism technicalInnovationFailureMechanism = assessmentSection.TechnicalInnovation;
            CollectionAssert.IsEmpty(technicalInnovationFailureMechanism.Sections);
            CollectionAssert.IsEmpty(technicalInnovationFailureMechanism.SectionResults);
            CollectionAssert.Contains(observables, technicalInnovationFailureMechanism);
            CollectionAssert.Contains(observables, technicalInnovationFailureMechanism.SectionResults);

            CollectionAssert.AreEqual(referenceLine.Points, assessmentSection.ReferenceLine.Points);

            mocks.VerifyAll();
        }

        [Test]
        public void DoPostUpdateActions_NoUpdateCalled_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var handler = new ReferenceLineUpdateHandler(assessmentSection, viewCommands);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll(); // Expect not calls in 'viewCommands'
        }

        [Test]
        public void DoPostUpdateActions_AfterUpdatingReferenceLine_CloseViewsForRemovedData()
        {
            // Setup
            const int expectedNumberOfRemovedInstances = 195;

            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(Arg<object>.Is.NotNull))
                        .Repeat.Times(expectedNumberOfRemovedInstances);
            mocks.ReplayAll();

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            var handler = new ReferenceLineUpdateHandler(assessmentSection, viewCommands);
            handler.Update(assessmentSection.ReferenceLine, new ReferenceLine());

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostUpdateActions_CalledSecondTimeAfterUpdateAndUpdateCycle_DoNothing()
        {
            // Setup
            const int expectedNumberOfRemovedInstances = 195;

            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(Arg<object>.Is.NotNull))
                        .Repeat.Times(expectedNumberOfRemovedInstances);
            mocks.ReplayAll();

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            var handler = new ReferenceLineUpdateHandler(assessmentSection, viewCommands);
            handler.Update(assessmentSection.ReferenceLine, new ReferenceLine());
            handler.DoPostUpdateActions();

            // Call
            handler.DoPostUpdateActions(); // Expected number should be identical to that of DoPostUpdateActions_AfterUpdatingReferenceLine_CloseViewsForRemovedData

            // Assert
            mocks.VerifyAll();
        }
    }
}