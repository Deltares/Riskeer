using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.IO.Exporters;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationContextExportInfoTest
    {
        private MacroStabilityInwardsPlugin plugin;
        private ExportInfo info;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.Replay(gui);
            mocks.Replay(mainWindow);

            plugin = new MacroStabilityInwardsPlugin
            {
                Gui = gui
            };

            info = plugin.GetExportInfos().First(ei => ei.DataType == typeof(MacroStabilityInwardsCalculationScenarioContext)
                                                       && ei.Name.Equals("D-GEO Suite Stability Project"));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual("D-GEO Suite Stability Project", info.Name);
            Assert.AreEqual("stix", info.Extension);
            Assert.IsNotNull(info.CreateFileExporter);
            Assert.IsNotNull(info.IsEnabled);
            Assert.AreEqual("Algemeen", info.Category);
            TestHelper.AssertImagesAreEqual(Resources.ExportIcon, info.Image);
            Assert.IsNotNull(info.GetExportPath);
        }

        [Test]
        public void CreateFileExporter_WithContext_ReturnFileExporter()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new MacroStabilityInwardsCalculationScenarioContext(new MacroStabilityInwardsCalculationScenario(),
                                                                              new CalculationGroup(),
                                                                              Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                              Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                              new MacroStabilityInwardsFailureMechanism(),
                                                                              assessmentSection);

            // Call
            IFileExporter fileExporter = info.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsCalculationExporter>(fileExporter);
        }

        [Test]
        public void IsEnabled_CalculationWithoutOutput_ReturnFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new MacroStabilityInwardsCalculationScenarioContext(new MacroStabilityInwardsCalculationScenario(),
                                                                              new CalculationGroup(),
                                                                              Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                              Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                              new MacroStabilityInwardsFailureMechanism(),
                                                                              assessmentSection);

            // Call
            bool isEnabled = info.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
        }

        [Test]
        public void IsEnabled_CalculationWithOutput_ReturnTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            var context = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                              new CalculationGroup(),
                                                                              Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                              Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                              new MacroStabilityInwardsFailureMechanism(),
                                                                              assessmentSection);

            // Call
            bool isEnabled = info.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }
    }
}