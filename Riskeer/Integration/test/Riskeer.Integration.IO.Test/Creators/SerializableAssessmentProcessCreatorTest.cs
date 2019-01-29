// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Integration.IO.Creators;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableAssessmentProcessCreatorTest
    {
        [Test]
        public void Create_IdGeneratorNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableAssessmentProcessCreator.Create(null,
                                                                                  new SerializableAssessmentSection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("idGenerator", exception.ParamName);
        }

        [Test]
        public void Create_WithValidArguments_ReturnsSerializableAssessmentProcess()
        {
            // Setup
            const string assessmentSectionId = "assessmentSectionId";
            var serializableAssessmentSection = new SerializableAssessmentSection(assessmentSectionId,
                                                                                  string.Empty,
                                                                                  CreateGeometry());

            var idGenerator = new IdentifierGenerator();

            // Call
            SerializableAssessmentProcess serializableProcess =
                SerializableAssessmentProcessCreator.Create(idGenerator, serializableAssessmentSection);

            // Assert
            Assert.AreEqual("Bp.0", serializableProcess.Id);
            Assert.AreEqual(serializableAssessmentSection.Id, serializableProcess.AssessmentSectionId);
        }

        private static IEnumerable<Point2D> CreateGeometry()
        {
            return new[]
            {
                new Point2D(1, 1),
                new Point2D(4, 4),
                new Point2D(5, -1)
            };
        }
    }
}