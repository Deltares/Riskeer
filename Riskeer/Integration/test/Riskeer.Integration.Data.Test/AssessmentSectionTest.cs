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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Data.Test
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
            Assert.AreEqual(composition, assessmentSection.Composition);
            Assert.IsInstanceOf<FailureMechanismContribution>(assessmentSection.FailureMechanismContribution);

            ReferenceLine referenceLine = assessmentSection.ReferenceLine;
            Assert.IsNotNull(referenceLine);
            CollectionAssert.IsEmpty(referenceLine.Points);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            Assert.IsNotNull(hydraulicBoundaryDatabase);
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabase.Locations);
            Assert.IsNull(hydraulicBoundaryDatabase.FilePath);
            Assert.IsNull(hydraulicBoundaryDatabase.Version);
            Assert.IsFalse(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor);

            Assert.IsEmpty(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities);
            Assert.IsEmpty(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities);

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

            BackgroundData backgroundData = assessmentSection.BackgroundData;
            Assert.IsTrue(backgroundData.IsVisible);
            Assert.AreEqual(0.60, backgroundData.Transparency, backgroundData.Transparency.GetAccuracy());
            Assert.AreEqual("Bing Maps - Satelliet", backgroundData.Name);
            var configuration = (WellKnownBackgroundDataConfiguration) backgroundData.Configuration;
            Assert.AreEqual(RiskeerWellKnownTileSource.BingAerial, configuration.WellKnownTileSource);

            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);

            AssertFailureProbabilityMarginFactor(composition, assessmentSection);
        }

        [Test]
        public void Constructor_InvalidAssessmentSectionComposition_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 99;

            // Call
            void Call() => new AssessmentSection((AssessmentSectionComposition) invalidValue);

            // Assert
            var expectedMessage = $"The value of argument 'newComposition' ({invalidValue}) is invalid for Enum type '{nameof(AssessmentSectionComposition)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage).ParamName;
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
            void Call() => new AssessmentSection(composition, invalidNorm, 0.000001);

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
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
            void Call() => new AssessmentSection(composition, 0.1, invalidNorm);

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
            StringAssert.StartsWith(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_SignalingNormLargerThanLowerLimitNorm_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var random = new Random(21);
            var composition = random.NextEnumValue<AssessmentSectionComposition>();

            // Call
            void Call() => new AssessmentSection(composition, 0.01, 0.1);

            // Assert
            const string expectedMessage = "De signaleringswaarde moet gelijk zijn aan of kleiner zijn dan de ondergrens.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
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
            void Call() => assessmentSection.ChangeComposition((AssessmentSectionComposition) invalidValue);

            // Assert
            var expectedMessage = $"The value of argument 'newComposition' ({invalidValue}) is invalid for Enum type '{nameof(AssessmentSectionComposition)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage).ParamName;
            Assert.AreEqual("newComposition", parameterName);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune)]
        public void ChangeComposition_ToTargetValue_UpdateContributions(AssessmentSectionComposition composition)
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
            AssertFailureProbabilityMarginFactor(composition, assessmentSection);
        }

        [Test]
        public void SetHydraulicBoundaryLocationCalculations_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            void Call() => assessmentSection.SetHydraulicBoundaryLocationCalculations(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocations", paramName);
        }

        [Test]
        public void SetHydraulicBoundaryLocationCalculations_Always_PreviousCalculationsCleared()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            var waterLevelCalculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
            assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Add(waterLevelCalculationsForTargetProbability);

            var waveHeightCalculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01);
            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Add(waveHeightCalculationsForTargetProbability);

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new HydraulicBoundaryLocation[]
            {
                new TestHydraulicBoundaryLocation()
            });

            // Precondition
            CollectionAssert.IsNotEmpty(assessmentSection.WaterLevelCalculationsForSignalingNorm);
            CollectionAssert.IsNotEmpty(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
            CollectionAssert.IsNotEmpty(waterLevelCalculationsForTargetProbability.HydraulicBoundaryLocationCalculations);
            CollectionAssert.IsNotEmpty(waveHeightCalculationsForTargetProbability.HydraulicBoundaryLocationCalculations);

            // Call
            assessmentSection.SetHydraulicBoundaryLocationCalculations(Enumerable.Empty<HydraulicBoundaryLocation>());

            // Assert
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForSignalingNorm);
            CollectionAssert.IsEmpty(assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
            CollectionAssert.IsEmpty(waterLevelCalculationsForTargetProbability.HydraulicBoundaryLocationCalculations);
            CollectionAssert.IsEmpty(waveHeightCalculationsForTargetProbability.HydraulicBoundaryLocationCalculations);
        }

        [Test]
        public void SetHydraulicBoundaryLocationCalculations_MultipleHydraulicBoundaryLocations_SetsExpectedCalculations()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Add(new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1));
            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Add(new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01));

            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();
            TestHydraulicBoundaryLocation[] hydraulicBoundaryLocations =
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
            void Call() => setNewFailureMechanismAction(assessmentSection, newFailureMechanism);

            // Then
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, "De contributie van het nieuwe toetsspoor moet gelijk zijn aan het oude toetsspoor.");
        }

        private static void AssertFailureProbabilityMarginFactor(AssessmentSectionComposition composition, AssessmentSection assessmentSection)
        {
            Assert.AreEqual(2, assessmentSection.FailureProbabilityMarginFactor.NumberOfDecimalPlaces);

            if (composition == AssessmentSectionComposition.Dune)
            {
                Assert.AreEqual(0, assessmentSection.FailureProbabilityMarginFactor.Value);
            }
            else
            {
                Assert.AreEqual(0.58, assessmentSection.FailureProbabilityMarginFactor.Value);
            }
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
            Assert.AreEqual(contributions[12], assessmentSection.OtherFailureMechanism.Contribution);
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
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaterLevelCalculationsForSignalingNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Count());
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.First().HydraulicBoundaryLocationCalculations.Count());
            Assert.AreEqual(expectedNumberOfCalculations, assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.First().HydraulicBoundaryLocationCalculations.Count());
        }

        private static void AssertDefaultHydraulicBoundaryLocationCalculations(AssessmentSection assessmentSection, int index, HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.First().HydraulicBoundaryLocationCalculations.ElementAt(index), hydraulicBoundaryLocation);
            AssertDefaultHydraulicBoundaryLocationCalculation(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.First().HydraulicBoundaryLocationCalculations.ElementAt(index), hydraulicBoundaryLocation);
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