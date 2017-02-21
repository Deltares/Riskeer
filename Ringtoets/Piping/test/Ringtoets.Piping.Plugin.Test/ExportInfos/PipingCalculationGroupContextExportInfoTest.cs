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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.IO.Exporters;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class PipingCalculationGroupContextExportInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new PipingCalculationGroupContext(new CalculationGroup(),
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<StochasticSoilModel>(),
                                                            new PipingFailureMechanism(),
                                                            assessmentSection);

            using (PipingPlugin plugin = new PipingPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = info.CreateFileExporter(context, "test");

                // Assert
                Assert.IsInstanceOf<PipingConfigurationExporter>(fileExporter);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilter_Always_ReturnFileFilter()
        {
            // Setup
            using (PipingPlugin plugin = new PipingPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                FileFilterGenerator fileFilter = info.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Ringtoets berekeningen configuratie. (*.xml)|*.xml", fileFilter.Filter);
            }
        }

        [Test]
        public void IsEnabled_CalculationGroupNoChildren_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new PipingCalculationGroupContext(new CalculationGroup(),
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<StochasticSoilModel>(),
                                                            new PipingFailureMechanism(),
                                                            assessmentSection);

            using (PipingPlugin plugin = new PipingPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void IsEnabled_CalculationGroupWithChildren_ReturnTrue(bool hasNestedGroup, bool hasCalculation)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            if (hasNestedGroup)
            {
                calculationGroup.Children.Add(new CalculationGroup());
            }

            if (hasCalculation)
            {
                calculationGroup.Children.Add(new PipingCalculation(new GeneralPipingInput()));
            }

            var context = new PipingCalculationGroupContext(calculationGroup,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<StochasticSoilModel>(),
                                                            new PipingFailureMechanism(),
                                                            assessmentSection);

            using (PipingPlugin plugin = new PipingPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }
        }

        private static ExportInfo GetExportInfo(PipingPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(PipingCalculationGroupContext));
        }
    }
}