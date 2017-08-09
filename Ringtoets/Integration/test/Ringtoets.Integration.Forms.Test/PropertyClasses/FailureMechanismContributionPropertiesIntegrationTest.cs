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
using System.Globalization;
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
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
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
        private readonly string newLowerLimitNorm = 0.01.ToString(CultureInfo.CurrentCulture);
        private readonly string newSignalingNorm = 0.000001.ToString(CultureInfo.CurrentCulture);
        private const NormType newNormativeNorm = NormType.Signaling;

        #region AllDataAndOutputSet

        [Test]
        public void LowerLimitNorm_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForAllDataSet(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForAllDataSet(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForAllDataSet(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region NoPermissionGiven

        [Test]
        public void LowerLimitNorm_PermissionNotGiven_DoesnotClearDependentDataNorNotifiesObserversAndLogsMessages()
        {
            ChangeValueNoPermissionGivenAndVerifyNoNotifcationsAndOutputForAllDataSet(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_PermissionNotGiven_DoesnotClearDependentDataNorNotifiesObserversAndLogsMessages()
        {
            ChangeValueNoPermissionGivenAndVerifyNoNotifcationsAndOutputForAllDataSet(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_PermissionNotGiven_DoesnotClearDependentDataNorNotifiesObserversAndLogsMessages()
        {
            ChangeValueNoPermissionGivenAndVerifyNoNotifcationsAndOutputForAllDataSet(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region HydraulicBoundarySetAndCalculationsNoOutput

        [Test]
        public void LowerLimitNorm_HydraulicBoundarySetAndCalculationsNoOutput_HydraulicBoundaryDatabaseObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundarySetAndCalculationsNoOutput(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_HydraulicBoundarySetAndCalculationsNoOutput_HydraulicBoundaryDatabaseObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundarySetAndCalculationsNoOutput(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_HydraulicBoundarySetAndCalculationsNoOutput_HydraulicBoundaryDatabaseObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundarySetAndCalculationsNoOutput(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region HydraulicBoundaryLocationNoOutputAndCalculationWithOutput

        [Test]
        public void LowerLimitNorm_HydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_HydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_HydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region NoHydraulicBoundaryLocationNoOutputAndCalculationWithOutput

        [Test]
        public void LowerLimitNorm_NoHydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForNoHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_NoHydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForNoHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_NoHydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForNoHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region HydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutput

        [Test]
        public void LowerLimitNorm_HydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutput(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_HydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutput(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_HydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutput(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region NoHydraulicBoundaryDatabaseAndNoCalculationsWithOutput

        [Test]
        public void LowerLimitNorm_NoHydraulicBoundaryDatabaseAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForNoHydraulicBoundaryDatabaseAndNoCalculationsWithOutput(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_NoHydraulicBoundaryDatabaseAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForNoHydraulicBoundaryDatabaseAndNoCalculationsWithOutput(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_NoHydraulicBoundaryDatabaseAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotifcationsAndOutputForNoHydraulicBoundaryDatabaseAndNoCalculationsWithOutput(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        private void SetPropertyAndVerifyNotifcationsAndOutputForAllDataSet(Action<FailureMechanismContributionProperties> setPropertyAction)
        {
            // Setup
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
            var failureMechanismContributionObserver = mockRepository.StrictMock<IObserver>();
            failureMechanismContributionObserver.Expect(o => o.UpdateObserver());

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

            failureMechanismContribution.Attach(failureMechanismContributionObserver);
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

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => setPropertyAction(properties);

            // Assert
            TestHelper.AssertLogMessages(call, msgs =>
            {
                string[] messages = msgs.ToArray();
                Assert.AreEqual(string.Format(messageCalculationsremoved, numberOfCalculations), messages[0]);
                Assert.AreEqual(messageAllHydraulicBoundaryLocationOutputCleared, messages[1]);
            });

            AssertNormValues(properties, failureMechanismContribution);

            AssertLocationsOutputClear(hydraulicBoundaryLocation, grassCoverErosionOutwardsHydraulicBoundaryLocation);
            AssertCalculationsOutputClear(pipingCalculation, grassCoverErosionInwardsCalculation, heightStructuresCalculation);

            mockRepository.VerifyAll();
        }

        private void ChangeValueNoPermissionGivenAndVerifyNoNotifcationsAndOutputForAllDataSet(Action<FailureMechanismContributionProperties> setPropertyAction)
        {
            // Setup
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
            var failureMechanismContributionObserver = mockRepository.StrictMock<IObserver>();
            var pipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            var grassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            var heightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();
            var emptyPipingCalculationObserver = mockRepository.StrictMock<IObserver>();
            var emptyGrassCoverErosionInwardsCalculationObserver = mockRepository.StrictMock<IObserver>();
            var emptyHeightStructuresCalculationObserver = mockRepository.StrictMock<IObserver>();
            var hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>();
            var grassCoverErosionOutwardsObserver = mockRepository.StrictMock<IObserver>();

            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(failureMechanismContributionObserver);
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

            double originalLowerLimitNorm = failureMechanismContribution.LowerLimitNorm;
            double originalSignalingNorm = failureMechanismContribution.SignalingNorm;
            NormType originalNormativeNorm = failureMechanismContribution.NormativeNorm;
            double originalNorm = failureMechanismContribution.Norm;

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickCancel();
            };

            // Call
            Action call = () => setPropertyAction(properties);

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.AreEqual(originalLowerLimitNorm, failureMechanismContribution.LowerLimitNorm);
            Assert.AreEqual(originalSignalingNorm, failureMechanismContribution.SignalingNorm);
            Assert.AreEqual(originalNormativeNorm, failureMechanismContribution.NormativeNorm);
            Assert.AreEqual(originalNorm, failureMechanismContribution.Norm);

            Assert.IsTrue(hydraulicBoundaryLocation.WaveHeightCalculation.HasOutput);
            Assert.IsTrue(hydraulicBoundaryLocation.DesignWaterLevelCalculation.HasOutput);
            Assert.IsTrue(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculation.HasOutput);
            Assert.IsTrue(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevelCalculation.HasOutput);
            Assert.IsNotNull(pipingCalculation.Output);
            Assert.IsNotNull(pipingCalculation.SemiProbabilisticOutput);
            Assert.IsNotNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNotNull(heightStructuresCalculation.Output);

            mockRepository.VerifyAll();
        }

        private void SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundarySetAndCalculationsNoOutput(Action<FailureMechanismContributionProperties> setPropertyAction)
        {
            // Setup
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

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => setPropertyAction(properties);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, messageAllHydraulicBoundaryLocationOutputCleared, 1);

            AssertNormValues(properties, failureMechanismContribution);
            AssertLocationsOutputClear(hydraulicBoundaryLocation);

            mockRepository.VerifyAll();
        }

        private void SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(Action<FailureMechanismContributionProperties> setPropertyAction)
        {
            // Setup
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

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => setPropertyAction(properties);

            // Assert
            string expectedMessage = string.Format(messageCalculationsremoved,
                                                   numberOfCalculations);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            AssertNormValues(properties, failureMechanismContribution);
            AssertCalculationsOutputClear(pipingCalculation, grassCoverErosionInwardsCalculation, heightStructuresCalculation);

            mockRepository.VerifyAll();
        }

        private void SetPropertyAndVerifyNotifcationsAndOutputForNoHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(Action<FailureMechanismContributionProperties> setPropertyAction)
        {
            // Setup
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

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => setPropertyAction(properties);

            // Assert
            string expectedMessage = string.Format(messageCalculationsremoved,
                                                   numberOfCalculations);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            AssertNormValues(properties, failureMechanismContribution);
            AssertCalculationsOutputClear(pipingCalculation, grassCoverErosionInwardsCalculation, heightStructuresCalculation);

            mockRepository.VerifyAll();
        }

        private void SetPropertyAndVerifyNotifcationsAndOutputForHydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutput(Action<FailureMechanismContributionProperties> setPropertyAction)
        {
            // Setup
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

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => setPropertyAction(properties);

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            AssertNormValues(properties, failureMechanismContribution);

            mockRepository.VerifyAll(); // No update observer expected.
        }

        private void SetPropertyAndVerifyNotifcationsAndOutputForNoHydraulicBoundaryDatabaseAndNoCalculationsWithOutput(Action<FailureMechanismContributionProperties> setPropertyAction)
        {
            // Setup
            var mockRepository = new MockRepository();
            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

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

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => setPropertyAction(properties);

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            AssertNormValues(properties, failureMechanismContribution);
            mockRepository.VerifyAll();
        }

        #region Asserts

        private static void AssertCalculationsOutputClear(PipingCalculation pipingCalculation,
                                                          GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation,
                                                          StructuresCalculation<HeightStructuresInput> heightStructuresCalculation)
        {
            Assert.IsNull(pipingCalculation.Output);
            Assert.IsNull(pipingCalculation.SemiProbabilisticOutput);
            Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNull(heightStructuresCalculation.Output);
        }

        private static void AssertLocationsOutputClear(params HydraulicBoundaryLocation[] hydraulicBoundaryLocations)
        {
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in hydraulicBoundaryLocations)
            {
                Assert.IsFalse(hydraulicBoundaryLocation.WaveHeightCalculation.HasOutput);
                Assert.IsFalse(hydraulicBoundaryLocation.DesignWaterLevelCalculation.HasOutput);
            }
        }

        private static void AssertNormValues(FailureMechanismContributionProperties properties, FailureMechanismContribution failureMechanismContribution)
        {
            Assert.AreEqual(properties.LowerLimitNorm, ProbabilityFormattingHelper.Format(failureMechanismContribution.LowerLimitNorm));
            Assert.AreEqual(properties.SignalingNorm, ProbabilityFormattingHelper.Format(failureMechanismContribution.SignalingNorm));
            Assert.AreEqual(properties.NormativeNorm, failureMechanismContribution.NormativeNorm);

            double expectedNorm = failureMechanismContribution.NormativeNorm == NormType.LowerLimit
                                      ? failureMechanismContribution.LowerLimitNorm
                                      : failureMechanismContribution.SignalingNorm;

            Assert.AreEqual(expectedNorm, failureMechanismContribution.Norm);
        }

        #endregion
    }
}