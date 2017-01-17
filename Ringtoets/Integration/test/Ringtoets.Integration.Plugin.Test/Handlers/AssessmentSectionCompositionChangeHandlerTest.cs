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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.Service;
using Ringtoets.Integration.TestUtils;

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
        public void ChangeComposition_ChangeToDifferentValue_ChangeCompositionAndClearAllCalculationOutputAndReturnsAllAffectedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();
            var newComposition = AssessmentSectionComposition.DikeAndDune;

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, newComposition);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                                                   .OfType<GrassCoverErosionOutwardsFailureMechanism>()
                                                                                                                   .First();

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .First();

            IObservable[] expectedAffectedObjects = assessmentSection.GetFailureMechanisms().Cast<IObservable>()
                                                                     .Concat(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(c => c.HasOutput))
                                                                     .Concat(new[]
                                                                     {
                                                                         assessmentSection
                                                                     })
                                                                     .Concat(new IObservable[]
                                                                     {
                                                                         grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations,
                                                                         duneErosionFailureMechanism.DuneLocations
                                                                     })
                                                                     .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            string[] expectedMessage =
            {
                "De resultaten van 32 berekeningen zijn verwijderd.",
                "Alle berekende resultaten voor alle hydraulische randvoorwaardenlocaties van grasbekleding erosie binnentalud en duinen zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessage, 2);

            Assert.AreEqual(newComposition, assessmentSection.Composition);
            Assert.True(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).All(c => !c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);

            foreach (HydraulicBoundaryLocation location in assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations)
            {
                Assert.IsNaN(location.DesignWaterLevel);
                Assert.IsNaN(location.WaveHeight);
                Assert.AreEqual(CalculationConvergence.NotCalculated, location.DesignWaterLevelCalculationConvergence);
                Assert.AreEqual(CalculationConvergence.NotCalculated, location.WaveHeightCalculationConvergence);
            }
            foreach (DuneLocation duneLocation in assessmentSection.DuneErosion.DuneLocations)
            {
                Assert.IsNull(duneLocation.Output);
            }
        }

        [Test]
        public void ChangeComposition_ChangeToDifferentValueAndNoCalculationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();
            RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection);
            var newComposition = AssessmentSectionComposition.Dune;

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, newComposition);
            CollectionAssert.IsEmpty(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(c => c.HasOutput));

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                                                   .OfType<GrassCoverErosionOutwardsFailureMechanism>()
                                                                                                                   .First();

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                       .OfType<DuneErosionFailureMechanism>()
                                                                                       .First();

            IObservable[] expectedAffectedObjects = assessmentSection.GetFailureMechanisms().Cast<IObservable>()
                                                                     .Concat(new[]
                                                                     {
                                                                         assessmentSection
                                                                     })
                                                                     .Concat(new IObservable[]
                                                                     {
                                                                         grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations,
                                                                         duneErosionFailureMechanism.DuneLocations
                                                                     })
                                                                     .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Alle berekende resultaten voor alle hydraulische randvoorwaardenlocaties van grasbekleding erosie binnentalud en duinen zijn verwijderd.", 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);
            Assert.True(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).All(c => !c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);

            foreach (HydraulicBoundaryLocation location in assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations)
            {
                Assert.IsNaN(location.DesignWaterLevel);
                Assert.IsNaN(location.WaveHeight);
                Assert.AreEqual(CalculationConvergence.NotCalculated, location.DesignWaterLevelCalculationConvergence);
                Assert.AreEqual(CalculationConvergence.NotCalculated, location.WaveHeightCalculationConvergence);
            }
            foreach (DuneLocation duneLocation in assessmentSection.DuneErosion.DuneLocations)
            {
                Assert.IsNull(duneLocation.Output);
            }
        }

        [Test]
        public void ChangeComposition_ChangeToDifferentValueAndNoHydraulicBoudaryLocationsWithOutput_ChangeCompositionAndReturnsAllAffectedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetFullyConfiguredAssessmentSection();
            RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(assessmentSection.GrassCoverErosionOutwards,
                                                                                     assessmentSection.DuneErosion);
            var newComposition = AssessmentSectionComposition.Dune;

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, newComposition);
            CollectionAssert.IsEmpty(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Where(loc => loc.DesignWaterLevelOutput != null
                                                                                                                         || loc.WaveHeightOutput != null));
            CollectionAssert.IsEmpty(assessmentSection.DuneErosion.DuneLocations.Where(dl => dl.Output != null));

            IObservable[] expectedAffectedObjects = assessmentSection.GetFailureMechanisms().Cast<IObservable>()
                                                                     .Concat(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).Where(c => c.HasOutput))
                                                                     .Concat(new[]
                                                                     {
                                                                         assessmentSection
                                                                     })
                                                                     .ToArray();

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = null;
            Action call = () => affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "De resultaten van 32 berekeningen zijn verwijderd.", 1);

            Assert.AreEqual(newComposition, assessmentSection.Composition);
            Assert.True(assessmentSection.GetFailureMechanisms().SelectMany(fm => fm.Calculations).All(c => !c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }
    }
}