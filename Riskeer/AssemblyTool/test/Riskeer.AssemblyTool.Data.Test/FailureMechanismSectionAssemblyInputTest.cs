﻿using System;
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
            double lowerLimitNorm = random.NextDouble();
            double signalingNorm = random.NextDouble();
            bool isRelevant = random.NextBoolean();
            double profileProbability = random.NextDouble();
            double sectionProbability = random.NextDouble();
            bool furtherAnalysisNeeded = random.NextBoolean();
            double refinedProfileProbability = random.NextDouble();
            double refinedSectionProbability = random.NextDouble();

            // Call
            var input = new FailureMechanismSectionAssemblyInput(lowerLimitNorm,
                                                                 signalingNorm,
                                                                 isRelevant, profileProbability,
                                                                 sectionProbability,
                                                                 furtherAnalysisNeeded, refinedProfileProbability, refinedSectionProbability);

            // Assert
            Assert.AreEqual(signalingNorm, input.SignalingNorm);
            Assert.AreEqual(lowerLimitNorm, input.LowerLimitNorm);
            
            Assert.AreEqual(isRelevant, input.IsRelevant);
            Assert.AreEqual(profileProbability, input.InitialProfileProbability);
            Assert.AreEqual(sectionProbability, input.InitialSectionProbability);
            Assert.AreEqual(furtherAnalysisNeeded, input.FurtherAnalysisNeeded);
            Assert.AreEqual(refinedProfileProbability, input.RefinedProfileProbability);
            Assert.AreEqual(refinedSectionProbability, input.RefinedSectionProbability);
        }
    }
}