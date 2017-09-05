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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
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
            SetProperties(soilLayer, layer.Properties);

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
            SetProperties(soilLayer, layer.Properties);

            return layer;
        }

        /// <summary>
        /// Sets the properties of the <see cref="MacroStabilityInwardsSoilLayerProperties"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to get the properties from.</param>
        /// <param name="properties">The properties to set the data upon.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        private static void SetProperties(SoilLayerBase soilLayer, MacroStabilityInwardsSoilLayerProperties properties)
        {
            properties.ShearStrengthModel = TransformShearStrengthModel(soilLayer.ShearStrengthModel);
            properties.UsePop = TransformUsePop(soilLayer.UsePop);

            properties.MaterialName = soilLayer.MaterialName;
            properties.IsAquifer = SoilLayerIsAquiferConverter.Convert(soilLayer.IsAquifer);
            properties.Color = SoilLayerColorConverter.Convert(soilLayer.Color);
            properties.AbovePhreaticLevelMean = soilLayer.AbovePhreaticLevelMean;
            properties.AbovePhreaticLevelCoefficientOfVariation = soilLayer.AbovePhreaticLevelCoefficientOfVariation;
            properties.AbovePhreaticLevelShift = soilLayer.AbovePhreaticLevelShift;
            properties.BelowPhreaticLevelMean = soilLayer.BelowPhreaticLevelMean;
            properties.BelowPhreaticLevelCoefficientOfVariation = soilLayer.BelowPhreaticLevelCoefficientOfVariation;
            properties.BelowPhreaticLevelShift = soilLayer.BelowPhreaticLevelShift;
            properties.CohesionMean = soilLayer.CohesionMean;
            properties.CohesionCoefficientOfVariation = soilLayer.CohesionCoefficientOfVariation;
            properties.FrictionAngleMean = soilLayer.FrictionAngleMean;
            properties.FrictionAngleCoefficientOfVariation = soilLayer.FrictionAngleCoefficientOfVariation;
            properties.ShearStrengthRatioMean = soilLayer.ShearStrengthRatioMean;
            properties.ShearStrengthRatioCoefficientOfVariation = soilLayer.ShearStrengthRatioCoefficientOfVariation;
            properties.StrengthIncreaseExponentMean = soilLayer.StrengthIncreaseExponentMean;
            properties.StrengthIncreaseExponentCoefficientOfVariation = soilLayer.StrengthIncreaseExponentCoefficientOfVariation;
            properties.PopMean = soilLayer.PopMean;
            properties.PopCoefficientOfVariation = soilLayer.PopCoefficientOfVariation;
        }

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

            throw new ImportedDataTransformException(string.Format(Resources.MacroStabilityInwardsSoilLayerTransformer_Invalid_value_ParameterName_0,
                                                                   Resources.SoilLayerProperties_UsePop_Description));
        }

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
                return MacroStabilityInwardsShearStrengthModel.None;
            }

            throw new ImportedDataTransformException(string.Format(Resources.MacroStabilityInwardsSoilLayerTransformer_Invalid_value_ParameterName_0,
                                                                   Resources.SoilLayerProperties_ShearStrengthModel_Description));
        }

        private static Ring TransformSegmentToRing(IEnumerable<Segment2D> segments)
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

        /// <summary>
        /// Validates whether the values of the distribution and shift for the stochastic parameters 
        /// are correct for creating a soil layer.
        /// </summary>
        /// <param name="soilLayer">The soil layer to validate.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when any of the distributions of the
        /// stochastic parameters is not defined as lognormal or is shifted when it should not be.</exception>
        private static void ValidateStochasticParameters(SoilLayerBase soilLayer)
        {
            SoilLayerDistributionHelper.ValidateIsLogNormal(soilLayer.AbovePhreaticLevelDistribution,
                                                            Resources.SoilLayerProperties_AbovePhreaticLevelDistribution_Description);

            SoilLayerDistributionHelper.ValidateIsLogNormal(
                soilLayer.BelowPhreaticLevelDistribution,
                Resources.SoilLayerProperties_BelowPhreaticLevelDistribution_Description);

            SoilLayerDistributionHelper.ValidateIsNonShiftedLogNormal(
                soilLayer.CohesionDistribution,
                soilLayer.CohesionShift,
                Resources.SoilLayerProperties_CohesionDistribution_Description);

            SoilLayerDistributionHelper.ValidateIsNonShiftedLogNormal(
                soilLayer.FrictionAngleDistribution,
                soilLayer.FrictionAngleShift,
                Resources.SoilLayerProperties_FrictionAngleDistribution_Description);

            SoilLayerDistributionHelper.ValidateIsNonShiftedLogNormal(
                soilLayer.ShearStrengthRatioDistribution,
                soilLayer.ShearStrengthRatioShift,
                Resources.SoilLayerProperties_ShearStrengthRatioDistribution_Description);

            SoilLayerDistributionHelper.ValidateIsNonShiftedLogNormal(
                soilLayer.StrengthIncreaseExponentDistribution,
                soilLayer.StrengthIncreaseExponentShift,
                Resources.SoilLayerProperties_StrengthIncreaseExponentDistribution_Description);

            SoilLayerDistributionHelper.ValidateIsNonShiftedLogNormal(
                soilLayer.PopDistribution,
                soilLayer.PopShift,
                Resources.SoilLayerProperties_PopDistribution_Description);
        }
    }
}