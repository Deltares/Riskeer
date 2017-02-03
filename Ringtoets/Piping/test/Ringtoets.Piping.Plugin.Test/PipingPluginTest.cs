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
using System.Threading;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test
{
    [TestFixture]
    public class PipingPluginTest
    {
        [Test]
        [Apartment(ApartmentState.STA)] // For creation of XAML UI component (PipingRibbon)
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new PipingPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
                Assert.IsInstanceOf<PipingRibbon>(plugin.RibbonCommandHandler);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyClassesWithExpectedValues()
        {
            // Setup
            using (var plugin = new PipingPlugin())
            {
                // Call
                var mocks = new MockRepository();
                mocks.ReplayAll();

                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(7, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(PipingFailureMechanismContext),
                    typeof(PipingFailureMechanismContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(PipingInputContext),
                    typeof(PipingInputContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(PipingOutputContext),
                    typeof(PipingOutputContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(RingtoetsPipingSurfaceLine),
                    typeof(RingtoetsPipingSurfaceLineProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StochasticSoilModelCollectionContext),
                    typeof(StochasticSoilModelCollectionProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StochasticSoilModel),
                    typeof(StochasticSoilModelProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StochasticSoilProfile),
                    typeof(StochasticSoilProfileProperties));

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            mocks.ReplayAll();

            using (var plugin = new PipingPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(13, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RingtoetsPipingSurfaceLinesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RingtoetsPipingSurfaceLine)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StochasticSoilModelCollectionContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StochasticSoilModel)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StochasticSoilProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingCalculationScenarioContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingOutputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingScenariosContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyPipingOutput)));
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            mocks.ReplayAll();

            using (var plugin = new PipingPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(5, viewInfos.Length);

                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingFailureMechanismView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingCalculationsView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingFailureMechanismResultView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingInputView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingScenariosView)));
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetFileInfos_Always_ReturnsExpectedFileInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            mocks.ReplayAll();

            using (var plugin = new PipingPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, importInfos.Length);
                Assert.AreEqual(1, importInfos.Count(i => i.DataType == typeof(RingtoetsPipingSurfaceLinesContext)));
                Assert.AreEqual(1, importInfos.Count(i => i.DataType == typeof(StochasticSoilModelCollectionContext)));
            }
            mocks.VerifyAll();
        }
    }
}