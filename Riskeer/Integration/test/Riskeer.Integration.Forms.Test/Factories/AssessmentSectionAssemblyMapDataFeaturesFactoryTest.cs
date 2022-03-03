﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.TestUtil;
using Riskeer.Integration.Forms.Factories;
using Riskeer.Integration.Util;

namespace Riskeer.Integration.Forms.Test.Factories
{
    [TestFixture]
    public class AssessmentSectionAssemblyMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateCombinedFailureMechanismSectionAssemblyFeatures_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionAssemblyMapDataFeaturesFactory.CreateCombinedFailureMechanismSectionAssemblyFeatures(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCombinedFailureMechanismSectionAssemblyFeatures_WithAssessmentSection_ReturnsFeatureCollection()
        {
            // Setup
            var random = new Random(21);

            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(0, 0),
                new Point2D(2, 2)
            });

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                CombinedFailureMechanismSectionAssembly[] failureMechanismSectionAssembly =
                {
                    CombinedFailureMechanismSectionAssemblyTestFactory.Create(assessmentSection, 20),
                    CombinedFailureMechanismSectionAssemblyTestFactory.Create(assessmentSection, 21)
                };
                calculator.CombinedFailureMechanismSectionAssemblyOutput = failureMechanismSectionAssembly;

                // Call
                IEnumerable<MapFeature> features = AssessmentSectionAssemblyMapDataFeaturesFactory.CreateCombinedFailureMechanismSectionAssemblyFeatures(assessmentSection);

                // Assert
                IEnumerable<CombinedFailureMechanismSectionAssemblyResult> expectedAssemblyResults =
                    AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection);
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
                    Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(expectedAssemblyResult.TotalResult),
                                    features.ElementAt(i).MetaData["Duidingsklasse"]);
                }
            }
        }

        [Test]
        public void CreateCombinedFailureMechanismSectionAssemblyFeatures_AssemblyCalculatorThrowsException_ReturnsEmptyFeatureCollection()
        {
            // Setup
            var random = new Random(21);

            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(0, 0),
                new Point2D(2, 2)
            });

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                IEnumerable<MapFeature> features = AssessmentSectionAssemblyMapDataFeaturesFactory.CreateCombinedFailureMechanismSectionAssemblyFeatures(assessmentSection);

                // Assert
                CollectionAssert.IsEmpty(features);
            }
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