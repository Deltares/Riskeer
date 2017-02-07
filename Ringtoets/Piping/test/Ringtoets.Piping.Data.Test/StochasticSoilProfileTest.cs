// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Drawing;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class StochasticSoilProfileTest
    {
        [Test]
        [TestCase(0.1, SoilProfileType.SoilProfile1D, 123L)]
        [TestCase(0.26, SoilProfileType.SoilProfile2D, 123L)]
        public void Constructor_Always_ExpectedValues(double probability, SoilProfileType soilProfileType, long soilProfileId)
        {
            // Call
            var stochasticSoilProfileProbability = new StochasticSoilProfile(probability, soilProfileType, soilProfileId);

            // Assert
            Assert.IsInstanceOf<StochasticSoilProfile>(stochasticSoilProfileProbability);
            Assert.AreEqual(probability, stochasticSoilProfileProbability.Probability);
            Assert.AreEqual(soilProfileType, stochasticSoilProfileProbability.SoilProfileType);
            Assert.AreEqual(soilProfileId, stochasticSoilProfileProbability.SoilProfileId);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.23)]
        [TestCase(0.41)]
        [TestCase(double.NaN)]
        [TestCase(double.MaxValue)]
        [TestCase(double.MinValue)]
        public void AddProbability_DifferentValues_ProbabilityIncreasedAsExpected(double probabilityToAdd)
        {
            // Setup
            double startProbability = new Random(21).NextDouble() * 0.5;
            var profile = new StochasticSoilProfile(startProbability, SoilProfileType.SoilProfile1D, -1);

            // Call
            profile.AddProbability(probabilityToAdd);

            // Assert
            Assert.AreEqual(startProbability + probabilityToAdd, profile.Probability);
        }

        [Test]
        public void Update_WithNullProfile_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0);

            // Call
            TestDelegate test = () => stochasticProfile.Update(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("fromProfile", paramName);
        }

        [Test]
        public void Update_WithValidProfile_UpdatesProperties()
        {
            // Setup
            var newProbability = 1.0;
            var newProfile = new TestPipingSoilProfile();
            var otherStochasticProfile = new StochasticSoilProfile(newProbability, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = newProfile
            };

            var stochasticProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0);

            // Call
            bool updated = stochasticProfile.Update(otherStochasticProfile);

            // Assert
            Assert.IsTrue(updated);
            Assert.AreEqual(newProbability, stochasticProfile.Probability);
            Assert.AreSame(newProfile, stochasticProfile.SoilProfile);
        }

        [Test]
        public void Update_WithEqualProfile_ReturnsFalse()
        {
            // Setup
            var probability = 1.0;
            var profile = new TestPipingSoilProfile();
            var stochasticProfile = new StochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = profile
            };

            var otherStochasticProfile = new StochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = profile
            };

            // Precondition
            Assert.AreEqual(stochasticProfile, otherStochasticProfile);

            // Call
            bool updated = stochasticProfile.Update(otherStochasticProfile);

            // Assert
            Assert.IsFalse(updated);
            Assert.AreEqual(probability, stochasticProfile.Probability);
            Assert.AreSame(profile, stochasticProfile.SoilProfile);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            var stochasticProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0);

            // Call
            bool areEqual = stochasticProfile.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(StochasticProfileCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(StochasticSoilProfile profile, StochasticSoilProfile otherProfile, bool expectedEqual)
        {
            // Call
            bool areEqualOne = profile.Equals(otherProfile);
            bool areEqualTwo = profile.Equals(otherProfile);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        [Test]
        public void ToString_WithNullName_ReturnsStringEmpty()
        {
            // Setup
            var stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(null, 0.0, new[]
                {
                    new PipingSoilLayer(0.0)
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            string text = stochasticSoilProfile.ToString();

            // Assert
            Assert.IsEmpty(text);
        }

        [Test]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            var stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(name, 0.0, new[]
                {
                    new PipingSoilLayer(0.0)
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            string text = stochasticSoilProfile.ToString();

            // Assert
            Assert.AreEqual(name, text);
        }

        private static TestCaseData[] StochasticProfileCombinations()
        {
            StochasticSoilProfile profileA = CreateRandomStochasticProfile(21);
            StochasticSoilProfile profileB = CreateRandomStochasticProfile(21);
            StochasticSoilProfile profileC = CreateRandomStochasticProfile(73);

            return new[]
            {
                new TestCaseData(profileA, profileB, true)
                {
                    TestName = "Equals_ProfileAProfileB_True"
                },
                new TestCaseData(profileB, profileC, false)
                {
                    TestName = "Equals_ProfileBProfileC_False"
                }
            };
        }

        private static StochasticSoilProfile CreateRandomStochasticProfile(int randomSeed)
        {
            var random = new Random(randomSeed);
            return new StochasticSoilProfile(random.NextDouble(), SoilProfileType.SoilProfile1D, random.Next())
            {
                SoilProfile = CreateRandomProfile(random)
            };
        }

        private static PipingSoilProfile CreateRandomProfile(Random random)
        {
            return new PipingSoilProfile(GetRandomName(random), -1.0 - random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    MaterialName = GetRandomName(random),
                    Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                    IsAquifer = random.NextBoolean(),
                    BelowPhreaticLevelDeviation = random.NextDouble(),
                    BelowPhreaticLevelMean = random.NextDouble(),
                    BelowPhreaticLevelShift = random.NextDouble(),
                    DiameterD70Deviation = random.NextDouble(),
                    DiameterD70Mean = random.NextDouble(),
                    PermeabilityDeviation = random.NextDouble(),
                    PermeabilityMean = random.NextDouble()
                }
            }, SoilProfileType.SoilProfile1D, random.Next());
        }

        private static string GetRandomName(Random random)
        {
            return new string('x', random.Next(0, 40));
        }
    }
}