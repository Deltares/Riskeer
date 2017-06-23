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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;

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
            var section = new AssessmentSection(composition);

            const string pipingName = "Dijken en dammen - Piping";
            const string grassErosionInsideName = "Dijken en dammen - Grasbekleding erosie kruin en binnentalud";
            const string macrostabilityInwardName = "Dijken en dammen - Macrostabiliteit binnenwaarts";
            const string stoneRevetmentName = "Dijken en dammen - Stabiliteit steenzetting";
            const string waveImpactAsphaltName = "Dijken en dammen - Golfklappen op asfaltbekleding";
            const string grassCoverErosionOutwardsName = "Dijken en dammen - Grasbekleding erosie buitentalud";
            const string heightStructuresName = "Kunstwerken - Hoogte kunstwerk";
            const string closingStructuresName = "Kunstwerken - Betrouwbaarheid sluiting kunstwerk";
            const string pipingStructuresName = "Kunstwerken - Piping bij kunstwerk";
            const string stabilityPointStructuresName = "Kunstwerken - Sterkte en stabiliteit puntconstructies";
            const string duneErosionName = "Duinwaterkering - Duinafslag";
            const string otherName = "Overig";

            const string pipingCode = "STPH";
            const string grassErosionInsideCode = "GEKB";
            const string macrostabilityInwardCode = "STBI";
            const string stoneRevetmentCode = "ZST";
            const string waveImpactAsphaltCode = "AGK";
            const string grassCoverErosionOutwardsCode = "GEBU";
            const string heightStructuresCode = "HTKW";
            const string closingStructuresCode = "BSKW";
            const string pipingStructuresCode = "PKW";
            const string stabilityPointStructuresCode = "STKWp";
            const string duneErosionCode = "DA";
            const string otherCode = "-";

            var names = new[]
            {
                pipingName,
                grassErosionInsideName,
                macrostabilityInwardName,
                stoneRevetmentName,
                waveImpactAsphaltName,
                grassCoverErosionOutwardsName,
                heightStructuresName,
                closingStructuresName,
                pipingStructuresName,
                stabilityPointStructuresName,
                duneErosionName,
                otherName
            };

            var codes = new[]
            {
                pipingCode,
                grassErosionInsideCode,
                macrostabilityInwardCode,
                stoneRevetmentCode,
                waveImpactAsphaltCode,
                grassCoverErosionOutwardsCode,
                heightStructuresCode,
                closingStructuresCode,
                pipingStructuresCode,
                stabilityPointStructuresCode,
                duneErosionCode,
                otherCode
            };

            // Assert
            Assert.IsInstanceOf<Observable>(section);
            Assert.IsInstanceOf<IAssessmentSection>(section);

            Assert.AreEqual("Traject", section.Name);
            Assert.IsNull(section.Comments.Body);
            Assert.IsNull(section.ReferenceLine);
            Assert.AreEqual(composition, section.Composition);
            Assert.IsInstanceOf<FailureMechanismContribution>(section.FailureMechanismContribution);

            CollectionAssert.IsEmpty(section.PipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(section.PipingFailureMechanism.SurfaceLines);

            Assert.NotNull(section.PipingFailureMechanism);
            Assert.NotNull(section.GrassCoverErosionInwards);
            Assert.NotNull(section.MacroStabilityInwards);
            Assert.NotNull(section.MacrostabilityOutwards);
            Assert.NotNull(section.Microstability);
            Assert.NotNull(section.StabilityStoneCover);
            Assert.NotNull(section.WaveImpactAsphaltCover);
            Assert.NotNull(section.WaterPressureAsphaltCover);
            Assert.NotNull(section.GrassCoverErosionOutwards);
            Assert.NotNull(section.GrassCoverSlipOffOutwards);
            Assert.NotNull(section.GrassCoverSlipOffInwards);
            Assert.NotNull(section.HeightStructures);
            Assert.NotNull(section.ClosingStructures);
            Assert.NotNull(section.PipingStructure);
            Assert.NotNull(section.StabilityPointStructures);
            Assert.NotNull(section.StrengthStabilityLengthwiseConstruction);
            Assert.NotNull(section.DuneErosion);
            Assert.NotNull(section.TechnicalInnovation);

            AssertExpectedContributions(composition, section);

            Assert.AreEqual(names, section.FailureMechanismContribution.Distribution.Select(d => d.Assessment));
            Assert.AreEqual(codes, section.FailureMechanismContribution.Distribution.Select(d => d.AssessmentCode));
            Assert.AreEqual(Enumerable.Repeat(1.0 / 30000.0, 12), section.FailureMechanismContribution.Distribution.Select(d => d.Norm));

            Assert.AreEqual(double.NaN, section.PipingFailureMechanism.PipingProbabilityAssessmentInput.SectionLength);

            Assert.AreEqual(sum, section.FailureMechanismContribution.Distribution.Sum(d => d.Contribution));

            Assert.IsTrue(section.BackgroundData.IsVisible);
            Assert.AreEqual(0.0, section.BackgroundData.Transparency.Value);
            Assert.AreEqual("Bing Maps - Satelliet", section.BackgroundData.Name);
            var configuration = (WellKnownBackgroundDataConfiguration) section.BackgroundData.Configuration;
            Assert.AreEqual(RingtoetsWellKnownTileSource.BingAerial, configuration.WellKnownTileSource);
        }

        [Test]
        public void Name_SetingNewValue_GetNewValue()
        {
            // Setup
            var section = new AssessmentSection(AssessmentSectionComposition.Dike);
            const string newValue = "new value";

            // Call
            section.Name = newValue;

            // Assert
            Assert.AreEqual(newValue, section.Name);
        }

        [Test]
        public void Comments_SettingNewValue_GetNewValue()
        {
            // Setup
            var section = new AssessmentSection(AssessmentSectionComposition.Dike);
            const string newValue = "new comment value";

            // Call
            section.Comments.Body = newValue;

            // Assert
            Assert.AreEqual(newValue, section.Comments.Body);
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
                assessmentSection.PipingFailureMechanism,
                assessmentSection.GrassCoverErosionInwards,
                assessmentSection.MacroStabilityInwards,
                assessmentSection.MacrostabilityOutwards,
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
        [TestCaseSource(nameof(GetFailureMechanismRelevancy))]
        public void ChangeComposition_ToTargetValue_UpdateContributionsAndFailureMechanismRelevancies(AssessmentSectionComposition composition,
                                                                                                      bool[] relevancies)
        {
            // Setup
            AssessmentSectionComposition initialComposition = composition == AssessmentSectionComposition.Dike ?
                                                                  AssessmentSectionComposition.Dune :
                                                                  AssessmentSectionComposition.Dike;
            var assessmentSection = new AssessmentSection(initialComposition);

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, composition);

            // Call
            assessmentSection.ChangeComposition(composition);

            // Assert
            AssertExpectedContributions(composition, assessmentSection);
            Assert.AreEqual(relevancies[0], assessmentSection.PipingFailureMechanism.IsRelevant);
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
        public void ReferenceLine_SomeReferenceLine_GeneralPipingInputSectionLengthSet()
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
            Assert.AreEqual(Math2D.Length(referenceLine.Points), assessmentSection.PipingFailureMechanism.PipingProbabilityAssessmentInput.SectionLength);
        }

        [Test]
        public void ReferenceLine_Null_GeneralPipingInputSectionLengthNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            assessmentSection.ReferenceLine = null;

            // Assert
            Assert.AreEqual(double.NaN, assessmentSection.PipingFailureMechanism.PipingProbabilityAssessmentInput.SectionLength);
        }

        [Test]
        public void HydraulicBoundaryDatabase_SetNewValue_GetNewValue()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            // Assert
            Assert.AreSame(hydraulicBoundaryDatabase, assessmentSection.HydraulicBoundaryDatabase);
        }

        private IFailureMechanism[] GetExpectedContributingFailureMechanisms(AssessmentSection section)
        {
            return new IFailureMechanism[]
            {
                section.PipingFailureMechanism,
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

        private void AssertExpectedContributions(AssessmentSectionComposition composition, AssessmentSection assessmentSection)
        {
            double[] contributions = GetContributionsArray(composition);

            Assert.AreEqual(contributions[0], assessmentSection.PipingFailureMechanism.Contribution);
            Assert.AreEqual(contributions[1], assessmentSection.GrassCoverErosionInwards.Contribution);
            Assert.AreEqual(contributions[2], assessmentSection.MacroStabilityInwards.Contribution);
            Assert.AreEqual(contributions[3], assessmentSection.StabilityStoneCover.Contribution);
            Assert.AreEqual(contributions[4], assessmentSection.WaveImpactAsphaltCover.Contribution);
            Assert.AreEqual(contributions[5], assessmentSection.GrassCoverErosionOutwards.Contribution);
            Assert.AreEqual(contributions[6], assessmentSection.HeightStructures.Contribution);
            Assert.AreEqual(contributions[7], assessmentSection.ClosingStructures.Contribution);
            Assert.AreEqual(contributions[8], assessmentSection.PipingStructure.Contribution);
            Assert.AreEqual(contributions[9], assessmentSection.StabilityPointStructures.Contribution);
            Assert.AreEqual(contributions[10], assessmentSection.DuneErosion.Contribution);

            CollectionAssert.AreEqual(contributions, assessmentSection.FailureMechanismContribution.Distribution.Select(d => d.Contribution));
        }

        private static double[] GetContributionsArray(AssessmentSectionComposition composition)
        {
            double[] contributions = null;
            switch (composition)
            {
                case AssessmentSectionComposition.Dike:
                    contributions = new double[]
                    {
                        24,
                        24,
                        4,
                        5,
                        5,
                        5,
                        24,
                        4,
                        2,
                        2,
                        0,
                        30
                    };
                    break;
                case AssessmentSectionComposition.Dune:
                    contributions = new double[]
                    {
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        70,
                        30
                    };
                    break;
                case AssessmentSectionComposition.DikeAndDune:
                    contributions = new double[]
                    {
                        24,
                        24,
                        4,
                        5,
                        5,
                        5,
                        24,
                        4,
                        2,
                        2,
                        10,
                        20
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
    }
}