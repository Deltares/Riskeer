﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.Properties;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.IO.SoilProfiles
{
    /// <summary>
    /// Transforms generic <see cref="SoilLayerBase"/> into <see cref="MacroStabilityInwardsSoilLayer1D"/>
    /// or <see cref="MacroStabilityInwardsSoilLayer2D"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilLayerTransformer
    {
        private const double tolerance = 1e-6;

        /// <summary>
        /// Transforms the generic <paramref name="soilLayer"/> into a
        /// <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to use in the transformation.</param>
        /// <returns>A <see cref="MacroStabilityInwardsSoilLayer1D"/> based on the given data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/> is
        /// <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        public static MacroStabilityInwardsSoilLayer1D Transform(SoilLayer1D soilLayer)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            ValidateStochasticParameters(soilLayer);

            var layer = new MacroStabilityInwardsSoilLayer1D(soilLayer.Top);
            SetProperties(soilLayer, layer.Data);

            return layer;
        }

        /// <summary>
        /// Transforms the generic <paramref name="soilLayer"/> into a
        /// <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to use in the transformation.</param>
        /// <returns>A <see cref="MacroStabilityInwardsSoilLayer1D"/> based on the given data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/> is
        /// <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        public static MacroStabilityInwardsSoilLayer2D Transform(SoilLayer2D soilLayer)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            ValidateStochasticParameters(soilLayer);

            if (soilLayer.OuterLoop == null)
            {
                throw new ImportedDataTransformException();
            }

            Ring outerRing = TransformSegmentToRing(soilLayer.OuterLoop);
            Ring[] innerRings = soilLayer.InnerLoops.Select(TransformSegmentToRing).ToArray();

            var layer = new MacroStabilityInwardsSoilLayer2D(outerRing, innerRings);
            SetProperties(soilLayer, layer.Data);

            return layer;
        }

        /// <summary>
        /// Sets the properties of the <see cref="MacroStabilityInwardsSoilLayerData"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to get the data from.</param>
        /// <param name="data">The data to set the properties upon.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        private static void SetProperties(SoilLayerBase soilLayer, MacroStabilityInwardsSoilLayerData data)
        {
            data.ShearStrengthModel = TransformShearStrengthModel(soilLayer.ShearStrengthModel);
            data.UsePop = TransformUsePop(soilLayer.UsePop);

            data.MaterialName = soilLayer.MaterialName;
            data.IsAquifer = TransformIsAquifer(soilLayer.IsAquifer);
            data.Color = SoilLayerColorConverter.Convert(soilLayer.Color);
            data.AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) soilLayer.AbovePhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) soilLayer.AbovePhreaticLevelCoefficientOfVariation,
                Shift = (RoundedDouble) soilLayer.AbovePhreaticLevelShift
            };
            data.BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) soilLayer.BelowPhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) soilLayer.BelowPhreaticLevelCoefficientOfVariation,
                Shift = (RoundedDouble) soilLayer.BelowPhreaticLevelShift
            };
            data.Cohesion = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) soilLayer.CohesionMean,
                CoefficientOfVariation = (RoundedDouble) soilLayer.CohesionCoefficientOfVariation
            };
            data.FrictionAngle = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) soilLayer.FrictionAngleMean,
                CoefficientOfVariation = (RoundedDouble) soilLayer.FrictionAngleCoefficientOfVariation
            };
            data.ShearStrengthRatio = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) soilLayer.ShearStrengthRatioMean,
                CoefficientOfVariation = (RoundedDouble) soilLayer.ShearStrengthRatioCoefficientOfVariation
            };
            data.StrengthIncreaseExponent = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) soilLayer.StrengthIncreaseExponentMean,
                CoefficientOfVariation = (RoundedDouble) soilLayer.StrengthIncreaseExponentCoefficientOfVariation
            };
            data.Pop = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) soilLayer.PopMean,
                CoefficientOfVariation = (RoundedDouble) soilLayer.PopCoefficientOfVariation
            };
        }

        /// <summary>
        /// Transforms a <see cref="double"/> to a <see cref="bool"/> for the 
        /// is aquifer property of soil layers.
        /// </summary>
        /// <param name="isAquifer">The value to transform.</param>
        /// <returns>A <see cref="bool"/> based on <paramref name="isAquifer"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when
        /// <paramref name="isAquifer"/> could not be transformed.</exception>
        private static bool TransformIsAquifer(double? isAquifer)
        {
            try
            {
                return SoilLayerIsAquiferConverter.Convert(isAquifer);
            }
            catch (NotSupportedException)
            {
                throw new ImportedDataTransformException(string.Format(RingtoetsCommonIOResources.Transform_Invalid_value_ParameterName_0,
                                                                       RingtoetsCommonIOResources.SoilLayerData_IsAquifer_DisplayName));
            }
        }

        /// <summary>
        /// Transforms a <see cref="double"/> to a <see cref="bool"/> for the 
        /// use POP property of soil layers.
        /// </summary>
        /// <param name="usePop">The value to transform.</param>
        /// <returns>A <see cref="bool"/> based on <paramref name="usePop"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when
        /// <paramref name="usePop"/> could not be transformed.</exception>
        private static bool TransformUsePop(double? usePop)
        {
            if (!usePop.HasValue)
            {
                return true;
            }

            if (Math.Abs(usePop.Value) < tolerance)
            {
                return false;
            }

            throw new ImportedDataTransformException(string.Format(RingtoetsCommonIOResources.Transform_Invalid_value_ParameterName_0,
                                                                   Resources.SoilLayerData_UsePop_Description));
        }

        /// <summary>
        /// Transforms a <see cref="double"/> to a <see cref="MacroStabilityInwardsShearStrengthModel"/> for the 
        /// shear strength model of soil layers.
        /// </summary>
        /// <param name="shearStrengthModel">The value to transform.</param>
        /// <returns>A <see cref="MacroStabilityInwardsShearStrengthModel"/> based 
        /// on <paramref name="shearStrengthModel"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when
        /// <paramref name="shearStrengthModel"/> could not be transformed.</exception>
        private static MacroStabilityInwardsShearStrengthModel TransformShearStrengthModel(double? shearStrengthModel)
        {
            if (!shearStrengthModel.HasValue)
            {
                return MacroStabilityInwardsShearStrengthModel.CPhi;
            }
            if (Math.Abs(shearStrengthModel.Value - 6) < tolerance)
            {
                return MacroStabilityInwardsShearStrengthModel.SuCalculated;
            }
            if (Math.Abs(shearStrengthModel.Value - 9) < tolerance)
            {
                return MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated;
            }
            if (Math.Abs(shearStrengthModel.Value - 1) < tolerance)
            {
                throw new ImportedDataTransformException(Resources.MacroStabilityInwardsSoilLayerTransformer_TransformShearStrengthModel_No_MacroStabilityInwardsShearStrengthModel);
            }

            throw new ImportedDataTransformException(string.Format(RingtoetsCommonIOResources.Transform_Invalid_value_ParameterName_0,
                                                                   Resources.SoilLayerData_ShearStrengthModel_Description));
        }

        /// <summary>
        /// Transforms a collection of <see cref="Segment2D"/> to <see cref="Ring"/>.
        /// </summary>
        /// <param name="segments">The segments to transform.</param>
        /// <returns>A <see cref="Ring"/> based on <paramref name="segments"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when 
        /// <paramref name="segments"/> could not be transformed to a <see cref="Ring"/>.</exception>
        private static Ring TransformSegmentToRing(IEnumerable<Segment2D> segments)
        {
            try
            {
                var points = new List<Point2D>();
                foreach (Segment2D segment in segments)
                {
                    points.AddRange(new[]
                    {
                        segment.FirstPoint,
                        segment.SecondPoint
                    });
                }
                return new Ring(points.Distinct());
            }
            catch (ArgumentException e)
            {
                throw new ImportedDataTransformException(Resources.MacroStabilityInwardsSoilLayerTransformer_TransformSegmentToRing_Invalid_geometry_for_Ring, e);
            }
        }

        /// <summary>
        /// Validates whether the values of the distribution and shift for the stochastic parameters 
        /// are correct for creating a soil layer.
        /// </summary>
        /// <param name="soilLayer">The soil layer to validate.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when any of the distributions of the
        /// stochastic parameters is not defined as log normal or is shifted when it should not be.</exception>
        private static void ValidateStochasticParameters(SoilLayerBase soilLayer)
        {
            DistributionHelper.ValidateIsLogNormal(soilLayer.AbovePhreaticLevelDistributionType,
                                                   Resources.SoilLayerData_AbovePhreaticLevelDistribution_Description);

            DistributionHelper.ValidateIsLogNormal(
                soilLayer.BelowPhreaticLevelDistributionType,
                Resources.SoilLayerData_BelowPhreaticLevelDistribution_DisplayName);

            DistributionHelper.ValidateIsNonShiftedLogNormal(
                soilLayer.CohesionDistributionType,
                soilLayer.CohesionShift,
                Resources.SoilLayerData_CohesionDistribution_DisplayName);

            DistributionHelper.ValidateIsNonShiftedLogNormal(
                soilLayer.FrictionAngleDistributionType,
                soilLayer.FrictionAngleShift,
                Resources.SoilLayerData_FrictionAngleDistribution_DisplayName);

            DistributionHelper.ValidateIsNonShiftedLogNormal(
                soilLayer.ShearStrengthRatioDistributionType,
                soilLayer.ShearStrengthRatioShift,
                Resources.SoilLayerData_ShearStrengthRatioDistribution_DisplayName);

            DistributionHelper.ValidateIsNonShiftedLogNormal(
                soilLayer.StrengthIncreaseExponentDistributionType,
                soilLayer.StrengthIncreaseExponentShift,
                Resources.SoilLayerData_StrengthIncreaseExponentDistribution_DisplayName);

            DistributionHelper.ValidateIsNonShiftedLogNormal(
                soilLayer.PopDistributionType,
                soilLayer.PopShift,
                Resources.SoilLayerData_PopDistribution_DisplayName);
        }
    }
}