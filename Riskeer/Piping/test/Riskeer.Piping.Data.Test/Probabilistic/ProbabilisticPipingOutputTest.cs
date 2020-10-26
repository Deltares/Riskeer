﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingOutputTest
    {
        [Test]
        public void Constructor_SectionSpecificOutputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ProbabilisticPipingOutput(null,
                                                         PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("sectionSpecificOutput", paramName);
        }

        [Test]
        public void Constructor_ProfileSpecificOutputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ProbabilisticPipingOutput(PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(),
                                                         null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("profileSpecificOutput", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            PartialProbabilisticPipingOutput sectionSpecificOutput = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput();
            PartialProbabilisticPipingOutput profileSpecificOutput = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput();

            // Call
            var output = new ProbabilisticPipingOutput(sectionSpecificOutput, profileSpecificOutput);

            // Assert
            Assert.IsInstanceOf<CloneableObservable>(output);
            Assert.IsInstanceOf<ICalculationOutput>(output);
            Assert.AreSame(sectionSpecificOutput, output.SectionSpecificOutput);
            Assert.AreSame(profileSpecificOutput, output.ProfileSpecificOutput);
        }

        [Test]
        public void ClearIllustrationPoints_OutputWithGeneralResult_ClearsGeneralResultAndOtherOutputIsNotAffected()
        {
            // Setup
            var random = new Random(39);
            double sectionSpecificReliability = random.NextDouble();
            double profileSpecificReliability = random.NextDouble();
            var sectionSpecificOutput = new PartialProbabilisticPipingOutput(sectionSpecificReliability,
                                                                             new TestGeneralResultFaultTreeIllustrationPoint());
            var profileSpecificOutput = new PartialProbabilisticPipingOutput(profileSpecificReliability,
                                                                             new TestGeneralResultFaultTreeIllustrationPoint());
            var output = new ProbabilisticPipingOutput(sectionSpecificOutput, profileSpecificOutput);

            // Call
            output.ClearIllustrationPoints();

            // Assert
            Assert.AreEqual(sectionSpecificReliability, output.SectionSpecificOutput.Reliability);
            Assert.IsFalse(output.SectionSpecificOutput.HasGeneralResult);
            Assert.IsNull(output.SectionSpecificOutput.GeneralResult);
            Assert.AreEqual(profileSpecificReliability, output.ProfileSpecificOutput.Reliability);
            Assert.IsFalse(output.ProfileSpecificOutput.HasGeneralResult);
            Assert.IsNull(output.ProfileSpecificOutput.GeneralResult);
        }

        [Test]
        public void ClearIllustrationPoints_OutputWithoutGeneralResult_OtherOutputIsNotAffected()
        {
            // Setup
            var random = new Random(39);
            double sectionSpecificReliability = random.NextDouble();
            double profileSpecificReliability = random.NextDouble();
            var sectionSpecificOutput = new PartialProbabilisticPipingOutput(sectionSpecificReliability, null);
            var profileSpecificOutput = new PartialProbabilisticPipingOutput(profileSpecificReliability, null);
            var output = new ProbabilisticPipingOutput(sectionSpecificOutput, profileSpecificOutput);

            // Call
            output.ClearIllustrationPoints();

            // Assert
            Assert.AreEqual(sectionSpecificReliability, output.SectionSpecificOutput.Reliability);
            Assert.IsFalse(output.SectionSpecificOutput.HasGeneralResult);
            Assert.IsNull(output.SectionSpecificOutput.GeneralResult);
            Assert.AreEqual(profileSpecificReliability, output.ProfileSpecificOutput.Reliability);
            Assert.IsFalse(output.ProfileSpecificOutput.HasGeneralResult);
            Assert.IsNull(output.ProfileSpecificOutput.GeneralResult);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            ProbabilisticPipingOutput original = PipingTestDataGenerator.GetRandomProbabilisticPipingOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }
    }
}