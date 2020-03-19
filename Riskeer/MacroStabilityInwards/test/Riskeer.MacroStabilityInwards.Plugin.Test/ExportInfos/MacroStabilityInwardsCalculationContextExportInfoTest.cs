using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using Core.Common.Util;
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
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                // Call
                ExportInfo info = GetExportInfo(plugin);

                // Assert
                Assert.IsNotNull(info.CreateFileExporter);
                Assert.IsNotNull(info.IsEnabled);
                Assert.AreEqual("D-GEO Suite Stability Project", info.Name);
                Assert.AreEqual("Algemeen", info.Category);
                TestHelper.AssertImagesAreEqual(Resources.ExportIcon, info.Image);
                Assert.IsNotNull(info.FileFilterGenerator);
            }
        }

        [Test]
        public void CreateFileExporter_WithContext_ReturnFileExporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new MacroStabilityInwardsCalculationScenarioContext(new MacroStabilityInwardsCalculationScenario(),
                                                                              new CalculationGroup(),
                                                                              Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                              Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                              new MacroStabilityInwardsFailureMechanism(),
                                                                              assessmentSection);

            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = info.CreateFileExporter(context, "test");

                // Assert
                Assert.IsInstanceOf<MacroStabilityInwardsCalculationExporter>(fileExporter);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnFileFilter()
        {
            // Setup
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = info.FileFilterGenerator;

                // Assert
                Assert.AreEqual("D-GEO Suite Stability Project (*.stix)|*.stix", fileFilterGenerator.Filter);
            }
        }

        [Test]
        public void IsEnabled_CalculationWithoutOutput_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();
            
            var context = new MacroStabilityInwardsCalculationScenarioContext(new MacroStabilityInwardsCalculationScenario(),
                                                                              new CalculationGroup(),
                                                                              Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                              Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                              new MacroStabilityInwardsFailureMechanism(),
                                                                              assessmentSection);

            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_CalculationWithOutput_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
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

            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }

            mocks.VerifyAll();
        }

        private static ExportInfo GetExportInfo(MacroStabilityInwardsPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(MacroStabilityInwardsCalculationScenarioContext)
                                                       && ei.Name.Equals("D-GEO Suite Stability Project"));
        }
    }
}