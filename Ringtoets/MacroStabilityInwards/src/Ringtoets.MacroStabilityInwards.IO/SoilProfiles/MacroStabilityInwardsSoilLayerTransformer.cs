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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
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
            return new MacroStabilityInwardsSoilLayer1D(soilLayer.Top, ConvertSoilLayerData(soilLayer));
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
            return ConvertLayerRecursively(soilLayer);
        }

        /// <summary>
        /// Converts <see cref="SoilLayer2D"/> into <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to convert.</param>
        /// <returns>The converted <see cref="MacroStabilityInwardsSoilLayer2D"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        private static MacroStabilityInwardsSoilLayer2D ConvertLayerRecursively(SoilLayer2D soilLayer)
        {
            return new MacroStabilityInwardsSoilLayer2D(TransformSegmentsToRing(soilLayer.OuterLoop.Segments, soilLayer.MaterialName),
                                                        ConvertSoilLayerData(soilLayer),
                                                        soilLayer.NestedLayers.Select(ConvertLayerRecursively).ToArray());
        }

        /// <summary>
        /// Converts <see cref="SoilLayerBase"/> into <see cref="MacroStabilityInwardsSoilLayerData"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to get the data from.</param>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        private static MacroStabilityInwardsSoilLayerData ConvertSoilLayerData(SoilLayerBase soilLayer)
        {
            string soilLayerMaterialName = soilLayer.MaterialName;
            return new MacroStabilityInwardsSoilLayerData
            {
                ShearStrengthModel = TransformShearStrengthModel(soilLayer.ShearStrengthModel, soilLayerMaterialName),
                UsePop = TransformUsePop(soilLayer.UsePop, soilLayerMaterialName),

                MaterialName = soilLayerMaterialName,
                IsAquifer = TransformIsAquifer(soilLayer.IsAquifer, soilLayerMaterialName),
                Color = SoilLayerColorConverter.Convert(soilLayer.Color),

                AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) soilLayer.AbovePhreaticLevelMean,
                    CoefficientOfVariation = (RoundedDouble) soilLayer.AbovePhreaticLevelCoefficientOfVariation,
                    Shift = (RoundedDouble) soilLayer.AbovePhreaticLevelShift
                },
                BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) soilLayer.BelowPhreaticLevelMean,
                    CoefficientOfVariation = (RoundedDouble) soilLayer.BelowPhreaticLevelCoefficientOfVariation,
                    Shift = (RoundedDouble) soilLayer.BelowPhreaticLevelShift
                },
                Cohesion = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) soilLayer.CohesionMean,
                    CoefficientOfVariation = (RoundedDouble) soilLayer.CohesionCoefficientOfVariation
                },
                FrictionAngle = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) soilLayer.FrictionAngleMean,
                    CoefficientOfVariation = (RoundedDouble) soilLayer.FrictionAngleCoefficientOfVariation
                },
                ShearStrengthRatio = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) soilLayer.ShearStrengthRatioMean,
                    CoefficientOfVariation = (RoundedDouble) soilLayer.ShearStrengthRatioCoefficientOfVariation
                },
                StrengthIncreaseExponent = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) soilLayer.StrengthIncreaseExponentMean,
                    CoefficientOfVariation = (RoundedDouble) soilLayer.StrengthIncreaseExponentCoefficientOfVariation
                },
                Pop = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) soilLayer.PopMean,
                    CoefficientOfVariation = (RoundedDouble) soilLayer.PopCoefficientOfVariation
                }
            };
        }

        /// <summary>
        /// Transforms a <see cref="double"/> to a <see cref="bool"/> for the 
        /// is aquifer property of soil layers.
        /// </summary>
        /// <param name="isAquifer">The value to transform.</param>
        /// <param name="soilLayerName">The name of the soil layer.</param>
        /// <returns>A <see cref="bool"/> based on <paramref name="isAquifer"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when
        /// <paramref name="isAquifer"/> could not be transformed.</exception>
        private static bool TransformIsAquifer(double? isAquifer, string soilLayerName)
        {
            try
            {
                return SoilLayerIsAquiferConverter.Convert(isAquifer);
            }
            catch (NotSupportedException e)
            {
                string exceptionMessage = CreateErrorMessage(soilLayerName,
                                                             string.Format(RingtoetsCommonIOResources.Transform_Invalid_value_ParameterName_0,
                                                                            RingtoetsCommonIOResources.SoilLayerData_IsAquifer_DisplayName));
                throw new ImportedDataTransformException(exceptionMessage, e);
            }
        }

        /// <summary>
        /// Transforms a <see cref="double"/> to a <see cref="bool"/> for the 
        /// use POP property of soil layers.
        /// </summary>
        /// <param name="usePop">The value to transform.</param>
        /// <param name="soilLayerName">The name of the soil layer.</param>
        /// <returns>A <see cref="bool"/> based on <paramref name="usePop"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when
        /// <paramref name="usePop"/> could not be transformed.</exception>
        private static bool TransformUsePop(double? usePop, string soilLayerName)
        {
            if (!usePop.HasValue)
            {
                return true;
            }

            if (Math.Abs(usePop.Value) < tolerance)
            {
                return false;
            }

            string exceptionMessage = CreateErrorMessage(soilLayerName,
                                                         string.Format(RingtoetsCommonIOResources.Transform_Invalid_value_ParameterName_0,
                                                                       Resources.SoilLayerData_UsePop_Description));
            throw new ImportedDataTransformException(exceptionMessage);
        }

        /// <summary>
        /// Transforms a <see cref="double"/> to a <see cref="MacroStabilityInwardsShearStrengthModel"/> for the 
        /// shear strength model of soil layers.
        /// </summary>
        /// <param name="shearStrengthModel">The value to transform.</param>
        /// <param name="soilLayerName">The name of the soil layer.</param>
        /// <returns>A <see cref="MacroStabilityInwardsShearStrengthModel"/> based 
        /// on <paramref name="shearStrengthModel"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when
        /// <paramref name="shearStrengthModel"/> could not be transformed.</exception>
        private static MacroStabilityInwardsShearStrengthModel TransformShearStrengthModel(double? shearStrengthModel,
                                                                                           string soilLayerName)
        {
            string exceptionMessage;
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
                exceptionMessage = CreateErrorMessage(soilLayerName,
                                                      Resources.MacroStabilityInwardsSoilLayerTransformer_TransformShearStrengthModel_No_MacroStabilityInwardsShearStrengthModel);
                throw new ImportedDataTransformException(exceptionMessage);
            }

            exceptionMessage = CreateErrorMessage(soilLayerName,
                                                  string.Format(RingtoetsCommonIOResources.Transform_Invalid_value_ParameterName_0,
                                                                Resources.SoilLayerData_ShearStrengthModel_Description));
            throw new ImportedDataTransformException(exceptionMessage);
        }

        /// <summary>
        /// Transforms a collection of <see cref="Segment2D"/> to <see cref="Ring"/>.
        /// </summary>
        /// <param name="segments">The segments to transform.</param>
        /// <param name="soilLayerName">The name of the soil layer.</param>
        /// <returns>A <see cref="Ring"/> based on <paramref name="segments"/>.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when 
        /// <paramref name="segments"/> could not be transformed to a <see cref="Ring"/>.</exception>
        private static Ring TransformSegmentsToRing(IEnumerable<Segment2D> segments, string soilLayerName)
        {
            try
            {
                return new Ring(segments.Select(s => s.FirstPoint));
            }
            catch (ArgumentException e)
            {
                string exceptionMessage = CreateErrorMessage(soilLayerName,
                                                             Resources.MacroStabilityInwardsSoilLayerTransformer_TransformSegmentToRing_Invalid_geometry_for_Ring);
                throw new ImportedDataTransformException(exceptionMessage, e);
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
            try
            {
                DistributionHelper.ValidateShiftedLogNormalDistribution(soilLayer.AbovePhreaticLevelDistributionType,
                                                                        Resources.SoilLayerData_AbovePhreaticLevelDistribution_Description);

                DistributionHelper.ValidateShiftedLogNormalDistribution(
                    soilLayer.BelowPhreaticLevelDistributionType,
                    Resources.SoilLayerData_BelowPhreaticLevelDistribution_DisplayName);

                DistributionHelper.ValidateLogNormalDistribution(
                    soilLayer.CohesionDistributionType,
                    soilLayer.CohesionShift,
                    Resources.SoilLayerData_CohesionDistribution_DisplayName);

                DistributionHelper.ValidateLogNormalDistribution(
                    soilLayer.FrictionAngleDistributionType,
                    soilLayer.FrictionAngleShift,
                    Resources.SoilLayerData_FrictionAngleDistribution_DisplayName);

                DistributionHelper.ValidateLogNormalDistribution(
                    soilLayer.ShearStrengthRatioDistributionType,
                    soilLayer.ShearStrengthRatioShift,
                    Resources.SoilLayerData_ShearStrengthRatioDistribution_DisplayName);

                DistributionHelper.ValidateLogNormalDistribution(
                    soilLayer.StrengthIncreaseExponentDistributionType,
                    soilLayer.StrengthIncreaseExponentShift,
                    Resources.SoilLayerData_StrengthIncreaseExponentDistribution_DisplayName);

                DistributionHelper.ValidateLogNormalDistribution(
                    soilLayer.PopDistributionType,
                    soilLayer.PopShift,
                    Resources.SoilLayerData_PopDistribution_DisplayName);
            }
            catch (ImportedDataTransformException e)
            {
                string errorMessage = CreateErrorMessage(soilLayer.MaterialName, e.Message);
                throw new ImportedDataTransformException(errorMessage, e);
            }
        }

        private static string CreateErrorMessage(string soilLayerName, string errorMessage)
        {
            return string.Format(RingtoetsCommonIOResources.Transform_Error_occurred_when_transforming_SoilLayer_0_errorMessage_1,
                                 soilLayerName,
                                 errorMessage);
        }
    }
}