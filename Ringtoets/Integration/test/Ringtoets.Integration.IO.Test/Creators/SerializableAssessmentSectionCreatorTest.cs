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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.Helpers;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableAssessmentSectionCreatorTest
    {
        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SerializableAssessmentSectionCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Create_WithAssessmentSection_ReturnsSerializableAssessmentSection()
        {
            // Setup
            const string assessmentSectionName = "Assessment Section Name";
            const string assessmentSectionId = "Assessment Section Id";

            ExportableAssessmentSection assessmentSection = CreateAssessmentSection(assessmentSectionName,
                                                                                    assessmentSectionId);

            // Call
            SerializableAssessmentSection serializableAssessmentSection =
                SerializableAssessmentSectionCreator.Create(assessmentSection);

            // Assert
            Assert.AreEqual($"Wks.{assessmentSection.Id}", serializableAssessmentSection.Id);
            Assert.AreEqual(assessmentSectionName, serializableAssessmentSection.Name);

            IEnumerable<Point2D> expectedGeometry = assessmentSection.Geometry;
            Assert.AreEqual(Math2D.Length(expectedGeometry), serializableAssessmentSection.ReferenceLineLength.Value);
            Assert.AreEqual(GeometrySerializationFormatter.Format(expectedGeometry),
                            serializableAssessmentSection.ReferenceLineGeometry.LineString.Geometry);
        }

        private static ExportableAssessmentSection CreateAssessmentSection(string name, string id)
        {
            return new ExportableAssessmentSection(name,
                                                   id,
                                                   CreateGeometry(),
                                                   ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                   ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                   ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                   Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>(),
                                                   Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                   Enumerable.Empty<ExportableCombinedSectionAssembly>());
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