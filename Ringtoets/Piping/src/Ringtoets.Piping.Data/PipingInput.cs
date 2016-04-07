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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Data.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Class that holds all piping calculation specific input parameters, e.g. the values
    /// that can differ across various calculations.
    /// </summary>
    public class PipingInput : Observable
    {
        private readonly GeneralPipingInput generalInputParameters;
        private readonly NormalDistribution phreaticLevelExit;
        private readonly LognormalDistribution dampingFactorExit;
        private readonly ShiftedLognormalDistribution saturatedVolumicWeightOfCoverageLayer;
        private readonly LognormalDistribution darcyPermeability;
        private readonly LognormalDistribution diameter70;
        private RoundedDouble exitPointL;
        private RoundedDouble entryPointL;
        private RingtoetsPipingSurfaceLine surfaceLine;
        private StochasticSoilModel stochasticSoilModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingInput"/> class.
        /// </summary>
        /// <param name="generalInputParameters">General piping calculation parameters that
        /// are the same across all piping calculations.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="generalInputParameters"/>
        /// is <c>null</c>.</exception>
        public PipingInput(GeneralPipingInput generalInputParameters)
        {
            if (generalInputParameters == null)
            {
                throw new ArgumentNullException("generalInputParameters");
            }

            this.generalInputParameters = generalInputParameters;

            exitPointL = new RoundedDouble(2, double.NaN);
            entryPointL = new RoundedDouble(2, double.NaN);

            phreaticLevelExit = new NormalDistribution(3);
            dampingFactorExit = new LognormalDistribution(3)
            {
                Mean = (RoundedDouble) 0.7,
                StandardDeviation = (RoundedDouble) 0.0
            };
            saturatedVolumicWeightOfCoverageLayer = new ShiftedLognormalDistribution(2)
            {
                Shift = (RoundedDouble) 10,
                Mean = (RoundedDouble) 17.5,
                StandardDeviation = (RoundedDouble) 0
            };
            diameter70 = new LognormalDistribution(2);
            darcyPermeability = new LognormalDistribution(3);
        }

        /// <summary>
        /// Gets or sets the l-coordinate of the entry point, which, together with
        /// the l-coordinate of the exit point, is used to determine the seepage 
        /// length of <see cref="PipingInput"/>.
        /// [m]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.</exception>
        public RoundedDouble EntryPointL
        {
            get
            {
                return entryPointL;
            }
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.PipingInput_EntryPointL_Value_must_be_greater_than_or_equal_to_zero);
                }
                entryPointL = value.ToPrecision(entryPointL.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the l-coordinate of the exit point, which, together with
        /// the l-coordinate of the entry point, is used to determine the seepage 
        /// length of <see cref="PipingInput"/>.
        /// [m]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than or equal to 0.</exception>
        public RoundedDouble ExitPointL
        {
            get
            {
                return exitPointL;
            }
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.PipingInput_ExitPointL_Value_must_be_greater_than_zero);
                }
                exitPointL = value.ToPrecision(exitPointL.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the surface line.
        /// </summary>
        public RingtoetsPipingSurfaceLine SurfaceLine
        {
            get
            {
                return surfaceLine;
            }
            set
            {
                surfaceLine = value;
                UpdateEntryAndExitPoint();
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil model which is linked to the <see cref="StochasticSoilProfile"/>.
        /// </summary>
        public StochasticSoilModel StochasticSoilModel
        {
            get
            {
                return stochasticSoilModel;
            }
            set
            {
                stochasticSoilModel = value;
                UpdateStochasticSoilProfile();
            }
        }

        /// <summary>
        /// Gets or sets the profile which contains a 1 dimensional definition of soil layers with properties.
        /// </summary>
        public StochasticSoilProfile StochasticSoilProfile { get; set; }

        /// <summary>
        /// Gets or set the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        private void UpdateStochasticSoilProfile()
        {
            if (stochasticSoilModel == null || !stochasticSoilModel.StochasticSoilProfiles.Contains(StochasticSoilProfile))
            {
                StochasticSoilProfile = null;
            }
        }

        private void UpdateEntryAndExitPoint()
        {
            if (SurfaceLine == null)
            {
                ExitPointL = (RoundedDouble) double.NaN;
            }
            else
            {
                int entryPointIndex = Array.IndexOf(SurfaceLine.Points, SurfaceLine.DikeToeAtRiver);
                int exitPointIndex = Array.IndexOf(SurfaceLine.Points, SurfaceLine.DikeToeAtPolder);

                Point2D[] localGeometry = SurfaceLine.ProjectGeometryToLZ().ToArray();

                double tempEntryPointL = localGeometry[0].X;
                double tempExitPointL = localGeometry[localGeometry.Length - 1].X;

                bool isDifferentPoints = entryPointIndex < 0 || exitPointIndex < 0 || entryPointIndex < exitPointIndex;
                if (isDifferentPoints && exitPointIndex > 0)
                {
                    tempExitPointL = localGeometry.ElementAt(exitPointIndex).X;
                }
                if (isDifferentPoints && entryPointIndex > -1)
                {
                    tempEntryPointL = localGeometry.ElementAt(entryPointIndex).X;
                }

                ExitPointL = (RoundedDouble) tempExitPointL;
                EntryPointL = (RoundedDouble) tempEntryPointL;
            }
        }

        #region Derived input

        /// <summary>
        /// Gets or sets the outside high water level.
        /// [m]
        /// </summary>
        public RoundedDouble AssessmentLevel
        {
            get
            {
                return new DerivedPipingInput(this).AssessmentLevel;
            }
        }

        /// <summary>
        /// Gets or sets the piezometric head at the exit point.
        /// [m]
        /// </summary>
        public RoundedDouble PiezometricHeadExit
        {
            get
            {
                return new DerivedPipingInput(this).PiezometricHeadExit;
            }
        }

        #endregion

        #region General input parameters

        /// <summary>
        /// Gets the reduction factor Sellmeijer.
        /// </summary>
        public double SellmeijerReductionFactor
        {
            get
            {
                return generalInputParameters.SellmeijerReductionFactor;
            }
        }

        /// <summary>
        /// Gets the volumetric weight of water.
        /// [kN/m³]
        /// </summary>
        public double WaterVolumetricWeight
        {
            get
            {
                return generalInputParameters.WaterVolumetricWeight;
            }
        }

        /// <summary>
        /// Gets the (lowerbound) volumic weight of sand grain material of a sand layer under water.
        /// [kN/m³]
        /// </summary>
        public double SandParticlesVolumicWeight
        {
            get
            {
                return generalInputParameters.SandParticlesVolumicWeight;
            }
        }

        /// <summary>
        /// Gets the White's drag coefficient.
        /// </summary>
        public double WhitesDragCoefficient
        {
            get
            {
                return generalInputParameters.WhitesDragCoefficient;
            }
        }

        /// <summary>
        /// Gets the kinematic viscosity of water at 10 degrees Celsius.
        /// [m²/s]
        /// </summary>
        public double WaterKinematicViscosity
        {
            get
            {
                return generalInputParameters.WaterKinematicViscosity;
            }
        }

        /// <summary>
        /// Gets the gravitational acceleration.
        /// [m/s²]
        /// </summary>
        public double Gravity
        {
            get
            {
                return generalInputParameters.Gravity;
            }
        }

        /// <summary>
        /// Gets the mean diameter of small scale tests applied to different kinds of sand, on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double MeanDiameter70
        {
            get
            {
                return generalInputParameters.MeanDiameter70;
            }
        }

        /// <summary>
        /// Gets the angle of the force balance representing the amount in which sand grains resist rolling.
        /// [°]
        /// </summary>
        public double BeddingAngle
        {
            get
            {
                return generalInputParameters.BeddingAngle;
            }
        }

        /// <summary>
        /// Gets the calculation value used to account for uncertainty in the model for uplift.
        /// </summary>
        public double UpliftModelFactor
        {
            get
            {
                return generalInputParameters.UpliftModelFactor;
            }
        }

        /// <summary>
        /// Gets the calculation value used to account for uncertainty in the model for Sellmeijer.
        /// </summary>
        public double SellmeijerModelFactor
        {
            get
            {
                return generalInputParameters.SellmeijerModelFactor;
            }
        }

        /// <summary>
        /// Gets the critical exit gradient for heave.
        /// </summary>
        public double CriticalHeaveGradient
        {
            get
            {
                return generalInputParameters.CriticalHeaveGradient;
            }
        }

        #endregion

        #region Probabilistic parameters

        /// <summary>
        /// Gets or sets the phreatic level at the exit point.
        /// [m]
        /// </summary>
        public NormalDistribution PhreaticLevelExit
        {
            get
            {
                return phreaticLevelExit;
            }
            set
            {
                phreaticLevelExit.Mean = value.Mean;
                phreaticLevelExit.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal distance between entry and exit point.
        /// [m]
        /// </summary>
        public LognormalDistribution SeepageLength
        {
            get
            {
                return new DerivedPipingInput(this).SeepageLength;
            }
        }

        /// <summary>
        /// Gets or sets the sieve size through which 70% fraction of the grains of the top part of the aquifer passes.
        /// [m]
        /// </summary>
        public LognormalDistribution Diameter70
        {
            get
            {
                return diameter70;
            }
            set
            {
                diameter70.Mean = value.Mean;
                diameter70.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public LognormalDistribution DarcyPermeability
        {
            get
            {
                return darcyPermeability;
            }
            set
            {
                darcyPermeability.Mean = value.Mean;
                darcyPermeability.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the aquifer layer.
        /// [m]
        /// </summary>
        public LognormalDistribution ThicknessAquiferLayer
        {
            get
            {
                return new DerivedPipingInput(this).ThicknessAquiferLayer;
            }
        }

        /// <summary>
        /// Gets or sets the total thickness of the coverage layer at the exit point.
        /// [m]
        /// </summary>
        public LognormalDistribution ThicknessCoverageLayer
        {
            get
            {
                return new DerivedPipingInput(this).ThicknessCoverageLayer;
            }
        }

        /// <summary>
        /// Gets or sets the damping factor at the exit point.
        /// </summary>
        public LognormalDistribution DampingFactorExit
        {
            get
            {
                return dampingFactorExit;
            }
            set
            {
                dampingFactorExit.Mean = value.Mean;
                dampingFactorExit.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the volumic weight of the saturated coverage layer.
        /// </summary>
        public ShiftedLognormalDistribution SaturatedVolumicWeightOfCoverageLayer
        {
            get
            {
                return saturatedVolumicWeightOfCoverageLayer;
            }
            set
            {
                saturatedVolumicWeightOfCoverageLayer.Mean = value.Mean;
                saturatedVolumicWeightOfCoverageLayer.StandardDeviation = value.StandardDeviation;
                saturatedVolumicWeightOfCoverageLayer.Shift = value.Shift;
            }
        }

        #endregion
    }
}