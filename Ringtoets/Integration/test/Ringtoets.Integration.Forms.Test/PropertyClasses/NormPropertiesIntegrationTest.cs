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

            SetOutputToAllHydraulicBoundaryLocationCalculations(assessmentSection, random);

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Add(hydraulicBoundaryLocation);
            assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
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
            AttachObserver(mockRepository, assessmentSection.HydraulicBoundaryDatabase, false);
            AttachObserver(mockRepository, assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations, false);
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
            AssertHydraulicBoundaryOutputCleared(assessmentSection);
            AssertHydraulicBoundaryLocationOutputCleared(hydraulicBoundaryLocation);
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

            SetOutputToAllHydraulicBoundaryLocationCalculations(assessmentSection, random);

            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Add(hydraulicBoundaryLocation);
            assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
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
            AttachObserver(mockRepository, assessmentSection.FailureMechanismContribution, false);
            AttachObserver(mockRepository, assessmentSection.HydraulicBoundaryDatabase, false);
            AttachObserver(mockRepository, assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations, false);
            AttachObserver(mockRepository, pipingCalculation, false);
            AttachObserver(mockRepository, emptyPipingCalculation, false);
            AttachObserver(mockRepository, grassCoverErosionInwardsCalculation, false);
            AttachObserver(mockRepository, emptyGrassCoverErosionInwardsCalculation, false);
            AttachObserver(mockRepository, heightStructuresCalculation, false);
            AttachObserver(mockRepository, emptyHeightStructuresCalculation, false);
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

            AssertHydraulicBoundaryOutputNotCleared(assessmentSection);
            Assert.IsTrue(hydraulicBoundaryLocation.WaveHeightCalculation1.HasOutput);
            Assert.IsTrue(hydraulicBoundaryLocation.DesignWaterLevelCalculation1.HasOutput);
            Assert.IsTrue(pipingCalculation.HasOutput);
            Assert.IsTrue(grassCoverErosionInwardsCalculation.HasOutput);
            Assert.IsTrue(heightStructuresCalculation.HasOutput);

            mockRepository.VerifyAll();
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

        private static void AttachObserver(MockRepository mockRepository, IObservable observable, bool expectUpdateObserver = true)
        {
            var observer = mockRepository.StrictMock<IObserver>();

            if (expectUpdateObserver)
            {
                observer.Expect(o => o.UpdateObserver());
            }

            observable.Attach(observer);
        }

        #region AllDataAndOutputSet

        [Test]
        public void LowerLimitNorm_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotificationsAndOutputSet(properties => properties.LowerLimitNorm = newLowerLimitNorm);
        }

        [Test]
        public void SignalingNorm_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotificationsAndOutputSet(properties => properties.SignalingNorm = newSignalingNorm);
        }

        [Test]
        public void NormativeNorm_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            SetPropertyAndVerifyNotificationsAndOutputSet(properties => properties.NormativeNorm = newNormativeNorm);
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

        private static void AssertHydraulicBoundaryOutputCleared(IAssessmentSection assessmentSection)
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

        private static void AssertHydraulicBoundaryOutputNotCleared(IAssessmentSection assessmentSection)
        {
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.All(c => c.Output != null));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForSignalingNorm.All(c => c.Output != null));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.All(c => c.Output != null));
            Assert.IsTrue(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.All(c => c.Output != null));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.All(c => c.Output != null));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForSignalingNorm.All(c => c.Output != null));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForLowerLimitNorm.All(c => c.Output != null));
            Assert.IsTrue(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.All(c => c.Output != null));
        }

        private static void AssertHydraulicBoundaryLocationOutputCleared(HydraulicBoundaryLocation hydraulicBoundaryLocation)
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