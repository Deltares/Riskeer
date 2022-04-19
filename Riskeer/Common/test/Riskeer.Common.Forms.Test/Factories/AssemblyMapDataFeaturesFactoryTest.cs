// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Factories
{
    [TestFixture]
    public class AssemblyMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateAssemblyGroupFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssemblyMapDataFeaturesFactory.CreateAssemblyGroupFeatures<FailureMechanismSectionResult>(
                null, sr => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateAssemblyGroupFeatures_PerformAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssemblyMapDataFeaturesFactory.CreateAssemblyGroupFeatures<FailureMechanismSectionResult>(
                new TestFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performAssemblyFunc", exception.ParamName);
        }

        [Test]
        public void CreateAssemblyGroupFeatures_PerformAssemblyFuncThrowsAssemblyExceptionOnFirstSection_FirstSectionSkipped()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, 2);

            var expectedAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

            // Call
            var shouldThrowException = true;
            IEnumerable<MapFeature> features = AssemblyMapDataFeaturesFactory.CreateAssemblyGroupFeatures<FailureMechanismSectionResult>(
                failureMechanism, sr =>
                {
                    if (shouldThrowException)
                    {
                        shouldThrowException = false;
                        throw new AssemblyException();
                    }

                    return expectedAssemblyResult;
                });

            // Assert
            AssertAssemblyGroupMapFeature(failureMechanism.Sections.ElementAt(1), features.Single(), expectedAssemblyResult);
        }

        [Test]
        public void CreateAssemblyGroupFeatures_ValidParameters_ReturnsExpectedFeatures()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(0, 10));

            var expectedAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

            // Call
            IEnumerable<MapFeature> features = AssemblyMapDataFeaturesFactory.CreateAssemblyGroupFeatures<FailureMechanismSectionResult>(
                failureMechanism, sr => expectedAssemblyResult);

            // Assert
            Assert.AreEqual(failureMechanism.Sections.Count(), features.Count());

            for (var i = 0; i < features.Count(); i++)
            {
                FailureMechanismSection section = failureMechanism.Sections.ElementAt(i);
                MapFeature mapFeature = features.ElementAt(i);
                AssertAssemblyGroupMapFeature(section, mapFeature, expectedAssemblyResult);
            }
        }

        private static void AssertAssemblyGroupMapFeature(FailureMechanismSection section, MapFeature mapFeature, FailureMechanismSectionAssemblyResult expectedAssemblyResult)
        {
            IEnumerable<MapGeometry> mapGeometries = mapFeature.MapGeometries;

            Assert.AreEqual(1, mapGeometries.Count());
            CollectionAssert.AreEqual(section.Points, mapGeometries.Single().PointCollections.First());
            Assert.AreEqual(2, mapFeature.MetaData.Keys.Count);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(expectedAssemblyResult.FailureMechanismSectionAssemblyGroup),
                            mapFeature.MetaData["Duidingsklasse"]);
            Assert.AreEqual(expectedAssemblyResult.SectionProbability, mapFeature.MetaData["Rekenwaarde faalkans per vak"]);
        }
    }
}