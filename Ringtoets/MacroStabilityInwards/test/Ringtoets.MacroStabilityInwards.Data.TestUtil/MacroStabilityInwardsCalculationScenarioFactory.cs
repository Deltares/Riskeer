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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating different instances of <see cref="MacroStabilityInwardsCalculationScenario"/>
    /// for easier testing.
    /// </summary>
    public static class MacroStabilityInwardsCalculationScenarioFactory
    {
        /// <summary>
        /// Creates a calculated scenario for which the surface line on the input intersects with <paramref name="section"/>.
        /// </summary>
        /// <param name="probability">The value for <see cref="MacroStabilityInwardsSemiProbabilisticOutput.MacroStabilityInwardsProbability"/>.</param>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateMacroStabilityInwardsCalculationScenario(double probability,
                                                                                                              FailureMechanismSection section)
        {
            MacroStabilityInwardsCalculationScenario scenario = CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            var random = new Random(21);
            scenario.SemiProbabilisticOutput = new MacroStabilityInwardsSemiProbabilisticOutput(
                random.NextDouble(),
                random.NextDouble(),
                (RoundedDouble) probability,
                random.NextDouble(),
                random.NextDouble());

            scenario.Output = new TestMacroStabilityInwardsOutput();

            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// the calculation has failed.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateFailedMacroStabilityInwardsCalculationScenario(FailureMechanismSection section)
        {
            return CreateMacroStabilityInwardsCalculationScenario(RoundedDouble.NaN, section);
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// is marked as not relevant for the assessment.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateIrrelevantMacroStabilityInwardsCalculationScenario(FailureMechanismSection section)
        {
            MacroStabilityInwardsCalculationScenario scenario = CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            scenario.IsRelevant = false;
            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// the calculation has not been performed.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateNotCalculatedMacroStabilityInwardsCalculationScenario(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            Point2D p = section.Points.First();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(p.X, p.Y, 0),
                new Point3D(p.X + 2, p.Y + 2, 0)
            });
            surfaceLine.ReferenceLineIntersectionWorldPoint = section.Points.First();

            var scenario = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                IsRelevant = true,
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };
            return scenario;
        }

        /// <summary>
        /// Creates a scenario with invalid input.
        /// </summary>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput()
        {
            return new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
        }

        /// <summary>
        /// Creates a scenario with valid input.
        /// </summary>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateMacroStabilityInwardsCalculationScenarioWithValidInput()
        {
            const double bottom = 1.12;
            const double top = 10.56;
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, new MacroStabilityInwardsSoilProfile1D(string.Empty, 0.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(top)
                {
                    Properties =
                    {
                        IsAquifer = false
                    }
                },
                new MacroStabilityInwardsSoilLayer1D(top / 2)
                {
                    Properties =
                    {
                        IsAquifer = true
                    }
                }
            }));

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var firstCharacteristicPointLocation = new Point3D(0.2, 0.0, bottom + 3 * top / 4);
            var secondCharacteristicPointLocation = new Point3D(0.3, 0.0, bottom + 2 * top / 4);
            var thirdCharacteristicPointLocation = new Point3D(0.4, 0.0, bottom + top / 4);
            var fourthCharacteristicPointLocation = new Point3D(0.5, 0.0, bottom + 2 * top / 4);
            var fifthCharacteristicPointLocation = new Point3D(0.6, 0.0, bottom + 3 * top / 4);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 0.0),
                firstCharacteristicPointLocation,
                secondCharacteristicPointLocation,
                thirdCharacteristicPointLocation,
                fourthCharacteristicPointLocation,
                fifthCharacteristicPointLocation,
                new Point3D(1.0, 0.0, top)
            });

            HydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.0);
            return new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    StochasticSoilProfile = stochasticSoilProfile,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
        }
    }
}