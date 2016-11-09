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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class FailureMechanismContributionViewIntegrationTest
    {
        private const string messageAllHydraulicBoundaryLocationOutputCleared =
            "Alle berekende resultaten voor alle hydraulische randvoorwaardenlocaties zijn verwijderd.";

        private const string messageGrassCoverErosionOutwardsHydraulicBoundaryLocationOutputCleared =
            "De berekende waterstanden en golfhoogtes bij doorsnede-eis voor alle hydraulische randvoorwaarden locaties zijn verwijderd.";

        private const string messageCalculationsremoved = "De resultaten van {0} berekeningen zijn verwijderd.";

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
            IObserver calculationObserver = mockRepository.StrictMock<IObserver>();
            calculationObserver.Expect(co => co.UpdateObserver()).Repeat.Times(numberOfCalculations);
            IObserver hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>();
            hydraulicBoundaryDatabaseObserver.Expect(hbdo => hbdo.UpdateObserver());
            IObserver grassCoverErosionOutwardsObserver = mockRepository.StrictMock<IObserver>();
            grassCoverErosionOutwardsObserver.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            pipingCalculation.Attach(calculationObserver);
            grassCoverErosionInwardsCalculation.Attach(calculationObserver);
            heightStructuresCalculation.Attach(calculationObserver);

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsObserver);

            using (var form = new Form())
            using (var distributionView = new FailureMechanismContributionView
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                var normTester = new ControlTester("normInput");

                // Precondition
                Assert.AreEqual(failureMechanismContribution.Norm.ToString(), normTester.Text);
                Assert.AreEqual(waveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
                Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
                Assert.AreEqual(waveHeight, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight.GetAccuracy());
                Assert.AreEqual(designWaterLevel, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());

                Assert.IsNotNull(pipingCalculation.Output);
                Assert.IsNotNull(pipingCalculation.SemiProbabilisticOutput);
                Assert.IsNotNull(grassCoverErosionInwardsCalculation.Output);
                Assert.IsNotNull(heightStructuresCalculation.Output);

                // Call
                Action call = () => normTester.Properties.Text = normValue.ToString();

                // Assert
                TestHelper.AssertLogMessages(call, msgs =>
                {
                    string[] messages = msgs.ToArray();
                    Assert.AreEqual(string.Format(messageCalculationsremoved, numberOfCalculations), messages[0]);
                    Assert.AreEqual(messageAllHydraulicBoundaryLocationOutputCleared, messages[1]);
                });
                Assert.AreEqual(normValue, failureMechanismContribution.Norm);
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
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester("normInput");

                HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];

                // Precondition
                Assert.AreEqual(failureMechanismContribution.Norm.ToString(), normTester.Text);
                Assert.AreEqual(waveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
                Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
                // Call
                Action call = () => normTester.Properties.Text = normValue.ToString();

                // Assert
                TestHelper.AssertLogMessageIsGenerated(call, messageAllHydraulicBoundaryLocationOutputCleared, 1);

                Assert.AreEqual(normValue, failureMechanismContribution.Norm);
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
            IObserver calculationObserver = mockRepository.StrictMock<IObserver>();
            calculationObserver.Expect(co => co.UpdateObserver()).Repeat.Times(numberOfCalculations);
            IObserver hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>(); // No update observer expected.
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            pipingCalculation.Attach(calculationObserver);
            grassCoverErosionInwardsCalculation.Attach(calculationObserver);
            heightStructuresCalculation.Attach(calculationObserver);

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester("normInput");

                // Precondition
                Assert.AreEqual(failureMechanismContribution.Norm.ToString(), normTester.Text);
                Assert.IsNotNull(pipingCalculation.Output);
                Assert.IsNotNull(pipingCalculation.SemiProbabilisticOutput);
                Assert.IsNotNull(grassCoverErosionInwardsCalculation.Output);
                Assert.IsNotNull(heightStructuresCalculation.Output);

                // Call
                Action call = () => normTester.Properties.Text = normValue.ToString();

                // Assert
                string expectedMessage = string.Format(messageCalculationsremoved,
                                                       numberOfCalculations);
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

                Assert.AreEqual(normValue, failureMechanismContribution.Norm);
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
            IObserver calculationObserver = mockRepository.StrictMock<IObserver>();
            calculationObserver.Expect(co => co.UpdateObserver()).Repeat.Times(numberOfCalculations);
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);

            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            pipingCalculation.Attach(calculationObserver);
            grassCoverErosionInwardsCalculation.Attach(calculationObserver);
            heightStructuresCalculation.Attach(calculationObserver);

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester("normInput");

                // Precondition
                Assert.AreEqual(failureMechanismContribution.Norm.ToString(), normTester.Text);
                Assert.IsNotNull(pipingCalculation.Output);
                Assert.IsNotNull(pipingCalculation.SemiProbabilisticOutput);
                Assert.IsNotNull(grassCoverErosionInwardsCalculation.Output);
                Assert.IsNotNull(heightStructuresCalculation.Output);

                // Call
                Action call = () => normTester.Properties.Text = normValue.ToString();

                // Assert
                string expectedMessage = string.Format(messageCalculationsremoved,
                                                       numberOfCalculations);
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

                Assert.AreEqual(normValue, failureMechanismContribution.Norm);
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
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester("normInput");

                // Precondition
                Assert.AreEqual(failureMechanismContribution.Norm.ToString(), normTester.Text);

                // Call
                Action call = () => normTester.Properties.Text = normValue.ToString();

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
                Assert.AreEqual(normValue, failureMechanismContribution.Norm);
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
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester("normInput");

                // Precondition
                Assert.AreEqual(failureMechanismContribution.Norm.ToString(), normTester.Text);

                // Call
                Action call = () => normTester.Properties.Text = normValue.ToString();

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
                Assert.AreEqual(normValue, failureMechanismContribution.Norm);
            }
            mockRepository.VerifyAll(); // No update observer expected.
        }

        [Test]
        public void AssessmentSectionCompositionComboBox_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            // Setup
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

            var grassCoverErosionOutwardsHydraulicBoundaryLocation = new HydraulicBoundaryLocation(hydraulicBoundaryLocation.Id,
                                                                                                   hydraulicBoundaryLocation.Name,
                                                                                                   hydraulicBoundaryLocation.Location.X,
                                                                                                   hydraulicBoundaryLocation.Location.Y)
            {
                WaveHeight = waveHeight,
                DesignWaterLevel = designWaterLevel
            };

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Add(grassCoverErosionOutwardsHydraulicBoundaryLocation);

            var mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            IObserver hydraulicBoundaryDatabaseObserverMock = mockRepository.StrictMock<IObserver>();
            IObserver grassCoverErosionOutwardsObserverMock = mockRepository.StrictMock<IObserver>();
            grassCoverErosionOutwardsObserverMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            assessmentSection.Attach(observerMock);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserverMock);
            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsObserverMock);

            using (var form = new Form())
            using (var distributionView = new FailureMechanismContributionView
            {
                Data = assessmentSection.FailureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                var compositionTester = (ComboBox) new ControlTester("assessmentSectionCompositionComboBox").TheObject;

                // Precondition
                Assert.AreEqual(waveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
                Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
                Assert.AreEqual(waveHeight, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight.GetAccuracy());
                Assert.AreEqual(designWaterLevel, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel, grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());

                // Call
                Action call = () => compositionTester.SelectedValue = AssessmentSectionComposition.DikeAndDune;

                // Assert
                TestHelper.AssertLogMessages(call, msgs =>
                {
                    string[] messages = msgs.ToArray();
                    Assert.AreEqual(1, messages.Length);
                    Assert.AreEqual(messageGrassCoverErosionOutwardsHydraulicBoundaryLocationOutputCleared, messages[0]);
                });
                Assert.AreEqual(waveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
                Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
                Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
                Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void AssessmentSectionCompositionComboBox_HydraulicBoundaryDatabaseAndNoLocationsWithOutputAndValueChanged_NoObserversNotified()
        {
            // Setup
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

            var grassCoverErosionOutwardsHydraulicBoundaryLocation = new HydraulicBoundaryLocation(hydraulicBoundaryLocation.Id,
                                                                                                   hydraulicBoundaryLocation.Name,
                                                                                                   hydraulicBoundaryLocation.Location.X,
                                                                                                   hydraulicBoundaryLocation.Location.Y);

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Add(grassCoverErosionOutwardsHydraulicBoundaryLocation);

            var mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            IObserver hydraulicBoundaryDatabaseObserverMock = mockRepository.StrictMock<IObserver>();
            IObserver grassCoverErosionOutwardsObserverMock = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            assessmentSection.Attach(observerMock);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserverMock);
            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsObserverMock);

            using (var form = new Form())
            using (var distributionView = new FailureMechanismContributionView
            {
                Data = assessmentSection.FailureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                var compositionTester = (ComboBox) new ControlTester("assessmentSectionCompositionComboBox").TheObject;

                // Precondition
                Assert.AreEqual(waveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
                Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
                Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
                Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel);

                // Call
                compositionTester.SelectedValue = AssessmentSectionComposition.DikeAndDune;

                // Assert
                Assert.AreEqual(waveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
                Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
                Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
                Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevel);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void AssessmentSectionCompositionComboBox_NoHydraulicBoundaryDatabaseAndValueChanged_NoObserversNotified()
        {
            // Setup
            var mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            IObserver grassCoverErosionOutwardsObserverMock = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.Attach(observerMock);
            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsObserverMock);

            using (var form = new Form())
            using (var distributionView = new FailureMechanismContributionView
            {
                Data = assessmentSection.FailureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                var compositionTester = (ComboBox) new ControlTester("assessmentSectionCompositionComboBox").TheObject;

                // Call
                compositionTester.SelectedValue = AssessmentSectionComposition.DikeAndDune;
            }

            // Assert
            mockRepository.VerifyAll(); // Expect UpdateObserver call
        }
    }
}