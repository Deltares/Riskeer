﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class SerializableAssessmentProcessTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var assessmentProcess = new SerializableAssessmentProcess();

            // Assert
            Assert.IsInstanceOf<SerializableFeatureMember>(assessmentProcess);
            Assert.IsNull(assessmentProcess.Id);
            Assert.IsNull(assessmentProcess.AssessmentSectionId);
            Assert.Zero(assessmentProcess.StartYear);
            Assert.Zero(assessmentProcess.EndYear);

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableAssessmentProcess>(
                nameof(SerializableAssessmentProcess.Id), "BeoordelingsprocesID");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableAssessmentProcess>(
                nameof(SerializableAssessmentProcess.AssessmentSectionId), "WaterkeringstelselIDRef");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableAssessmentProcess>(
                nameof(SerializableAssessmentProcess.StartYear), "beginJaarBeoordelingsronde");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableAssessmentProcess>(
                nameof(SerializableAssessmentProcess.EndYear), "eindJaarBeoordelingsronde");
        }

        [Test]
        public void Constructor_IdNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssessmentProcess(null,
                                                                        new SerializableAssessmentSection(),
                                                                        random.Next(),
                                                                        random.Next());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("id", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssessmentProcess("id",
                                                                        null,
                                                                        random.Next(),
                                                                        random.Next());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "section id";

            var random = new Random(39);
            var assessmentSection = new SerializableAssessmentSection(
                "assessment section id",
                "name",
                random.NextDouble(),
                new[]
                {
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble())
                });
            int startYear = random.Next();
            int endYear = random.Next();

            // Call
            var assessmentProcess = new SerializableAssessmentProcess(id,
                                                                      assessmentSection,
                                                                      startYear,
                                                                      endYear);

            // Assert
            Assert.AreEqual(id, assessmentProcess.Id);
            Assert.AreEqual(assessmentSection.Id, assessmentProcess.AssessmentSectionId);
            Assert.AreEqual(startYear, assessmentProcess.StartYear);
            Assert.AreEqual(endYear, assessmentProcess.EndYear);
        }
    }
}