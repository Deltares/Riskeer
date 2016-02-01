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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

using Deltares.WTIPiping;

using Ringtoets.Piping.Calculation.Properties;

namespace Ringtoets.Piping.Calculation
{
    /// <summary>
    /// This class represents a combination of piping sub-calculations, which together can be used
    /// to assess based on piping.
    /// </summary>
    public class PipingCalculator
    {
        private readonly PipingCalculatorInput input;

        /// <summary>
        /// Constructs a new <see cref="PipingCalculator"/>. The <paramref name="input"/> is used to
        /// obtain the parameters used in the different sub calculations.
        /// </summary>
        /// <param name="input">The <see cref="PipingCalculatorInput"/> containing all the values required
        /// for performing a piping calculation.</param>
        public PipingCalculator(PipingCalculatorInput input)
        {
            this.input = input;
        }

        /// <summary>
        /// Performs the actual sub calculations and returns a <see cref="PipingCalculatorResult"/>, which
        /// contains the results of all sub calculations.
        /// </summary>
        /// <returns>A <see cref="PipingCalculatorResult"/> containing the results of the sub calculations.</returns>
        /// <exception cref="PipingCalculatorException">Thrown when any of the invocations of the sub-calculations from the kernel throws an Exception.</exception>
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
                sellmeijerResult.FoSp
            );
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
            if (soilProfileValidationResults.Count == 0 && surfaceLineValidationResults.Count == 0)
            {
                upliftCalculatorValidationResults = ValidateUpliftCalculator();
            }
            List<string> heaveCalculatorValidationResults = CreateHeaveCalculator().Validate();
            List<string> sellmeijerCalculatorValidationResults = CreateSellmeijerCalculator().Validate();

            return upliftCalculatorValidationResults
                .Concat(surfaceLineValidationResults)
                .Concat(soilProfileValidationResults)
                .Concat(heaveCalculatorValidationResults)
                .Concat(sellmeijerCalculatorValidationResults)
                .ToList();
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
                EffectiveThicknessCalculator effectiveThicknessCalculator = CalculateEffectiveThickness();
                return CreateUpliftCalculator(effectiveThicknessCalculator.EffectiveStress).Validate();
            }
            catch (Exception exception)
            {
                return new List<string>
                {
                    exception.Message
                };
            }
        }

        private Sellmeijer2011Calculator CalculateSellmeijer()
        {
            Sellmeijer2011Calculator sellmeijerCalculator = CreateSellmeijerCalculator();

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

        private HeaveCalculator CalculateHeave()
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

        private WTIUpliftCalculator CalculateUplift()
        {
            EffectiveThicknessCalculator calculatedEffectiveStressResult = CalculateEffectiveThickness();
            WTIUpliftCalculator upliftCalculator = CreateUpliftCalculator(calculatedEffectiveStressResult.EffectiveStress);
            
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

        private HeaveCalculator CreateHeaveCalculator()
        {
            var calculator = new HeaveCalculator
            {
                Ich = input.CriticalHeaveGradient,
                PhiExit = input.PiezometricHeadExit,
                DTotal = input.ThicknessCoverageLayer,
                PhiPolder = input.PiezometricHeadPolder,
                RExit = input.DampingFactorExit,
                HExit = input.PhreaticLevelExit
            };
            return calculator;
        }

        private WTIUpliftCalculator CreateUpliftCalculator(double effectiveStress)
        {
            var calculator = new WTIUpliftCalculator
            {
                VolumetricWeightOfWater = input.WaterVolumetricWeight,
                ModelFactorUplift = input.UpliftModelFactor,
                EffectiveStress = effectiveStress,
                HRiver = input.AssessmentLevel,
                PhiExit = input.PiezometricHeadExit,
                RExit = input.DampingFactorExit,
                HExit = input.PhreaticLevelExit,
                PhiPolder = input.PiezometricHeadPolder
            };
            return calculator;
        }

        private Sellmeijer2011Calculator CreateSellmeijerCalculator()
        {
            var calculator = new Sellmeijer2011Calculator
            {
                ModelFactorPiping = input.SellmeijerModelFactor,
                HRiver = input.AssessmentLevel,
                HExit = input.PhreaticLevelExit,
                Rc = input.SellmeijerReductionFactor,
                DTotal = input.ThicknessCoverageLayer,
                SeepageLength = input.SeepageLength,
                GammaSubParticles = input.SandParticlesVolumicWeight,
                WhitesDragCoefficient = input.WhitesDragCoefficient,
                D70 = input.Diameter70,
                VolumetricWeightOfWater = input.WaterVolumetricWeight,
                DarcyPermeability = input.DarcyPermeability,
                KinematicViscosityWater = input.WaterKinematicViscosity,
                Gravity = input.Gravity,
                DAquifer = input.ThicknessAquiferLayer,
                D70Mean = input.MeanDiameter70,
                BeddingAngle = input.BeddingAngle
            };
            return calculator;
        }

        private EffectiveThicknessCalculator CalculateEffectiveThickness()
        {
            var calculator = new EffectiveThicknessCalculator
            {
                ExitPointXCoordinate = input.ExitPointXCoordinate,
                PhreaticLevel = input.PhreaticLevelExit,
                SoilProfile = PipingProfileCreator.Create(input.SoilProfile),
                SurfaceLine = PipingSurfaceLineCreator.Create(input.SurfaceLine),
                VolumicWeightOfWater = input.WaterVolumetricWeight
            };
            calculator.Calculate();
            return calculator;
        }
    }
}