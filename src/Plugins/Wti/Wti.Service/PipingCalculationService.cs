using System.Collections.Generic;
using Wti.Calculation.Piping;
using Wti.Data;

namespace Wti.Service
{
    /// <summary>
    /// This class controls the <see cref="PipingData"/> and its PipingDataNodePresenter.
    /// Interactions from the PipingDataNodePresenter are handles by this class.
    /// </summary>
    public static class PipingCalculationService
    {
        /// <summary>
        /// Performs a piping calculation based on the supplied <see cref="PipingData"/> and sets <see cref="PipingData.Output"/>
        /// to the <see cref="PipingCalculationResult"/> if the calculation was successful.
        /// </summary>
        /// <param name="pipingData">The <see cref="PipingData"/> to base the input for the calculation upon.</param>
        /// <returns>A <see cref="List{T}"/> with all the messages that were returned due to validation errors in the given <paramref name="pipingData"/>
        /// or error message that occurred when performing the calculation.</returns>
        public static List<string> PerfromValidatedCalculation(PipingData pipingData)
        {
            var validationResults = Validate(pipingData);
            if (validationResults.Count > 0)
            {
                ClearOutput(pipingData);
                return validationResults;
            }
            try
            {
                Calculate(pipingData);
            }
            catch (PipingCalculationException e)
            {
                ClearOutput(pipingData);
                return new List<string>{ e.Message };
            }
            return new List<string>();
        }

        private static void ClearOutput(PipingData pipingData)
        {
            pipingData.Output = null;
        }

        private static void Calculate(PipingData pipingData)
        {
            var input = CreateInputFromData(pipingData);
            var pipingCalculation = new PipingCalculation(input);

            var pipingResult = pipingCalculation.Calculate();

            pipingData.Output = new PipingOutput(pipingResult.UpliftZValue,
                                                 pipingResult.UpliftFactorOfSafety,
                                                 pipingResult.HeaveZValue, pipingResult.HeaveFactorOfSafety, pipingResult.SellmeijerZValue, pipingResult.SellmeijerFactorOfSafety);
        }

        public static List<string> Validate(PipingData pipingData)
        {
            var input = CreateInputFromData(pipingData);

            return new PipingCalculation(input).Validate();
        } 

        private static PipingCalculationInput CreateInputFromData(PipingData pipingData)
        {
            return new PipingCalculationInput(
                pipingData.WaterVolumetricWeight,
                pipingData.UpliftModelFactor,
                pipingData.AssessmentLevel,
                pipingData.PiezometricHeadExit,
                pipingData.DampingFactorExit,
                pipingData.PhreaticLevelExit,
                pipingData.PiezometricHeadPolder,
                pipingData.CriticalHeaveGradient,
                pipingData.ThicknessCoverageLayer,
                pipingData.SellmeijerModelFactor,
                pipingData.SellmeijerReductionFactor,
                pipingData.SeepageLength,
                pipingData.SandParticlesVolumicWeight,
                pipingData.WhitesDragCoefficient,
                pipingData.Diameter70,
                pipingData.DarcyPermeability,
                pipingData.WaterKinematicViscosity,
                pipingData.Gravity,
                pipingData.ThicknessAquiferLayer,
                pipingData.MeanDiameter70,
                pipingData.BeddingAngle,
                pipingData.ExitPointXCoordinate
                );
        }
    }
}