﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismContributionPropertiesIntegrationTest : NUnitFormTest
    {
        private const string messageAllHydraulicBoundaryLocationOutputCleared =
            "Alle berekende resultaten voor alle hydraulische randvoorwaardenlocaties zijn verwijderd.";

        private const string messageCalculationsremoved = "De resultaten van {0} berekeningen zijn verwijderd.";

        [Test]
        public void ReturnPeriodProperty_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            // Setup
            const int newReturnPeriod = 200;
            const int numberOfCalculations = 3;

            TestHydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateFullyCalculated();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            TestHydraulicBoundaryLocation grassCoverErosionOutwardsHydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateFullyCalculated();

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Add(grassCoverErosionOutwardsHydraulicBoundaryLocation);

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var pipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            pipingCalculationObserver.Expect(o => o.UpdateObserver());
            var grassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            grassCoverErosionInwardsCalculationObserver.Expect(o => o.UpdateObserver());
            var heightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();
            heightStructuresCalculationObserver.Expect(o => o.UpdateObserver());
            var emptyPipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            var emptyGrassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            var emptyHeightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();

            var hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>();
            hydraulicBoundaryDatabaseObserver.Expect(hbdo => hbdo.UpdateObserver());
            var grassCoverErosionOutwardsObserver = mockRepository.StrictMock<IObserver>();
            grassCoverErosionOutwardsObserver.Expect(o => o.UpdateObserver());

            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observer);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsObserver);

            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                new FailureMechanismContributionNormChangeHandler(assessmentSection),
                new AssessmentSectionCompositionChangeHandler(viewCommands));

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0 / failureMechanismContribution.Norm);
            Assert.AreEqual(originalReturnPeriodValue, properties.ReturnPeriod);
            Assert.IsTrue(hydraulicBoundaryLocation.WaveHeightCalculation.HasOutput);
            Assert.IsTrue(hydraulicBoundaryLocation.DesignWaterLevelCalculation.HasOutput);
            Assert.IsTrue(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculation.HasOutput);
            Assert.IsTrue(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevelCalculation.HasOutput);
            Assert.IsNotNull(pipingCalculation.Output);
            Assert.IsNotNull(pipingCalculation.SemiProbabilisticOutput);
            Assert.IsNotNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNotNull(heightStructuresCalculation.Output);

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => properties.ReturnPeriod = newReturnPeriod;

            // Assert
            TestHelper.AssertLogMessages(call, msgs =>
            {
                string[] messages = msgs.ToArray();
                Assert.AreEqual(string.Format(messageCalculationsremoved, numberOfCalculations), messages[0]);
                Assert.AreEqual(messageAllHydraulicBoundaryLocationOutputCleared, messages[1]);
            });
            Assert.AreEqual(1.0 / 30000, failureMechanismContribution.Norm);
            Assert.IsFalse(hydraulicBoundaryLocation.WaveHeightCalculation.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocation.DesignWaterLevelCalculation.HasOutput);
            Assert.IsFalse(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculation.HasOutput);
            Assert.IsFalse(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevelCalculation.HasOutput);
            Assert.IsNull(pipingCalculation.Output);
            Assert.IsNull(pipingCalculation.SemiProbabilisticOutput);
            Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNull(heightStructuresCalculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        public void ReturnPeriodProperty_HydraulicBoundarySetAndCalculationsNoOutput_HydraulicBoundaryDatabaseObserversNotifiedAndMessagesLogged()
        {
            // Setup
            const int newReturnPeriod = 200;
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    TestHydraulicBoundaryLocation.CreateFullyCalculated()
                }
            };

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculationObserver = mockRepository.StrictMock<IObserver>(); // No update observers expected.
            var hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>();
            hydraulicBoundaryDatabaseObserver.Expect(hbdo => hbdo.UpdateObserver());

            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observer);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                new FailureMechanismContributionNormChangeHandler(assessmentSection),
                new AssessmentSectionCompositionChangeHandler(viewCommands));

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0 / failureMechanismContribution.Norm);
            Assert.AreEqual(originalReturnPeriodValue, properties.ReturnPeriod);
            Assert.IsFalse(double.IsNaN(hydraulicBoundaryLocation.WaveHeight));
            Assert.IsFalse(double.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel));

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => properties.ReturnPeriod = newReturnPeriod;

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, messageAllHydraulicBoundaryLocationOutputCleared, 1);

            Assert.AreEqual(1.0 / 30000, failureMechanismContribution.Norm);
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);

            mockRepository.VerifyAll();
        }

        [Test]
        public void ReturnPeriodProperty_HydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            // Setup
            const int newReturnPeriod = 200;
            const int numberOfCalculations = 3;

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new TestHydraulicBoundaryLocation());

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var pipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            pipingCalculationObserver.Expect(o => o.UpdateObserver());
            var grassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            grassCoverErosionInwardsCalculationObserver.Expect(o => o.UpdateObserver());
            var heightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();
            heightStructuresCalculationObserver.Expect(o => o.UpdateObserver());
            var emptyPipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            var emptyGrassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            var emptyHeightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();

            var hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>(); // No update observer expected.

            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observer);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                new FailureMechanismContributionNormChangeHandler(assessmentSection),
                new AssessmentSectionCompositionChangeHandler(viewCommands));

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0 / failureMechanismContribution.Norm);
            Assert.AreEqual(originalReturnPeriodValue, properties.ReturnPeriod);
            Assert.IsNotNull(pipingCalculation.Output);
            Assert.IsNotNull(pipingCalculation.SemiProbabilisticOutput);
            Assert.IsNotNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNotNull(heightStructuresCalculation.Output);

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => properties.ReturnPeriod = newReturnPeriod;

            // Assert
            string expectedMessage = string.Format(messageCalculationsremoved,
                                                   numberOfCalculations);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(1.0 / 30000, failureMechanismContribution.Norm);
            Assert.IsNull(pipingCalculation.Output);
            Assert.IsNull(pipingCalculation.SemiProbabilisticOutput);
            Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNull(heightStructuresCalculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        public void ReturnPeriodProperty_NoHydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            // Setup
            const int newReturnPeriod = 200;
            const int numberOfCalculations = 3;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var pipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            pipingCalculationObserver.Expect(o => o.UpdateObserver());
            var grassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            grassCoverErosionInwardsCalculationObserver.Expect(o => o.UpdateObserver());
            var heightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();
            heightStructuresCalculationObserver.Expect(o => o.UpdateObserver());
            var emptyPipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            var emptyGrassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            var emptyHeightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();

            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observer);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                new FailureMechanismContributionNormChangeHandler(assessmentSection),
                new AssessmentSectionCompositionChangeHandler(viewCommands));

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0 / failureMechanismContribution.Norm);
            Assert.AreEqual(originalReturnPeriodValue, properties.ReturnPeriod);
            Assert.IsNotNull(pipingCalculation.Output);
            Assert.IsNotNull(pipingCalculation.SemiProbabilisticOutput);
            Assert.IsNotNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNotNull(heightStructuresCalculation.Output);

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => properties.ReturnPeriod = newReturnPeriod;

            // Assert
            string expectedMessage = string.Format(messageCalculationsremoved,
                                                   numberOfCalculations);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(1.0 / 30000, failureMechanismContribution.Norm);
            Assert.IsNull(pipingCalculation.Output);
            Assert.IsNull(pipingCalculation.SemiProbabilisticOutput);
            Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNull(heightStructuresCalculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        public void ReturnPeriodProperty_HydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            // Setup
            const int newReturnPeriod = 200;

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new TestHydraulicBoundaryLocation());

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculationObserver = mockRepository.StrictMock<IObserver>();
            var hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>();

            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observer);
            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                new FailureMechanismContributionNormChangeHandler(assessmentSection),
                new AssessmentSectionCompositionChangeHandler(viewCommands));

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0 / failureMechanismContribution.Norm);
            Assert.AreEqual(originalReturnPeriodValue, properties.ReturnPeriod);

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => properties.ReturnPeriod = newReturnPeriod;

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.AreEqual(1.0 / 30000, failureMechanismContribution.Norm);

            mockRepository.VerifyAll(); // No update observer expected.
        }

        [Test]
        public void ReturnPeriodProperty_NoHydraulicBoundaryDatabaseAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            // Setup
            var mockRepository = new MockRepository();
            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            const int newReturnPeriod = 200;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var properties = new FailureMechanismContributionProperties(
                failureMechanismContribution,
                assessmentSection,
                new FailureMechanismContributionNormChangeHandler(assessmentSection),
                new AssessmentSectionCompositionChangeHandler(viewCommands));

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0 / failureMechanismContribution.Norm);
            Assert.AreEqual(originalReturnPeriodValue, properties.ReturnPeriod);

            // Call
            failureMechanismContribution.Norm = 1.0 / newReturnPeriod;
            Action call = () => failureMechanismContribution.NotifyObservers();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.AreEqual(1.0 / 30000, failureMechanismContribution.Norm);
            mockRepository.VerifyAll();
        }
    }
}