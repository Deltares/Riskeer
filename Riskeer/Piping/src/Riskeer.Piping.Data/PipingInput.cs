// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Globalization;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Piping.Data.Properties;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Class that holds all piping calculation specific input parameters, i.e. the values
    /// that can differ across various calculations.
    /// </summary>
    public class PipingInput : CloneableObservable, ICalculationInputWithHydraulicBoundaryLocation
    {
        private readonly GeneralPipingInput generalInputParameters;
        private NormalDistribution phreaticLevelExit;
        private LogNormalDistribution dampingFactorExit;
        private RoundedDouble exitPointL;
        private RoundedDouble entryPointL;
        private PipingSurfaceLine surfaceLine;
        private RoundedDouble assessmentLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingInput"/> class.
        /// </summary>
        /// <param name="generalInputParameters">General piping calculation parameters that
        /// are the same across all piping calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalInputParameters"/>
        /// is <c>null</c>.</exception>
        public PipingInput(GeneralPipingInput generalInputParameters)
        {
            if (generalInputParameters == null)
            {
                throw new ArgumentNullException(nameof(generalInputParameters));
            }

            this.generalInputParameters = generalInputParameters;

            exitPointL = new RoundedDouble(2, double.NaN);
            entryPointL = new RoundedDouble(2, double.NaN);

            phreaticLevelExit = new NormalDistribution(3)
            {
                StandardDeviation = (RoundedDouble) 0.1
            };
            dampingFactorExit = new LogNormalDistribution(3)
            {
                Mean = (RoundedDouble) 0.7,
                StandardDeviation = (RoundedDouble) 0.1
            };

            assessmentLevel = new RoundedDouble(2, double.NaN);
        }

        /// <summary>
        /// Gets or sets the l-coordinate of the entry point, which, together with
        /// the l-coordinate of the exit point, is used to determine the seepage 
        /// length of <see cref="PipingInput"/>.
        /// [m]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="value"/> is smaller or equal to <see cref="ExitPointL"/>.</item>
        /// <item><paramref name="value"/> does not fall within the local X-coordinate range of <see cref="SurfaceLine"/></item>
        /// </list>
        /// </exception>
        public RoundedDouble EntryPointL
        {
            get
            {
                return entryPointL;
            }
            set
            {
                RoundedDouble newEntryPointL = value.ToPrecision(entryPointL.NumberOfDecimalPlaces);

                if (!double.IsNaN(newEntryPointL))
                {
                    if (!double.IsNaN(exitPointL))
                    {
                        ValidateEntryExitPoint(newEntryPointL, exitPointL);
                    }

                    if (surfaceLine != null)
                    {
                        ValidatePointOnSurfaceLine(newEntryPointL);
                    }
                }

                entryPointL = newEntryPointL;
            }
        }

        /// <summary>
        /// Gets or sets the l-coordinate of the exit point, which, together with
        /// the l-coordinate of the entry point, is used to determine the seepage 
        /// length of <see cref="PipingInput"/>.
        /// [m]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="value"/> is smaller or equal to <see cref="EntryPointL"/>.</item>
        /// <item><paramref name="value"/> does not fall within the local X-coordinate range of <see cref="SurfaceLine"/></item>
        /// </list>
        /// </exception>
        public RoundedDouble ExitPointL
        {
            get
            {
                return exitPointL;
            }
            set
            {
                RoundedDouble newExitPointL = value.ToPrecision(exitPointL.NumberOfDecimalPlaces);

                if (!double.IsNaN(newExitPointL))
                {
                    if (!double.IsNaN(entryPointL))
                    {
                        ValidateEntryExitPoint(entryPointL, newExitPointL);
                    }

                    if (surfaceLine != null)
                    {
                        ValidatePointOnSurfaceLine(newExitPointL);
                    }
                }

                exitPointL = newExitPointL;
            }
        }

        /// <summary>
        /// Gets or sets the surface line.
        /// </summary>
        public PipingSurfaceLine SurfaceLine
        {
            get
            {
                return surfaceLine;
            }
            set
            {
                surfaceLine = value;
                SynchronizeEntryAndExitPointInput();
            }
        }

        /// <summary>
        /// Gets or sets the stochastic soil model which is linked to the <see cref="PipingStochasticSoilProfile"/>.
        /// </summary>
        public PipingStochasticSoilModel StochasticSoilModel { get; set; }

        /// <summary>
        /// Gets or sets the profile which contains a 1 dimensional definition of soil layers with properties.
        /// </summary>
        public PipingStochasticSoilProfile StochasticSoilProfile { get; set; }

        /// <summary>
        /// Gets or sets whether the assessment level is manual input for the calculation.
        /// </summary>
        public bool UseAssessmentLevelManualInput { get; set; }

        /// <summary>
        /// Gets the value <c>true</c> if the entry point and exit point of the
        /// instance of <see cref="PipingInput"/> match the entry point and
        /// exit point of <see cref="SurfaceLine"/>; or <c>false</c> if this is
        /// not the case, or if there is no <see cref="SurfaceLine"/> assigned.
        /// </summary>
        public bool IsEntryAndExitPointInputSynchronized
        {
            get
            {
                if (SurfaceLine == null)
                {
                    return false;
                }

                double newEntryPointL;
                double newExitPointL;
                GetEntryExitPointFromSurfaceLine(out newEntryPointL, out newExitPointL);

                return Math.Abs(newEntryPointL - EntryPointL) < 1e-6
                       && Math.Abs(newExitPointL - ExitPointL) < 1e-6;
            }
        }

        /// <summary>
        /// Gets or sets the outside high water level.
        /// [m+NAP]
        /// </summary>
        /// <remarks>This property is only used for calculations when <see cref="UseAssessmentLevelManualInput"/> is <c>true</c>.</remarks>
        public RoundedDouble AssessmentLevel
        {
            get
            {
                return assessmentLevel;
            }
            set
            {
                assessmentLevel = value.ToPrecision(assessmentLevel.NumberOfDecimalPlaces);
            }
        }

        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Applies the entry point and exit point of the <see cref="SurfaceLine"/> to the
        /// parameters of the instance of <see cref="PipingInput"/>.
        /// </summary>
        /// <remarks>When no surface line is present, the entry and exit point are set to <see cref="double.NaN"/>.</remarks>
        public void SynchronizeEntryAndExitPointInput()
        {
            if (SurfaceLine == null)
            {
                EntryPointL = RoundedDouble.NaN;
                ExitPointL = RoundedDouble.NaN;
            }
            else
            {
                double tempEntryPointL;
                double tempExitPointL;
                GetEntryExitPointFromSurfaceLine(out tempEntryPointL, out tempExitPointL);

                if (tempExitPointL <= ExitPointL)
                {
                    EntryPointL = (RoundedDouble) tempEntryPointL;
                    ExitPointL = (RoundedDouble) tempExitPointL;
                }
                else
                {
                    ExitPointL = (RoundedDouble) tempExitPointL;
                    EntryPointL = (RoundedDouble) tempEntryPointL;
                }
            }
        }

        public override object Clone()
        {
            var clone = (PipingInput) base.Clone();

            clone.phreaticLevelExit = (NormalDistribution) PhreaticLevelExit.Clone();
            clone.dampingFactorExit = (LogNormalDistribution) DampingFactorExit.Clone();

            return clone;
        }

        private void GetEntryExitPointFromSurfaceLine(out double tempEntryPointL, out double tempExitPointL)
        {
            Point3D[] surfaceLineGeometry = SurfaceLine.Points.ToArray();
            int entryPointIndex = Array.IndexOf(surfaceLineGeometry, SurfaceLine.DikeToeAtRiver);
            int exitPointIndex = Array.IndexOf(surfaceLineGeometry, SurfaceLine.DikeToeAtPolder);

            Point2D[] localGeometry = SurfaceLine.LocalGeometry.ToArray();

            tempEntryPointL = localGeometry[0].X;
            tempExitPointL = localGeometry[localGeometry.Length - 1].X;

            bool isDifferentPoints = entryPointIndex < 0 || exitPointIndex < 0 || entryPointIndex < exitPointIndex;
            if (isDifferentPoints && exitPointIndex > 0)
            {
                tempExitPointL = localGeometry.ElementAt(exitPointIndex).X;
            }

            if (isDifferentPoints && entryPointIndex > -1)
            {
                tempEntryPointL = localGeometry.ElementAt(entryPointIndex).X;
            }
        }

        private static void ValidateEntryExitPoint(RoundedDouble entryPointLocalXCoordinate, RoundedDouble exitPointLocalXCoordinate)
        {
            if (entryPointLocalXCoordinate >= exitPointLocalXCoordinate)
            {
                throw new ArgumentOutOfRangeException(null, Resources.PipingInput_EntryPointL_greater_or_equal_to_ExitPointL);
            }
        }

        private void ValidatePointOnSurfaceLine(RoundedDouble newLocalXCoordinate)
        {
            if (!surfaceLine.ValidateInRange(newLocalXCoordinate))
            {
                var validityRange = new Range<double>(surfaceLine.LocalGeometry.First().X, surfaceLine.LocalGeometry.Last().X);
                string outOfRangeMessage = string.Format(Resources.PipingInput_ValidatePointOnSurfaceLine_Length_must_be_in_Range_0_,
                                                         validityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
                throw new ArgumentOutOfRangeException(null, outOfRangeMessage);
            }
        }

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
        /// Gets the kinematic viscosity of water at 10 °C.
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
        /// Gets or sets the damping factor at the exit point.
        /// </summary>
        public LogNormalDistribution DampingFactorExit
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

        #endregion
    }
}