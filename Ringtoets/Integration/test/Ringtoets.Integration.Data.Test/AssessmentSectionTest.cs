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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Integration.Data.Test
{
    [TestFixture]
    public class AssessmentSectionTest
    {
        [Test]
        [TestCase(AssessmentSectionComposition.Dike, 129)]
        [TestCase(AssessmentSectionComposition.Dune, 100)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, 129)]
        public void Constructor_ExpectedValues(AssessmentSectionComposition composition, int sum)
        {
            // Call
            var assessmentSection = new AssessmentSection(composition);

            const string pipingName = "Dijken en dammen - Piping";
            const string grassErosionInsideName = "Dijken en dammen - Grasbekleding erosie kruin en binnentalud";
            const string macroStabilityInwardsName = "Dijken en dammen - Macrostabiliteit binnenwaarts";
            const string stoneRevetmentName = "Dijken en dammen - Stabiliteit steenzetting";
            const string waveImpactAsphaltName = "Dijken en dammen - Golfklappen op asfaltbekleding";
            const string grassCoverErosionOutwardsName = "Dijken en dammen - Grasbekleding erosie buitentalud";
            const string heightStructuresName = "Kunstwerken - Hoogte kunstwerk";
            const string closingStructuresName = "Kunstwerken - Betrouwbaarheid sluiting kunstwerk";
            const string pipingStructureName = "Kunstwerken - Piping bij kunstwerk";
            const string stabilityPointStructuresName = "Kunstwerken - Sterkte en stabiliteit puntconstructies";
            const string duneErosionName = "Duinwaterkering - Duinafslag";
            const string otherName = "Overig";

            const string pipingCode = "STPH";
            const string grassErosionInsideCode = "GEKB";
            const string macroStabilityInwardsCode = "STBI";
            const string stoneRevetmentCode = "ZST";
            const string waveImpactAsphaltCode = "AGK";
            const string grassCoverErosionOutwardsCode = "GEBU";
            const string heightStructuresCode = "HTKW";
            const string closingStructuresCode = "BSKW";
            const string pipingStructureCode = "PKW";
            const string stabilityPointStructuresCode = "STKWp";
            const string duneErosionCode = "DA";
            const string otherCode = "-";

            var names = new[]
            {
                pipingName,
                grassErosionInsideName,
                macroStabilityInwardsName,
                stoneRevetmentName,
                waveImpactAsphaltName,
                grassCoverErosionOutwardsName,
                heightStructuresName,
                closingStructuresName,
                pipingStructureName,
                stabilityPointStructuresName,
                duneErosionName,
                otherName
            };

            var codes = new[]
            {
                pipingCode,
                grassErosionInsideCode,
                macroStabilityInwardsCode,
                stoneRevetmentCode,
                waveImpactAsphaltCode,
                grassCoverErosionOutwardsCode,
                heightStructuresCode,
                closingStructuresCode,
                pipingStructureCode,
                stabilityPointStructuresCode,
                duneErosionCode,
                otherCode
            };

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

            Assert.AreEqual(names, assessmentSection.FailureMechanismContribution.Distribution.Select(d => d.Assessment));
            Assert.AreEqual(codes, assessmentSection.FailureMechanismContribution.Distribution.Select(d => d.AssessmentCode));
            Assert.AreEqual(Enumerable.Repeat(1.0 / 30000.0, 12), assessmentSection.FailureMechanismContribution.Distribution.Select(d => d.Norm));

            Assert.AreEqual(double.NaN, assessmentSection.Piping.PipingProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(double.NaN, assessmentSection.MacroStabilityInwards.MacroStabilityInwardsProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(double.NaN, assessmentSection.MacroStabilityOutwards.MacroStabilityOutwardsProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(double.NaN, assessmentSection.WaveImpactAsphaltCover.GeneralWaveImpactAsphaltCoverInput.SectionLength);

            Assert.AreEqual(sum, assessmentSection.FailureMechanismContribution.Distribution.Sum(d => d.Contribution));

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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
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
        public void FailureMechanismContribution_DefaultConstructed_FailureMechanismContributionWithItemsForFailureMechanismsAndOther(
            AssessmentSectionComposition composition)
        {
            // Setup
            var assessmentSection = new AssessmentSection(composition);

            const double norm = 1.0 / 30000;

            // Call
            FailureMechanismContributionItem[] contribution = assessmentSection.FailureMechanismContribution.Distribution.ToArray();

            // Assert
            IFailureMechanism[] failureMechanisms = GetExpectedContributingFailureMechanisms(assessmentSection);

            Assert.AreEqual(12, contribution.Length);
            for (var i = 0; i < 11; i++)
            {
                Assert.AreEqual(failureMechanisms[i].Name, contribution[i].Assessment);
                Assert.AreEqual(failureMechanisms[i].Contribution, contribution[i].Contribution);
                Assert.AreEqual(norm, contribution[i].Norm);
                Assert.AreEqual(100.0 / (norm * contribution[i].Contribution), contribution[i].ProbabilitySpace);
            }

            FailureMechanismContributionItem otherContributionItem = contribution[11];
            Assert.AreEqual("Overig", otherContributionItem.Assessment);
            double expectedOtherContribution = composition == AssessmentSectionComposition.DikeAndDune ? 20.0 : 30.0;
            Assert.AreEqual(expectedOtherContribution, otherContributionItem.Contribution);
            Assert.AreEqual(norm, otherContributionItem.Norm);
            double expectedNorm = composition == AssessmentSectionComposition.DikeAndDune ? 150000 : 100000;
            Assert.AreEqual(expectedNorm, otherContributionItem.ProbabilitySpace, 1e-6);
        }

        [Test]
        public void ChangeComposition_InvalidAssessmentSectionComposition_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 99;
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
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

            CollectionAssert.AreEqual(contributionTuples.Where(tuple => tuple.Item2).Select(tuple => tuple.Item1),
                                      assessmentSection.FailureMechanismContribution.Distribution.Select(d => d.Contribution));
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
    }
}