﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Forms.Observers;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Forms.Test.Observers
{
    [TestFixture]
    public class AssessmentSectionResultObserverTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionResultObserver(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenReferenceLineNotified_ThenAttachedObserverNotified()
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
                assessmentSection.ReferenceLine.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenSpecificFailureMechanismCollectionNotified_ThenAttachedObserverNotified()
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
                assessmentSection.SpecificFailureMechanisms.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenSpecificFailureMechanismInCollectionNotified_ThenAttachedObserverNotified()
        {
            // Given
            var specificFailureMechanism = new SpecificFailureMechanism();

            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.SpecificFailureMechanisms.Add(specificFailureMechanism);

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                specificFailureMechanism.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenSpecificFailureMechanismAddedAndNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                var specificFailureMechanism = new SpecificFailureMechanism();
                assessmentSection.SpecificFailureMechanisms.Add(specificFailureMechanism);
                assessmentSection.SpecificFailureMechanisms.NotifyObservers();
                resultObserver.Attach(observer);

                // When
                specificFailureMechanism.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenSpecificFailureMechanismRemovedAndNotified_ThenAttachedObserverNotNotified()
        {
            // Given
            var failureMechanismToRemove = new SpecificFailureMechanism();
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.SpecificFailureMechanisms.AddRange(new[]
            {
                failureMechanismToRemove,
                new SpecificFailureMechanism()
            });

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                mocks.ReplayAll();

                assessmentSection.SpecificFailureMechanisms.Remove(failureMechanismToRemove);
                assessmentSection.SpecificFailureMechanisms.NotifyObservers();

                resultObserver.Attach(observer);

                // When
                failureMechanismToRemove.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismReplaceData))]
        public void GivenAssessmentSectionWithFailureMechanismsReplaced_WhenOldFailureMechanismNotified_ThenAssessmentSectionResultObserverNotNotified<TFailureMechanism>(
            AssessmentSection assessmentSection, Func<AssessmentSection, TFailureMechanism> getFailureMechanismFunc, Action setNewFailureMechanismAction)
            where TFailureMechanism : IFailureMechanism
        {
            // Given
            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                mocks.ReplayAll();

                TFailureMechanism oldFailureMechanism = getFailureMechanismFunc(assessmentSection);

                setNewFailureMechanismAction();
                assessmentSection.NotifyObservers();

                resultObserver.Attach(observer);

                // When
                oldFailureMechanism.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismReplaceData))]
        public void GivenAssessmentSectionWithFailureMechanismsReplaced_WhenNewFailureMechanismNotified_ThenAssessmentSectionResultObserverNotified<TFailureMechanism>(
            AssessmentSection assessmentSection, Func<AssessmentSection, TFailureMechanism> getFailureMechanismFunc, Action setNewFailureMechanismAction)
            where TFailureMechanism : IFailureMechanism
        {
            // Given
            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                setNewFailureMechanismAction();
                assessmentSection.NotifyObservers();

                resultObserver.Attach(observer);

                TFailureMechanism newFailureMechanism = getFailureMechanismFunc(assessmentSection);

                // When
                newFailureMechanism.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionWithPipingFailureMechanismsReplaced_WhenOldPipingScenarioConfigurationsPerFailureMechanismSectionNotified_ThenAssessmentSectionResultObserverNotNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                mocks.ReplayAll();

                PipingFailureMechanism oldFailureMechanism = assessmentSection.Piping;
                FailureMechanismTestHelper.SetSections(oldFailureMechanism, new[]
                {
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1")
                });

                assessmentSection.Piping = new PipingFailureMechanism();
                assessmentSection.NotifyObservers();

                resultObserver.Attach(observer);

                // When
                oldFailureMechanism.ScenarioConfigurationsPerFailureMechanismSection.First().NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionWithPipingFailureMechanismsReplaced_WhenNewPipingScenarioConfigurationsPerFailureMechanismSectionNotified_ThenAssessmentSectionResultObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                PipingFailureMechanism newFailureMechanism = assessmentSection.Piping;
                FailureMechanismTestHelper.SetSections(newFailureMechanism, new[]
                {
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1")
                });

                assessmentSection.Piping = newFailureMechanism;
                assessmentSection.NotifyObservers();

                resultObserver.Attach(observer);

                // When
                newFailureMechanism.ScenarioConfigurationsPerFailureMechanismSection.First().NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenClosingStructuresCalculationNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var calculation = new TestClosingStructuresCalculationScenario();
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
            var calculation = new TestHeightStructuresCalculationScenario();
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
            var calculationScenario =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithInvalidInput<SemiProbabilisticPipingCalculationScenario>();
            assessmentSection.Piping.CalculationsGroup.Children.Add(calculationScenario);

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                calculationScenario.NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenPipingScenarioConfigurationsPerFailureMechanismSectionNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            assessmentSection.Piping = new PipingFailureMechanism();
            FailureMechanismTestHelper.SetSections(assessmentSection.Piping, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1")
            });

            using (var resultObserver = new AssessmentSectionResultObserver(assessmentSection))
            {
                var mocks = new MockRepository();
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                resultObserver.Attach(observer);

                // When
                assessmentSection.Piping.ScenarioConfigurationsPerFailureMechanismSection.First().NotifyObservers();

                // Then
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionResultObserverWithAttachedObserver_WhenStabilityPointStructuresCalculationNotified_ThenAttachedObserverNotified()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var calculation = new TestStabilityPointStructuresCalculationScenario();
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

            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, PipingFailureMechanism>(assessmentSection => assessmentSection.Piping),
                                          new Action(() => assessmentSection1.Piping = assessmentSection2.Piping));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, GrassCoverErosionInwardsFailureMechanism>(assessmentSection => assessmentSection.GrassCoverErosionInwards),
                                          new Action(() => assessmentSection1.GrassCoverErosionInwards = assessmentSection2.GrassCoverErosionInwards));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, MacroStabilityInwardsFailureMechanism>(assessmentSection => assessmentSection.MacroStabilityInwards),
                                          new Action(() => assessmentSection1.MacroStabilityInwards = assessmentSection2.MacroStabilityInwards));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, MicrostabilityFailureMechanism>(assessmentSection => assessmentSection.Microstability),
                                          new Action(() => assessmentSection1.Microstability = assessmentSection2.Microstability));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, StabilityStoneCoverFailureMechanism>(assessmentSection => assessmentSection.StabilityStoneCover),
                                          new Action(() => assessmentSection1.StabilityStoneCover = assessmentSection2.StabilityStoneCover));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, WaveImpactAsphaltCoverFailureMechanism>(assessmentSection => assessmentSection.WaveImpactAsphaltCover),
                                          new Action(() => assessmentSection1.WaveImpactAsphaltCover = assessmentSection2.WaveImpactAsphaltCover));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, ClosingStructuresFailureMechanism>(assessmentSection => assessmentSection.ClosingStructures),
                                          new Action(() => assessmentSection1.ClosingStructures = assessmentSection2.ClosingStructures));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, HeightStructuresFailureMechanism>(assessmentSection => assessmentSection.HeightStructures),
                                          new Action(() => assessmentSection1.HeightStructures = assessmentSection2.HeightStructures));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, StabilityPointStructuresFailureMechanism>(assessmentSection => assessmentSection.StabilityPointStructures),
                                          new Action(() => assessmentSection1.StabilityPointStructures = assessmentSection2.StabilityPointStructures));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, PipingStructureFailureMechanism>(assessmentSection => assessmentSection.PipingStructure),
                                          new Action(() => assessmentSection1.PipingStructure = assessmentSection2.PipingStructure));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, GrassCoverErosionOutwardsFailureMechanism>(assessmentSection => assessmentSection.GrassCoverErosionOutwards),
                                          new Action(() => assessmentSection1.GrassCoverErosionOutwards = assessmentSection2.GrassCoverErosionOutwards));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, GrassCoverSlipOffInwardsFailureMechanism>(assessmentSection => assessmentSection.GrassCoverSlipOffInwards),
                                          new Action(() => assessmentSection1.GrassCoverSlipOffInwards = assessmentSection2.GrassCoverSlipOffInwards));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, GrassCoverSlipOffOutwardsFailureMechanism>(assessmentSection => assessmentSection.GrassCoverSlipOffOutwards),
                                          new Action(() => assessmentSection1.GrassCoverSlipOffOutwards = assessmentSection2.GrassCoverSlipOffOutwards));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, WaterPressureAsphaltCoverFailureMechanism>(assessmentSection => assessmentSection.WaterPressureAsphaltCover),
                                          new Action(() => assessmentSection1.WaterPressureAsphaltCover = assessmentSection2.WaterPressureAsphaltCover));
            yield return new TestCaseData(assessmentSection1, new Func<AssessmentSection, DuneErosionFailureMechanism>(assessmentSection => assessmentSection.DuneErosion),
                                          new Action(() => assessmentSection1.DuneErosion = assessmentSection2.DuneErosion));
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            return new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());
        }
    }
}