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
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class StabilityStoneCoverSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new StabilityStoneCoverSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(new StabilityStoneCoverFailureMechanismSectionResult(new TestFailureMechanismSection()), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_SectionResultIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new StabilityStoneCoverSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(null, new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        [TestCase(true, AssessmentLayerTwoAResult.Failed)]
        [TestCase(false, AssessmentLayerTwoAResult.Successful)]
        [TestCase(false, AssessmentLayerTwoAResult.Failed)]
        public void Read_WithDecimalParameterValues_ReturnStabilityStoneCoverSectionResultWithDoubleParameterValues(bool layerOne, AssessmentLayerTwoAResult layerTwoA)
        {
            // Setup
            var random = new Random(21);
            var entityId = random.Next(1, 502);
            double layerThree = random.NextDouble();
            var collector = new ReadConversionCollector();

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new StabilityStoneCoverSectionResultEntity
            {
                StabilityStoneCoverSectionResultEntityId = entityId,
                LayerThree = Convert.ToDecimal(layerThree),
                LayerTwoA = Convert.ToByte(layerTwoA),
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            var sectionResult = new StabilityStoneCoverFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.IsNotNull(sectionResult);
            Assert.AreEqual(entityId, sectionResult.StorageId);
            Assert.AreEqual(layerTwoA, sectionResult.AssessmentLayerTwoA);
            Assert.AreEqual(layerThree, sectionResult.AssessmentLayerThree, 1e-6);
        }

        [Test]
        [TestCase(true, AssessmentLayerTwoAResult.Failed)]
        [TestCase(false, AssessmentLayerTwoAResult.Successful)]
        [TestCase(false, AssessmentLayerTwoAResult.Failed)]
        public void Read_WithNullLayerThree_ReturnStabilityStoneCoverSectionResultWithNullParameters(bool layerOne, AssessmentLayerTwoAResult layerTwoA)
        {
            // Setup
            var collector = new ReadConversionCollector();
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new StabilityStoneCoverSectionResultEntity
            {
                LayerTwoA = Convert.ToByte(layerTwoA),
                LayerThree = null,
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            var sectionResult = new StabilityStoneCoverFailureMechanismSectionResult(new TestFailureMechanismSection()); 

            // Call
            entity.Read(sectionResult, collector);

            // Assert
            Assert.IsNaN(sectionResult.AssessmentLayerThree);
        } 
    }
}