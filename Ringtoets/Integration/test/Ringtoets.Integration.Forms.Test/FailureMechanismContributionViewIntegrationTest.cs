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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Commands;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class FailureMechanismContributionViewIntegrationTest : NUnitFormTest
    {
        private const string messageAllHydraulicBoundaryLocationOutputCleared =
            "Alle berekende resultaten voor alle hydraulische randvoorwaardenlocaties zijn verwijderd.";

        private const string messageCalculationsremoved = "De resultaten van {0} berekeningen zijn verwijderd.";

        private const string normInputTextBoxName = "normInput";
        private const string dataGridViewControlName = "dataGridView";
        private const string assessmentSectionCompositionComboBoxName = "assessmentSectionCompositionComboBox";
        private const int isRelevantColumnIndex = 0;
        private const int nameColumnIndex = 1;
        private const int codeColumnIndex = 2;
        private const int contributionColumnIndex = 3;
        private const int probabilitySpaceColumnIndex = 4;

        [Test]
        public void NormTextBox_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            // Setup
            const int normValue = 200;
            const int numberOfCalculations = 3;

            var waveHeight = (RoundedDouble) 3.0;
            var designWaterLevel = (RoundedDouble) 4.2;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0.0, 0.0)
            {
                WaveHeight = waveHeight,
                DesignWaterLevel = designWaterLevel
            };
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
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var grassCoverErosionOutwardsHydraulicBoundaryLocation = hydraulicBoundaryLocation;
            grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight = waveHeight;
            grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel = designWaterLevel;

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

            var viewCommands = mockRepository.Stub<IViewCommands>();
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

            var handler1 = new FailureMechanismContributionNormChangeHandler();
            var handler2 = new AssessmentSectionCompositionChangeHandler();

            using (var form = new Form())
            using (var distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                var normTester = new ControlTester(normInputTextBoxName);

                // Precondition
                int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
                Assert.AreEqual(originalReturnPeriodValue.ToString(), normTester.Text);
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
                Action call = () => SimulateUserCommittingNormValue(normTester, normValue);

                // Assert
                TestHelper.AssertLogMessages(call, msgs =>
                {
                    string[] messages = msgs.ToArray();
                    Assert.AreEqual(string.Format(messageCalculationsremoved, numberOfCalculations), messages[0]);
                    Assert.AreEqual(messageAllHydraulicBoundaryLocationOutputCleared, messages[1]);
                });
                Assert.AreEqual(1.0/normValue, failureMechanismContribution.Norm);
                Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
                Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
                Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
                Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel);
                Assert.IsNull(pipingCalculation.Output);
                Assert.IsNull(pipingCalculation.SemiProbabilisticOutput);
                Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
                Assert.IsNull(heightStructuresCalculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void NormTextBox_HydraulicBoundarySetAndCalculationsNoOutput_HydraulicBoundaryDatabaseObserversNotifiedAndMessagesLogged()
        {
            // Setup
            const int normValue = 200;

            var waveHeight = (RoundedDouble) 3.0;
            var designWaterLevel = (RoundedDouble) 4.2;
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 0.0, 0.0)
            {
                WaveHeight = waveHeight,
                DesignWaterLevel = designWaterLevel
            });

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

            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            var handler1 = new FailureMechanismContributionNormChangeHandler();
            var handler2 = new AssessmentSectionCompositionChangeHandler();

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester(normInputTextBoxName);

                HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];

                // Precondition
                int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
                Assert.AreEqual(originalReturnPeriodValue.ToString(), normTester.Text);
                Assert.AreEqual(waveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
                Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialogTester = new MessageBoxTester(wnd);
                    dialogTester.ClickOk();
                };

                // Call
                Action call = () => SimulateUserCommittingNormValue(normTester, normValue);

                // Assert
                TestHelper.AssertLogMessageIsGenerated(call, messageAllHydraulicBoundaryLocationOutputCleared, 1);

                Assert.AreEqual(1.0/normValue, failureMechanismContribution.Norm);
                Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
                Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void NormTextBox_HydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            // Setup
            const int normValue = 200;
            const int numberOfCalculations = 3;

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 0.0, 0.0));

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
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
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

            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            var handler1 = new FailureMechanismContributionNormChangeHandler();
            var handler2 = new AssessmentSectionCompositionChangeHandler();

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester(normInputTextBoxName);

                // Precondition
                int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
                Assert.AreEqual(originalReturnPeriodValue.ToString(), normTester.Text);
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
                Action call = () => SimulateUserCommittingNormValue(normTester, normValue);

                // Assert
                string expectedMessage = string.Format(messageCalculationsremoved,
                                                       numberOfCalculations);
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

                Assert.AreEqual(1.0/normValue, failureMechanismContribution.Norm);
                Assert.IsNull(pipingCalculation.Output);
                Assert.IsNull(pipingCalculation.SemiProbabilisticOutput);
                Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
                Assert.IsNull(heightStructuresCalculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void NormTextBox_NoHydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            // Setup
            const int normValue = 200;
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
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
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

            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            var handler1 = new FailureMechanismContributionNormChangeHandler();
            var handler2 = new AssessmentSectionCompositionChangeHandler();

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester(normInputTextBoxName);

                // Precondition
                int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
                Assert.AreEqual(originalReturnPeriodValue.ToString(), normTester.Text);
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
                Action call = () => SimulateUserCommittingNormValue(normTester, normValue);

                // Assert
                string expectedMessage = string.Format(messageCalculationsremoved,
                                                       numberOfCalculations);
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

                Assert.AreEqual(1.0/normValue, failureMechanismContribution.Norm);
                Assert.IsNull(pipingCalculation.Output);
                Assert.IsNull(pipingCalculation.SemiProbabilisticOutput);
                Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
                Assert.IsNull(heightStructuresCalculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void NormTextBox_HydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            // Setup
            const int normValue = 200;

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 0.0, 0.0));

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

            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            var handler1 = new FailureMechanismContributionNormChangeHandler();
            var handler2 = new AssessmentSectionCompositionChangeHandler();

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester(normInputTextBoxName);

                // Precondition
                int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
                Assert.AreEqual(originalReturnPeriodValue.ToString(), normTester.Text);

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialogTester = new MessageBoxTester(wnd);
                    dialogTester.ClickOk();
                };

                // Call
                Action call = () => SimulateUserCommittingNormValue(normTester, normValue);

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
                Assert.AreEqual(1.0/normValue, failureMechanismContribution.Norm);
            }
            mockRepository.VerifyAll(); // No update observer expected.
        }

        [Test]
        public void NormTextBox_NoHydraulicBoundaryDatabaseAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            // Setup
            const int normValue = 200;

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

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

            var viewCommands = mockRepository.Stub<IViewCommands>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            var handler1 = new FailureMechanismContributionNormChangeHandler();
            var handler2 = new AssessmentSectionCompositionChangeHandler();

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester(normInputTextBoxName);

                // Precondition
                int originalReturnPeriodValue = Convert.ToInt32(1.0/failureMechanismContribution.Norm);
                Assert.AreEqual(originalReturnPeriodValue.ToString(), normTester.Text);

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialogTester = new MessageBoxTester(wnd);
                    dialogTester.ClickOk();
                };

                // Call
                Action call = () => SimulateUserCommittingNormValue(normTester, normValue);

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
                Assert.AreEqual(1.0/normValue, failureMechanismContribution.Norm);
            }
            mockRepository.VerifyAll(); // No update observer expected.
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune)]
        public void GivenViewWithAssessmentSection_WhenChangingCompositionComboBoxAndOk_ThenUpdateAssessmentSectionContributionAndView(AssessmentSectionComposition initialComposition, AssessmentSectionComposition newComposition)
        {
            // Given
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(initialComposition);

            var handler1 = new FailureMechanismContributionNormChangeHandler();
            var handler2 = new AssessmentSectionCompositionChangeHandler();

            using (var form = new Form())
            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = assessmentSection.FailureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(view);
                form.Show();

                // Precondition
                Assert.AreNotEqual(assessmentSection.Composition, newComposition);

                bool dataGridInvalidated = false;
                var contributionGridView = (DataGridView) new ControlTester(dataGridViewControlName).TheObject;
                contributionGridView.Invalidated += (sender, args) => dataGridInvalidated = true;

                var compositionComboBox = (ComboBox) new ControlTester(assessmentSectionCompositionComboBoxName).TheObject;

                // When
                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = new MessageBoxTester(wnd);
                    tester.ClickOk();
                };
                ControlsTestHelper.FakeUserSelectingNewValue(compositionComboBox, newComposition);

                // Then
                Assert.AreEqual(newComposition, compositionComboBox.SelectedValue);

                Assert.IsTrue(dataGridInvalidated,
                              "Expect the DataGridView to be flagged for redrawing.");
                AssertDataGridViewDataSource(assessmentSection.FailureMechanismContribution.Distribution, contributionGridView);
            }
            mocks.VerifyAll();
        }

        private void AssertDataGridViewDataSource(IEnumerable<FailureMechanismContributionItem> expectedDistributionElements, DataGridView dataGridView)
        {
            FailureMechanismContributionItem[] itemArray = expectedDistributionElements.ToArray();
            Assert.AreEqual(itemArray.Length, dataGridView.RowCount);
            for (int i = 0; i < itemArray.Length; i++)
            {
                FailureMechanismContributionItem expectedElement = itemArray[i];
                DataGridViewRow row = dataGridView.Rows[i];
                Assert.AreEqual(expectedElement.IsRelevant, row.Cells[isRelevantColumnIndex].Value);
                Assert.AreEqual(expectedElement.Assessment, row.Cells[nameColumnIndex].Value);
                Assert.AreEqual(expectedElement.AssessmentCode, row.Cells[codeColumnIndex].Value);
                Assert.AreEqual(expectedElement.Contribution, row.Cells[contributionColumnIndex].Value);
                Assert.AreEqual(expectedElement.ProbabilitySpace, row.Cells[probabilitySpaceColumnIndex].Value);
            }
        }

        private static void SimulateUserCommittingNormValue(ControlTester normTester, int normValue)
        {
            var normInput = (NumericUpDown) normTester.TheObject;
            normInput.Value = normValue;
            var eventArgs = new CancelEventArgs();
            EventHelper.RaiseEvent(normTester.TheObject, "Validating", eventArgs);
            if (!eventArgs.Cancel)
            {
                normTester.FireEvent("Validated");
            }
        }
    }
}