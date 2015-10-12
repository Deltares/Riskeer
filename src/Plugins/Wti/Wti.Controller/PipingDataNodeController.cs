using System;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using log4net;
using Wti.Calculation.Piping;
using Wti.Data;
using Wti.Forms.NodePresenters;
using Wti.Forms.Properties;

namespace Wti.Controller
{
    /// <summary>
    /// This class controls the <see cref="PipingData"/> and its <see cref="PipingDataNodePresenter"/>.
    /// Interactions from the <see cref="PipingDataNodePresenter"/> are handles by this class.
    /// </summary>
    public class PipingDataNodeController : IContextMenuProvider
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(PipingCalculationInput));
        private readonly PipingDataNodePresenter nodePresenter = new PipingDataNodePresenter();

        /// <summary>
        /// Creates a instance of a controller for the <see cref="PipingDataNodePresenter"/>.
        /// </summary>
        public PipingDataNodeController()
        {
            nodePresenter.ContextMenu = GetContextMenu;
        }

        /// <summary>
        /// The <see cref="PipingDataNodePresenter"/> which this class controls.
        /// </summary>
        public PipingDataNodePresenter NodePresenter
        {
            get
            {
                return nodePresenter;
            }
        }

        /// <summary>
        /// Creates a <see cref="PipingContextMenuStrip"/> which actions are tied to this controller.
        /// </summary>
        /// <param name="pipingData"></param>
        /// <returns></returns>
        public IMenuItem GetContextMenu(object pipingData)
        {
            var contextMenu = new PipingContextMenuStrip((PipingData) pipingData);
            contextMenu.OnCalculationClick += OnCalculationClick;
            var contextMenuAdapter = new MenuItemContextMenuStripAdapter(contextMenu);
            return contextMenuAdapter;
        }

        private void OnCalculationClick(PipingData pipingData)
        {
            try
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
            catch (PipingCalculationException e)
            {
                logger.Warn(String.Format(Resources.ErrorInPipingCalculation_0, e.Message));
            }
        }
    }
}