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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Read.WaveImpactAsphaltCover;
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Application.Ringtoets.Storage.Test.Read.WaveImpactAsphaltCover
{
    [TestFixture]
    public class WaveImpactAsphaltCoverSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_SectionResultIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new WaveImpactAsphaltCoverSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NeedsDetailedAssessment)]
        [TestCase(AssessmentLayerOneState.Sufficient)]
        public void Read_WithDecimalParameterValues_ReturnSectionResultWithDoubleParameterValues(AssessmentLayerOneState layerOne)
        {
            // Setup
            var random = new Random(21);
            double layerThree = random.NextDouble();
            double layerTwoA = random.NextDouble();
            var collector = new ReadConversionCollector();

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new WaveImpactAsphaltCoverSectionResultEntity
            {
                LayerThree = layerThree,
                LayerTwoA = layerTwoA,
                LayerOne = Convert.ToByte(layerOne),
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            var sectionResult = new WaveImpactAsphaltCoverFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.IsNotNull(sectionResult);
            Assert.AreEqual(layerOne, sectionResult.AssessmentLayerOne);
            Assert.AreEqual(layerTwoA, sectionResult.AssessmentLayerTwoA, 1e-6);
            Assert.AreEqual(layerThree, sectionResult.AssessmentLayerThree, 1e-6);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithNullLayerTwoA_ReturnWaveImpactAsphaltCoverSectionResultWithNullParameters(bool layerOne)
        {
            // Setup
            var collector = new ReadConversionCollector();
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new WaveImpactAsphaltCoverSectionResultEntity
            {
                LayerOne = Convert.ToByte(layerOne),
                LayerTwoA = null,
                LayerThree = new Random(21).NextDouble(),
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            var sectionResult = new WaveImpactAsphaltCoverFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.IsNaN(sectionResult.AssessmentLayerTwoA);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithNullLayerThree_ReturnWaveImpactAsphaltCoverSectionResultWithNullParameters(bool layerOne)
        {
            // Setup
            var collector = new ReadConversionCollector();
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new WaveImpactAsphaltCoverSectionResultEntity
            {
                LayerOne = Convert.ToByte(layerOne),
                LayerTwoA = new Random(21).NextDouble(),
                LayerThree = null,
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            var sectionResult = new WaveImpactAsphaltCoverFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.IsNaN(sectionResult.AssessmentLayerThree);
        }
    }
}