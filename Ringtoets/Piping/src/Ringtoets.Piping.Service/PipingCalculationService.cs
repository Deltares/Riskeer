using System;

using log4net;

using Ringtoets.Piping.Calculation.Piping;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service.Properties;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// This class is responsible for invoking operations on the <see cref="PipingCalculation"/>. Error and status information is 
    /// logged during the execution of the operation. At the end of an operation, a <see cref="PipingCalculationResult"/> is returned,
    /// representing the result of the operation.
    /// </summary>
    public static class PipingCalculationService
    {
        private static readonly ILog pipingDataLogger = LogManager.GetLogger(typeof(PipingData));

        /// <summary>
        /// Performs validation over the values on the given <paramref name="pipingData"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="pipingData">The <see cref="PipingData"/> for which to validate the values.</param>
        /// <returns>False if <paramref name="pipingData"/> contains validation errors; True otherwise.</returns>
        public static bool Validate(PipingData pipingData)
        {
            pipingDataLogger.Info(String.Format(Resources.Validation_Subject_0_started_Time_1_,
                                                pipingData.Name, DateTimeService.CurrentTimeAsString));

            var validationResults = new PipingCalculation(CreateInputFromData(pipingData)).Validate();
            LogMessagesAsError(Resources.Error_in_piping_validation_0, validationResults.ToArray());

            pipingDataLogger.Info(String.Format(Resources.Validation_Subject_0_ended_Time_1_,
                                                pipingData.Name, DateTimeService.CurrentTimeAsString));

            return validationResults.Count == 0;
        }

        /// <summary>
        /// Performs a piping calculation based on the supplied <see cref="PipingData"/> and sets <see cref="PipingData.Output"/>
        /// to the <see cref="PipingCalculationResult"/> if the calculation was successful. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="pipingData">The <see cref="PipingData"/> to base the input for the calculation upon.</param>
        /// <remarks>Consider calling <see cref="Validate"/> first to see if calculation is possible.</remarks>
        public static void Calculate(PipingData pipingData)
        {
            pipingDataLogger.Info(String.Format(Resources.Calculation_Subject_0_started_Time_1_,
                                                pipingData.Name, DateTimeService.CurrentTimeAsString));

            try
            {
                var pipingResult = new PipingCalculation(CreateInputFromData(pipingData)).Calculate();

                pipingData.Output = new PipingOutput(pipingResult.UpliftZValue,
                                                     pipingResult.UpliftFactorOfSafety,
                                                     pipingResult.HeaveZValue,
                                                     pipingResult.HeaveFactorOfSafety,
                                                     pipingResult.SellmeijerZValue,
                                                     pipingResult.SellmeijerFactorOfSafety);
            }
            catch (PipingCalculationException e)
            {
                LogMessagesAsError(Resources.Error_in_piping_calculation_0, e.Message);
            }
            finally
            {
                pipingDataLogger.Info(String.Format(Resources.Calculation_Subject_0_ended_Time_1_,
                                                    pipingData.Name, DateTimeService.CurrentTimeAsString));
            }
        }

        private static void LogMessagesAsError(string format, params string[] errorMessages)
        {
            foreach (var errorMessage in errorMessages)
            {
                pipingDataLogger.Error(string.Format(format, errorMessage));
            }
        }

        private static PipingCalculationInput CreateInputFromData(PipingData pipingData)
        {
            return new PipingCalculationInput(
                pipingData.WaterVolumetricWeight,
                pipingData.UpliftModelFactor,
                pipingData.AssessmentLevel,
                pipingData.PiezometricHeadExit,
                PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(pipingData).GetDesignValue(),
                PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(pipingData).GetDesignValue(),
                pipingData.PiezometricHeadPolder,
                pipingData.CriticalHeaveGradient,
                PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(pipingData).GetDesignValue(),
                pipingData.SellmeijerModelFactor,
                pipingData.SellmeijerReductionFactor,
                PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(pipingData).GetDesignValue(),
                PipingSemiProbabilisticDesignValueFactory.GetSandParticlesVolumicWeight(pipingData).GetDesignValue(),
                pipingData.WhitesDragCoefficient,
                PipingSemiProbabilisticDesignValueFactory.GetDiameter70(pipingData).GetDesignValue(),
                PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(pipingData).GetDesignValue(),
                pipingData.WaterKinematicViscosity,
                pipingData.Gravity,
                PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(pipingData).GetDesignValue(),
                pipingData.MeanDiameter70,
                pipingData.BeddingAngle,
                pipingData.ExitPointXCoordinate,
                pipingData.SurfaceLine,
                pipingData.SoilProfile
                );
        }
    }
}