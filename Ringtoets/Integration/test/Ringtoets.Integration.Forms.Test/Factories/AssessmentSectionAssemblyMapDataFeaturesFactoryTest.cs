﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.Forms;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.Forms.Factories;
using Ringtoets.Integration.Util;

namespace Ringtoets.Integration.Forms.Test.Factories
{
    [TestFixture]
    public class AssessmentSectionAssemblyMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateCombinedFailureMechanismSectionAssemblyFeatures_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssessmentSectionAssemblyMapDataFeaturesFactory.CreateCombinedFailureMechanismSectionAssemblyFeatures(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCombinedFailureMechanismSectionAssemblyFeatures_WithAssessmentSection_ReturnsFeatureCollection()
        {
            // Setup
            var random = new Random(21);
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0, 0),
                new Point2D(2, 2)
            });
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                ReferenceLine = referenceLine
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                CombinedFailureMechanismSectionAssembly[] failureMechanismSectionAssembly =
                {
                    CreateCombinedFailureMechanismSectionAssembly(assessmentSection, 20),
                    CreateCombinedFailureMechanismSectionAssembly(assessmentSection, 21)
                };
                calculator.CombinedFailureMechanismSectionAssemblyOutput = failureMechanismSectionAssembly;

                // Call
                IEnumerable<MapFeature> features = AssessmentSectionAssemblyMapDataFeaturesFactory.CreateCombinedFailureMechanismSectionAssemblyFeatures(assessmentSection);

                // Assert
                IEnumerable<CombinedFailureMechanismSectionAssemblyResult> expectedAssemblyResults =
                    AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection, true);
                int expectedNrOfResults = expectedAssemblyResults.Count();
                Assert.AreEqual(expectedNrOfResults, features.Count());

                for (var i = 0; i < expectedNrOfResults; i++)
                {
                    CombinedFailureMechanismSectionAssemblyResult expectedAssemblyResult = expectedAssemblyResults.ElementAt(i);
                    MapFeature actualFeature = features.ElementAt(i);

                    MapGeometry mapGeometry = actualFeature.MapGeometries.Single();
                    AssertEqualPointCollections(assessmentSection.ReferenceLine,
                                                expectedAssemblyResult,
                                                mapGeometry);

                    Assert.AreEqual(2, actualFeature.MetaData.Keys.Count);
                    Assert.AreEqual(expectedAssemblyResult.SectionNumber, actualFeature.MetaData["Vaknummer"]);
                    Assert.AreEqual(new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(
                                        DisplayFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(expectedAssemblyResult.TotalResult)).DisplayName,
                                    features.ElementAt(i).MetaData["Categorie"]);
                }
            }
        }

        private static CombinedFailureMechanismSectionAssembly CreateCombinedFailureMechanismSectionAssembly(AssessmentSection assessmentSection, int seed)
        {
            var random = new Random(seed);
            return new CombinedFailureMechanismSectionAssembly(new CombinedAssemblyFailureMechanismSection(random.NextDouble(),
                                                                                                           random.NextDouble(),
                                                                                                           random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                                                               assessmentSection.GetFailureMechanisms()
                                                                                .Where(fm => fm.IsRelevant)
                                                                                .Select(fm => random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()).ToArray());
        }

        private static void AssertEqualPointCollections(ReferenceLine referenceLine,
                                                        CombinedFailureMechanismSectionAssemblyResult sectionAssemblyResult,
                                                        MapGeometry geometry)
        {
            IEnumerable<Point2D> expectedGeometry = FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(
                referenceLine,
                sectionAssemblyResult.SectionStart,
                sectionAssemblyResult.SectionEnd).ToArray();
            CollectionAssert.IsNotEmpty(expectedGeometry);

            CollectionAssert.AreEqual(expectedGeometry, geometry.PointCollections.Single());
        }
    }
}