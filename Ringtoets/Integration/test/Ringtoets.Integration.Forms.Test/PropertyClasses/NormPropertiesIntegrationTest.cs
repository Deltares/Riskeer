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
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
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

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class NormPropertiesIntegrationTest : NUnitFormTest
    {
        private const string messageAllHydraulicBoundaryOutputCleared = "Alle berekende resultaten voor alle hydraulische randvoorwaardenlocaties zijn verwijderd.";
        private const string messageCalculationsRemoved = "De resultaten van {0} berekeningen zijn verwijderd.";
        private const NormType newNormativeNorm = NormType.Signaling;
        private const double newLowerLimitNorm = 0.01;
        private const double newSignalingNorm = 0.000001;

        private void SetPropertyAndVerifyNotificationsAndOutputForAllDataSet(Action<NormProperties> setPropertyAction)
        {
            // Setup
            const int numberOfCalculations = 3;

            var random = new Random();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    Locations =
                    {
                        hydraulicBoundaryLocation
                    }
                }
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            SetOutputToAllHydraulicBoundaryLocationCalculations(assessmentSection, random);

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
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

            assessmentSection.Piping.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.Piping.CalculationsGroup.Children.Add(pipingCalculation);
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
            var grassCoverErosionOutwardsObserver = mockRepository.StrictMock<IObserver>();

            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(failureMechanismContributionObserver);
            assessmentSection.HydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsObserver);

            var properties = new NormProperties(
                failureMechanismContribution,
                new FailureMechanismContributionNormChangeHandler(assessmentSection));

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
                Assert.AreEqual(string.Format(messageCalculationsRemoved, numberOfCalculations), messages[0]);
                Assert.AreEqual(messageAllHydraulicBoundaryOutputCleared, messages[1]);
            });

            AssertNormValues(properties, failureMechanismContribution);

            AssertHydraulicBoundaryOutputClear(assessmentSection);
            AssertHydraulicBoundaryLocationOutputClear(grassCoverErosionOutwardsHydraulicBoundaryLocation);

            AssertCalculationOutputClear(pipingCalculation);
            AssertCalculationOutputClear(grassCoverErosionInwardsCalculation);
            AssertCalculationOutputClear(heightStructuresCalculation);

            mockRepository.VerifyAll();
        }

        private void ChangeValueNoPermissionGivenAndVerifyNoNotificationsAndOutputForAllDataSet(Action<NormProperties> setPropertyAction)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation
            {
                DesignWaterLevelCalculation1 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(1.1)
                },
                DesignWaterLevelCalculation2 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(2.2)
                },
                DesignWaterLevelCalculation3 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(3.3)
                },
                DesignWaterLevelCalculation4 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(4.4)
                },
                WaveHeightCalculation1 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(5.5)
                },
                WaveHeightCalculation2 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(6.6)
                },
                WaveHeightCalculation3 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(7.7)
                },
                WaveHeightCalculation4 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(8.8)
                }
            };

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    Locations =
                    {
                        hydraulicBoundaryLocation
                    }
                }
            };

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
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

            assessmentSection.Piping.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.Piping.CalculationsGroup.Children.Add(pipingCalculation);
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

            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(failureMechanismContributionObserver);
            assessmentSection.HydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsObserver);

            var properties = new NormProperties(
                failureMechanismContribution,
                new FailureMechanismContributionNormChangeHandler(assessmentSection));

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

            Assert.IsTrue(hydraulicBoundaryLocation.WaveHeightCalculation1.HasOutput);
            Assert.IsTrue(hydraulicBoundaryLocation.WaveHeightCalculation2.HasOutput);
            Assert.IsTrue(hydraulicBoundaryLocation.WaveHeightCalculation3.HasOutput);
            Assert.IsTrue(hydraulicBoundaryLocation.WaveHeightCalculation4.HasOutput);
            Assert.IsTrue(hydraulicBoundaryLocation.DesignWaterLevelCalculation1.HasOutput);
            Assert.IsTrue(hydraulicBoundaryLocation.DesignWaterLevelCalculation2.HasOutput);
            Assert.IsTrue(hydraulicBoundaryLocation.DesignWaterLevelCalculation3.HasOutput);
            Assert.IsTrue(hydraulicBoundaryLocation.DesignWaterLevelCalculation4.HasOutput);
            Assert.IsTrue(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculation1.HasOutput);
            Assert.IsTrue(grassCoverErosionOutwardsHydraulicBoundaryLocation.DesignWaterLevelCalculation1.HasOutput);
            Assert.IsNotNull(pipingCalculation.Output);
            Assert.IsNotNull(grassCoverErosionInwardsCalculation.Output);
            Assert.IsNotNull(heightStructuresCalculation.Output);

            mockRepository.VerifyAll();
        }

        private void SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundarySetAndCalculationsNoOutput(Action<NormProperties> setPropertyAction)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    Locations =
                    {
                        TestHydraulicBoundaryLocation.CreateFullyCalculated()
                    }
                }
            };

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();

            assessmentSection.Piping.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mockRepository = new MockRepository();
            var failureMechanismContributionObserver = mockRepository.StrictMock<IObserver>();
            failureMechanismContributionObserver.Expect(o => o.UpdateObserver());
            var calculationObserver = mockRepository.StrictMock<IObserver>(); // No update observers expected.
            var hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>(); // No update observers expected.
            var hydraulicBoundaryLocationObserver = mockRepository.StrictMock<IObserver>();
            hydraulicBoundaryLocationObserver.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(failureMechanismContributionObserver);
            assessmentSection.HydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];
            hydraulicBoundaryLocation.Attach(hydraulicBoundaryLocationObserver);

            var properties = new NormProperties(
                failureMechanismContribution,
                new FailureMechanismContributionNormChangeHandler(assessmentSection));

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => setPropertyAction(properties);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, messageAllHydraulicBoundaryOutputCleared, 1);

            AssertNormValues(properties, failureMechanismContribution);
            AssertHydraulicBoundaryOutputClear(assessmentSection);

            mockRepository.VerifyAll();
        }

        private void SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(Action<NormProperties> setPropertyAction)
        {
            // Setup
            const int numberOfCalculations = 3;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    Locations =
                    {
                        new TestHydraulicBoundaryLocation()
                    }
                }
            };

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
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

            assessmentSection.Piping.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.Piping.CalculationsGroup.Children.Add(pipingCalculation);
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

            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observer);
            assessmentSection.HydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            var properties = new NormProperties(
                failureMechanismContribution,
                new FailureMechanismContributionNormChangeHandler(assessmentSection));

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => setPropertyAction(properties);

            // Assert
            string expectedMessage = string.Format(messageCalculationsRemoved,
                                                   numberOfCalculations);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            AssertNormValues(properties, failureMechanismContribution);
            AssertCalculationOutputClear(pipingCalculation);
            AssertCalculationOutputClear(grassCoverErosionInwardsCalculation);
            AssertCalculationOutputClear(heightStructuresCalculation);

            mockRepository.VerifyAll();
        }

        private void SetPropertyAndVerifyNotificationsAndOutputForNoHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(Action<NormProperties> setPropertyAction)
        {
            // Setup
            const int numberOfCalculations = 3;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
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

            assessmentSection.Piping.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.Piping.CalculationsGroup.Children.Add(pipingCalculation);
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

            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observer);

            emptyPipingCalculation.Attach(emptyPipingCalculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(emptyGrassCoverErosionInwardsCalculationObserver);
            emptyHeightStructuresCalculation.Attach(emptyHeightStructuresCalculationObserver);

            pipingCalculation.Attach(pipingCalculationObserver);
            grassCoverErosionInwardsCalculation.Attach(grassCoverErosionInwardsCalculationObserver);
            heightStructuresCalculation.Attach(heightStructuresCalculationObserver);

            var properties = new NormProperties(
                failureMechanismContribution,
                new FailureMechanismContributionNormChangeHandler(assessmentSection));

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => setPropertyAction(properties);

            // Assert
            string expectedMessage = string.Format(messageCalculationsRemoved,
                                                   numberOfCalculations);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);

            AssertNormValues(properties, failureMechanismContribution);
            AssertCalculationOutputClear(pipingCalculation);
            AssertCalculationOutputClear(grassCoverErosionInwardsCalculation);
            AssertCalculationOutputClear(heightStructuresCalculation);

            mockRepository.VerifyAll();
        }

        private void SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutput(Action<NormProperties> setPropertyAction)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    Locations =
                    {
                        new TestHydraulicBoundaryLocation()
                    }
                }
            };

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();

            assessmentSection.Piping.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var calculationObserver = mockRepository.StrictMock<IObserver>();
            var hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>();

            mockRepository.ReplayAll();
            failureMechanismContribution.Attach(observer);
            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);
            assessmentSection.HydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            var properties = new NormProperties(
                failureMechanismContribution,
                new FailureMechanismContributionNormChangeHandler(assessmentSection));

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

        private void SetPropertyAndVerifyNotificationsAndNoOutputForHydraulicBoundaryDatabaseAndNoCalculationsWithOutput(Action<NormProperties> setPropertyAction)
        {
            // Setup
            var mockRepository = new MockRepository();
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();

            assessmentSection.Piping.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var properties = new NormProperties(
                failureMechanismContribution,
                new FailureMechanismContributionNormChangeHandler(assessmentSection));

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

        private static void SetOutputToAllHydraulicBoundaryLocationCalculations(IAssessmentSection assessmentSection, Random random)
        {
            assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ForEachElementDo(c => c.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble()));
            assessmentSection.WaterLevelCalculationsForSignalingNorm.ForEachElementDo(c => c.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble()));
            assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ForEachElementDo(c => c.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble()));
            assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ForEachElementDo(c => c.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble()));
            assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.ForEachElementDo(c => c.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble()));
            assessmentSection.WaveHeightCalculationsForSignalingNorm.ForEachElementDo(c => c.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble()));
            assessmentSection.WaveHeightCalculationsForLowerLimitNorm.ForEachElementDo(c => c.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble()));
            assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ForEachElementDo(c => c.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble()));
        }

        #region AllDataAndOutputSet

        [Test]
        public void LowerLimitNorm_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotificationsAndOutputForAllDataSet(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotificationsAndOutputForAllDataSet(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotificationsAndOutputForAllDataSet(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region NoPermissionGiven

        [Test]
        public void LowerLimitNorm_PermissionNotGiven_DoesnotClearDependentDataNorNotifiesObserversAndLogsMessages()
        {
            ChangeValueNoPermissionGivenAndVerifyNoNotificationsAndOutputForAllDataSet(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_PermissionNotGiven_DoesnotClearDependentDataNorNotifiesObserversAndLogsMessages()
        {
            ChangeValueNoPermissionGivenAndVerifyNoNotificationsAndOutputForAllDataSet(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_PermissionNotGiven_DoesnotClearDependentDataNorNotifiesObserversAndLogsMessages()
        {
            ChangeValueNoPermissionGivenAndVerifyNoNotificationsAndOutputForAllDataSet(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region HydraulicBoundarySetAndCalculationsNoOutput

        [Test]
        public void LowerLimitNorm_HydraulicBoundarySetAndCalculationsNoOutput_HydraulicBoundaryLocationObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundarySetAndCalculationsNoOutput(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_HydraulicBoundarySetAndCalculationsNoOutput_HydraulicBoundaryLocationObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundarySetAndCalculationsNoOutput(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_HydraulicBoundarySetAndCalculationsNoOutput_HydraulicBoundaryLocationObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundarySetAndCalculationsNoOutput(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region HydraulicBoundaryLocationNoOutputAndCalculationWithOutput

        [Test]
        public void LowerLimitNorm_HydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_HydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_HydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region NoHydraulicBoundaryLocationNoOutputAndCalculationWithOutput

        [Test]
        public void LowerLimitNorm_NoHydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForNoHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_NoHydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForNoHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_NoHydraulicBoundaryLocationNoOutputAndCalculationWithOutputAndValueChanged_CalculationObserverNotifiedAndMessageLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForNoHydraulicBoundaryLocationNoOutputAndCalculationWithOutput(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region HydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutput

        [Test]
        public void LowerLimitNorm_HydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutput(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_HydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutput(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_HydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndOutputForHydraulicBoundaryLocationNoOutputAndNoCalculationsWithOutput(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region NoHydraulicBoundaryDatabaseAndNoCalculationsWithOutput

        [Test]
        public void LowerLimitNorm_NoHydraulicBoundaryDatabaseOutputAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndNoOutputForHydraulicBoundaryDatabaseAndNoCalculationsWithOutput(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_NoHydraulicBoundaryDatabaseOutputAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndNoOutputForHydraulicBoundaryDatabaseAndNoCalculationsWithOutput(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_NoHydraulicBoundaryDatabaseOutputAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndNoOutputForHydraulicBoundaryDatabaseAndNoCalculationsWithOutput(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region Asserts

        private static void AssertCalculationOutputClear(ICalculation calculation)
        {
            Assert.IsFalse(calculation.HasOutput);
        }

        private static void AssertHydraulicBoundaryOutputClear(IAssessmentSection assessmentSection)
        {
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => c.Output == null));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForSignalingNorm.All(c => c.Output == null));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.All(c => c.Output == null));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.All(c => c.Output == null));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.All(c => c.Output == null));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForSignalingNorm.All(c => c.Output == null));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForLowerLimitNorm.All(c => c.Output == null));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.All(c => c.Output == null));
        }

        private static void AssertHydraulicBoundaryLocationOutputClear(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            Assert.IsFalse(hydraulicBoundaryLocation.WaveHeightCalculation1.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocation.WaveHeightCalculation2.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocation.WaveHeightCalculation3.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocation.WaveHeightCalculation4.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocation.DesignWaterLevelCalculation1.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocation.DesignWaterLevelCalculation2.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocation.DesignWaterLevelCalculation3.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocation.DesignWaterLevelCalculation4.HasOutput);
        }

        private static void AssertNormValues(NormProperties properties, FailureMechanismContribution failureMechanismContribution)
        {
            Assert.AreEqual(properties.LowerLimitNorm, failureMechanismContribution.LowerLimitNorm);
            Assert.AreEqual(properties.SignalingNorm, failureMechanismContribution.SignalingNorm);
            Assert.AreEqual(properties.NormativeNorm, failureMechanismContribution.NormativeNorm);

            double expectedNorm = failureMechanismContribution.NormativeNorm == NormType.LowerLimit
                                      ? failureMechanismContribution.LowerLimitNorm
                                      : failureMechanismContribution.SignalingNorm;

            Assert.AreEqual(expectedNorm, failureMechanismContribution.Norm);
        }

        #endregion
    }
}