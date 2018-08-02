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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Observers;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Data.TestUtil;

namespace Ringtoets.Integration.Forms.Test.Observers
{
    [TestFixture]
    public class AssessmentSectionResultObserverTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new AssessmentSectionResultObserver(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithAssessmentSection_ExpectedProperties()
        {
            // Call
            using (var resultObserver = new AssessmentSectionResultObserver(new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>())))
            {
                // Assert
                Assert.IsInstanceOf<Observable>(resultObserver);
                Assert.IsInstanceOf<IDisposable>(resultObserver);
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenAssessmentSectionNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismReplaceData))]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenAssessmentSectionNotified_ThenFailureMechanismObserversResubscribed<TFailureMechanism>(
            AssessmentSection assessmentSection, TFailureMechanism oldFailureMechanism, Action setNewFailureMechanismAction)
            where TFailureMechanism : IFailureMechanism
        {
            // Given
            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                setNewFailureMechanismAction();
                assessmentSection.NotifyObservers();
                oldFailureMechanism.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenClosingStructuresCalculationNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var calculation = new TestClosingStructuresCalculation();
            assessmentSection.ClosingStructures.CalculationsGroup.Children.Add(calculation);

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                calculation.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenDuneErosionFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.DuneErosion.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenGrassCoverErosionInwardsCalculationNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var calculation = new GrassCoverErosionInwardsCalculation();
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(calculation);

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                calculation.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenGrassCoverErosionOutwardsFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.GrassCoverErosionOutwards.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenHeightStructuresCalculationNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var calculation = new TestHeightStructuresCalculation();
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(calculation);

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                calculation.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenMacroStabilityInwardsCalculationScenarioNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            MacroStabilityInwardsCalculationScenario calculation =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();
            assessmentSection.MacroStabilityInwards.CalculationsGroup.Children.Add(calculation);

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                calculation.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenPipingCalculationScenarioNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            PipingCalculationScenario calculation = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithInvalidInput();
            assessmentSection.MacroStabilityInwards.CalculationsGroup.Children.Add(calculation);

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                calculation.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenStabilityPointStructuresCalculationNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var calculation = new TestStabilityPointStructuresCalculation();
            assessmentSection.StabilityPointStructures.CalculationsGroup.Children.Add(calculation);

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                calculation.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenStabilityStoneCoverFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.StabilityStoneCover.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenWaveImpactAsphaltCoverFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.WaveImpactAsphaltCover.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenGrassCoverSlipOffInwardsFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.GrassCoverSlipOffInwards.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenGrassCoverSlipOffOutwardsFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.GrassCoverSlipOffOutwards.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenMacroStabilityOutwardsFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.MacroStabilityOutwards.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenMicrostabilityFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.Microstability.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenPipingStructureFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.PipingStructure.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenStrengthStabilityLengthwiseConstructionFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.StrengthStabilityLengthwiseConstruction.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenTechnicalInnovationFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.TechnicalInnovation.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenWaterPressureAsphaltCoverFailureMechanismNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.WaterPressureAsphaltCover.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        private static IEnumerable<TestCaseData> GetFailureMechanismReplaceData()
        {
            AssessmentSection assessmentSection1 = CreateAssessmentSection();
            AssessmentSection assessmentSection2 = CreateAssessmentSection();

            yield return new TestCaseData(assessmentSection1, assessmentSection1.Piping,
                                          new Action(() => assessmentSection1.Piping = assessmentSection2.Piping));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.GrassCoverErosionInwards,
                                          new Action(() => assessmentSection1.GrassCoverErosionInwards = assessmentSection2.GrassCoverErosionInwards));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.MacroStabilityInwards,
                                          new Action(() => assessmentSection1.MacroStabilityInwards = assessmentSection2.MacroStabilityInwards));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.MacroStabilityOutwards,
                                          new Action(() => assessmentSection1.MacroStabilityOutwards = assessmentSection2.MacroStabilityOutwards));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.Microstability,
                                          new Action(() => assessmentSection1.Microstability = assessmentSection2.Microstability));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.StabilityStoneCover,
                                          new Action(() => assessmentSection1.StabilityStoneCover = assessmentSection2.StabilityStoneCover));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.WaveImpactAsphaltCover,
                                          new Action(() => assessmentSection1.WaveImpactAsphaltCover = assessmentSection2.WaveImpactAsphaltCover));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.ClosingStructures,
                                          new Action(() => assessmentSection1.ClosingStructures = assessmentSection2.ClosingStructures));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.HeightStructures,
                                          new Action(() => assessmentSection1.HeightStructures = assessmentSection2.HeightStructures));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.StabilityPointStructures,
                                          new Action(() => assessmentSection1.StabilityPointStructures = assessmentSection2.StabilityPointStructures));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.PipingStructure,
                                          new Action(() => assessmentSection1.PipingStructure = assessmentSection2.PipingStructure));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.GrassCoverErosionOutwards,
                                          new Action(() => assessmentSection1.GrassCoverErosionOutwards = assessmentSection2.GrassCoverErosionOutwards));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.GrassCoverSlipOffInwards,
                                          new Action(() => assessmentSection1.GrassCoverSlipOffInwards = assessmentSection2.GrassCoverSlipOffInwards));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.GrassCoverSlipOffOutwards,
                                          new Action(() => assessmentSection1.GrassCoverSlipOffOutwards = assessmentSection2.GrassCoverSlipOffOutwards));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.WaterPressureAsphaltCover,
                                          new Action(() => assessmentSection1.WaterPressureAsphaltCover = assessmentSection2.WaterPressureAsphaltCover));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.StrengthStabilityLengthwiseConstruction,
                                          new Action(() => assessmentSection1.StrengthStabilityLengthwiseConstruction = assessmentSection2.StrengthStabilityLengthwiseConstruction));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.DuneErosion,
                                          new Action(() => assessmentSection1.DuneErosion = assessmentSection2.DuneErosion));
            yield return new TestCaseData(assessmentSection1, assessmentSection1.TechnicalInnovation,
                                          new Action(() => assessmentSection1.TechnicalInnovation = assessmentSection2.TechnicalInnovation));
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            return new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());
        }
    }
}