﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.KernelWrapper.Properties;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using EffectiveThicknessCalculator = Ringtoets.Piping.KernelWrapper.SubCalculator.EffectiveThicknessCalculator;
using HeaveCalculator = Ringtoets.Piping.KernelWrapper.SubCalculator.HeaveCalculator;

namespace Ringtoets.Piping.KernelWrapper
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
                throw new ArgumentNullException("input", "PipingCalculatorInput required for creating a PipingCalculator.");
            }
            if (factory == null)
            {
                throw new ArgumentNullException("factory", "IPipingSubCalculatorFactory required for creating a PipingCalculator.");
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
            var upliftResult = CalculateUplift();
            var heaveResult = CalculateHeave();
            var sellmeijerResult = CalculateSellmeijer();

            return new PipingCalculatorResult(
                upliftResult.Zu,
                upliftResult.FoSu,
                heaveResult.Zh,
                heaveResult.FoSh,
                sellmeijerResult.Zp,
                sellmeijerResult.FoSp,
                heaveResult.Gradient,
                sellmeijerResult.CreepCoefficient,
                sellmeijerResult.CriticalFall,
                sellmeijerResult.ReducedFall);
        }

        /// <summary>
        /// Returns a list of validation messages. The validation messages are based on the values of the <see cref="PipingCalculatorInput"/>
        /// which was provided to this <see cref="PipingCalculator"/> and are determined by the Piping kernel.
        /// </summary>
        public List<string> Validate()
        {
            List<string> soilProfileValidationResults = ValidateSoilProfile();
            List<string> surfaceLineValidationResults = ValidateSurfaceLine();
            List<string> upliftCalculatorValidationResults = new List<string>();
            List<string> pipingProfilePropertyCalculatorValidationResults = new List<string>();
            List<string> heaveCalculatorValidationResults = new List<string>();
            List<string> sellmeijerCalculatorValidationResults = new List<string>();
            if (soilProfileValidationResults.Count == 0 && surfaceLineValidationResults.Count == 0)
            {
                upliftCalculatorValidationResults = ValidateUpliftCalculator();
                pipingProfilePropertyCalculatorValidationResults = CreatePipingProfilePropertyCalculator().Validate();
                heaveCalculatorValidationResults = CreateHeaveCalculator().Validate();
                sellmeijerCalculatorValidationResults = CreateSellmeijerCalculator().Validate();
            }

            return upliftCalculatorValidationResults
                .Concat(surfaceLineValidationResults)
                .Concat(soilProfileValidationResults)
                .Concat(heaveCalculatorValidationResults)
                .Concat(sellmeijerCalculatorValidationResults)
                .Concat(pipingProfilePropertyCalculatorValidationResults)
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
                var calculator = factory.CreateEffectiveThicknessCalculator();
                calculator.ExitPointXCoordinate = input.ExitPointXCoordinate;
                calculator.PhreaticLevel = input.PhreaticLevelExit;
                calculator.SoilProfile = PipingProfileCreator.Create(input.SoilProfile);
                calculator.SurfaceLine = PipingSurfaceLineCreator.Create(input.SurfaceLine);
                calculator.VolumicWeightOfWater = input.WaterVolumetricWeight;
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
            var calculator = factory.CreatePiezometricHeadAtExitCalculator();
            calculator.PhiPolder = input.PhreaticLevelExit;
            calculator.HRiver = input.AssessmentLevel;
            calculator.RExit = input.DampingFactorExit;
            calculator.Calculate();

            return calculator.PhiExit;
        }

        private List<string> ValidateSurfaceLine()
        {
            var validationResults = new List<string>();
            if (input.SurfaceLine == null)
            {
                validationResults.Add(Resources.PipingCalculation_Validate_Lacks_surfaceline_uplift);
            }
            else
            {
                try
                {
                    PipingSurfaceLineCreator.Create(input.SurfaceLine).Validate();
                }
                catch (PipingSurfaceLineException e)
                {
                    validationResults.Add(e.Message);
                }
            }
            return validationResults;
        }

        private List<string> ValidateSoilProfile()
        {
            var validationResults = new List<string>();
            if (input.SoilProfile == null)
            {
                validationResults.Add(Resources.PipingCalculation_Validate_Lacks_SoilProfile_uplift);
            }
            else
            {
                try
                {
                    PipingProfileCreator.Create(input.SoilProfile).Validate();
                }
                catch (PipingProfileException e)
                {
                    validationResults.Add(e.Message);
                }
            }
            return validationResults;
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
            var heaveCalculator = CreateHeaveCalculator();

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

        private IHeaveCalculator CreateHeaveCalculator()
        {
            var calculator = factory.CreateHeaveCalculator();
            calculator.Ich = input.CriticalHeaveGradient;
            calculator.PhiExit = input.PiezometricHeadExit;
            calculator.DTotal = input.ThicknessCoverageLayer;
            calculator.PhiPolder = input.PhreaticLevelExit;
            calculator.RExit = input.DampingFactorExit;
            calculator.HExit = input.PhreaticLevelExit;
            calculator.BottomLevelAquitardAboveExitPointZ = GetBottomAquitardLayerAboveExitPointZ();
            return calculator;
        }

        private IUpliftCalculator CreateUpliftCalculator()
        {
            var effectiveStress = DetermineEffectiveStressForOneLayerProfile(input.ThicknessCoverageLayer, input.SaturatedVolumicWeightOfCoverageLayer, input.WaterVolumetricWeight);

            var calculator = factory.CreateUpliftCalculator();
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
            var calculator = factory.CreateSellmeijerCalculator();
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

        private double GetBottomAquitardLayerAboveExitPointZ()
        {
            var pipingProfilePropertyCalculator = CreatePipingProfilePropertyCalculator();
            pipingProfilePropertyCalculator.Calculate();
            var bottomAquitardLayerAboveExitPointZ = pipingProfilePropertyCalculator.BottomAquitardLayerAboveExitPointZ;
            return bottomAquitardLayerAboveExitPointZ;
        }

        private IPipingProfilePropertyCalculator CreatePipingProfilePropertyCalculator()
        {
            var calculator = factory.CreatePipingProfilePropertyCalculator();
            calculator.SoilProfile = PipingProfileCreator.Create(input.SoilProfile);
            calculator.SurfaceLine = PipingSurfaceLineCreator.Create(input.SurfaceLine);
            calculator.ExitPointX = input.ExitPointXCoordinate;
            return calculator;
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
            return thicknessOfCoverageLayer*(volumicWeightOfCoverageLayer - waterVolumetricWeight);
        }
    }
}