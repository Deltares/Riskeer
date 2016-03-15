using System;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service
{
    public class PipingInputSynchronizer : IObserver
    {
        private const double seepageLengthStandardDeviationFraction = 0.1;

        private readonly PipingInput input;

        private PipingInputSynchronizer(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "Cannot create PipingInputSynchronizer without PipingInput.");
            }
            input.Attach(this);
            this.input = input;

            SynchronizeDerivedProperties();
        }

        /// <summary>
        /// Starts the synchronization of the given <see cref="PipingInput"/>.
        /// </summary>
        /// <param name="input">The input to synchronize the values for.</param>
        public static void Synchronize(PipingInput input)
        {
            new PipingInputSynchronizer(input);
        }

        public void UpdateObserver()
        {
            SynchronizeDerivedProperties();
        }

        private void SynchronizeDerivedProperties()
        {
            UpdateAssessmentLevel();
            UpdateSeepageLength();
            UpdateThicknessCoverageLayer();
            UpdateThicknessAquiferLayer();
            UpdatePiezometricHeadExit();
        }

        private void UpdateThicknessAquiferLayer()
        {
            PipingSoilProfile soilProfile = input.SoilProfile;
            RingtoetsPipingSurfaceLine surfaceLine = input.SurfaceLine;
            double exitPointL = input.ExitPointL;

            double thicknessTopAquiferLayer = GetThicknessTopAquiferLayer(soilProfile, surfaceLine, exitPointL);
            TrySetThicknessAquiferLayerMean(input, thicknessTopAquiferLayer);
        }

        private static double GetThicknessTopAquiferLayer(PipingSoilProfile soilProfile, RingtoetsPipingSurfaceLine surfaceLine, double exitPointL)
        {
            var thicknessTopAquiferLayer = double.NaN;

            if (soilProfile != null && surfaceLine != null && !double.IsNaN(exitPointL))
            {
                thicknessTopAquiferLayer = TryGetThicknessTopAquiferLayer(soilProfile, surfaceLine, exitPointL);
            }

            return thicknessTopAquiferLayer;
        }

        private static double TryGetThicknessTopAquiferLayer(PipingSoilProfile soilProfile, RingtoetsPipingSurfaceLine surfaceLine, double exitPointL)
        {
            try
            {
                var zAtL = surfaceLine.GetZAtL(exitPointL);
                return soilProfile.GetTopAquiferLayerThicknessBelowLevel(zAtL);
            }
            catch (ArgumentOutOfRangeException)
            {
                return double.NaN;
            }
        }

        private static void TrySetThicknessAquiferLayerMean(PipingInput input, double thicknessTopAquiferLayer)
        {
            try
            {
                input.ThicknessAquiferLayer.Mean = (RoundedDouble) thicknessTopAquiferLayer;
            }
            catch (ArgumentOutOfRangeException)
            {
                input.ThicknessAquiferLayer.Mean = (RoundedDouble) double.NaN;
            }
        }

        private void UpdateThicknessCoverageLayer()
        {
            try
            {
                input.ThicknessCoverageLayer.Mean = (RoundedDouble) PipingCalculationService.CalculateThicknessCoverageLayer(input);
            }
            catch (ArgumentOutOfRangeException)
            {
                input.ThicknessCoverageLayer.Mean = (RoundedDouble) double.NaN;
            }
        }

        private void UpdatePiezometricHeadExit()
        {
            try
            {
                input.PiezometricHeadExit = (RoundedDouble) PipingCalculationService.CalculatePiezometricHeadAtExit(input);
            }
            catch (ArgumentOutOfRangeException)
            {
                input.PiezometricHeadExit = (RoundedDouble) double.NaN;
            }
        }

        private void UpdateAssessmentLevel()
        {
            input.AssessmentLevel =
                input.HydraulicBoundaryLocation == null ?
                    (RoundedDouble) double.NaN :
                    (RoundedDouble) input.HydraulicBoundaryLocation.DesignWaterLevel;
        }

        private void UpdateSeepageLength()
        {
            try
            {
                input.SeepageLength.Mean = input.ExitPointL - input.EntryPointL;
            }
            catch (ArgumentOutOfRangeException)
            {
                input.SeepageLength.Mean = (RoundedDouble) double.NaN;
            }
            input.SeepageLength.StandardDeviation = input.SeepageLength.Mean*seepageLengthStandardDeviationFraction;
        }
    }
}