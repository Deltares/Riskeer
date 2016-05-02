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

using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.GrassCoverErosionInwards.Data.Properties;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// This class holds the grass cover erosion inwards information which can be made visible in the graphical interface of the application.
    /// </summary>
    public class GrassCoverErosionInwardsCalculation : Observable, ICalculation
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        public GrassCoverErosionInwardsCalculation()
        {
            Name = Resources.GrassCoverErosionInwardsCalculation_DefaultName;
            InputParameters = new GrassCoverErosionInwardsInput();
            AddDemoInput();
        }

        /// <summary>
        /// Gets the input parameters to perform a grass cover erosion inwards calculation with.
        /// </summary>
        public GrassCoverErosionInwardsInput InputParameters { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="GrassCoverErosionInwardsOutput"/>, which contains the results of a grass cover erosion inwards calculation.
        /// </summary>
        public GrassCoverErosionInwardsOutput Output { get; set; }

        public string Comments { get; set; }

        public string Name { get; set; }

        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        public void ClearOutput()
        {
            Output = null;
        }

        public void ClearHydraulicBoundaryLocation()
        {
            InputParameters.HydraulicBoundaryLocation = null;
        }

        public ICalculationInput GetObservableInput()
        {
            return InputParameters;
        }

        private void AddDemoInput()
        {
            // BreakWater
            InputParameters.BreakWater = new BreakWater(BreakWaterType.Dam, 10);
            InputParameters.UseBreakWater = true;

            // Orientation
            InputParameters.Orientation = new RoundedDouble(RoundedDouble.MaximumNumberOfDecimalPlaces, 5.5);

            // CriticalFlowRate
            InputParameters.CriticalFlowRate = new LognormalDistribution(3);

            // Dike and Foreshore
            var dikeSections = new[]
            {
                new RoughnessProfileSection(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4), 1.1)
            };
            var foreshoreSections = new[]
            {
                new RoughnessProfileSection(new Point2D(3.3, 4.4), new Point2D(5.5, 6.6), 2.2)
            };
            InputParameters.SetDikeGeometry(dikeSections);
            InputParameters.SetForeshoreGeometry(foreshoreSections);
            InputParameters.UseForeshore = true;

            // Hydraulic boundaries location
            InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, "Demo", 0.0, 1.1);

            // Dike height
            InputParameters.DikeHeight = new RoundedDouble(InputParameters.DikeHeight.NumberOfDecimalPlaces, 10);
        }
    }
}