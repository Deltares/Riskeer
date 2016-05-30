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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
    public class PipingCalculationScenarioServiceTest
    {
        [Test]
        public void SyncCalculationScenarioWithNewSurfaceLine_CalculationScenarioNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = GetFailureMechanism();

            // Call
            TestDelegate test = () => PipingCalculationScenarioService.SyncCalculationScenarioWithNewSurfaceLine(null, failureMechanism, new RingtoetsPipingSurfaceLine());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationScenario", exception.ParamName);
        }

        [Test]
        public void SyncCalculationScenarioWithNewSurfaceLine_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var pipingCalculationScenario = new PipingCalculationScenario(new GeneralPipingInput(), new PipingProbabilityAssessmentInput());
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate call = () => PipingCalculationScenarioService.SyncCalculationScenarioWithNewSurfaceLine(pipingCalculationScenario, null, surfaceLine);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void SyncCalculationScenarioWithNewSurfaceLine_OldSurfaceLineNull_SectionResultsUpdated()
        {
            // Setup
            var failureMechanism = GetFailureMechanism();
            var newSurfaceLine = failureMechanism.SurfaceLines.ElementAt(1);

            var calculationGroup = failureMechanism.CalculationsGroup.Children[0] as CalculationGroup;
            var calculationToSync = calculationGroup.Children[0] as PipingCalculationScenario;

            // Precondition
            Assert.IsNotNull(calculationToSync);

            calculationToSync.InputParameters.SurfaceLine = newSurfaceLine;

            var sectionResults = failureMechanism.SectionResults.ToArray();

            var sectionResultScenariosBeforeSync = sectionResults[0].CalculationScenarios.ToArray();
            var sectionResultScenariosBeforeSync2 = sectionResults[1].CalculationScenarios.ToArray();

            // Call
            PipingCalculationScenarioService.SyncCalculationScenarioWithNewSurfaceLine(calculationToSync, failureMechanism, null);

            // Assert
            CollectionAssert.AreNotEqual(sectionResultScenariosBeforeSync, sectionResults[0].CalculationScenarios);
            CollectionAssert.AreNotEqual(sectionResultScenariosBeforeSync2, sectionResults[1].CalculationScenarios);
        }

        [Test]
        public void SyncCalculationScenarioWithNewSurfaceLine_NewSurfaceLineSameAsOld_SectionResultsNotUpdated()
        {
            // Setup
            var failureMechanism = GetFailureMechanism();
            var calculationGroup = failureMechanism.CalculationsGroup.Children[0] as CalculationGroup;
            var calculationToSync = calculationGroup.Children[0] as PipingCalculationScenario;

            // Precondition
            Assert.IsNotNull(calculationToSync);

            var sectionResults = failureMechanism.SectionResults.ToArray();

            var expectedSectionResultScenarios = sectionResults[0].CalculationScenarios.ToArray();

            // Call
            PipingCalculationScenarioService.SyncCalculationScenarioWithNewSurfaceLine(calculationToSync, failureMechanism, calculationToSync.InputParameters.SurfaceLine);

            // Assert
            CollectionAssert.AreEqual(expectedSectionResultScenarios, sectionResults[0].CalculationScenarios);
        }

        [Test]
        public void SyncCalculationScenarioWithNewSurfaceLine_NewSurfaceLine_SectionResultsUpdated()
        {
            // Setup
            var failureMechanism = GetFailureMechanism();
            var surfaceLines = failureMechanism.SurfaceLines.ToArray();
            var newSurfaceLine = surfaceLines[1];

            var calculationGroup = failureMechanism.CalculationsGroup.Children[0] as CalculationGroup;
            var calculationToSync = calculationGroup.Children[0] as PipingCalculationScenario;

            // Precondition
            Assert.IsNotNull(calculationToSync);

            var oldSurfaceLine = calculationToSync.InputParameters.SurfaceLine;
            calculationToSync.InputParameters.SurfaceLine = newSurfaceLine;

            var sectionResults = failureMechanism.SectionResults.ToArray();

            var sectionResultScenariosBeforeSync = sectionResults[0].CalculationScenarios.ToArray();
            var sectionResultScenariosBeforeSync2 = sectionResults[1].CalculationScenarios.ToArray();

            // Call
            PipingCalculationScenarioService.SyncCalculationScenarioWithNewSurfaceLine(calculationToSync, failureMechanism, oldSurfaceLine);

            // Assert
            CollectionAssert.AreNotEqual(sectionResultScenariosBeforeSync, sectionResults[0].CalculationScenarios);
            CollectionAssert.AreNotEqual(sectionResultScenariosBeforeSync2, sectionResults[1].CalculationScenarios);
        }

        [Test]
        public void RemoveCalculationScenarioFromSectionResult_CalculationScenarioNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = GetFailureMechanism();

            // Call
            TestDelegate call = () => PipingCalculationScenarioService.RemoveCalculationScenarioFromSectionResult(null, failureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationScenario", exception.ParamName);
        }

        [Test]
        public void RemoveCalculationScenarioFromSectionResult_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput(), new PipingProbabilityAssessmentInput());

            // Call
            TestDelegate call = () => PipingCalculationScenarioService.RemoveCalculationScenarioFromSectionResult(calculation, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveCalculationScenarioFromSectionResult_ScenarioNotInSectionResult_ScenarioNotRemovedFromSectionResult()
        {
            // Setup
            var failureMechanism = GetFailureMechanism();

            var calculationToRemove = new PipingCalculationScenario(new GeneralPipingInput(), new PipingProbabilityAssessmentInput());

            var sectionResults = failureMechanism.SectionResults.ToArray();
            var sectionResultScenariosBeforeRemove = sectionResults[0].CalculationScenarios.ToArray();
            var sectionResultScenariosBeforeRemove2 = sectionResults[1].CalculationScenarios.ToArray();

            // Call
            PipingCalculationScenarioService.RemoveCalculationScenarioFromSectionResult(calculationToRemove, failureMechanism);

            // Assert
            CollectionAssert.AreEqual(sectionResultScenariosBeforeRemove, sectionResults[0].CalculationScenarios);
            CollectionAssert.AreEqual(sectionResultScenariosBeforeRemove2, sectionResults[1].CalculationScenarios);
        }

        [Test]
        public void RemoveCalculationScenarioFromSectionResult_ScenarioInSectionResult_RemovesScenarioFromSectionResult()
        {
            // Setup
            var failureMechanism = GetFailureMechanism();

            var calculationGroup = failureMechanism.CalculationsGroup.Children[0] as CalculationGroup;
            var calculationToRemove = calculationGroup.Children[0] as PipingCalculationScenario;

            var sectionResults = failureMechanism.SectionResults.ToArray();

            // Precondition
            Assert.IsNotNull(calculationToRemove);
            CollectionAssert.Contains(sectionResults[0].CalculationScenarios, calculationToRemove);

            // Call
            PipingCalculationScenarioService.RemoveCalculationScenarioFromSectionResult(calculationToRemove, failureMechanism);

            // Assert
            CollectionAssert.DoesNotContain(sectionResults[0].CalculationScenarios, calculationToRemove);
        }

        private static PipingFailureMechanism GetFailureMechanism()
        {
            var surfaceLine1 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 1",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 2",
                ReferenceLineIntersectionWorldPoint = new Point2D(10.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(10.0, 10.0, 0.0),
                new Point3D(10.0, 0.0, 5.0),
                new Point3D(10.0, -10.0, 0.0)
            });

            var failureMechanism = new PipingFailureMechanism
            {
                SurfaceLines =
                {
                    surfaceLine1,
                    surfaceLine2
                },
                StochasticSoilModels =
                {
                    new TestStochasticSoilModel
                    {
                        Geometry =
                        {
                            new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                        },
                    }
                }
            };

            failureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            failureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            var calculationsStructure = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                failureMechanism.SurfaceLines,
                failureMechanism.StochasticSoilModels,
                failureMechanism.GeneralInput,
                failureMechanism.PipingProbabilityAssessmentInput);

            foreach (var item in calculationsStructure)
            {
                failureMechanism.CalculationsGroup.Children.Add(item);
            }

            failureMechanism.CalculationsGroup.AddCalculationScenariosToFailureMechanismSectionResult(failureMechanism);

            return failureMechanism;
        }
    }
}