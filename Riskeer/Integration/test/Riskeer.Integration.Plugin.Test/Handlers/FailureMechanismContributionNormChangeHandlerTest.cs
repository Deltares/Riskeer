﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Util.Extensions;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Revetment.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class FailureMechanismContributionNormChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismContributionNormChangeHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismContributionNormChangeHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ChangeNormativeNormType_ActionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            void Call() => handler.ChangeNormativeNormType(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("action", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeNormativeNormType_WithAction_ConfirmationRequired()
        {
            // Setup
            var title = "";
            var message = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                title = tester.Title;
                message = tester.Text;

                tester.ClickCancel();
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            handler.ChangeNormativeNormType(() => {});

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u de norm aanpast, dan worden de rekenresultaten van semi-probabilistische berekeningen zonder handmatige waterstand verwijderd."
                                     + Environment.NewLine
                                     + Environment.NewLine
                                     + "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsWithOutput_WhenChangingNormativeNormType_ThenAllDependingOutputClearedAndActionPerformedAndAllAffectedObjectsNotified()
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            var expectedAffectedCalculations = new List<ICalculation>();
            expectedAffectedCalculations.AddRange(assessmentSection.Piping.Calculations
                                                                   .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                                   .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput));
            expectedAffectedCalculations.AddRange(assessmentSection.MacroStabilityInwards.Calculations
                                                                   .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                   .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput));

            IEnumerable<IObservable> expectedAffectedObjects =
                expectedAffectedCalculations.Concat(new IObservable[]
                                            {
                                                assessmentSection.FailureMechanismContribution
                                            })
                                            .ToArray();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(expectedAffectedObjects.Count());
            mocks.ReplayAll();

            expectedAffectedObjects.ForEachElementDo(obj => obj.Attach(observer));

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNormativeNormType(() => actionPerformed = true);

            // Then
            var expectedMessages = new[]
            {
                $"De resultaten van {expectedAffectedCalculations.Count} semi-probabilistische berekeningen zonder handmatige waterstand zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(Call, expectedMessages, 1);
            Assert.IsTrue(actionPerformed);
            CollectionAssert.IsEmpty(expectedAffectedCalculations.Where(c => c.HasOutput));
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsWithoutOutput_WhenChangingNormativeNormType_ThenActionPerformedAndContributionNotified()
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            var calculations = new List<ICalculation>();
            calculations.AddRange(assessmentSection.Piping.Calculations
                                                   .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                   .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput));
            calculations.AddRange(assessmentSection.MacroStabilityInwards.Calculations
                                                   .OfType<MacroStabilityInwardsCalculationScenario>()
                                                   .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput));

            calculations.ForEachElementDo(c => c.ClearOutput());

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Once();
            mocks.ReplayAll();

            assessmentSection.FailureMechanismContribution.Attach(observer);

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNormativeNormType(() => actionPerformed = true);

            // Then
            TestHelper.AssertLogMessagesCount(Call, 0);
            Assert.IsTrue(actionPerformed);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeNormativeNormActionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            void Call() => handler.ChangeNormativeNorm(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("action", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeNormativeNorm_WithAction_ConfirmationRequired()
        {
            // Setup
            var title = "";
            var message = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                title = tester.Title;
                message = tester.Text;

                tester.ClickCancel();
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            handler.ChangeNormativeNorm(() => {});

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u de norm aanpast, dan worden de rekenresultaten van alle hydraulische belastingenlocaties behorende bij deze norm en semi-probabilistische berekeningen zonder handmatige waterstand verwijderd."
                                     + Environment.NewLine
                                     + Environment.NewLine
                                     + "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetChangeNormativeNormCases))]
        public void GivenCalculationsWithOutput_WhenChangingNormativeNorm_ThenAllDependingOutputClearedAndActionPerformedAndAllAffectedObjectsNotified(
            NormativeProbabilityType normativeProbabilityType, Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getLocationCalculationsFunc,
            WaveConditionsInputWaterLevelType waterLevelType)
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            assessmentSection.FailureMechanismContribution.NormativeNorm = normativeProbabilityType;

            IEnumerable<HydraulicBoundaryLocationCalculation> expectedLocationCalculationsToClear = getLocationCalculationsFunc(assessmentSection);

            var waveConditionsCalculations = new List<ICalculation<WaveConditionsInput>>();
            waveConditionsCalculations.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                                 .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                                 .Where(c => c.HasOutput));
            waveConditionsCalculations.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                                 .Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                                                 .Where(c => c.HasOutput));
            waveConditionsCalculations.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                                 .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                                                 .Where(c => c.HasOutput));

            waveConditionsCalculations.ForEachElementDo(c => c.InputParameters.WaterLevelType = waterLevelType);

            var expectedAffectedSemiProbabilisticCalculations = new List<ICalculation>();
            expectedAffectedSemiProbabilisticCalculations.AddRange(assessmentSection.Piping.Calculations
                                                                                    .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                                                    .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput));
            expectedAffectedSemiProbabilisticCalculations.AddRange(assessmentSection.MacroStabilityInwards.Calculations
                                                                                    .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                    .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput));

            IEnumerable<IObservable> expectedAffectedObjects =
                expectedAffectedSemiProbabilisticCalculations.Concat(new IObservable[]
                                                             {
                                                                 assessmentSection.FailureMechanismContribution
                                                             })
                                                             .Concat(expectedLocationCalculationsToClear)
                                                             .Concat(waveConditionsCalculations)
                                                             .ToArray();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(expectedAffectedObjects.Count());
            mocks.ReplayAll();

            expectedAffectedObjects.ForEachElementDo(obj => obj.Attach(observer));

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Precondition
            CollectionAssert.IsNotEmpty(expectedAffectedSemiProbabilisticCalculations.Where(c => c.HasOutput));
            CollectionAssert.IsNotEmpty(expectedLocationCalculationsToClear.Where(c => c.HasOutput));
            CollectionAssert.IsNotEmpty(waveConditionsCalculations.Where(c => c.HasOutput));

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNormativeNorm(() => actionPerformed = true);

            // Then
            var expectedMessages = new[]
            {
                "Alle berekende hydraulische belastingen behorende bij de gewijzigde norm zijn verwijderd.",
                $"De resultaten van {expectedAffectedSemiProbabilisticCalculations.Count} semi-probabilistische berekeningen zonder handmatige waterstand zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(Call, expectedMessages, 2);

            Assert.IsTrue(actionPerformed);
            CollectionAssert.IsEmpty(expectedAffectedSemiProbabilisticCalculations.Where(c => c.HasOutput));
            CollectionAssert.IsEmpty(expectedLocationCalculationsToClear.Where(c => c.HasOutput));
            CollectionAssert.IsEmpty(waveConditionsCalculations.Where(c => c.HasOutput));
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetChangeNormativeNormCases))]
        public void GivenCalculationsWithoutOutput_WhenChangingNormativeNorm_ThenActionPerformedAndContributionNotified(
            NormativeProbabilityType normativeProbabilityType, Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getLocationCalculationsFunc,
            WaveConditionsInputWaterLevelType waterLevelType)
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            assessmentSection.FailureMechanismContribution.NormativeNorm = normativeProbabilityType;

            IEnumerable<HydraulicBoundaryLocationCalculation> calculationsBelongingToNorm = getLocationCalculationsFunc(assessmentSection);

            var waveConditionsCalculations = new List<ICalculation<WaveConditionsInput>>();
            waveConditionsCalculations.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                                 .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                                 .Cast<StabilityStoneCoverWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                                 .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>());

            waveConditionsCalculations.ForEachElementDo(c => c.InputParameters.WaterLevelType = waterLevelType);

            var calculations = new List<ICalculation>();
            calculations.AddRange(assessmentSection.Piping.Calculations
                                                   .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                   .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput));
            calculations.AddRange(assessmentSection.MacroStabilityInwards.Calculations
                                                   .OfType<MacroStabilityInwardsCalculationScenario>()
                                                   .Where(c => c.HasOutput && !c.InputParameters.UseAssessmentLevelManualInput));
            calculations.AddRange(waveConditionsCalculations);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Once();
            mocks.ReplayAll();

            calculations.ForEachElementDo(c =>
            {
                c.ClearOutput();
                c.Attach(observer);
            });
            calculationsBelongingToNorm.ForEachElementDo(c =>
            {
                c.Output = null;
                c.Attach(observer);
            });
            assessmentSection.FailureMechanismContribution.Attach(observer);

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNormativeNorm(() => actionPerformed = true);

            // Then
            TestHelper.AssertLogMessagesCount(Call, 0);
            Assert.IsTrue(actionPerformed);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeNormActionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            void Call() => handler.ChangeNorm(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("action", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ChangeNorm_WithAction_ConfirmationRequired()
        {
            // Setup
            var title = "";
            var message = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                title = tester.Title;
                message = tester.Text;

                tester.ClickCancel();
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Call
            handler.ChangeNorm(() => {});

            // Assert
            Assert.AreEqual("Bevestigen", title);
            string expectedMessage = "Als u de norm aanpast, dan worden de rekenresultaten van alle hydraulische belastingenlocaties behorende bij deze norm verwijderd."
                                     + Environment.NewLine
                                     + Environment.NewLine
                                     + "Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedMessage, message);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetChangeNormCases))]
        public void GivenCalculationsWithOutput_WhenChangingNorm_ThenAllDependingOutputClearedAndActionPerformedAndAllAffectedObjectsNotified(
            NormativeProbabilityType normativeProbabilityType, Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc,
            WaveConditionsInputWaterLevelType waterLevelType)
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            assessmentSection.FailureMechanismContribution.NormativeNorm = normativeProbabilityType;

            IEnumerable<HydraulicBoundaryLocationCalculation> expectedCalculationsToClear = getCalculationsFunc(assessmentSection);

            var waveConditionsCalculations = new List<ICalculation<WaveConditionsInput>>();
            waveConditionsCalculations.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                                 .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                                 .Where(c => c.HasOutput));
            waveConditionsCalculations.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                                 .Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                                                 .Where(c => c.HasOutput));
            waveConditionsCalculations.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                                 .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                                                 .Where(c => c.HasOutput));

            waveConditionsCalculations.ForEachElementDo(c => c.InputParameters.WaterLevelType = waterLevelType);

            IEnumerable<IObservable> expectedAffectedObjects = new IObservable[]
                                                               {
                                                                   assessmentSection.FailureMechanismContribution
                                                               }
                                                               .Concat(expectedCalculationsToClear)
                                                               .Concat(waveConditionsCalculations)
                                                               .ToArray();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(expectedAffectedObjects.Count());
            mocks.ReplayAll();

            expectedAffectedObjects.ForEachElementDo(obj => obj.Attach(observer));

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // Precondition
            CollectionAssert.IsNotEmpty(expectedCalculationsToClear.Where(c => c.HasOutput));
            CollectionAssert.IsNotEmpty(waveConditionsCalculations.Where(c => c.HasOutput));

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNorm(() => actionPerformed = true);

            // Then
            var expectedMessages = new[]
            {
                "Alle berekende hydraulische belastingen behorende bij de gewijzigde norm zijn verwijderd."
            };
            TestHelper.AssertLogMessagesAreGenerated(Call, expectedMessages, 1);

            Assert.IsTrue(actionPerformed);
            CollectionAssert.IsEmpty(expectedCalculationsToClear.Where(c => c.HasOutput));
            CollectionAssert.IsEmpty(waveConditionsCalculations.Where(c => c.HasOutput));
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetChangeNormCases))]
        public void GivenCalculationsWithoutOutput_WhenChangingNorm_ThenActionPerformedAndContributionNotified(
            NormativeProbabilityType normativeProbabilityType, Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getLocationCalculationsFunc,
            WaveConditionsInputWaterLevelType waterLevelType)
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            assessmentSection.FailureMechanismContribution.NormativeNorm = normativeProbabilityType;

            IEnumerable<HydraulicBoundaryLocationCalculation> calculationsBelongingToNorm = getLocationCalculationsFunc(assessmentSection);

            var waveConditionsCalculations = new List<ICalculation<WaveConditionsInput>>();
            waveConditionsCalculations.AddRange(assessmentSection.GrassCoverErosionOutwards.Calculations
                                                                 .Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.StabilityStoneCover.Calculations
                                                                 .Cast<StabilityStoneCoverWaveConditionsCalculation>());
            waveConditionsCalculations.AddRange(assessmentSection.WaveImpactAsphaltCover.Calculations
                                                                 .Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>());

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Once();
            mocks.ReplayAll();

            calculationsBelongingToNorm.ForEachElementDo(c =>
            {
                c.Output = null;
                c.Attach(observer);
            });
            waveConditionsCalculations.ForEachElementDo(c =>
            {
                c.InputParameters.WaterLevelType = waterLevelType;
                c.ClearOutput();
                c.Attach(observer);
            });

            assessmentSection.FailureMechanismContribution.Attach(observer);

            var handler = new FailureMechanismContributionNormChangeHandler(assessmentSection);

            // When
            var actionPerformed = false;
            void Call() => handler.ChangeNorm(() => actionPerformed = true);

            // Then
            TestHelper.AssertLogMessagesCount(Call, 0);
            Assert.IsTrue(actionPerformed);
            mocks.VerifyAll();
        }

        private static IEnumerable<TestCaseData> GetChangeNormativeNormCases()
        {
            yield return new TestCaseData(
                NormativeProbabilityType.MaximumAllowableFloodingProbability, new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                    section => section.WaterLevelCalculationsForLowerLimitNorm),
                WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability);
            yield return new TestCaseData(
                NormativeProbabilityType.SignalFloodingProbability, new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                    section => section.WaterLevelCalculationsForSignalingNorm),
                WaveConditionsInputWaterLevelType.SignalFloodingProbability);
        }

        private static IEnumerable<TestCaseData> GetChangeNormCases()
        {
            yield return new TestCaseData(
                NormativeProbabilityType.MaximumAllowableFloodingProbability, new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                    section => section.WaterLevelCalculationsForSignalingNorm),
                WaveConditionsInputWaterLevelType.SignalFloodingProbability);
            yield return new TestCaseData(
                NormativeProbabilityType.SignalFloodingProbability, new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                    section => section.WaterLevelCalculationsForLowerLimitNorm),
                WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability);
        }
    }
}