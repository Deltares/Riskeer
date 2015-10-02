using System;
using System.Linq.Expressions;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using log4net;
using log4net.Core;
using Wti.Calculation.Piping;
using Wti.Data;
using Wti.Forms.NodePresenters;
using Wti.Forms.Properties;

namespace Wti.Controller
{
    public class PipingDataNodeController : IContextMenuProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(PipingCalculationInput));
        private PipingDataNodePresenter nodePresenter = new PipingDataNodePresenter();

        public PipingDataNodePresenter NodePresenter
        {
            get
            {
                return nodePresenter;
            }
        }

        public PipingDataNodeController()
        {
            nodePresenter.ContextMenu = GetContextMenu;
        }

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

                pipingData.Output = new PipingOuput(
                    pipingResult.HeaveFactorOfSafety, 
                    pipingResult.HeaveZValue, 
                    pipingResult.UpliftFactorOfSafety, 
                    pipingResult.UpliftZValue, 
                    pipingResult.SellmeijerFactorOfSafety, 
                    pipingResult.SellmeijerZValue
                );
            }
            catch (PipingCalculationException e)
            {
                logger.Warn(String.Format(Resources.ErrorInPipingCalculation_0, e.Message));
            }
        }
    }
}