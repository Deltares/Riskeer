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
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
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

        private void SetPropertyAndVerifyNotificationsAndOutputSet(Action<NormProperties> setPropertyAction)
        {
            // Setup
            const int numberOfCalculationsWithOutput = 3;

            var random = new Random();
            HydraulicBoundaryLocation hydraulicBoundaryLocation1 = TestHydraulicBoundaryLocation.CreateFullyCalculated();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    Locations =
                    {
                        hydraulicBoundaryLocation1,
                        hydraulicBoundaryLocation2
                    }
                }
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            SetOutputToHydraulicBoundaryLocationCalculations(assessmentSection, hydraulicBoundaryLocation1, random);

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Add(hydraulicBoundaryLocation1);
            assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1
            });

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

            var mockRepository = new MockRepository();
            AttachObserver(mockRepository, assessmentSection.FailureMechanismContribution);
            AttachObserver(mockRepository, assessmentSection, hydraulicBoundaryLocation1);
            AttachObserver(mockRepository, assessmentSection, hydraulicBoundaryLocation2, false);
            AttachObserver(mockRepository, pipingCalculation);
            AttachObserver(mockRepository, emptyPipingCalculation, false);
            AttachObserver(mockRepository, grassCoverErosionInwardsCalculation);
            AttachObserver(mockRepository, emptyGrassCoverErosionInwardsCalculation, false);
            AttachObserver(mockRepository, heightStructuresCalculation);
            AttachObserver(mockRepository, emptyHeightStructuresCalculation, false);
            mockRepository.ReplayAll();

            var properties = new NormProperties(assessmentSection.FailureMechanismContribution, new FailureMechanismContributionNormChangeHandler(assessmentSection));

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
                Assert.AreEqual(string.Format(messageCalculationsRemoved, numberOfCalculationsWithOutput), messages[0]);
                Assert.AreEqual(messageAllHydraulicBoundaryOutputCleared, messages[1]);
            });

            AssertNormValues(properties, assessmentSection.FailureMechanismContribution);
            AssertHydraulicBoundaryOutput(assessmentSection, hydraulicBoundaryLocation1, false);
            AssertHydraulicBoundaryLocationOutputCleared(hydraulicBoundaryLocation1);
            Assert.IsFalse(pipingCalculation.HasOutput);
            Assert.IsFalse(grassCoverErosionInwardsCalculation.HasOutput);
            Assert.IsFalse(heightStructuresCalculation.HasOutput);

            mockRepository.VerifyAll();
        }

        private void ChangeValueNoPermissionGivenAndVerifyNoNotificationsAndOutputForAllDataSet(Action<NormProperties> setPropertyAction)
        {
            // Setup
            var random = new Random();
            TestHydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateFullyCalculated();
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

            SetOutputToHydraulicBoundaryLocationCalculations(assessmentSection, hydraulicBoundaryLocation, random);

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Add(hydraulicBoundaryLocation);
            assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
            };
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            var heightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            assessmentSection.Piping.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            var mockRepository = new MockRepository();
            AttachObserver(mockRepository, assessmentSection.FailureMechanismContribution, false);
            AttachObserver(mockRepository, assessmentSection, hydraulicBoundaryLocation, false);
            AttachObserver(mockRepository, pipingCalculation, false);
            AttachObserver(mockRepository, grassCoverErosionInwardsCalculation, false);
            AttachObserver(mockRepository, heightStructuresCalculation, false);
            mockRepository.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            var properties = new NormProperties(failureMechanismContribution, new FailureMechanismContributionNormChangeHandler(assessmentSection));

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

            AssertHydraulicBoundaryOutput(assessmentSection, hydraulicBoundaryLocation, true);
            Assert.IsTrue(hydraulicBoundaryLocation.WaveHeightCalculation1.HasOutput);
            Assert.IsTrue(hydraulicBoundaryLocation.DesignWaterLevelCalculation1.HasOutput);
            Assert.IsTrue(pipingCalculation.HasOutput);
            Assert.IsTrue(grassCoverErosionInwardsCalculation.HasOutput);
            Assert.IsTrue(heightStructuresCalculation.HasOutput);

            mockRepository.VerifyAll();
        }

        private void SetPropertyAndVerifyNotificationsAndNoOutputSet(Action<NormProperties> setPropertyAction)
        {
            // Setup
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

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Add(hydraulicBoundaryLocation);
            assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            var emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            var emptyHeightStructuresCalculation = new StructuresCalculation<HeightStructuresInput>();

            assessmentSection.Piping.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);

            var mockRepository = new MockRepository();
            AttachObserver(mockRepository, assessmentSection.FailureMechanismContribution);
            AttachObserver(mockRepository, assessmentSection.HydraulicBoundaryDatabase, false);
            AttachObserver(mockRepository, assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations, false);
            AttachObserver(mockRepository, emptyPipingCalculation, false);
            AttachObserver(mockRepository, emptyGrassCoverErosionInwardsCalculation, false);
            AttachObserver(mockRepository, emptyHeightStructuresCalculation, false);
            mockRepository.ReplayAll();

            var properties = new NormProperties(assessmentSection.FailureMechanismContribution, new FailureMechanismContributionNormChangeHandler(assessmentSection));

            DialogBoxHandler = (name, wnd) =>
            {
                var dialogTester = new MessageBoxTester(wnd);
                dialogTester.ClickOk();
            };

            // Call
            Action call = () => setPropertyAction(properties);

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            AssertNormValues(properties, assessmentSection.FailureMechanismContribution);
            mockRepository.VerifyAll();
        }

        private static void SetOutputToHydraulicBoundaryLocationCalculations(IAssessmentSection assessmentSection,
                                                                             HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                             Random random)
        {
            assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm
                             .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                             .Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            assessmentSection.WaterLevelCalculationsForSignalingNorm
                             .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                             .Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            assessmentSection.WaterLevelCalculationsForLowerLimitNorm
                             .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                             .Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm
                             .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                             .Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm
                             .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                             .Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            assessmentSection.WaveHeightCalculationsForSignalingNorm
                             .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                             .Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            assessmentSection.WaveHeightCalculationsForLowerLimitNorm
                             .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                             .Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm
                             .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                             .Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
        }

        private static void AttachObserver(MockRepository mockRepository,
                                           IAssessmentSection assessmentSection,
                                           HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                           bool expectUpdateObserver = true)
        {
            AttachObserver(mockRepository,
                           assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm
                                            .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation)),
                           expectUpdateObserver);

            AttachObserver(mockRepository,
                           assessmentSection.WaterLevelCalculationsForSignalingNorm
                                            .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation)),
                           expectUpdateObserver);

            AttachObserver(mockRepository,
                           assessmentSection.WaterLevelCalculationsForLowerLimitNorm
                                            .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation)),
                           expectUpdateObserver);

            AttachObserver(mockRepository,
                           assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm
                                            .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation)),
                           expectUpdateObserver);

            AttachObserver(mockRepository,
                           assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm
                                            .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation)),
                           expectUpdateObserver);

            AttachObserver(mockRepository,
                           assessmentSection.WaveHeightCalculationsForSignalingNorm
                                            .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation)),
                           expectUpdateObserver);

            AttachObserver(mockRepository,
                           assessmentSection.WaveHeightCalculationsForLowerLimitNorm
                                            .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation)),
                           expectUpdateObserver);

            AttachObserver(mockRepository,
                           assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm
                                            .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation)),
                           expectUpdateObserver);
        }

        private static void AttachObserver(MockRepository mockRepository,
                                           IObservable observable,
                                           bool expectUpdateObserver = true)
        {
            var observer = mockRepository.StrictMock<IObserver>();

            if (expectUpdateObserver)
            {
                observer.Expect(o => o.UpdateObserver());
            }

            observable.Attach(observer);
        }

        private static void AssertHydraulicBoundaryOutput(IAssessmentSection assessmentSection,
                                                          HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                          bool expectedHasOutput)
        {
            Assert.AreEqual(expectedHasOutput, assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm
                                                                .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                                                                .HasOutput);
            Assert.AreEqual(expectedHasOutput, assessmentSection.WaterLevelCalculationsForSignalingNorm
                                                                .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                                                                .HasOutput);
            Assert.AreEqual(expectedHasOutput, assessmentSection.WaterLevelCalculationsForLowerLimitNorm
                                                                .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                                                                .HasOutput);
            Assert.AreEqual(expectedHasOutput, assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm
                                                                .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                                                                .HasOutput);
            Assert.AreEqual(expectedHasOutput, assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm
                                                                .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                                                                .HasOutput);
            Assert.AreEqual(expectedHasOutput, assessmentSection.WaveHeightCalculationsForSignalingNorm
                                                                .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                                                                .HasOutput);
            Assert.AreEqual(expectedHasOutput, assessmentSection.WaveHeightCalculationsForLowerLimitNorm
                                                                .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                                                                .HasOutput);
            Assert.AreEqual(expectedHasOutput, assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm
                                                                .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))
                                                                .HasOutput);
        }

        private static void AssertHydraulicBoundaryLocationOutputCleared(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            Assert.IsFalse(hydraulicBoundaryLocation.WaveHeightCalculation1.HasOutput);
            Assert.IsFalse(hydraulicBoundaryLocation.DesignWaterLevelCalculation1.HasOutput);
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

        #region Data with output

        [Test]
        public void LowerLimitNorm_DataWithOutputAndValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotificationsAndOutputSet(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_DataWithOutputAndValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotificationsAndOutputSet(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_DataWithOutputAndValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotificationsAndOutputSet(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region Data with output but no permission given

        [Test]
        public void LowerLimitNorm_PermissionNotGiven_DoesNotClearDependentDataNorNotifiesObserversOrLogsMessages()
        {
            ChangeValueNoPermissionGivenAndVerifyNoNotificationsAndOutputForAllDataSet(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_PermissionNotGiven_DoesNotClearDependentDataNorNotifiesObserversOrLogsMessages()
        {
            ChangeValueNoPermissionGivenAndVerifyNoNotificationsAndOutputForAllDataSet(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_PermissionNotGiven_DoesNotClearDependentDataNorNotifiesObserversOrLogsMessages()
        {
            ChangeValueNoPermissionGivenAndVerifyNoNotificationsAndOutputForAllDataSet(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion

        #region Data without output

        [Test]
        public void LowerLimitNorm_DataWithoutOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndNoOutputSet(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_DataWithoutOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndNoOutputSet(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_DataWithoutOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            SetPropertyAndVerifyNotificationsAndNoOutputSet(properties => properties.NormativeNorm = newNormativeNorm);
        }

        #endregion
    }
}