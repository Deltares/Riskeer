// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Drawing;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.ViewHost;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.DuneErosion.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DuneErosionFailurePathContextTreeNodeInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNotNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNotNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.CheckedState);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_WithContext_ReturnsName()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new DuneErosionFailurePathContext(new DuneErosionFailureMechanism(), assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(context);

                // Assert
                Assert.AreEqual("Duinafslag", text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsFailureMechanismIcon()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.FailureMechanismIcon, image);
            }
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismInAssemblyTrue_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();
            var context = new DuneErosionFailurePathContext(failureMechanism, assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] children = info.ChildNodeObjects(context).ToArray();

                // Assert
                Assert.AreEqual(2, children.Length);
                var inputsFolder = (CategoryTreeFolder) children[0];
                Assert.AreEqual("Invoer", inputsFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

                Assert.AreEqual(2, inputsFolder.Contents.Count());
                var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputsFolder.Contents.ElementAt(0);
                Assert.AreSame(failureMechanism, failureMechanismSectionsContext.WrappedData);
                Assert.AreSame(assessmentSection, failureMechanismSectionsContext.AssessmentSection);

                var inputComment = (Comment) inputsFolder.Contents.ElementAt(1);
                Assert.AreSame(failureMechanism.InAssemblyInputComments, inputComment);

                var outputsFolder = (CategoryTreeFolder) children[1];
                Assert.AreEqual("Oordeel", outputsFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);

                Assert.AreEqual(3, outputsFolder.Contents.Count());

                var failureMechanismAssemblyCategoriesContext = (FailureMechanismAssemblyCategoriesContext) outputsFolder.Contents.ElementAt(0);
                Assert.AreSame(failureMechanism, failureMechanismAssemblyCategoriesContext.WrappedData);
                Assert.AreSame(assessmentSection, failureMechanismAssemblyCategoriesContext.AssessmentSection);

                using (new AssemblyToolCalculatorFactoryConfig())
                {
                    var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                    AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                    failureMechanismAssemblyCategoriesContext.GetFailureMechanismSectionAssemblyCategoriesFunc();
                    Assert.AreEqual(failureMechanism.GeneralInput.N, calculator.AssemblyCategoriesInput.N);
                }

                var failureMechanismResultsContext = (FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>) outputsFolder.Contents.ElementAt(1);
                Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
                Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.WrappedData);

                var outputComment = (Comment) outputsFolder.Contents.ElementAt(2);
                Assert.AreSame(failureMechanism.InAssemblyOutputComments, outputComment);
            }
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismInAssemblyFalse_ReturnChildDataNodes()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();
            
            var failureMechanism = new DuneErosionFailureMechanism
            {
                InAssembly = false
            };
            var context = new DuneErosionFailurePathContext(failureMechanism, assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] children = info.ChildNodeObjects(context).ToArray();

                // Assert
                Assert.AreEqual(1, children.Length);

                var comment = (Comment) children[0];
                Assert.AreSame(failureMechanism.NotInAssemblyComments, comment);
            }
            
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismInAssemblyTrue_CallsContextMenuBuilderMethods()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new DuneErosionFailureMechanism();
                var context = new DuneErosionFailurePathContext(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
                mocks.ReplayAll();

                using (var plugin = new DuneErosionPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(context, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismInAssemblyFalse_CallsContextMenuBuilderMethods()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new DuneErosionFailureMechanism
                {
                    InAssembly = false
                };
                var context = new DuneErosionFailurePathContext(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
                mocks.ReplayAll();

                using (var plugin = new DuneErosionPlugin())
                {
                    plugin.Gui = gui;

                    TreeNodeInfo info = GetInfo(plugin);

                    // Call
                    info.ContextMenuStrip(context, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }

        [TestFixture]
        public class DuneErosionFailurePathContextInAssemblyTreeNodeInfoTest :
            FailurePathInAssemblyTreeNodeInfoTestFixtureBase<DuneErosionPlugin, DuneErosionFailureMechanism, DuneErosionFailurePathContext>
        {
            public DuneErosionFailurePathContextInAssemblyTreeNodeInfoTest() : base(2, 0) {}

            protected override DuneErosionFailurePathContext CreateFailureMechanismContext(DuneErosionFailureMechanism failureMechanism,
                                                                                           IAssessmentSection assessmentSection)
            {
                return new DuneErosionFailurePathContext(failureMechanism, assessmentSection);
            }
        }

        private static TreeNodeInfo GetInfo(DuneErosionPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DuneErosionFailurePathContext));
        }
    }
}