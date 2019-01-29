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
using System.Collections.Generic;
using System.Linq;
using Deltares.WTIPiping;
using Ringtoets.Piping.KernelWrapper.Creators;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using EffectiveThicknessCalculator = Ringtoets.Piping.KernelWrapper.SubCalculator.EffectiveThicknessCalculator;
using HeaveCalculator = Ringtoets.Piping.KernelWrapper.SubCalculator.HeaveCalculator;

namespace Riskeer.Piping.KernelWrapper
{
    /// <summary>
    /// This class represents a combination of piping sub calculations, which together can be used
    /// to assess based on piping.
    /// </summary>
    public class PipingCalculator
    {
        private readonly PipingCalculatorInput input;
        private readonly IPipingSubCalculatorFactory factory;

        /// <summary>
        /// Constructs a new <see cref="PipingCalculator"/>. The <paramref name="input"/> is used to
        /// obtain the parameters used in the different sub calculations.
        /// </summary>
        /// <param name="input">The <see cref="PipingCalculatorInput"/> containing all the values required
        /// for performing a piping calculation.</param>
        /// <param name="factory">The factory responsible for creating the sub calculators.</param>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> or <paramref name="factory"/> is <c>null</c>.</exception>
        public PipingCalculator(PipingCalculatorInput input, IPipingSubCalculatorFactory factory)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), @"PipingCalculatorInput required for creating a PipingCalculator.");
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory), @"IPipingSubCalculatorFactory required for creating a PipingCalculator.");
            }

            this.input = input;
            this.factory = factory;
        }

        /// <summary>
        /// Performs the actual sub calculations and returns a <see cref="PipingCalculatorResult"/>, which
        /// contains the results of all sub calculations.
        /// </summary>
        /// <returns>A <see cref="PipingCalculatorResult"/> containing the results of the sub calculations.</returns>
        /// <exception cref="PipingCalculatorException">Thrown when any of the invocations of the sub calculations from the kernel throws an Exception.</exception>
        public PipingCalculatorResult Calculate()
        {
            IUpliftCalculator upliftResult = CalculateUplift();
            IHeaveCalculator heaveResult = CalculateHeave();
            ISellmeijerCalculator sellmeijerResult = CalculateSellmeijer();

            return new PipingCalculatorResult(new PipingCalculatorResult.ConstructionProperties
            {
                UpliftZValue = upliftResult.Zu,
                UpliftFactorOfSafety = upliftResult.FoSu,
                HeaveZValue = heaveResult.Zh,
                HeaveFactorOfSafety = heaveResult.FoSh,
                SellmeijerZValue = sellmeijerResult.Zp,
                SellmeijerFactorOfSafety = sellmeijerResult.FoSp,
                UpliftEffectiveStress = upliftResult.EffectiveStress,
                HeaveGradient = heaveResult.Gradient,
                SellmeijerCreepCoefficient = sellmeijerResult.CreepCoefficient,
                SellmeijerCriticalFall = sellmeijerResult.CriticalFall,
                SellmeijerReducedFall = sellmeijerResult.ReducedFall
            });
        }

        /// <summary>
        /// Returns a list of validation messages. The validation messages are based on the values of the <see cref="PipingCalculatorInput"/>
        /// which was provided to this <see cref="PipingCalculator"/> and are determined by the Piping kernel.
        /// </summary>
        public List<string> Validate()
        {
            List<string> effectiveThicknessCalculatorValidationResults = CreateEffectiveThicknessCalculator().Validate();
            List<string> pipingProfilePropertyCalculatorValidationResults = CreatePipingProfilePropertyCalculator().Validate();
            List<string> upliftCalculatorValidationResults = ValidateUpliftCalculator();
            List<string> heaveCalculatorValidationResults = CreateHeaveCalculator().Validate();
            var sellmeijerCalculatorValidationResults = new List<string>();

            if (!pipingProfilePropertyCalculatorValidationResults.Any())
            {
                sellmeijerCalculatorValidationResults.AddRange(CreateSellmeijerCalculator().Validate());
            }

            return upliftCalculatorValidationResults.Concat(heaveCalculatorValidationResults)
                                                    .Concat(sellmeijerCalculatorValidationResults)
                                                    .Concat(pipingProfilePropertyCalculatorValidationResults)
                                                    .Concat(effectiveThicknessCalculatorValidationResults)
                                                    .Distinct()
                                                    .ToList();
        }

        /// <summary>
        /// Calculates the effective thickness of the coverage layer based on the values of the <see cref="PipingCalculatorInput"/>.
        /// </summary>
        /// <returns>The thickness of the coverage layer.</returns>
        /// <exception cref="PipingCalculatorException">Thrown when:
        /// <list type="bullet">
        /// <item>surface at exit point's x-coordinate is higher than the soil profile</item>
        /// <item>surface line is <c>null</c></item>
        /// <item>soil profile is <c>null</c></item>
        /// <item>soil profile's aquifer layer is not set</item>
        /// </list></exception>
        public double CalculateEffectiveThicknessCoverageLayer()
        {
            try
            {
                IEffectiveThicknessCalculator calculator = CreateEffectiveThicknessCalculator();
                calculator.Calculate();

                return calculator.EffectiveHeight;
            }
            catch (SoilVolumicMassCalculatorException e)
            {
                throw new PipingCalculatorException(e.Message, e);
            }
            catch (NullReferenceException e)
            {
                throw new PipingCalculatorException(e.Message, e);
            }
        }

        /// <summary>
        /// Calculates the piezometric head at the exit point based on the values of the <see cref="PipingCalculatorInput"/>.
        /// </summary>
        /// <returns>The piezometric head at the exit point.</returns>
        public double CalculatePiezometricHeadAtExit()
        {
            IPiezoHeadCalculator calculator = factory.CreatePiezometricHeadAtExitCalculator();
            calculator.PhiPolder = input.PhreaticLevelExit;
            calculator.HRiver = input.AssessmentLevel;
            calculator.RExit = input.DampingFactorExit;
            calculator.Calculate();

            return calculator.PhiExit;
        }

        private List<string> ValidateUpliftCalculator()
        {
            try
            {
                return CreateUpliftCalculator().Validate();
            }
            catch (Exception exception)
            {
                return new List<string>
                {
                    exception.Message
                };
            }
        }

        private ISellmeijerCalculator CalculateSellmeijer()
        {
            ISellmeijerCalculator sellmeijerCalculator = CreateSellmeijerCalculator();

            try
            {
                sellmeijerCalculator.Calculate();
            }
            catch (PipingException<Sellmeijer2011Calculator> e)
            {
                throw new PipingCalculatorException(e.Message, e);
            }
            catch (PipingException<SellmeijerBaseCalculator> e)
            {
                throw new PipingCalculatorException(e.Message, e);
            }

            return sellmeijerCalculator;
        }

        private IHeaveCalculator CalculateHeave()
        {
            IHeaveCalculator heaveCalculator = CreateHeaveCalculator();

            try
            {
                heaveCalculator.Calculate();
            }
            catch (PipingException<HeaveCalculator> e)
            {
                throw new PipingCalculatorException(e.Message, e);
            }

            return heaveCalculator;
        }

        private IUpliftCalculator CalculateUplift()
        {
            IUpliftCalculator upliftCalculator = CreateUpliftCalculator();

            try
            {
                upliftCalculator.Calculate();
            }
            catch (WTIUpliftCalculatorException e)
            {
                throw new PipingCalculatorException(e.Message, e);
            }
            catch (PipingException<EffectiveThicknessCalculator> e)
            {
                throw new PipingCalculatorException(e.Message, e);
            }

            return upliftCalculator;
        }

        private IEffectiveThicknessCalculator CreateEffectiveThicknessCalculator()
        {
            IEffectiveThicknessCalculator calculator = factory.CreateEffectiveThicknessCalculator();
            calculator.ExitPointXCoordinate = input.ExitPointXCoordinate;
            calculator.PhreaticLevel = input.PhreaticLevelExit;
            calculator.SoilProfile = CreateSoilProfile();
            calculator.SurfaceLine = CreateSurfaceLine();
            calculator.VolumicWeightOfWater = input.WaterVolumetricWeight;
            return calculator;
        }

        private IHeaveCalculator CreateHeaveCalculator()
        {
            IHeaveCalculator calculator = factory.CreateHeaveCalculator();
            calculator.Ich = input.CriticalHeaveGradient;
            calculator.PhiExit = input.PiezometricHeadExit;
            calculator.DTotal = input.ThicknessCoverageLayer;
            calculator.PhiPolder = input.PhreaticLevelExit;
            calculator.RExit = input.DampingFactorExit;
            calculator.HExit = input.PhreaticLevelExit;
            return calculator;
        }

        private IUpliftCalculator CreateUpliftCalculator()
        {
            double effectiveStress = DetermineEffectiveStressForOneLayerProfile(input.EffectiveThicknessCoverageLayer, input.SaturatedVolumicWeightOfCoverageLayer, input.WaterVolumetricWeight);

            IUpliftCalculator calculator = factory.CreateUpliftCalculator();
            calculator.VolumetricWeightOfWater = input.WaterVolumetricWeight;
            calculator.ModelFactorUplift = input.UpliftModelFactor;
            calculator.EffectiveStress = effectiveStress;
            calculator.HRiver = input.AssessmentLevel;
            calculator.PhiExit = input.PiezometricHeadExit;
            calculator.RExit = input.DampingFactorExit;
            calculator.HExit = input.PhreaticLevelExit;
            calculator.PhiPolder = input.PhreaticLevelExit;
            return calculator;
        }

        private ISellmeijerCalculator CreateSellmeijerCalculator()
        {
            ISellmeijerCalculator calculator = factory.CreateSellmeijerCalculator();
            calculator.ModelFactorPiping = input.SellmeijerModelFactor;
            calculator.HRiver = input.AssessmentLevel;
            calculator.HExit = input.PhreaticLevelExit;
            calculator.Rc = input.SellmeijerReductionFactor;
            calculator.DTotal = input.ThicknessCoverageLayer;
            calculator.SeepageLength = input.SeepageLength;
            calculator.GammaSubParticles = input.SandParticlesVolumicWeight;
            calculator.WhitesDragCoefficient = input.WhitesDragCoefficient;
            calculator.D70 = input.Diameter70;
            calculator.VolumetricWeightOfWater = input.WaterVolumetricWeight;
            calculator.DarcyPermeability = input.DarcyPermeability;
            calculator.KinematicViscosityWater = input.WaterKinematicViscosity;
            calculator.Gravity = input.Gravity;
            calculator.DAquifer = input.ThicknessAquiferLayer;
            calculator.D70Mean = input.MeanDiameter70;
            calculator.BeddingAngle = input.BeddingAngle;
            calculator.BottomLevelAquitardAboveExitPointZ = GetBottomAquitardLayerAboveExitPointZ();
            return calculator;
        }

        private IPipingProfilePropertyCalculator CreatePipingProfilePropertyCalculator()
        {
            IPipingProfilePropertyCalculator calculator = factory.CreatePipingProfilePropertyCalculator();
            calculator.SoilProfile = CreateSoilProfile();
            calculator.SurfaceLine = CreateSurfaceLine();
            calculator.ExitPointX = input.ExitPointXCoordinate;
            return calculator;
        }

        private PipingSurfaceLine CreateSurfaceLine()
        {
            return input.SurfaceLine == null ? null : PipingSurfaceLineCreator.Create(input.SurfaceLine);
        }

        private PipingProfile CreateSoilProfile()
        {
            return input.SoilProfile == null ? null : PipingProfileCreator.Create(input.SoilProfile);
        }

        private double GetBottomAquitardLayerAboveExitPointZ()
        {
            IPipingProfilePropertyCalculator pipingProfilePropertyCalculator = CreatePipingProfilePropertyCalculator();
            pipingProfilePropertyCalculator.Calculate();
            return pipingProfilePropertyCalculator.BottomAquitardLayerAboveExitPointZ;
        }

        /// <summary>
        /// Determines the effective stress for a one layer profile.
        /// </summary>
        /// <param name="thicknessOfCoverageLayer">The thickness of the aquitard layer.</param>
        /// <param name="volumicWeightOfCoverageLayer">The saturated volumic weight of the aquitard layer.</param>
        /// <param name="waterVolumetricWeight">The volumetric weight of water.</param>
        /// <returns>The effective stress.</returns>
        private static double DetermineEffectiveStressForOneLayerProfile(double thicknessOfCoverageLayer, double volumicWeightOfCoverageLayer, double waterVolumetricWeight)
        {
            return thicknessOfCoverageLayer * (volumicWeightOfCoverageLayer - waterVolumetricWeight);
        }
    }
}