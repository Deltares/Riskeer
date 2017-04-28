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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Read.Piping;
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Read.Piping
{
    [TestFixture]
    public class PipingSectionResultEntityReadExtensionsTest
    {
        [Test]
        public void Read_SectionResultIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new PipingSectionResultEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        [Combinatorial]
        public void Read_ParameterValues_SectionResultWithParameterValues(
            [Values(AssessmentLayerOneState.NotAssessed, AssessmentLayerOneState.NoVerdict,
                AssessmentLayerOneState.Sufficient)] AssessmentLayerOneState layerOne,
            [Values(AssessmentLayerTwoAResult.NotCalculated, AssessmentLayerTwoAResult.Failed,
                AssessmentLayerTwoAResult.Successful)] AssessmentLayerTwoAResult layerTwoA,
            [Values(1.1, 2.2, null)] double? layerThree)
        {
            // Setup
            var collector = new ReadConversionCollector();

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity();
            collector.Read(failureMechanismSectionEntity, new TestFailureMechanismSection());
            var entity = new PipingSectionResultEntity
            {
                LayerThree = layerThree,
                LayerOne = Convert.ToByte(layerOne),
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };

            var sectionResult = new PipingFailureMechanismSectionResult(new TestFailureMechanismSection());

            // Call
            entity.Read(sectionResult);

            // Assert
            Assert.IsNotNull(sectionResult);
            Assert.AreEqual(layerOne, sectionResult.AssessmentLayerOne);
            Assert.AreEqual(layerThree ?? double.NaN, sectionResult.AssessmentLayerThree, 1e-6);
        }
    }
}