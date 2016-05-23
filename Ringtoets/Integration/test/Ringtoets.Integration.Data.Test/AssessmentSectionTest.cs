﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Piping.Data;
using RingtoetsIntegrationResources = Ringtoets.Integration.Data.Properties.Resources;

namespace Ringtoets.Integration.Data.Test
{
    [TestFixture]
    public class AssessmentSectionTest
    {
        [Test]
        [TestCase(AssessmentSectionComposition.Dike, 124)]
        [TestCase(AssessmentSectionComposition.Dune, 100)]
        [TestCase(AssessmentSectionComposition.DikeAndDune, 124)]
        public void Constructor_ExpectedValues(AssessmentSectionComposition composition, int sum)
        {
            // Call
            var section = new AssessmentSection(composition);

            var pipingName = "Dijken en dammen - Piping";
            var grassErosionInsideName = "Dijken en dammen - Grasbekleding erosie kruin en binnentalud";
            var macrostabilityInwardName = "Dijken en dammen - Macrostabiliteit binnenwaarts";
            var stoneRevetmentName = "Dijken en dammen - Stabiliteit steenzetting";
            var waveImpactAsphaltName = "Dijken en dammen - Golfklappen op asfaltbekleding";
            var grassCoverErosionOutwardsName = "Dijken en dammen - Grasbekleding erosie buitentalud";
            var grassCoverSlipOffOutsideName = "Dijken en dammen - Grasbekleding afschuiven buitentalud";
            var heightStructureName = "Kunstwerken - Hoogte kunstwerk";
            var closingStructureName = "Kunstwerken - Betrouwbaarheid sluiting kunstwerk";
            var pipingStructureName = "Kunstwerken - Piping bij kunstwerk";
            var strengthStabilityPointConstructionName = "Kunstwerken - Sterkte en stabiliteit puntconstructies";
            var duneErosionName = "Duinwaterkering - Duinafslag";
            var otherName = "Overig";

            var pipingCode = "STPH";
            var grassErosionInsideCode = "GEKB";
            var macrostabilityInwardCode = "STBI";
            var stoneRevetmentCode = "ZST";
            var waveImpactAsphaltCode = "AGK";
            var grassCoverErosionOutwardsCode = "GEBU";
            var grassCoverSlipOffOutsideCode = "GABU";
            var heightStructureCode = "HTKW";
            var closingStructureCode = "BSKW";
            var pipingStructureCode = "PKW";
            var strengthStabilityPointConstructionCode = "STKWp";
            var duneErosionCode = "DA";
            var otherCode = "-";

            var names = new[]
            {
                pipingName,
                grassErosionInsideName,
                macrostabilityInwardName,
                stoneRevetmentName,
                waveImpactAsphaltName,
                grassCoverErosionOutwardsName,
                grassCoverSlipOffOutsideName,
                heightStructureName,
                closingStructureName,
                pipingStructureName,
                strengthStabilityPointConstructionName,
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
                grassCoverSlipOffOutsideCode,
                heightStructureCode,
                closingStructureCode,
                pipingStructureCode,
                strengthStabilityPointConstructionCode,
                duneErosionCode,
                otherCode
            };

            // Assert
            Assert.IsInstanceOf<Observable>(section);
            Assert.IsInstanceOf<IAssessmentSection>(section);
            Assert.IsInstanceOf<ICommentable>(section);

            Assert.AreEqual("Traject", section.Name);
            Assert.IsNull(section.Comments);
            Assert.IsNull(section.ReferenceLine);
            Assert.AreEqual(composition, section.Composition);
            Assert.IsInstanceOf<FailureMechanismContribution>(section.FailureMechanismContribution);

            CollectionAssert.IsEmpty(section.PipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(section.PipingFailureMechanism.SurfaceLines);

            Assert.NotNull(section.PipingFailureMechanism);
            Assert.NotNull(section.GrassCoverErosionInwards);
            Assert.NotNull(section.MacrostabilityInwards);
            Assert.NotNull(section.MacrostabilityOutwards);
            Assert.NotNull(section.Microstability);
            Assert.NotNull(section.StabilityStoneCover);
            Assert.NotNull(section.WaveImpactAsphaltCover);
            Assert.NotNull(section.WaterPressureAsphaltCover);
            Assert.NotNull(section.GrassCoverErosionOutwards);
            Assert.NotNull(section.GrassCoverSlipOffOutwards);
            Assert.NotNull(section.GrassCoverSlipOffInwards);
            Assert.NotNull(section.HeightStructures);
            Assert.NotNull(section.ClosingStructure);
            Assert.NotNull(section.PipingStructure);
            Assert.NotNull(section.StrengthStabilityPointConstruction);
            Assert.NotNull(section.StrengthStabilityLengthwiseConstruction);
            Assert.NotNull(section.DuneErosion);
            Assert.NotNull(section.TechnicalInnovation);

            AssertExpectedContributions(composition, section);

            Assert.AreEqual(names, section.FailureMechanismContribution.Distribution.Select(d => d.Assessment));
            Assert.AreEqual(codes, section.FailureMechanismContribution.Distribution.Select(d => d.AssessmentCode));
            Assert.AreEqual(Enumerable.Repeat(30000.0, 13), section.FailureMechanismContribution.Distribution.Select(d => d.Norm));

            Assert.AreEqual(30000.0, section.PipingFailureMechanism.NormProbabilityInput.Norm);
            Assert.AreEqual(double.NaN, section.PipingFailureMechanism.NormProbabilityInput.SectionLength);

            Assert.AreEqual(sum, section.FailureMechanismContribution.Distribution.Sum(d => d.Contribution));
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
            section.Comments = newValue;

            // Assert
            Assert.AreEqual(newValue, section.Comments);
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
            var failureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();

            // Assert
            Assert.AreEqual(18, failureMechanisms.Length);
            CollectionAssert.AreEqual(new IFailureMechanism[]
            {
                assessmentSection.PipingFailureMechanism,
                assessmentSection.GrassCoverErosionInwards,
                assessmentSection.MacrostabilityInwards,
                assessmentSection.MacrostabilityOutwards,
                assessmentSection.Microstability,
                assessmentSection.StabilityStoneCover,
                assessmentSection.WaveImpactAsphaltCover,
                assessmentSection.WaterPressureAsphaltCover,
                assessmentSection.GrassCoverErosionOutwards,
                assessmentSection.GrassCoverSlipOffOutwards,
                assessmentSection.GrassCoverSlipOffInwards,
                assessmentSection.HeightStructures,
                assessmentSection.ClosingStructure,
                assessmentSection.PipingStructure,
                assessmentSection.StrengthStabilityPointConstruction,
                assessmentSection.StrengthStabilityLengthwiseConstruction,
                assessmentSection.DuneErosion,
                assessmentSection.TechnicalInnovation
            }, failureMechanisms);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        public void FailureMechanismContribution_DefaultConstructed_FailureMechanismContributionWithItemsForFailureMechanismsAndOther(AssessmentSectionComposition composition)
        {
            // Setup
            var assessmentSection = new AssessmentSection(composition);

            const int norm = 30000;

            // Call
            var contribution = assessmentSection.FailureMechanismContribution.Distribution.ToArray();

            // Assert
            var failureMechanisms = GetExpectedContributingFailureMechanisms(assessmentSection);

            Assert.AreEqual(13, contribution.Length);

            for (int i = 0; i < 12; i++)
            {
                Assert.AreEqual(failureMechanisms[i].Name, contribution[i].Assessment);
                Assert.AreEqual(failureMechanisms[i].Contribution, contribution[i].Contribution);
                Assert.AreEqual(norm, contribution[i].Norm);
                Assert.AreEqual((norm/contribution[i].Contribution)*100, contribution[i].ProbabilitySpace);
            }
            var otherContributionItem = contribution[12];
            Assert.AreEqual("Overig", otherContributionItem.Assessment);
            double expectedOtherContribution = composition == AssessmentSectionComposition.DikeAndDune ? 20.0 : 30.0;
            Assert.AreEqual(expectedOtherContribution, otherContributionItem.Contribution);
            Assert.AreEqual(norm, otherContributionItem.Norm);
            double expectedNorm = composition == AssessmentSectionComposition.DikeAndDune ? 150000 : 100000;
            Assert.AreEqual(expectedNorm, otherContributionItem.ProbabilitySpace);
        }

        private IFailureMechanism[] GetExpectedContributingFailureMechanisms(AssessmentSection section)
        {
            return new IFailureMechanism[]
            {
                section.PipingFailureMechanism,
                section.GrassCoverErosionInwards,
                section.MacrostabilityInwards,
                section.StabilityStoneCover,
                section.WaveImpactAsphaltCover,
                section.GrassCoverErosionOutwards,
                section.GrassCoverSlipOffOutwards,
                section.HeightStructures,
                section.ClosingStructure,
                section.PipingStructure,
                section.StrengthStabilityPointConstruction,
                section.DuneErosion,
            };
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        public void ChangeComposition_ToTargetValue_UpdateContributions(AssessmentSectionComposition composition)
        {
            // Setup
            var initialComposition = composition == AssessmentSectionComposition.Dike ?
                                         AssessmentSectionComposition.Dune :
                                         AssessmentSectionComposition.Dike;
            var assessmentSection = new AssessmentSection(initialComposition);

            // Precondition
            Assert.AreNotEqual(assessmentSection.Composition, composition);

            // Call
            assessmentSection.ChangeComposition(composition);

            // Assert
            AssertExpectedContributions(composition, assessmentSection);
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
            ReferenceLine referenceLine = new ReferenceLine();

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
            Assert.AreEqual(Math2D.Length(referenceLine.Points), assessmentSection.PipingFailureMechanism.NormProbabilityInput.SectionLength);
        }

        [Test]
        public void ReferenceLine_Null_GeneralPipingInputSectionLengthNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            assessmentSection.ReferenceLine = null;

            // Assert
            Assert.AreEqual(double.NaN, assessmentSection.PipingFailureMechanism.NormProbabilityInput.SectionLength);
        }

        private void AssertExpectedContributions(AssessmentSectionComposition composition, AssessmentSection assessmentSection)
        {
            double[] contributions = GetContributionsArray(composition);

            Assert.AreEqual(contributions[0], assessmentSection.PipingFailureMechanism.Contribution);
            Assert.AreEqual(contributions[1], assessmentSection.GrassCoverErosionInwards.Contribution);
            Assert.AreEqual(contributions[2], assessmentSection.MacrostabilityInwards.Contribution);
            Assert.AreEqual(contributions[3], assessmentSection.StabilityStoneCover.Contribution);
            Assert.AreEqual(contributions[4], assessmentSection.WaveImpactAsphaltCover.Contribution);
            Assert.AreEqual(contributions[5], assessmentSection.GrassCoverErosionOutwards.Contribution);
            Assert.AreEqual(contributions[6], assessmentSection.GrassCoverSlipOffOutwards.Contribution);
            Assert.AreEqual(contributions[7], assessmentSection.HeightStructures.Contribution);
            Assert.AreEqual(contributions[8], assessmentSection.ClosingStructure.Contribution);
            Assert.AreEqual(contributions[9], assessmentSection.PipingStructure.Contribution);
            Assert.AreEqual(contributions[10], assessmentSection.StrengthStabilityPointConstruction.Contribution);
            Assert.AreEqual(contributions[11], assessmentSection.DuneErosion.Contribution);

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
                        3,
                        1,
                        5,
                        1,
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
                        3,
                        1,
                        5,
                        1,
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
    }
}