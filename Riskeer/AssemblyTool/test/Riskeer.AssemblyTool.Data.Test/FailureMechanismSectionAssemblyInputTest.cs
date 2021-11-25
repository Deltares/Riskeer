using System;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.AssemblyTool.Data.Test
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyInputTest
    {
        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double profileProbability = random.NextDouble();
            double sectionProbability = random.NextDouble();
            bool needsRefinement = random.NextBoolean();
            double refinedProfileProbability = random.NextDouble();
            double refinedSectionProbability = random.NextDouble();

            // Call
            var input = new FailureMechanismSectionAssemblyInput(isRelevant,
                                                                 profileProbability, sectionProbability,
                                                                 needsRefinement,
                                                                 refinedProfileProbability, refinedSectionProbability);

            // Assert
            Assert.AreEqual(isRelevant, input.IsRelevant);
            Assert.AreEqual(profileProbability, input.ProfileProbability);
            Assert.AreEqual(sectionProbability, input.SectionProbability);
            Assert.AreEqual(needsRefinement, input.NeedsRefinement);
            Assert.AreEqual(refinedProfileProbability, input.RefinedProfileProbability);
            Assert.AreEqual(refinedSectionProbability, input.RefinedSectionProbability);
        }
    }
}