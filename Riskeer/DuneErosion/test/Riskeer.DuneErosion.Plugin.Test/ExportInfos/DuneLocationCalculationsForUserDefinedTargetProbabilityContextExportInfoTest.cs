﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.DuneErosion.IO;
using CoreGuiResources = Core.Gui.Properties.Resources;

namespace Riskeer.DuneErosion.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class DuneLocationCalculationsForUserDefinedTargetProbabilityContextExportInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                ExportInfo info = GetExportInfo(plugin);

                // Assert
                Assert.IsNotNull(info.CreateFileExporter);
                Assert.IsNotNull(info.IsEnabled);
                Assert.AreEqual("Hydraulische belastingen", info.Name);
                Assert.AreEqual("bnd", info.Extension);
                Assert.AreEqual("Algemeen", info.Category);
                TestHelper.AssertImagesAreEqual(CoreGuiResources.ExportIcon, info.Image);
                Assert.IsNotNull(info.GetExportPath);
            }
        }

        [Test]
        public void CreateFileExporter_Always_ReturnFileExporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(),
                                                                                             new DuneErosionFailureMechanism(),
                                                                                             assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = info.CreateFileExporter(context, "test");

                // Assert
                Assert.IsInstanceOf<DuneLocationCalculationsExporter>(fileExporter);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_CalculationsWithoutOutput_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability
            {
                DuneLocationCalculations =
                {
                    new DuneLocationCalculation(new TestDuneLocation())
                }
            };

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(calculationsForTargetProbability,
                                                                                             new DuneErosionFailureMechanism(),
                                                                                             assessmentSection);

            using (var plugin = new DuneErosionPlugin())
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
        public void IsEnabled_CalculationsWithOutput_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability
            {
                DuneLocationCalculations =
                {
                    new DuneLocationCalculation(new TestDuneLocation())
                    {
                        Output = new TestDuneLocationCalculationOutput()
                    }
                }
            };

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(calculationsForTargetProbability,
                                                                                             new DuneErosionFailureMechanism(),
                                                                                             assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }

            mocks.VerifyAll();
        }

        private static ExportInfo GetExportInfo(DuneErosionPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(DuneLocationCalculationsForUserDefinedTargetProbabilityContext));
        }
    }
}