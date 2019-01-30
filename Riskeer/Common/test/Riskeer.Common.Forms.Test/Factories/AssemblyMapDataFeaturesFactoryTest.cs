// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Util;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Forms;

namespace Riskeer.Common.Forms.Test.Factories
{
    [TestFixture]
    public class AssemblyMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateAssemblyFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyMapDataFeaturesFactory.CreateAssemblyFeatures<
                IHasSectionResults<FailureMechanismSectionResult>, FailureMechanismSectionResult>(null, sr => null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void CreateAssemblyFeatures_GetAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyMapDataFeaturesFactory.CreateAssemblyFeatures<
                TestFailureMechanism, FailureMechanismSectionResult>(new TestFailureMechanism(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("getAssemblyFunc", paramName);
        }

        [Test]
        public void CreateAssemblyFeatures_GetAssemblyFuncThrowsAssemblyExceptionOnFirstSection_FirstSectionSkipped()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, 2);
            var expectedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            // Call
            var shouldThrowException = true;
            IEnumerable<MapFeature> features = AssemblyMapDataFeaturesFactory.CreateAssemblyFeatures<
                TestFailureMechanism, FailureMechanismSectionResult>(failureMechanism, sr =>
            {
                if (shouldThrowException)
                {
                    shouldThrowException = false;
                    throw new AssemblyException();
                }

                return expectedAssembly;
            });

            // Assert
            AssertMapFeature(failureMechanism.Sections.ElementAt(1), features.Single(), expectedAssembly);
        }

        [Test]
        public void CreateAssemblyFeatures_ValidParameters_ReturnsExpectedFeatures()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(0, 10));

            var expectedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            // Call
            IEnumerable<MapFeature> features = AssemblyMapDataFeaturesFactory.CreateAssemblyFeatures<
                TestFailureMechanism, FailureMechanismSectionResult>(failureMechanism, sr => expectedAssembly);

            // Assert
            Assert.AreEqual(failureMechanism.Sections.Count(), features.Count());

            for (var i = 0; i < features.Count(); i++)
            {
                FailureMechanismSection section = failureMechanism.Sections.ElementAt(i);
                MapFeature mapFeature = features.ElementAt(i);
                AssertMapFeature(section, mapFeature, expectedAssembly);
            }
        }

        [Test]
        public void CreateAssemblyCategoryGroupFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyMapDataFeaturesFactory.CreateAssemblyCategoryGroupFeatures<
                IHasSectionResults<FailureMechanismSectionResult>, FailureMechanismSectionResult>(
                null,
                sr => new Random(39).NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void CreateAssemblyCategoryGroupFeatures_GetAssemblyCategoryGroupFuncNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyMapDataFeaturesFactory.CreateAssemblyCategoryGroupFeatures<
                TestFailureMechanism, FailureMechanismSectionResult>(new TestFailureMechanism(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("getAssemblyCategoryGroupFunc", paramName);
        }

        [Test]
        public void CreateAssemblyCategoryGroupFeatures_GetAssemblyCategoryGroupFuncThrowsAssemblyExceptionOnFirstSection_FirstSectionSkipped()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, 2);
            var expectedCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            // Call
            var shouldThrowException = true;
            IEnumerable<MapFeature> features = AssemblyMapDataFeaturesFactory.CreateAssemblyCategoryGroupFeatures<
                TestFailureMechanism, FailureMechanismSectionResult>(failureMechanism, sr =>
            {
                if (shouldThrowException)
                {
                    shouldThrowException = false;
                    throw new AssemblyException();
                }

                return expectedCategoryGroup;
            });

            // Assert
            AssertAssemblyCategoryGroupMapFeature(failureMechanism.Sections.ElementAt(1), features.Single(), expectedCategoryGroup);
        }

        [Test]
        public void CreateAssemblyCategoryGroupFeatures_ValidParameters_ReturnsExpectedFeatures()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(0, 10));

            var expectedCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            // Call
            IEnumerable<MapFeature> features = AssemblyMapDataFeaturesFactory.CreateAssemblyCategoryGroupFeatures<
                TestFailureMechanism, FailureMechanismSectionResult>(failureMechanism, sr => expectedCategoryGroup);

            // Assert
            Assert.AreEqual(failureMechanism.Sections.Count(), features.Count());

            for (var i = 0; i < features.Count(); i++)
            {
                FailureMechanismSection section = failureMechanism.Sections.ElementAt(i);
                MapFeature mapFeature = features.ElementAt(i);
                AssertAssemblyCategoryGroupMapFeature(section, mapFeature, expectedCategoryGroup);
            }
        }

        private static void AssertMapFeature(FailureMechanismSection section, MapFeature mapFeature, FailureMechanismSectionAssembly expectedAssembly)
        {
            IEnumerable<MapGeometry> mapGeometries = mapFeature.MapGeometries;

            Assert.AreEqual(1, mapGeometries.Count());
            CollectionAssert.AreEqual(section.Points, mapGeometries.Single().PointCollections.First());
            Assert.AreEqual(2, mapFeature.MetaData.Keys.Count);
            Assert.AreEqual(new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(
                                DisplayFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(expectedAssembly.Group)).DisplayName,
                            mapFeature.MetaData["Categorie"]);
            Assert.AreEqual(new NoProbabilityValueDoubleConverter().ConvertToString(expectedAssembly.Probability),
                            mapFeature.MetaData["Faalkans"]);
        }

        private static void AssertAssemblyCategoryGroupMapFeature(FailureMechanismSection section, MapFeature mapFeature, FailureMechanismSectionAssemblyCategoryGroup expectedAssembly)
        {
            IEnumerable<MapGeometry> mapGeometries = mapFeature.MapGeometries;

            Assert.AreEqual(1, mapGeometries.Count());
            CollectionAssert.AreEqual(section.Points, mapGeometries.Single().PointCollections.First());
            Assert.AreEqual(1, mapFeature.MetaData.Keys.Count);
            Assert.AreEqual(new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(
                                DisplayFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(expectedAssembly)).DisplayName,
                            mapFeature.MetaData["Categorie"]);
        }
    }
}