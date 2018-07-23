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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Commands;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.Merge;

namespace Ringtoets.Integration.Plugin.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionMergeHandlerTest
    {
        [Test]
        public void Constructor_ViewCommandsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMergeHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("viewCommands", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            // Call
            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionMergeHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void PerformMerge_targetAssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Call
            TestDelegate call = () => handler.PerformMerge(null, new AssessmentSection(AssessmentSectionComposition.Dike),
                                                           Enumerable.Empty<IFailureMechanism>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("targetAssessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void PerformMerge_sourceAssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Call
            TestDelegate call = () => handler.PerformMerge(new AssessmentSection(AssessmentSectionComposition.Dike),
                                                           null, Enumerable.Empty<IFailureMechanism>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourceAssessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void PerformMerge_FailureMechanismsToMergeNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Call
            TestDelegate call = () => handler.PerformMerge(new AssessmentSection(AssessmentSectionComposition.Dike),
                                                           new AssessmentSection(AssessmentSectionComposition.Dike),
                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismsToMerge", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void PerformMerge_WithAllData_AllViewsForTargetAssessmentSectionClosed()
        {
            // Setup
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(targetAssessmentSection));
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Call
            handler.PerformMerge(targetAssessmentSection,
                                 new AssessmentSection(AssessmentSectionComposition.Dike),
                                 Enumerable.Empty<IFailureMechanism>());

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void PerformMerge_WithAllData_LogMessageAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            targetAssessmentSection.Attach(observer);
            
            // Call
            Action call = () => handler.PerformMerge(targetAssessmentSection,
                                                     new AssessmentSection(AssessmentSectionComposition.Dike),
                                                     Enumerable.Empty<IFailureMechanism>());

            // Assert
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, new[]
            {
                new Tuple<string, LogLevelConstant>("Hydraulische belastingen zijn samengevoegd.", LogLevelConstant.Info)
            });
            mocks.VerifyAll();
        }

        [Test]
        public void PerformMerge_WithAllFailureMechanismsToMerge_SetNewFailureMechanisms()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            handler.PerformMerge(targetAssessmentSection, sourceAssessmentSection, sourceAssessmentSection.GetFailureMechanisms());

            // Assert
            Assert.AreSame(sourceAssessmentSection.Piping, targetAssessmentSection.Piping);
            Assert.AreSame(sourceAssessmentSection.GrassCoverErosionInwards, targetAssessmentSection.GrassCoverErosionInwards);
            Assert.AreSame(sourceAssessmentSection.MacroStabilityInwards, targetAssessmentSection.MacroStabilityInwards);
            Assert.AreSame(sourceAssessmentSection.MacroStabilityOutwards, targetAssessmentSection.MacroStabilityOutwards);
            Assert.AreSame(sourceAssessmentSection.Microstability, targetAssessmentSection.Microstability);
            Assert.AreSame(sourceAssessmentSection.StabilityStoneCover, targetAssessmentSection.StabilityStoneCover);
            Assert.AreSame(sourceAssessmentSection.WaveImpactAsphaltCover, targetAssessmentSection.WaveImpactAsphaltCover);
            Assert.AreSame(sourceAssessmentSection.WaterPressureAsphaltCover, targetAssessmentSection.WaterPressureAsphaltCover);
            Assert.AreSame(sourceAssessmentSection.GrassCoverSlipOffOutwards, targetAssessmentSection.GrassCoverSlipOffOutwards);
            Assert.AreSame(sourceAssessmentSection.GrassCoverSlipOffInwards, targetAssessmentSection.GrassCoverSlipOffInwards);
            Assert.AreSame(sourceAssessmentSection.HeightStructures, targetAssessmentSection.HeightStructures);
            Assert.AreSame(sourceAssessmentSection.ClosingStructures, targetAssessmentSection.ClosingStructures);
            Assert.AreSame(sourceAssessmentSection.PipingStructure, targetAssessmentSection.PipingStructure);
            Assert.AreSame(sourceAssessmentSection.StabilityPointStructures, targetAssessmentSection.StabilityPointStructures);
            Assert.AreSame(sourceAssessmentSection.StrengthStabilityLengthwiseConstruction, targetAssessmentSection.StrengthStabilityLengthwiseConstruction);
            Assert.AreSame(sourceAssessmentSection.TechnicalInnovation, targetAssessmentSection.TechnicalInnovation);
        }

        [Test]
        public void PerformMerge_WithNoFailureMechanismsToMerge_FailureMechanismsSame()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            handler.PerformMerge(targetAssessmentSection, sourceAssessmentSection, Enumerable.Empty<IFailureMechanism>());

            // Assert
            Assert.AreNotSame(sourceAssessmentSection.Piping, targetAssessmentSection.Piping);
            Assert.AreNotSame(sourceAssessmentSection.GrassCoverErosionInwards, targetAssessmentSection.GrassCoverErosionInwards);
            Assert.AreNotSame(sourceAssessmentSection.MacroStabilityInwards, targetAssessmentSection.MacroStabilityInwards);
            Assert.AreNotSame(sourceAssessmentSection.MacroStabilityOutwards, targetAssessmentSection.MacroStabilityOutwards);
            Assert.AreNotSame(sourceAssessmentSection.Microstability, targetAssessmentSection.Microstability);
            Assert.AreNotSame(sourceAssessmentSection.StabilityStoneCover, targetAssessmentSection.StabilityStoneCover);
            Assert.AreNotSame(sourceAssessmentSection.WaveImpactAsphaltCover, targetAssessmentSection.WaveImpactAsphaltCover);
            Assert.AreNotSame(sourceAssessmentSection.WaterPressureAsphaltCover, targetAssessmentSection.WaterPressureAsphaltCover);
            Assert.AreNotSame(sourceAssessmentSection.GrassCoverSlipOffOutwards, targetAssessmentSection.GrassCoverSlipOffOutwards);
            Assert.AreNotSame(sourceAssessmentSection.GrassCoverSlipOffInwards, targetAssessmentSection.GrassCoverSlipOffInwards);
            Assert.AreNotSame(sourceAssessmentSection.HeightStructures, targetAssessmentSection.HeightStructures);
            Assert.AreNotSame(sourceAssessmentSection.ClosingStructures, targetAssessmentSection.ClosingStructures);
            Assert.AreNotSame(sourceAssessmentSection.PipingStructure, targetAssessmentSection.PipingStructure);
            Assert.AreNotSame(sourceAssessmentSection.StabilityPointStructures, targetAssessmentSection.StabilityPointStructures);
            Assert.AreNotSame(sourceAssessmentSection.StrengthStabilityLengthwiseConstruction, targetAssessmentSection.StrengthStabilityLengthwiseConstruction);
            Assert.AreNotSame(sourceAssessmentSection.TechnicalInnovation, targetAssessmentSection.TechnicalInnovation);
        }

        [Test]
        public void PerformMerge_WithAllFailureMechanismsToMerge_LogMessages()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var handler = new AssessmentSectionMergeHandler(viewCommands);
            var targetAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var sourceAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            Action call = () => handler.PerformMerge(targetAssessmentSection, sourceAssessmentSection, sourceAssessmentSection.GetFailureMechanisms());

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(17, msgs.Length);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Piping' zijn vervangen.", msgs[1]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Grasbekleding erosie kruin en binnentalud' zijn vervangen.", msgs[2]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Macrostabiliteit binnenwaarts' zijn vervangen.", msgs[3]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Macrostabiliteit buitenwaarts' zijn vervangen.", msgs[4]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Microstabiliteit' zijn vervangen.", msgs[5]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Stabiliteit steenzetting' zijn vervangen.", msgs[6]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Golfklappen op asfaltbekleding' zijn vervangen.", msgs[7]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Wateroverdruk bij asfaltbekleding' zijn vervangen.", msgs[8]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Grasbekleding afschuiven buitentalud' zijn vervangen.", msgs[9]);
                Assert.AreEqual("Gegevens van toetsspoor 'Dijken en dammen - Grasbekleding afschuiven binnentalud' zijn vervangen.", msgs[10]);
                Assert.AreEqual("Gegevens van toetsspoor 'Kunstwerken - Hoogte kunstwerk' zijn vervangen.", msgs[11]);
                Assert.AreEqual("Gegevens van toetsspoor 'Kunstwerken - Betrouwbaarheid sluiting kunstwerk' zijn vervangen.", msgs[12]);
                Assert.AreEqual("Gegevens van toetsspoor 'Kunstwerken - Piping bij kunstwerk' zijn vervangen.", msgs[13]);
                Assert.AreEqual("Gegevens van toetsspoor 'Kunstwerken - Sterkte en stabiliteit puntconstructies' zijn vervangen.", msgs[14]);
                Assert.AreEqual("Gegevens van toetsspoor 'Kunstwerken - Sterkte en stabiliteit langsconstructies' zijn vervangen.", msgs[15]);
                Assert.AreEqual("Gegevens van toetsspoor 'Technische innovaties - Technische innovaties' zijn vervangen.", msgs[16]);
            });
        }

        #region Hydraulic Boundary Location Calculations

        [Test]
        [TestCaseSource(nameof(GetCalculationsFuncs))]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenTargetAssessmentSectionHasOutput_ThenCalculationsNotChanged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var locations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            SetOutput(targetCalculations);
            sourceCalculations.ForEachElementDo(c => c.InputParameters.ShouldIllustrationPointsBeCalculated = true);

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Precondition
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => !c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => !c.InputParameters.ShouldIllustrationPointsBeCalculated));

            // Call
            handler.PerformMerge(targetAssessmentSection, sourceAssessmentSection, Enumerable.Empty<IFailureMechanism>());

            // Assert
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => !c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => !c.InputParameters.ShouldIllustrationPointsBeCalculated));
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationsFuncs))]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenSourceAssessmentSectionHasOutput_ThenCalculationDataMerged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var locations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            SetOutput(sourceCalculations);
            sourceCalculations.ForEachElementDo(c => c.InputParameters.ShouldIllustrationPointsBeCalculated = true);

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Precondition
            Assert.IsTrue(targetCalculations.All(c => !c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => !c.InputParameters.ShouldIllustrationPointsBeCalculated));
            Assert.IsTrue(sourceCalculations.All(c => c.InputParameters.ShouldIllustrationPointsBeCalculated));

            // Call
            handler.PerformMerge(targetAssessmentSection, sourceAssessmentSection, Enumerable.Empty<IFailureMechanism>());

            // Assert
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => c.InputParameters.ShouldIllustrationPointsBeCalculated));
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationsFuncs))]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenTargetAssessmentSectionHasOutputWithIllustrationPoints_ThenCalculationsNotChanged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var locations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            SetOutput(targetCalculations, true);
            SetOutput(sourceCalculations);

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Precondition
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => c.Output.HasGeneralResult));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => !c.Output.HasGeneralResult));

            // Call
            handler.PerformMerge(targetAssessmentSection, sourceAssessmentSection, Enumerable.Empty<IFailureMechanism>());

            // Assert
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => c.Output.HasGeneralResult));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => !c.Output.HasGeneralResult));
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationsFuncs))]
        public void GivenAssessmentSectionWithHydraulicBoundaryLocationCalculations_WhenSourceAssessmentSectionHasOutputWithIllustrationPoints_ThenCalculationsMerged(
            Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var locations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            AssessmentSection targetAssessmentSection = CreateAssessmentSection(locations);
            AssessmentSection sourceAssessmentSection = CreateAssessmentSection(locations);

            IEnumerable<HydraulicBoundaryLocationCalculation> targetCalculations = getCalculationsFunc(targetAssessmentSection);
            IEnumerable<HydraulicBoundaryLocationCalculation> sourceCalculations = getCalculationsFunc(sourceAssessmentSection);

            SetOutput(targetCalculations);
            SetOutput(sourceCalculations, true);

            var handler = new AssessmentSectionMergeHandler(viewCommands);

            // Precondition
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => !c.Output.HasGeneralResult));
            Assert.IsTrue(sourceCalculations.All(c => c.HasOutput));
            Assert.IsTrue(sourceCalculations.All(c => c.Output.HasGeneralResult));

            // Call
            handler.PerformMerge(targetAssessmentSection, sourceAssessmentSection, Enumerable.Empty<IFailureMechanism>());

            // Assert
            Assert.IsTrue(targetCalculations.All(c => c.HasOutput));
            Assert.IsTrue(targetCalculations.All(c => c.Output.HasGeneralResult));
            mocks.VerifyAll();
        }

        private static void SetOutput(IEnumerable<HydraulicBoundaryLocationCalculation> calculations, bool illustrationPoints = false)
        {
            foreach (HydraulicBoundaryLocationCalculation calculation in calculations)
            {
                calculation.Output = illustrationPoints
                                         ? new TestHydraulicBoundaryLocationCalculationOutput(new TestGeneralResultSubMechanismIllustrationPoint())
                                         : new TestHydraulicBoundaryLocationCalculationOutput();
            }
        }

        private static IEnumerable<TestCaseData> GetCalculationsFuncs()
        {
            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaterLevelCalculationsForFactorizedSignalingNorm));
            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaterLevelCalculationsForSignalingNorm));
            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaterLevelCalculationsForLowerLimitNorm));
            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaterLevelCalculationsForFactorizedLowerLimitNorm));

            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaveHeightCalculationsForFactorizedSignalingNorm));
            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaveHeightCalculationsForSignalingNorm));
            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaveHeightCalculationsForLowerLimitNorm));
            yield return new TestCaseData(new Func<AssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                              section => section.WaveHeightCalculationsForFactorizedLowerLimitNorm));
        }

        private static AssessmentSection CreateAssessmentSection(TestHydraulicBoundaryLocation[] locations)
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.HydraulicBoundaryDatabase.Locations.AddRange(locations);
            assessmentSection.SetHydraulicBoundaryLocationCalculations(locations);
            return assessmentSection;
        }

        #endregion
    }
}