// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    /// Base class for piping calculation specific input parameters, i.e. values that can differ across various calculations.
    /// </summary>
    public abstract class PipingInput : CloneableObservable, ICalculationInputWithHydraulicBoundaryLocation
    {
        private NormalDistribution phreaticLevelExit;
        private LogNormalDistribution dampingFactorExit;
        private RoundedDouble exitPointL;
        private RoundedDouble entryPointL;
        private PipingSurfaceLine surfaceLine;

        /// <summary>
        /// Creates a new instance of <see cref="PipingInput"/>.
        /// </summary>
        protected PipingInput()
        {
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