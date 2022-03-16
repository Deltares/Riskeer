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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

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

            Assert.IsEmpty(assessmentSection.SpecificFailurePaths);

            Assert.IsEmpty(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities);
            Assert.IsEmpty(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities);

            CollectionAssert.IsEmpty(assessmentSection.Piping.StochasticSoilModels);
            CollectionAssert.IsEmpty(assessmentSection.Piping.SurfaceLines);

            Assert.NotNull(assessmentSection.Piping);
            Assert.NotNull(assessmentSection.GrassCoverErosionInwards);
            Assert.NotNull(assessmentSection.MacroStabilityInwards);
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
            Assert.NotNull(assessmentSection.DuneErosion);

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
            Assert.AreEqual(15, failureMechanisms.Length);
            CollectionAssert.AreEqual(new IFailureMechanism[]
            {
                assessmentSection.Piping,
                assessmentSection.GrassCoverErosionInwards,
                assessmentSection.MacroStabilityInwards,
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
                assessmentSection.DuneErosion
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
            Assert.AreEqual(11, failureMechanisms.Length);
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
                assessmentSection.DuneErosion
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
        [TestCaseSource(nameof(GetFailureMechanismInAssemblyStates))]
        public void ChangeComposition_ToTargetValue_UpdateFailureMechanismInAssemblyStates(AssessmentSectionComposition composition,
                                                                                           bool[] inAssemblyStates)
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
            Assert.AreEqual(inAssemblyStates[0], assessmentSection.Piping.InAssembly);
            Assert.AreEqual(inAssemblyStates[1], assessmentSection.GrassCoverErosionInwards.InAssembly);
            Assert.AreEqual(inAssemblyStates[2], assessmentSection.MacroStabilityInwards.InAssembly);
            Assert.AreEqual(inAssemblyStates[3], assessmentSection.StabilityStoneCover.InAssembly);
            Assert.AreEqual(inAssemblyStates[4], assessmentSection.WaveImpactAsphaltCover.InAssembly);
            Assert.AreEqual(inAssemblyStates[5], assessmentSection.GrassCoverErosionOutwards.InAssembly);
            Assert.AreEqual(inAssemblyStates[6], assessmentSection.HeightStructures.InAssembly);
            Assert.AreEqual(inAssemblyStates[7], assessmentSection.ClosingStructures.InAssembly);
            Assert.AreEqual(inAssemblyStates[8], assessmentSection.StabilityPointStructures.InAssembly);
            Assert.AreEqual(inAssemblyStates[9], assessmentSection.PipingStructure.InAssembly);
            Assert.AreEqual(inAssemblyStates[10], assessmentSection.DuneErosion.InAssembly);
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

        private static IEnumerable<TestCaseData> GetFailureMechanismInAssemblyStates()
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
    }
}