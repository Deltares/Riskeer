using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Piping.Data;

using RingtoetsIntegrationResources = Ringtoets.Integration.Data.Properties.Resources;

namespace Ringtoets.Integration.Data.Test
{
    [TestFixture]
    public class AssessmentSectionTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var section = new AssessmentSection();

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

            var pipingContribution = 24;
            var contributions = new double[] { pipingContribution, 24, 4, 2, 4, 2, 4, 3, 3, 0, 30 };
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
            Assert.AreEqual(AssessmentSectionComposition.Dike, section.Composition);
            Assert.IsInstanceOf<FailureMechanismContribution>(section.FailureMechanismContribution);

            CollectionAssert.IsEmpty(section.PipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(section.PipingFailureMechanism.SurfaceLines);

            Assert.IsInstanceOf<PipingFailureMechanism>(section.PipingFailureMechanism);
            Assert.AreEqual(grassErosionName, section.GrassErosionFailureMechanism.Name);
            Assert.AreEqual(macrostailityInwardName, section.MacrostabilityInwardFailureMechanism.Name);
            Assert.AreEqual(overtoppingName, section.OvertoppingFailureMechanism.Name);
            Assert.AreEqual(closingName, section.ClosingFailureMechanism.Name);
            Assert.AreEqual(failingOfConstructionName, section.FailingOfConstructionFailureMechanism.Name);
            Assert.AreEqual(stoneRevetmentName, section.StoneRevetmentFailureMechanism.Name);
            Assert.AreEqual(asphaltName, section.AsphaltRevetmentFailureMechanism.Name);
            Assert.AreEqual(grassRevetmentName, section.GrassRevetmentFailureMechanism.Name);
            Assert.AreEqual(duneErosionName, section.DuneErosionFailureMechanism.Name);

            Assert.AreEqual(24, section.PipingFailureMechanism.Contribution);
            Assert.AreEqual(24, section.GrassErosionFailureMechanism.Contribution);
            Assert.AreEqual(4, section.MacrostabilityInwardFailureMechanism.Contribution);
            Assert.AreEqual(2, section.OvertoppingFailureMechanism.Contribution);
            Assert.AreEqual(4, section.ClosingFailureMechanism.Contribution);
            Assert.AreEqual(2, section.FailingOfConstructionFailureMechanism.Contribution);
            Assert.AreEqual(4, section.StoneRevetmentFailureMechanism.Contribution);
            Assert.AreEqual(3, section.AsphaltRevetmentFailureMechanism.Contribution);
            Assert.AreEqual(3, section.GrassRevetmentFailureMechanism.Contribution);
            Assert.AreEqual(0, section.DuneErosionFailureMechanism.Contribution);

            Assert.AreEqual(contributions, section.FailureMechanismContribution.Distribution.Select(d => d.Contribution));
            Assert.AreEqual(names, section.FailureMechanismContribution.Distribution.Select(d => d.Assessment));
            Assert.AreEqual(Enumerable.Repeat(30000.0, 11), section.FailureMechanismContribution.Distribution.Select(d => d.Norm));

            Assert.AreEqual(pipingContribution, section.PipingFailureMechanism.SemiProbabilisticInput.Contribution);
            Assert.AreEqual(30000.0, section.PipingFailureMechanism.SemiProbabilisticInput.Norm);
            Assert.AreEqual(double.NaN, section.PipingFailureMechanism.SemiProbabilisticInput.SectionLength);

            Assert.AreEqual(100, section.FailureMechanismContribution.Distribution.Sum(d => d.Contribution));
        }

        [Test]
        public void Name_SetingNewValue_GetNewValue()
        {
            // Setup
            var section = new AssessmentSection();

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
            var section = new AssessmentSection();

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
            var assessmentSection = new AssessmentSection();
            assessmentSection.ChangeComposition(composition);

            // Call
            var failureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();

            // Assert
            Assert.AreEqual(10, failureMechanisms.Length);
            Assert.AreSame(assessmentSection.PipingFailureMechanism, failureMechanisms[0]);
            Assert.AreSame(assessmentSection.GrassErosionFailureMechanism, failureMechanisms[1]);
            Assert.AreSame(assessmentSection.MacrostabilityInwardFailureMechanism, failureMechanisms[2]);
            Assert.AreSame(assessmentSection.OvertoppingFailureMechanism, failureMechanisms[3]);
            Assert.AreSame(assessmentSection.ClosingFailureMechanism, failureMechanisms[4]);
            Assert.AreSame(assessmentSection.FailingOfConstructionFailureMechanism, failureMechanisms[5]);
            Assert.AreSame(assessmentSection.StoneRevetmentFailureMechanism, failureMechanisms[6]);
            Assert.AreSame(assessmentSection.AsphaltRevetmentFailureMechanism, failureMechanisms[7]);
            Assert.AreSame(assessmentSection.GrassRevetmentFailureMechanism, failureMechanisms[8]);
            Assert.AreSame(assessmentSection.DuneErosionFailureMechanism, failureMechanisms[9]);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        public void FailureMechanismContribution_DefaultConstructed_FailureMechanismContributionWithItemsForFailureMechanismsAndOther(AssessmentSectionComposition composition)
        {
            // Setup
            var assessmentSection = new AssessmentSection();
            assessmentSection.ChangeComposition(composition);

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
            var assessmentSection = new AssessmentSection();
            
            // Call
            assessmentSection.ChangeComposition(composition);

            // Assert
            AssertExpectedContributions(composition, assessmentSection);
        }

        private void AssertExpectedContributions(AssessmentSectionComposition composition, AssessmentSection assessmentSection)
        {
            double[] contributions = GetContributionsArray(composition);

            Assert.AreEqual(contributions[0], assessmentSection.PipingFailureMechanism.Contribution);
            Assert.AreEqual(contributions[1], assessmentSection.GrassErosionFailureMechanism.Contribution);
            Assert.AreEqual(contributions[2], assessmentSection.MacrostabilityInwardFailureMechanism.Contribution);
            Assert.AreEqual(contributions[3], assessmentSection.OvertoppingFailureMechanism.Contribution);
            Assert.AreEqual(contributions[4], assessmentSection.ClosingFailureMechanism.Contribution);
            Assert.AreEqual(contributions[5], assessmentSection.FailingOfConstructionFailureMechanism.Contribution);
            Assert.AreEqual(contributions[6], assessmentSection.StoneRevetmentFailureMechanism.Contribution);
            Assert.AreEqual(contributions[7], assessmentSection.AsphaltRevetmentFailureMechanism.Contribution);
            Assert.AreEqual(contributions[8], assessmentSection.GrassRevetmentFailureMechanism.Contribution);
            Assert.AreEqual(contributions[9], assessmentSection.DuneErosionFailureMechanism.Contribution);

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
            var assessmentSection = new AssessmentSection();
            
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
            var assessmentSection = new AssessmentSection();
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
            Assert.AreEqual(Math2D.Length(referenceLine.Points), assessmentSection.PipingFailureMechanism.SemiProbabilisticInput.SectionLength);
        }

        [Test]
        public void ReferenceLine_Null_GeneralPipingInputSectionLengthNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSection();

            // Call
            assessmentSection.ReferenceLine = null;

            // Assert
            Assert.AreEqual(double.NaN, assessmentSection.PipingFailureMechanism.SemiProbabilisticInput.SectionLength);
        }
    }
}