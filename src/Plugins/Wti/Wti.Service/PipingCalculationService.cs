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
        /// <exception cref="PipingCalculationException">Thrown when calling <see cref="Validate"/> would return a non empty <see cref="List{T}"/>.</exception>
        public static void Calculate(PipingData pipingData)
        {
            var input = new PipingCalculationInput(
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
            var pipingCalculation = new PipingCalculation(input);
            var pipingResult = pipingCalculation.Calculate();

            pipingData.Output = new PipingOutput(pipingResult.UpliftZValue,
                                                    pipingResult.UpliftFactorOfSafety,
                                                    pipingResult.HeaveZValue, pipingResult.HeaveFactorOfSafety, pipingResult.SellmeijerZValue, pipingResult.SellmeijerFactorOfSafety);
        }

        public static List<string> Validate(PipingData pipingData)
        {
            return null;
        } 
    }
}