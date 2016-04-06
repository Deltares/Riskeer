using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Contribution;

using RingtoetsIntegrationResources = Ringtoets.Integration.Data.Properties.Resources;

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
            var section = new AssessmentSection(composition);

            var pipingName = "Dijken - Piping";
            var grassErosionName = "Dijken - Graserosie kruin en binnentalud";
            var macrostailityInwardName = "Dijken - Macrostabiliteit binnenwaarts";
            var overtoppingName = "Kunstwerken - Overslag en overloop";
            var closingName = "Kunstwerken - Niet sluiten";
            var failingOfConstructionName = "Kunstwerken - Constructief falen";
            var stoneRevetmentName = "Dijken - Steenbekledingen";
            var asphaltName = "Dijken - Asfaltbekledingen";
            var grassRevetmentName = "Dijken - Grasbekledingen";
            var duneErosionName = "Duinen - Erosie";

            var names = new[] {
                pipingName,
                grassErosionName,
                macrostailityInwardName,
                overtoppingName,
                closingName,
                failingOfConstructionName,
                stoneRevetmentName,
                asphaltName,
                grassRevetmentName,
                duneErosionName,
                "Overig"
            };

            // Assert
            Assert.IsInstanceOf<Observable>(section);
            Assert.IsInstanceOf<IAssessmentSection>(section);

            Assert.AreEqual("Traject", section.Name);
            Assert.IsNull(section.Comments);
            Assert.IsNull(section.ReferenceLine);
            Assert.AreEqual(composition, section.Composition);
            Assert.IsInstanceOf<FailureMechanismContribution>(section.FailureMechanismContribution);

            CollectionAssert.IsEmpty(section.Piping.StochasticSoilModels);
            CollectionAssert.IsEmpty(section.Piping.SurfaceLines);

            Assert.IsInstanceOf<Piping.Data.Piping>(section.Piping);
            Assert.AreEqual(grassErosionName, section.GrassErosion.Name);
            Assert.AreEqual(macrostailityInwardName, section.MacrostabilityInward.Name);
            Assert.AreEqual(overtoppingName, section.Overtopping.Name);
            Assert.AreEqual(closingName, section.Closing.Name);
            Assert.AreEqual(failingOfConstructionName, section.FailingOfConstruction.Name);
            Assert.AreEqual(stoneRevetmentName, section.StoneRevetment.Name);
            Assert.AreEqual(asphaltName, section.AsphaltRevetment.Name);
            Assert.AreEqual(grassRevetmentName, section.GrassRevetment.Name);
            Assert.AreEqual(duneErosionName, section.DuneErosion.Name);

            AssertExpectedContributions(composition, section);

            Assert.AreEqual(names, section.FailureMechanismContribution.Distribution.Select(d => d.Assessment));
            Assert.AreEqual(Enumerable.Repeat(30000.0, 11), section.FailureMechanismContribution.Distribution.Select(d => d.Norm));

            Assert.AreEqual(30000.0, section.Piping.SemiProbabilisticInput.Norm);
            Assert.AreEqual(double.NaN, section.Piping.SemiProbabilisticInput.SectionLength);

            Assert.AreEqual(100, section.FailureMechanismContribution.Distribution.Sum(d => d.Contribution));
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
            Assert.AreEqual(10, failureMechanisms.Length);
            Assert.AreSame(assessmentSection.Piping, failureMechanisms[0]);
            Assert.AreSame(assessmentSection.GrassErosion, failureMechanisms[1]);
            Assert.AreSame(assessmentSection.MacrostabilityInward, failureMechanisms[2]);
            Assert.AreSame(assessmentSection.Overtopping, failureMechanisms[3]);
            Assert.AreSame(assessmentSection.Closing, failureMechanisms[4]);
            Assert.AreSame(assessmentSection.FailingOfConstruction, failureMechanisms[5]);
            Assert.AreSame(assessmentSection.StoneRevetment, failureMechanisms[6]);
            Assert.AreSame(assessmentSection.AsphaltRevetment, failureMechanisms[7]);
            Assert.AreSame(assessmentSection.GrassRevetment, failureMechanisms[8]);
            Assert.AreSame(assessmentSection.DuneErosion, failureMechanisms[9]);
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
            var failureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();

            Assert.AreEqual(11, contribution.Length);

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(failureMechanisms[i].Name, contribution[i].Assessment);
                Assert.AreEqual(failureMechanisms[i].Contribution, contribution[i].Contribution);
                Assert.AreEqual(norm, contribution[i].Norm);
                Assert.AreEqual((norm / contribution[i].Contribution) * 100, contribution[i].ProbabilitySpace);
            }
            Assert.AreEqual("Overig", contribution[10].Assessment);
            double expectedOtherContribution = composition == AssessmentSectionComposition.DikeAndDune ? 20.0 : 30.0;
            Assert.AreEqual(expectedOtherContribution, contribution[10].Contribution);
            Assert.AreEqual(norm, contribution[10].Norm);
            double expectedNorm = composition == AssessmentSectionComposition.DikeAndDune ? 150000 : 100000;
            Assert.AreEqual(expectedNorm, contribution[10].ProbabilitySpace);
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

        private void AssertExpectedContributions(AssessmentSectionComposition composition, AssessmentSection assessmentSection)
        {
            double[] contributions = GetContributionsArray(composition);

            Assert.AreEqual(contributions[0], assessmentSection.Piping.Contribution);
            Assert.AreEqual(contributions[1], assessmentSection.GrassErosion.Contribution);
            Assert.AreEqual(contributions[2], assessmentSection.MacrostabilityInward.Contribution);
            Assert.AreEqual(contributions[3], assessmentSection.Overtopping.Contribution);
            Assert.AreEqual(contributions[4], assessmentSection.Closing.Contribution);
            Assert.AreEqual(contributions[5], assessmentSection.FailingOfConstruction.Contribution);
            Assert.AreEqual(contributions[6], assessmentSection.StoneRevetment.Contribution);
            Assert.AreEqual(contributions[7], assessmentSection.AsphaltRevetment.Contribution);
            Assert.AreEqual(contributions[8], assessmentSection.GrassRevetment.Contribution);
            Assert.AreEqual(contributions[9], assessmentSection.DuneErosion.Contribution);

            CollectionAssert.AreEqual(contributions, assessmentSection.FailureMechanismContribution.Distribution.Select(d => d.Contribution));
        }

        private static double[] GetContributionsArray(AssessmentSectionComposition composition)
        {
            double[] contributions;
            switch (composition)
            {
                case AssessmentSectionComposition.Dike:
                    contributions = new double[]
                    {
                        24,
                        24,
                        4,
                        2,
                        4,
                        2,
                        4,
                        3,
                        3,
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
                        2,
                        4,
                        2,
                        4,
                        3,
                        3,
                        10,
                        20
                    };
                    break;
                default:
                    Assert.Fail("{0} does not have expectancy implemented!", composition);
                    contributions = null;
                    break;
            }
            return contributions;
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
            Assert.AreEqual(Math2D.Length(referenceLine.Points), assessmentSection.Piping.SemiProbabilisticInput.SectionLength);
        }

        [Test]
        public void ReferenceLine_Null_GeneralPipingInputSectionLengthNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            assessmentSection.ReferenceLine = null;

            // Assert
            Assert.AreEqual(double.NaN, assessmentSection.Piping.SemiProbabilisticInput.SectionLength);
        }
    }
}