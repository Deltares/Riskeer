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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
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

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class FailureMechanismContributionContextPropertiesIntegrationTest : NUnitFormTest
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

            var waveHeight = (RoundedDouble) 3.0;
            var designWaterLevel = (RoundedDouble) 4.2;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(designWaterLevel, waveHeight);
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
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new TestDikeHeightAssessmentOutput(0))
            };
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var grassCoverErosionOutwardsHydraulicBoundaryLocation = hydraulicBoundaryLocation;
            grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightOutput = new TestHydraulicBoundaryLocationOutput(
                waveHeight);
            grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(
                designWaterLevel);

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Add(grassCoverErosionOutwardsHydraulicBoundaryLocation);

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            MockRepository mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            IObserver pipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            pipingCalculationObserver.Expect(o => o.UpdateObserver());
            IObserver grassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            grassCoverErosionInwardsCalculationObserver.Expect(o => o.UpdateObserver());
            IObserver heightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();
            heightStructuresCalculationObserver.Expect(o => o.UpdateObserver());
            IObserver emptyPipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            IObserver emptyGrassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            IObserver emptyHeightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();

            IObserver hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>();
            hydraulicBoundaryDatabaseObserver.Expect(hbdo => hbdo.UpdateObserver());
            IObserver grassCoverErosionOutwardsObserver = mockRepository.StrictMock<IObserver>();
            grassCoverErosionOutwardsObserver.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsObserver);

            var properties = new FailureMechanismContributionContextProperties()
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection,
                NormChangeHandler = new FailureMechanismContributionNormChangeHandler()
            };

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
            Assert.AreEqual(originalReturnPeriodValue, properties.ReturnPeriod);
            Assert.AreEqual(waveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
            Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight.GetAccuracy());
            Assert.AreEqual(designWaterLevel, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());

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
            Assert.AreEqual(1.0/newReturnPeriod, failureMechanismContribution.Norm);
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
            Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel);
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

            var waveHeight = (RoundedDouble) 3.0;
            var designWaterLevel = (RoundedDouble) 4.2;
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new TestHydraulicBoundaryLocation(designWaterLevel, waveHeight));

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };

            PipingCalculation emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            GrassCoverErosionInwardsCalculation emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            MockRepository mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            IObserver calculationObserver = mockRepository.StrictMock<IObserver>(); // No update observers expected.
            IObserver hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>();
            hydraulicBoundaryDatabaseObserver.Expect(hbdo => hbdo.UpdateObserver());
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            var properties = new FailureMechanismContributionContextProperties()
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection,
                NormChangeHandler = new FailureMechanismContributionNormChangeHandler()
            };

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
            Assert.AreEqual(originalReturnPeriodValue, properties.ReturnPeriod);
            Assert.AreEqual(waveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
            Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => properties.ReturnPeriod = newReturnPeriod;

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, messageAllHydraulicBoundaryLocationOutputCleared, 1);

            Assert.AreEqual(1.0/newReturnPeriod, failureMechanismContribution.Norm);
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

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new TestHydraulicBoundaryLocation());

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };

            PipingCalculation emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            PipingCalculation pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            GrassCoverErosionInwardsCalculation emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new TestDikeHeightAssessmentOutput(0))
            };
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            MockRepository mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            IObserver pipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            pipingCalculationObserver.Expect(o => o.UpdateObserver());
            IObserver grassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            grassCoverErosionInwardsCalculationObserver.Expect(o => o.UpdateObserver());
            IObserver heightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();
            heightStructuresCalculationObserver.Expect(o => o.UpdateObserver());
            IObserver emptyPipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            IObserver emptyGrassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            IObserver emptyHeightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();

            IObserver hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>(); // No update observer expected.
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            var properties = new FailureMechanismContributionContextProperties()
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection,
                NormChangeHandler = new FailureMechanismContributionNormChangeHandler()
            };

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
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

            Assert.AreEqual(1.0/newReturnPeriod, failureMechanismContribution.Norm);
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

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            PipingCalculation emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            PipingCalculation pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            GrassCoverErosionInwardsCalculation emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new TestDikeHeightAssessmentOutput(0))
            };
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            MockRepository mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            IObserver pipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            pipingCalculationObserver.Expect(o => o.UpdateObserver());
            IObserver grassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            grassCoverErosionInwardsCalculationObserver.Expect(o => o.UpdateObserver());
            IObserver heightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();
            heightStructuresCalculationObserver.Expect(o => o.UpdateObserver());
            IObserver emptyPipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            IObserver emptyGrassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            IObserver emptyHeightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();

            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            var properties = new FailureMechanismContributionContextProperties()
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection,
                NormChangeHandler = new FailureMechanismContributionNormChangeHandler()
            };

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
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

            Assert.AreEqual(1.0/newReturnPeriod, failureMechanismContribution.Norm);
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

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new TestHydraulicBoundaryLocation());

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };

            PipingCalculation emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            GrassCoverErosionInwardsCalculation emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            MockRepository mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            IObserver calculationObserver = mockRepository.StrictMock<IObserver>();
            IObserver hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            var properties = new FailureMechanismContributionContextProperties()
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection,
                NormChangeHandler = new FailureMechanismContributionNormChangeHandler()
            };

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
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
            Assert.AreEqual(1.0/newReturnPeriod, failureMechanismContribution.Norm);

            mockRepository.VerifyAll(); // No update observer expected.
        }

        [Test]
        public void ReturnPeriodProperty_NoHydraulicBoundaryDatabaseAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            // Setup
            const int newReturnPeriod = 200;

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            PipingCalculation emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            GrassCoverErosionInwardsCalculation emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var properties = new FailureMechanismContributionContextProperties()
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection,
                NormChangeHandler = new FailureMechanismContributionNormChangeHandler()
            };

            // Precondition
            int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
            Assert.AreEqual(originalReturnPeriodValue, properties.ReturnPeriod);

            // Call
            failureMechanismContribution.Norm = 1.0/newReturnPeriod;
            Action call = () => failureMechanismContribution.NotifyObservers();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.AreEqual(1.0/newReturnPeriod, failureMechanismContribution.Norm);
        }
    }
}