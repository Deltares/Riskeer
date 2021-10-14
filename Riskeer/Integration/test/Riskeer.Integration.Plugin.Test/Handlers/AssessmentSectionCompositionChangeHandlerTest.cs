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
using Core.Gui.Commands;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.TestUtil;

namespace Riskeer.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class AssessmentSectionCompositionChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_ViewCommandsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionCompositionChangeHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
        public void ChangeComposition_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var handler = new AssessmentSectionCompositionChangeHandler(viewCommands);

            // Call
            void Call() => handler.ChangeComposition(null, AssessmentSectionComposition.Dike);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
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

            IEnumerable<DuneLocationCalculation> duneLocationCalculationsWithOutput = assessmentSection.DuneErosion.DuneLocationCalculationsForUserDefinedTargetProbabilities.SelectMany(tp => tp.DuneLocationCalculations)
                                                                                                       .Where(HasDuneLocationCalculationOutput)
                                                                                                       .ToArray();

            IEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculationsWithOutput = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Where(HasHydraulicBoundaryLocationCalculationOutput)
                                                                                                                                 .Concat(assessmentSection.WaterLevelCalculationsForSignalingNorm.Where(HasHydraulicBoundaryLocationCalculationOutput))
                                                                                                                                 .Concat(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.SelectMany(tp => tp.HydraulicBoundaryLocationCalculations))
                                                                                                                                 .ToArray();

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
                assessmentSection,
                assessmentSection.FailureMechanismContribution
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

        private static bool HasHydraulicBoundaryLocationCalculationOutput(HydraulicBoundaryLocationCalculation calculation)
        {
            return calculation.HasOutput;
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
    }
}