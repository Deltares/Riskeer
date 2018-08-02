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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Data.Test
{
    [TestFixture]
    public class AssessmentSectionTest
    {
        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        public void Constructor_ExpectedValues(AssessmentSectionComposition composition)
        {
            // Call
            var assessmentSection = new AssessmentSection(composition);

            // Assert
            Assert.IsInstanceOf<Observable>(assessmentSection);
            Assert.IsInstanceOf<IAssessmentSection>(assessmentSection);

            Assert.AreEqual("Traject", assessmentSection.Name);
            Assert.IsNull(assessmentSection.Comments.Body);
            Assert.IsNull(assessmentSection.ReferenceLine);
            Assert.AreEqual(composition, assessmentSection.Composition);
            Assert.IsInstanceOf<FailureMechanismContribution>(assessmentSection.FailureMechanismContribution);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            Assert.IsNotNull(hydraulicBoundaryDatabase);
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabase.Locations);
            Assert.IsNull(hydraulicBoundaryDatabase.FilePath);
            Assert.IsNull(hydraulicBoundaryDatabase.Version);
            Assert.IsFalse(hydraulicBoundaryDatabase.CanUsePreprocessor);

            CollectionAssert.IsEmpty(assessmentSection.Piping.StochasticSoilModels);
            CollectionAssert.IsEmpty(assessmentSection.Piping.SurfaceLines);

            Assert.NotNull(assessmentSection.Piping);
            Assert.NotNull(assessmentSection.GrassCoverErosionInwards);
            Assert.NotNull(assessmentSection.MacroStabilityInwards);
            Assert.NotNull(assessmentSection.MacroStabilityOutwards);
            Assert.NotNull(assessmentSection.Microstability);
            Assert.NotNull(assessmentSection.StabilityStoneCover);
            Assert.NotNull(assessmentSection.WaveImpactAsphaltCover);
            Assert.NotNull(assessmentSection.WaterPressureAsphaltCover);
            Assert.NotNull(assessmentSection.GrassCoverErosionOutwards);
            Assert.NotNull(assessmentSection.GrassCoverSlipOffOutwards);
            Assert.NotNull(assessmentSection.GrassCoverSlipOffInwards);
            Assert.NotNull(assessmentSection.HeightStructures);
            Assert.NotNull(assessmentSection.ClosingStructures);
            Assert.NotNull(assessmentSection.PipingStructure);
            Assert.NotNull(assessmentSection.StabilityPointStructures);
            Assert.NotNull(assessmentSection.StrengthStabilityLengthwiseConstruction);
            Assert.NotNull(assessmentSection.DuneErosion);
            Assert.NotNull(assessmentSection.TechnicalInnovation);

            AssertExpectedContributions(composition, assessmentSection);

            Assert.AreEqual(double.NaN, assessmentSection.Piping.PipingProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(double.NaN, assessmentSection.MacroStabilityInwards.MacroStabilityInwardsProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(double.NaN, assessmentSection.MacroStabilityOutwards.MacroStabilityOutwardsProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(double.NaN, assessmentSection.WaveImpactAsphaltCover.GeneralWaveImpactAsphaltCoverInput.SectionLength);

            Assert.IsTrue(assessmentSection.BackgroundData.IsVisible);
            Assert.AreEqual(0.0, assessmentSection.BackgroundData.Transparency.Value);
            Assert.AreEqual("Bing Maps - Satelliet", assessmentSection.BackgroundData.Name);
            var configuration = (WellKnownBackgroundDataConfiguration) assessmentSection.BackgroundData.Configuration;
            Assert.AreEqual(RingtoetsWellKnownTileSource.BingAerial, configuration.WellKnownTileSource);

            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForLowerLimitNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm);
        }

        [Test]
        public void Constructor_InvalidAssessmentSectionComposition_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 99;

            // Call
            TestDelegate call = () => new AssessmentSection((AssessmentSectionComposition) invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'newComposition' ({invalidValue}) is invalid for Enum type '{nameof(AssessmentSectionComposition)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("newComposition", parameterName);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues),
            new object[]
            {
                "Constructor_InvalidLowerLimitNorm_ThrowsArgumentOutOfRangeException"
            })]
        [SetCulture("nl-NL")]
        public void Constructor_InvalidLowerLimitNorm_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Setup
            var random = new Random(21);
            var composition = random.NextEnumValue<AssessmentSectionComposition>();

            // Call
            TestDelegate test = () => new AssessmentSection(composition,
                                                            invalidNorm,
                                                            0.000001);

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues),
            new object[]
            {
                "Constructor_InvalidSignalingNorm_ThrowsArgumentOutOfRangeException"
            })]
        [SetCulture("nl-NL")]
        public void Constructor_InvalidSignalingNorm_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Setup
            var random = new Random(21);
            var composition = random.NextEnumValue<AssessmentSectionComposition>();

            // Call
            TestDelegate test = () => new AssessmentSection(composition,
                                                            0.1,
                                                            invalidNorm);

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_SignalingNormLargerThanLowerLimitNorm_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var random = new Random(21);
            var composition = random.NextEnumValue<AssessmentSectionComposition>();

            // Call
            TestDelegate test = () => new AssessmentSection(composition,
                                                            0.01,
                                                            0.1);

            // Assert
            const string expectedMessage = "De signaleringswaarde moet gelijk zijn aan of kleiner zijn dan de ondergrens.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
        }

        [Test]
        public void Name_SetingNewValue_GetNewValue()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            const string newValue = "new value";

            // Call
            assessmentSection.Name = newValue;

            // Assert
            Assert.AreEqual(newValue, assessmentSection.Name);
        }

        [Test]
        public void Comments_SettingNewValue_GetNewValue()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            const string newValue = "new comment value";

            // Call
            assessmentSection.Comments.Body = newValue;

            // Assert
            Assert.AreEqual(newValue, assessmentSection.Comments.Body);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        public void GetFailureMechanisms_Always_ReturnAllFailureMechanisms(AssessmentSectionComposition composition)
        {
            // Setup
            var assessmentSection = new AssessmentSection(composition);

            // Call
            IFailureMechanism[] failureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();

            // Assert
            Assert.AreEqual(18, failureMechanisms.Length);
            CollectionAssert.AreEqual(new IFailureMechanism[]
            {
                assessmentSection.Piping,
                assessmentSection.GrassCoverErosionInwards,
                assessmentSection.MacroStabilityInwards,
                assessmentSection.MacroStabilityOutwards,
                assessmentSection.Microstability,
                assessmentSection.StabilityStoneCover,
                assessmentSection.WaveImpactAsphaltCover,
                assessmentSection.WaterPressureAsphaltCover,
                assessmentSection.GrassCoverErosionOutwards,
                assessmentSection.GrassCoverSlipOffOutwards,
                assessmentSection.GrassCoverSlipOffInwards,
                assessmentSection.HeightStructures,
                assessmentSection.ClosingStructures,
                assessmentSection.PipingStructure,
                assessmentSection.StabilityPointStructures,
                assessmentSection.StrengthStabilityLengthwiseConstruction,
                assessmentSection.DuneErosion,
                assessmentSection.TechnicalInnovation
            }, failureMechanisms);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        public void GetContributingFailureMechanisms_Always_ReturnContributingFailureMechanisms(AssessmentSectionComposition composition)
        {
            // Setup
            var assessmentSection = new AssessmentSection(composition);

            // Call
            IFailureMechanism[] failureMechanisms = assessmentSection.GetContributingFailureMechanisms()
                                                                     .ToArray();

            // Assert
            Assert.AreEqual(12, failureMechanisms.Length);
            CollectionAssert.AreEqual(new IFailureMechanism[]
            {
                assessmentSection.Piping,
                assessmentSection.GrassCoverErosionInwards,
                assessmentSection.MacroStabilityInwards,
                assessmentSection.StabilityStoneCover,
                assessmentSection.WaveImpactAsphaltCover,
                assessmentSection.GrassCoverErosionOutwards,
                assessmentSection.HeightStructures,
                assessmentSection.ClosingStructures,
                assessmentSection.PipingStructure,
                assessmentSection.StabilityPointStructures,
                assessmentSection.DuneErosion,
                assessmentSection.OtherFailureMechanism
            }, failureMechanisms);
        }

        [Test]
        public void ChangeComposition_InvalidAssessmentSectionComposition_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 99;
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            TestDelegate call = () => assessmentSection.ChangeComposition((AssessmentSectionComposition) invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'newComposition' ({invalidValue}) is invalid for Enum type '{nameof(AssessmentSectionComposition)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("newComposition", parameterName);
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismRelevancy))]
        public void ChangeComposition_ToTargetValue_UpdateContributionsAndFailureMechanismRelevancies(AssessmentSectionComposition composition,
                                                                                                      bool[] relevancies)
        {
            // Setup
            AssessmentSectionComposition initialComposition = composition == AssessmentSectionComposition.Dike
                                                                  ? AssessmentSectionComposition.Dune
                                                                  : AssessmentSectionComposition.Dike;
            var assessmentSection = new AssessmentSection(initialComposition);

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, composition);

            // Call
            assessmentSection.ChangeComposition(composition);

            // Assert
            AssertExpectedContributions(composition, assessmentSection);
            Assert.AreEqual(relevancies[0], assessmentSection.Piping.IsRelevant);
            Assert.AreEqual(relevancies[1], assessmentSection.GrassCoverErosionInwards.IsRelevant);
            Assert.AreEqual(relevancies[2], assessmentSection.MacroStabilityInwards.IsRelevant);
            Assert.AreEqual(relevancies[3], assessmentSection.StabilityStoneCover.IsRelevant);
            Assert.AreEqual(relevancies[4], assessmentSection.WaveImpactAsphaltCover.IsRelevant);
            Assert.AreEqual(relevancies[5], assessmentSection.GrassCoverErosionOutwards.IsRelevant);
            Assert.AreEqual(relevancies[6], assessmentSection.HeightStructures.IsRelevant);
            Assert.AreEqual(relevancies[7], assessmentSection.ClosingStructures.IsRelevant);
            Assert.AreEqual(relevancies[8], assessmentSection.StabilityPointStructures.IsRelevant);
            Assert.AreEqual(relevancies[9], assessmentSection.PipingStructure.IsRelevant);
            Assert.AreEqual(relevancies[10], assessmentSection.DuneErosion.IsRelevant);
        }

        [Test]
        public void ReferenceLine_SetNewValue_GetNewValue()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            var referenceLine = new ReferenceLine();

            // Call
            assessmentSection.ReferenceLine = referenceLine;

            // Assert
            Assert.AreSame(referenceLine, assessmentSection.ReferenceLine);
        }

        [Test]
        public void ReferenceLine_SomeReferenceLine_RelevantGeneralInputSectionLengthSet()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            var referenceLine = new ReferenceLine();

            Point2D[] somePointsCollection =
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            };
            referenceLine.SetGeometry(somePointsCollection);

            // Call
            assessmentSection.ReferenceLine = referenceLine;

            // Assert
            Assert.AreEqual(referenceLine.Length, assessmentSection.Piping.PipingProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(referenceLine.Length, assessmentSection.MacroStabilityInwards.MacroStabilityInwardsProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(referenceLine.Length, assessmentSection.MacroStabilityOutwards.MacroStabilityOutwardsProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(referenceLine.Length, assessmentSection.WaveImpactAsphaltCover.GeneralWaveImpactAsphaltCoverInput.SectionLength);
        }

        [Test]
        public void ReferenceLine_Null_RelevantGeneralInputSectionLengthNaN()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            assessmentSection.ReferenceLine = null;

            // Assert
            Assert.AreEqual(double.NaN, assessmentSection.Piping.PipingProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(double.NaN, assessmentSection.MacroStabilityInwards.MacroStabilityInwardsProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(double.NaN, assessmentSection.MacroStabilityOutwards.MacroStabilityOutwardsProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(double.NaN, assessmentSection.WaveImpactAsphaltCover.GeneralWaveImpactAsphaltCoverInput.SectionLength);
        }

        [Test]
        public void SetHydraulicBoundaryLocationCalculations_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            TestDelegate test = () => assessmentSection.SetHydraulicBoundaryLocationCalculations(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocations", paramName);
        }

        [Test]
        public void SetHydraulicBoundaryLocationCalculations_Always_PreviousCalculationsCleared()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new HydraulicBoundaryLocation[]
            {
                new TestHydraulicBoundaryLocation()
            });

            // Precondition
            CollectionAssert.IsNotEmpty(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm);
            CollectionAssert.IsNotEmpty(assessmentSection.WaterLevelCalculationsForSignalingNorm);
            CollectionAssert.IsNotEmpty(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
            CollectionAssert.IsNotEmpty(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm);
            CollectionAssert.IsNotEmpty(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm);
            CollectionAssert.IsNotEmpty(assessmentSection.WaveHeightCalculationsForSignalingNorm);
            CollectionAssert.IsNotEmpty(assessmentSection.WaveHeightCalculationsForLowerLimitNorm);
            CollectionAssert.IsNotEmpty(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm);

            // Call
            assessmentSection.SetHydraulicBoundaryLocationCalculations(Enumerable.Empty<HydraulicBoundaryLocation>());

            // Assert
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForLowerLimitNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm);
        }

        [Test]
        public void SetHydraulicBoundaryLocationCalculations_MultipleHydraulicBoundaryLocations_SetsExpectedCalculations()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocations = new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            };

            // Call
            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

            // Assert
            AssertNumberOfHydraulicBoundaryLocationCalculations(assessmentSection, 2);
            AssertDefaultHydraulicBoundaryLocationCalculations(assessmentSection, 0, hydraulicBoundaryLocation1);
            AssertDefaultHydraulicBoundaryLocationCalculations(assessmentSection, 1, hydraulicBoundaryLocation2);
        }

        [Test]
        [TestCaseSource(nameof(GetNewFailureMechanismsWithGetPropertyFunc))]
        public void GivenAssessmentSection_WhenSettingFailureMechanismWithSameContribution_ThenNewFailureMechanismSet(
            Action<AssessmentSection, IFailureMechanism> setNewFailureMechanismAction, IFailureMechanism newFailureMechanism,
            Func<AssessmentSection, IFailureMechanism> getFailureMechanismFunc)
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            newFailureMechanism.Contribution = getFailureMechanismFunc(assessmentSection).Contribution;

            // When
            setNewFailureMechanismAction(assessmentSection, newFailureMechanism);

            // Then
            Assert.AreSame(getFailureMechanismFunc(assessmentSection), newFailureMechanism);
        }

        [Test]
        [TestCaseSource(nameof(GetNewFailureMechanisms))]
        public void GivenAssessmentSection_WhenSettingFailureMechanismWithOtherContribution_ThenThrowsArgumentException(
            Action<AssessmentSection, IFailureMechanism> setNewFailureMechanismAction, IFailureMechanism newFailureMechanism)
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            newFailureMechanism.Contribution = random.Next(0, 100);

            // When
            TestDelegate call = () => setNewFailureMechanismAction(assessmentSection, newFailureMechanism);

            // Then
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "De contributie van het nieuwe toetsspoor moet gelijk zijn aan het oude toetsspoor.");
        }

        private static IFailureMechanism[] GetExpectedContributingFailureMechanisms(AssessmentSection section)
        {
            return new IFailureMechanism[]
            {
                section.Piping,
                section.GrassCoverErosionInwards,
                section.MacroStabilityInwards,
                section.StabilityStoneCover,
                section.WaveImpactAsphaltCover,
                section.GrassCoverErosionOutwards,
                section.HeightStructures,
                section.ClosingStructures,
                section.PipingStructure,
                section.StabilityPointStructures,
                section.DuneErosion
            };
        }

        private static void AssertExpectedContributions(AssessmentSectionComposition composition, AssessmentSection assessmentSection)
        {
            Tuple<double, bool>[] contributionTuples = GetContributionsTuples(composition).ToArray();
            double[] contributions = contributionTuples.Select(tuple => tuple.Item1).ToArray();

            Assert.AreEqual(contributions[0], assessmentSection.Piping.Contribution);
            Assert.AreEqual(contributions[1], assessmentSection.GrassCoverErosionInwards.Contribution);
            Assert.AreEqual(contributions[2], assessmentSection.MacroStabilityInwards.Contribution);
            Assert.AreEqual(contributions[3], assessmentSection.MacroStabilityOutwards.Contribution);
            Assert.AreEqual(contributions[4], assessmentSection.StabilityStoneCover.Contribution);
            Assert.AreEqual(contributions[5], assessmentSection.WaveImpactAsphaltCover.Contribution);
            Assert.AreEqual(contributions[6], assessmentSection.GrassCoverErosionOutwards.Contribution);
            Assert.AreEqual(contributions[7], assessmentSection.HeightStructures.Contribution);
            Assert.AreEqual(contributions[8], assessmentSection.ClosingStructures.Contribution);
            Assert.AreEqual(contributions[9], assessmentSection.PipingStructure.Contribution);
            Assert.AreEqual(contributions[10], assessmentSection.StabilityPointStructures.Contribution);
            Assert.AreEqual(contributions[11], assessmentSection.DuneErosion.Contribution);
        }

        private static IEnumerable<Tuple<double, bool>> GetContributionsTuples(AssessmentSectionComposition composition)
        {
            Tuple<double, bool>[] contributions = null;
            switch (composition)
            {
                case AssessmentSectionComposition.Dike:
                    contributions = new[]
                    {
                        new Tuple<double, bool>(24, true),
                        new Tuple<double, bool>(24, true),
                        new Tuple<double, bool>(4, true),
                        new Tuple<double, bool>(4, false),
                        new Tuple<double, bool>(5, true),
                        new Tuple<double, bool>(5, true),
                        new Tuple<double, bool>(5, true),
                        new Tuple<double, bool>(24, true),
                        new Tuple<double, bool>(4, true),
                        new Tuple<double, bool>(2, true),
                        new Tuple<double, bool>(2, true),
                        new Tuple<double, bool>(0, true),
                        new Tuple<double, bool>(30, true)
                    };
                    break;
                case AssessmentSectionComposition.Dune:
                    contributions = new[]
                    {
                        new Tuple<double, bool>(0, true),
                        new Tuple<double, bool>(0, true),
                        new Tuple<double, bool>(0, true),
                        new Tuple<double, bool>(4, false),
                        new Tuple<double, bool>(0, true),
                        new Tuple<double, bool>(0, true),
                        new Tuple<double, bool>(0, true),
                        new Tuple<double, bool>(0, true),
                        new Tuple<double, bool>(0, true),
                        new Tuple<double, bool>(0, true),
                        new Tuple<double, bool>(0, true),
                        new Tuple<double, bool>(70, true),
                        new Tuple<double, bool>(30, true)
                    };
                    break;
                case AssessmentSectionComposition.DikeAndDune:
                    contributions = new[]
                    {
                        new Tuple<double, bool>(24, true),
                        new Tuple<double, bool>(24, true),
                        new Tuple<double, bool>(4, true),
                        new Tuple<double, bool>(4, false),
                        new Tuple<double, bool>(5, true),
                        new Tuple<double, bool>(5, true),
                        new Tuple<double, bool>(5, true),
                        new Tuple<double, bool>(24, true),
                        new Tuple<double, bool>(4, true),
                        new Tuple<double, bool>(2, true),
                        new Tuple<double, bool>(2, true),
                        new Tuple<double, bool>(10, true),
                        new Tuple<double, bool>(20, true)
                    };
                    break;
                default:
                    Assert.Fail("{0} does not have expectancy implemented!", composition);
                    break;
            }

            return contributions;
        }

        private static IEnumerable<TestCaseData> GetFailureMechanismRelevancy()
        {
            yield return new TestCaseData(AssessmentSectionComposition.Dike, new[]
            {
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                false
            });

            yield return new TestCaseData(AssessmentSectionComposition.DikeAndDune, new[]
            {
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true
            });

            yield return new TestCaseData(AssessmentSectionComposition.Dune, new[]
            {
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                true
            });
        }

        private static IEnumerable<TestCaseData> GetInvalidNormValues(string name)
        {
            yield return new TestCaseData(double.PositiveInfinity)
                .SetName($"{name} positive infinity");
            yield return new TestCaseData(double.NegativeInfinity)
                .SetName($"{name} negative infinity");
            yield return new TestCaseData(double.MaxValue)
                .SetName($"{name} maxValue");
            yield return new TestCaseData(double.MinValue)
                .SetName($"{name} minValue");
            yield return new TestCaseData(double.NaN)
                .SetName($"{name} NaN");
            yield return new TestCaseData(0.1 + 1e-6)
                .SetName($"{name} maximum boundary");
            yield return new TestCaseData(0.000001 - 1e-6)
                .SetName($"{name} minimum boundary");
        }

        private static void AssertNumberOfHydraulicBoundaryLocationCalculations(AssessmentSection assessmentSection, int expectedNumberOfCalculations)
        {
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaterLevelCalculationsForSignalingNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaveHeightCalculationsForSignalingNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaveHeightCalculationsForLowerLimitNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.Count());
        }

        private static void AssertDefaultHydraulicBoundaryLocationCalculations(AssessmentSection assessmentSection, int index, HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaveHeightCalculationsForSignalingNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaveHeightCalculationsForLowerLimitNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ElementAt(index), hydraulicBoundaryLocation);
        }

        private static void AssertDefaultHydraulicBoundaryLocationCalculation(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                                                              HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            Assert.AreSame(hydraulicBoundaryLocation, hydraulicBoundaryLocationCalculation.HydraulicBoundaryLocation);
            Assert.IsNull(hydraulicBoundaryLocationCalculation.Output);
            Assert.IsFalse(hydraulicBoundaryLocationCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
        }

        private static IEnumerable<TestCaseData> GetNewFailureMechanismsWithGetPropertyFunc()
        {
            IEnumerable<FailureMechanismTestData> testData = GetFailureMechanismTestData();

            foreach (FailureMechanismTestData failureMechanismTestData in testData)
            {
                yield return new TestCaseData(failureMechanismTestData.SetNewFailureMechanismAction,
                                              failureMechanismTestData.NewFailureMechanism,
                                              failureMechanismTestData.GetFailureMechanismFunc);
            }
        }

        private static IEnumerable<TestCaseData> GetNewFailureMechanisms()
        {
            IEnumerable<FailureMechanismTestData> testData = GetFailureMechanismTestData();

            foreach (FailureMechanismTestData failureMechanismTestData in testData)
            {
                yield return new TestCaseData(failureMechanismTestData.SetNewFailureMechanismAction,
                                              failureMechanismTestData.NewFailureMechanism);
            }
        }

        private static IEnumerable<FailureMechanismTestData> GetFailureMechanismTestData()
        {
            yield return new FailureMechanismTestData((section, failureMechanism) => section.Piping = (PipingFailureMechanism) failureMechanism,
                                                      new PipingFailureMechanism(),
                                                      section => section.Piping);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.GrassCoverErosionInwards = (GrassCoverErosionInwardsFailureMechanism) failureMechanism,
                                                      new GrassCoverErosionInwardsFailureMechanism(),
                                                      section => section.GrassCoverErosionInwards);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.MacroStabilityInwards = (MacroStabilityInwardsFailureMechanism) failureMechanism,
                                                      new MacroStabilityInwardsFailureMechanism(),
                                                      section => section.MacroStabilityInwards);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.MacroStabilityOutwards = (MacroStabilityOutwardsFailureMechanism) failureMechanism,
                                                      new MacroStabilityOutwardsFailureMechanism(),
                                                      section => section.MacroStabilityOutwards);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.Microstability = (MicrostabilityFailureMechanism) failureMechanism,
                                                      new MicrostabilityFailureMechanism(),
                                                      section => section.Microstability);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.StabilityStoneCover = (StabilityStoneCoverFailureMechanism) failureMechanism,
                                                      new StabilityStoneCoverFailureMechanism(),
                                                      section => section.StabilityStoneCover);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.WaveImpactAsphaltCover = (WaveImpactAsphaltCoverFailureMechanism) failureMechanism,
                                                      new WaveImpactAsphaltCoverFailureMechanism(),
                                                      section => section.WaveImpactAsphaltCover);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.WaterPressureAsphaltCover = (WaterPressureAsphaltCoverFailureMechanism) failureMechanism,
                                                      new WaterPressureAsphaltCoverFailureMechanism(),
                                                      section => section.WaterPressureAsphaltCover);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.GrassCoverErosionOutwards = (GrassCoverErosionOutwardsFailureMechanism) failureMechanism,
                                                      new GrassCoverErosionOutwardsFailureMechanism(),
                                                      section => section.GrassCoverErosionOutwards);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.GrassCoverSlipOffOutwards = (GrassCoverSlipOffOutwardsFailureMechanism) failureMechanism,
                                                      new GrassCoverSlipOffOutwardsFailureMechanism(),
                                                      section => section.GrassCoverSlipOffOutwards);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.GrassCoverSlipOffInwards = (GrassCoverSlipOffInwardsFailureMechanism) failureMechanism,
                                                      new GrassCoverSlipOffInwardsFailureMechanism(),
                                                      section => section.GrassCoverSlipOffInwards);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.HeightStructures = (HeightStructuresFailureMechanism) failureMechanism,
                                                      new HeightStructuresFailureMechanism(),
                                                      section => section.HeightStructures);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.ClosingStructures = (ClosingStructuresFailureMechanism) failureMechanism,
                                                      new ClosingStructuresFailureMechanism(),
                                                      section => section.ClosingStructures);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.PipingStructure = (PipingStructureFailureMechanism) failureMechanism,
                                                      new PipingStructureFailureMechanism(),
                                                      section => section.PipingStructure);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.StabilityPointStructures = (StabilityPointStructuresFailureMechanism) failureMechanism,
                                                      new StabilityPointStructuresFailureMechanism(),
                                                      section => section.StabilityPointStructures);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.StrengthStabilityLengthwiseConstruction = (StrengthStabilityLengthwiseConstructionFailureMechanism) failureMechanism,
                                                      new StrengthStabilityLengthwiseConstructionFailureMechanism(),
                                                      section => section.StrengthStabilityLengthwiseConstruction);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.DuneErosion = (DuneErosionFailureMechanism) failureMechanism,
                                                      new DuneErosionFailureMechanism(),
                                                      section => section.DuneErosion);
            yield return new FailureMechanismTestData((section, failureMechanism) => section.TechnicalInnovation = (TechnicalInnovationFailureMechanism) failureMechanism,
                                                      new TechnicalInnovationFailureMechanism(),
                                                      section => section.TechnicalInnovation);
        }

        private class FailureMechanismTestData
        {
            public FailureMechanismTestData(Action<AssessmentSection, IFailureMechanism> setNewFailureMechanismAction,
                                            IFailureMechanism newFailureMechanism,
                                            Func<AssessmentSection, IFailureMechanism> getFailureMechanismFunc)
            {
                SetNewFailureMechanismAction = setNewFailureMechanismAction;
                NewFailureMechanism = newFailureMechanism;
                GetFailureMechanismFunc = getFailureMechanismFunc;
            }

            public Action<AssessmentSection, IFailureMechanism> SetNewFailureMechanismAction { get; }

            public IFailureMechanism NewFailureMechanism { get; }

            public Func<AssessmentSection, IFailureMechanism> GetFailureMechanismFunc { get; }
        }
    }
}