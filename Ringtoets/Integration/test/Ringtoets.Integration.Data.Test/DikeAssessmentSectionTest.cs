﻿using System;
using System.Collections.Generic;
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
    public class DikeAssessmentSectionTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var section = new DikeAssessmentSection();

            var pipingName = "Dijken - Piping";
            var grassErosionName = "Dijken - Graserosie kruin en binnentalud";
            var macrostailityInwardName = "Dijken - Macrostabiliteit binnenwaarts";
            var overtoppingName = "Kunstwerken - Overslag en overloop";
            var closingName = "Kunstwerken - Niet sluiten";
            var failingOfConstructionName = "Kunstwerken - Constructief falen";
            var stoneRevetmentName = "Dijken - Steenbekledingen";
            var asphaltName = "Dijken - Asfaltbekledingen";
            var grassRevetmentName = "Dijken - Grasbekledingen";

            var pipingContribution = 24;
            var contributions = new double[] { pipingContribution, 24, 4, 2, 4, 2, 4, 3, 3, 30 };
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
                "Overig"
            };

            // Assert
            Assert.IsInstanceOf<Observable>(section);
            Assert.IsInstanceOf<AssessmentSectionBase>(section);

            Assert.AreEqual("Dijktraject", section.Name);
            Assert.IsNull(section.ReferenceLine);
            Assert.IsInstanceOf<FailureMechanismContribution>(section.FailureMechanismContribution);

            CollectionAssert.IsEmpty(section.PipingFailureMechanism.SoilProfiles);
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

            Assert.AreEqual(24, section.PipingFailureMechanism.Contribution);
            Assert.AreEqual(24, section.GrassErosionFailureMechanism.Contribution);
            Assert.AreEqual(4, section.MacrostabilityInwardFailureMechanism.Contribution);
            Assert.AreEqual(2, section.OvertoppingFailureMechanism.Contribution);
            Assert.AreEqual(4, section.ClosingFailureMechanism.Contribution);
            Assert.AreEqual(2, section.FailingOfConstructionFailureMechanism.Contribution);
            Assert.AreEqual(4, section.StoneRevetmentFailureMechanism.Contribution);
            Assert.AreEqual(3, section.AsphaltRevetmentFailureMechanism.Contribution);
            Assert.AreEqual(3, section.GrassRevetmentFailureMechanism.Contribution);

            Assert.AreEqual(contributions, section.FailureMechanismContribution.Distribution.Select(d => d.Contribution));
            Assert.AreEqual(names, section.FailureMechanismContribution.Distribution.Select(d => d.Assessment));
            Assert.AreEqual(Enumerable.Repeat(30000.0, 10), section.FailureMechanismContribution.Distribution.Select(d => d.Norm));

            Assert.AreEqual(pipingContribution, section.PipingFailureMechanism.GeneralInput.Contribution);
            Assert.AreEqual(30000.0, section.PipingFailureMechanism.GeneralInput.Norm);
            Assert.AreEqual(double.NaN, section.PipingFailureMechanism.GeneralInput.SectionLength);

            Assert.AreEqual(100, section.FailureMechanismContribution.Distribution.Sum(d => d.Contribution));
        }

        [Test]
        public void Name_SetingNewValue_GetNewValue()
        {
            // Setup
            var section = new DikeAssessmentSection();

            const string newValue = "new value";

            // Call
            section.Name = newValue;

            // Assert
            Assert.AreEqual(newValue, section.Name);
        }

        [Test]
        public void GetFailureMechanisms_Always_ReturnAllFailureMechanisms()
        {
            // Setup
            var assessmentSection = new DikeAssessmentSection();

            // Call
            var failureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();

            // Assert
            Assert.AreEqual(9, failureMechanisms.Length);
            Assert.AreSame(assessmentSection.PipingFailureMechanism, failureMechanisms[0]);
            Assert.AreSame(assessmentSection.GrassErosionFailureMechanism, failureMechanisms[1]);
            Assert.AreSame(assessmentSection.MacrostabilityInwardFailureMechanism, failureMechanisms[2]);
            Assert.AreSame(assessmentSection.OvertoppingFailureMechanism, failureMechanisms[3]);
            Assert.AreSame(assessmentSection.ClosingFailureMechanism, failureMechanisms[4]);
            Assert.AreSame(assessmentSection.FailingOfConstructionFailureMechanism, failureMechanisms[5]);
            Assert.AreSame(assessmentSection.StoneRevetmentFailureMechanism, failureMechanisms[6]);
            Assert.AreSame(assessmentSection.AsphaltRevetmentFailureMechanism, failureMechanisms[7]);
            Assert.AreSame(assessmentSection.GrassRevetmentFailureMechanism, failureMechanisms[8]);
        }

        [Test]
        public void FailureMechanismContribution_DefaultConstructed_FailureMechanismContributionWithItemsForFailureMechanismsAndOther()
        {
            // Setup
            var assessmentSection = new DikeAssessmentSection();
            var norm = 30000;

            // Call
            var contribution = assessmentSection.FailureMechanismContribution.Distribution.ToArray();

            // Assert
            var failureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();

            Assert.AreEqual(10, contribution.Length);

            for (int i = 0; i < 9; i++)
            {
                Assert.AreEqual(failureMechanisms[i].Name, contribution[i].Assessment);
                Assert.AreEqual(failureMechanisms[i].Contribution, contribution[i].Contribution);
                Assert.AreEqual(norm, contribution[i].Norm);
                Assert.AreEqual((norm / contribution[i].Contribution) * 100, contribution[i].ProbabilitySpace);
            }
            Assert.AreEqual("Overig", contribution[9].Assessment);
            Assert.AreEqual(30, contribution[9].Contribution);
            Assert.AreEqual(norm, contribution[9].Norm);
            Assert.AreEqual((norm / contribution[9].Contribution) * 100, 100000);
        }

        [Test]
        public void ReferenceLine_SomeReferenceLine_GeneralPipingInputSectionLengthSet()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new DikeAssessmentSection();
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
            Assert.AreEqual(Math2D.Length(referenceLine.Points), assessmentSection.PipingFailureMechanism.GeneralInput.SectionLength);
        }

        [Test]
        public void ReferenceLine_Null_GeneralPipingInputSectionLengthNaN()
        {
            // Setup
            var assessmentSection = new DikeAssessmentSection();

            // Call
            assessmentSection.ReferenceLine = null;

            // Assert
            Assert.AreEqual(double.NaN, assessmentSection.PipingFailureMechanism.GeneralInput.SectionLength);
        }
    }
}