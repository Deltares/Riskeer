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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingCalculationGroupExtensionsTest
    {
        [Test]
        public void AddCalculationScenariosToFailureMechanismSectionResult_CalculationGroupNoChildren_NoScenariosAddedToFailureMechanismSectionResult()
        {
            // Setup
            var calculationGroup = new CalculationGroup();
            var failureMechanism = GetFailureMechanismWithSections();

            // Precondition
            foreach (var failureMechanismSectionResult in failureMechanism.SectionResults)
            {
                CollectionAssert.IsEmpty(failureMechanismSectionResult.CalculationScenarios);
            }

            // Call
            calculationGroup.AddCalculationScenariosToFailureMechanismSectionResult(failureMechanism);

            // Assert
            foreach (var failureMechanismSectionResult in failureMechanism.SectionResults)
            {
                CollectionAssert.IsEmpty(failureMechanismSectionResult.CalculationScenarios);
            }
        }

        [Test]
        public void AddCalculationScenariosToFailureMechanismSectionResult_CalculationGroupWithChildren_ScenariosAddedToFailureMechanismSectionResult()
        {
            // Setup
            var failureMechanism = GetFailureMechanismWithSections();

            var calculationsStructure = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                failureMechanism.SurfaceLines,
                failureMechanism.StochasticSoilModels,
                failureMechanism.GeneralInput,
                failureMechanism.NormProbabilityInput);

            foreach (var item in calculationsStructure)
            {
                failureMechanism.CalculationsGroup.Children.Add(item);
            }

            // Precondition
            foreach (var failureMechanismSectionResult in failureMechanism.SectionResults)
            {
                CollectionAssert.IsEmpty(failureMechanismSectionResult.CalculationScenarios);
            }

            // Call
            failureMechanism.CalculationsGroup.AddCalculationScenariosToFailureMechanismSectionResult(failureMechanism);

            // Assert
            var failureMechanismSectionResult1 = failureMechanism.SectionResults.First();
            var failureMechanismSectionResult2 = failureMechanism.SectionResults.ElementAt(1);

            Assert.AreEqual(4, failureMechanismSectionResult1.CalculationScenarios.Count);
            Assert.AreEqual(2, failureMechanismSectionResult2.CalculationScenarios.Count);
        }

        [Test]
        public void AddCalculationScenariosToFailureMechanismSectionResult_CalculationGroupChildrenNoScenarios_NoScenariosAddedToFailureMechanismSectionResult()
        {
            // Setup
            var failureMechanism = GetFailureMechanismWithSections();
            var stochasticSoilModel = failureMechanism.StochasticSoilModels.First();
            var calculation = new PipingCalculation(new GeneralPipingInput(), new NormProbabilityPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = failureMechanism.SurfaceLines.First(),
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilModel.StochasticSoilProfiles.First()
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Precondition
            foreach (var failureMechanismSectionResult in failureMechanism.SectionResults)
            {
                CollectionAssert.IsEmpty(failureMechanismSectionResult.CalculationScenarios);
            }

            // Call
            failureMechanism.CalculationsGroup.AddCalculationScenariosToFailureMechanismSectionResult(failureMechanism);

            // Assert
            foreach (var failureMechanismSectionResult in failureMechanism.SectionResults)
            {
                CollectionAssert.IsEmpty(failureMechanismSectionResult.CalculationScenarios);
            }
        }

        [Test]
        public void AddCalculationScenariosToFailureMechanismSectionResult_FailureMechanismWithoutSections_NoScenariosAddedToFailureMechanismSectionResult()
        {
            // Setup
            var failureMechanism = GetFailureMechanismWithoutSections();

            var calculationsStructure = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                failureMechanism.SurfaceLines,
                failureMechanism.StochasticSoilModels,
                failureMechanism.GeneralInput,
                failureMechanism.NormProbabilityInput);

            foreach (var item in calculationsStructure)
            {
                failureMechanism.CalculationsGroup.Children.Add(item);
            }

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);

            // Call
            failureMechanism.CalculationsGroup.AddCalculationScenariosToFailureMechanismSectionResult(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
        }

        [Test]
        public void AddCalculationScenariosToFailureMechanismSectionResult_CalculationAlreadyInFailureMechanismSectionResult_ScenarioNotAddedToFailureMechanismSectionResult()
        {
            // Setup
            var failureMechanism = GetFailureMechanismWithSections();

            var calculationsStructure = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                failureMechanism.SurfaceLines,
                failureMechanism.StochasticSoilModels,
                failureMechanism.GeneralInput,
                failureMechanism.NormProbabilityInput);

            foreach (var item in calculationsStructure)
            {
                failureMechanism.CalculationsGroup.Children.Add(item);
            }

            failureMechanism.CalculationsGroup.AddCalculationScenariosToFailureMechanismSectionResult(failureMechanism);

            // Precondition
            var failureMechanismSectionResult1 = failureMechanism.SectionResults.First();
            var failureMechanismSectionResult2 = failureMechanism.SectionResults.ElementAt(1);

            Assert.AreEqual(4, failureMechanismSectionResult1.CalculationScenarios.Count);
            Assert.AreEqual(2, failureMechanismSectionResult2.CalculationScenarios.Count);

            // Call
            failureMechanism.CalculationsGroup.AddCalculationScenariosToFailureMechanismSectionResult(failureMechanism);

            // Assert
            var failureMechanismSectionResult3 = failureMechanism.SectionResults.First();
            var failureMechanismSectionResult4 = failureMechanism.SectionResults.ElementAt(1);

            Assert.AreEqual(4, failureMechanismSectionResult3.CalculationScenarios.Count);
            Assert.AreEqual(2, failureMechanismSectionResult4.CalculationScenarios.Count);
        }

        private static PipingFailureMechanism GetFailureMechanismWithoutSections()
        {
            return GetFailureMechanism();
        }

        private static PipingFailureMechanism GetFailureMechanismWithSections()
        {
            var failureMechanism = GetFailureMechanism();

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

            return failureMechanism;
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
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
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

            return failureMechanism;
        }
    }
}