using System;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using NUnit.Framework;
using Wti.Data;
using Wti.Forms.NodePresenters;

namespace Wti.Controller.Test
{
    public class PipingDataNodeControllerTest
    {
        public PipingData ValidPipingData = new PipingData
        {
            AssessmentLevel = 1.0,
            BeddingAngle = 1.0,
            CriticalHeaveGradient = 1.0,
            DampingFactorExit = 1.0,
            DarcyPermeability = 1.0,
            Diameter70 = 1.0,
            ExitPointXCoordinate = 1.0,
            Gravity = 1.0,
            MeanDiameter70 = 1.0,
            PiezometricHeadExit = 1.0,
            PiezometricHeadPolder = 1.0,
            PhreaticLevelExit = 2.0,
            SandParticlesVolumicWeight = 1.0,
            SeepageLength = 1.0,
            SellmeijerModelFactor = 1.0,
            SellmeijerReductionFactor = 1.0,
            ThicknessAquiferLayer = 1.0,
            ThicknessCoverageLayer = 1.0,
            UpliftModelFactor = 1.0,
            WaterKinematicViscosity = 1.0,
            WaterVolumetricWeight = 1.0,
            WhitesDragCoefficient = 1.0
        };

        [Test]
        public void GivenDefaultPipingDataNodeController_WhenObtainingTheNodePresenter_ThenANodePresenterInstanceIsReturned()
        {
            PipingDataNodePresenter actual = new PipingDataNodeController().NodePresenter;
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<PipingDataNodePresenter>());
        }

        [Test]
        public void GivenAnObject_WhenObtainingAContextMenuBasedOnObject_ThenACastExceptionIsThrown()
        {
            object someData = new object();
            var pipingDataNodeController = new PipingDataNodeController();
            TestDelegate actual = () => pipingDataNodeController.GetContextMenu(someData);
            Assert.That(actual, Throws.InstanceOf<InvalidCastException>());
        }

        [Test]
        public void GivenPipingData_WhenObtainingAContextMenuBasedOnThePipingData_ThenAContextMenuAdapterWithAPipingContextMenuIsReturned()
        {
            var pipingData = new PipingData();
            var pipingDataNodeController = new PipingDataNodeController();
            IMenuItem actual = pipingDataNodeController.GetContextMenu(pipingData);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<MenuItemContextMenuStripAdapter>());
            Assert.That(((MenuItemContextMenuStripAdapter) actual).ContextMenuStrip, Is.InstanceOf<PipingContextMenuStrip>());
        }

        [Test]
        public void GivenPipingDataWithSomeValidInput_WhenInvokingCalculationThroughContextMenu_ThenPipingDataContainsOutput()
        {
            PipingData pipingData = ValidPipingData;
            var pipingDataNodeController = new PipingDataNodeController();
            MenuItemContextMenuStripAdapter contextMenu = pipingDataNodeController.GetContextMenu(pipingData) as MenuItemContextMenuStripAdapter;
            contextMenu.ContextMenuStrip.Items[0].PerformClick();
            var actual = pipingData.Output;
            Assert.That(actual, Is.Not.Null);
        }
    }
}