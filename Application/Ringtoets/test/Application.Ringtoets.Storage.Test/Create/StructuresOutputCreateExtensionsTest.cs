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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil.IllustrationPoints;
using NUnit.Framework;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    internal class StructuresOutputCreateExtensionsTest
    {
        [Test]
        public void Constructor_StructuresOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((StructuresOutput) null).Create<TestStructureOutputEntity>();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("structuresOutput", exception.ParamName);
        }

        [Test]
        public void Create_CalculationWithOutput_ReturnEntityWithOutput()
        {
            // Setup
            var random = new Random(567);
            var output = new StructuresOutput(
                new ProbabilityAssessmentOutput(random.NextDouble(),
                                                random.NextDouble(),
                                                random.NextDouble(),
                                                random.NextDouble(),
                                                random.NextDouble()), null);

            // Call
            var entity = output.Create<TestStructureOutputEntity>();

            // Assert
            Assert.IsNotNull(entity);
            ProbabilityAssessmentOutput probabilityAssessmentOutput = output.ProbabilityAssessmentOutput;
            Assert.AreEqual(probabilityAssessmentOutput.FactorOfSafety, entity.FactorOfSafety,
                            probabilityAssessmentOutput.FactorOfSafety.GetAccuracy());
            Assert.AreEqual(probabilityAssessmentOutput.Reliability, entity.Reliability,
                            probabilityAssessmentOutput.Reliability.GetAccuracy());
            Assert.AreEqual(probabilityAssessmentOutput.Probability, entity.Probability);
            Assert.AreEqual(probabilityAssessmentOutput.RequiredReliability, entity.RequiredReliability,
                            probabilityAssessmentOutput.RequiredReliability.GetAccuracy());
            Assert.AreEqual(probabilityAssessmentOutput.RequiredProbability, entity.RequiredProbability);
            GeneralResultEntityTestHelper.AssertGeneralResultEntity(output.GeneralResult, entity.GeneralResultFaultTreeIllustrationPointEntity);
        }

        [Test]
        public void Create_CalculationWithNaNOutput_ReturnEntityWithNullOutput()
        {
            // Setup
            var output = new StructuresOutput(
                new ProbabilityAssessmentOutput(double.NaN,
                                                double.NaN,
                                                double.NaN,
                                                double.NaN,
                                                double.NaN), null);

            // Call
            var entity = output.Create<TestStructureOutputEntity>();

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.FactorOfSafety);
            Assert.IsNull(entity.Reliability);
            Assert.IsNull(entity.Probability);
            Assert.IsNull(entity.RequiredReliability);
            Assert.IsNull(entity.RequiredProbability);
            GeneralResultEntityTestHelper.AssertGeneralResultEntity(output.GeneralResult, entity.GeneralResultFaultTreeIllustrationPointEntity);
        }

        [Test]
        public void Create_CalculationWithOutputAndGeneralResult_ReturnEntityWithOutputAndGeneralResult()
        {
            // Setup
            StructuresOutput output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint());

            // Call
            var entity = output.Create<TestStructureOutputEntity>();

            // Assert
            Assert.IsNotNull(entity);
            ProbabilityAssessmentOutput probabilityAssessmentOutput = output.ProbabilityAssessmentOutput;
            Assert.AreEqual(probabilityAssessmentOutput.FactorOfSafety, entity.FactorOfSafety,
                            probabilityAssessmentOutput.FactorOfSafety.GetAccuracy());
            Assert.AreEqual(probabilityAssessmentOutput.Reliability, entity.Reliability,
                            probabilityAssessmentOutput.Reliability.GetAccuracy());
            Assert.AreEqual(probabilityAssessmentOutput.Probability, entity.Probability);
            Assert.AreEqual(probabilityAssessmentOutput.RequiredReliability, entity.RequiredReliability,
                            probabilityAssessmentOutput.RequiredReliability.GetAccuracy());
            Assert.AreEqual(probabilityAssessmentOutput.RequiredProbability, entity.RequiredProbability);
            GeneralResultEntityTestHelper.AssertGeneralResultEntity(output.GeneralResult, entity.GeneralResultFaultTreeIllustrationPointEntity);
        }

        private class TestStructureOutputEntity : IProbabilityAssessmentOutputEntity,
                                                  IHasGeneralResultFaultTreeIllustrationPointEntity
        {
            public GeneralResultFaultTreeIllustrationPointEntity GeneralResultFaultTreeIllustrationPointEntity { get; set; }
            public double? RequiredProbability { get; set; }
            public double? RequiredReliability { get; set; }
            public double? Probability { get; set; }
            public double? Reliability { get; set; }
            public double? FactorOfSafety { get; set; }
        }
    }
}