// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Design;
using Core.Common.Base.Geometry;
using Core.Gui.PropertyBag;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Forms.UITypeEditors;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class MacroStabilityInwardsInputContextSurfaceLineSelectionEditorTest
    {
        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var provider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var context = Substitute.For<ITypeDescriptorContext>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var handler = Substitute.For<IObservablePropertyChangeHandler>();

            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                SurfaceLine = ValidSurfaceLine()
            };
            var inputParametersContext = new MacroStabilityInwardsInputContext(input,
                                                                               calculation,
                                                                               new[]
                                                                               {
                                                                                   new MacroStabilityInwardsSurfaceLine(string.Empty)
                                                                               },
                                                                               Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                               failureMechanism,
                                                                               assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(inputParametersContext,
                                                                             AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                                             handler);

            var editor = new MacroStabilityInwardsInputContextSurfaceLineSelectionEditor();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.GetService(Arg.Any<Type>()).Returns(service);
            // service.DropDownControl(Arg.Any<Control>());
            context.Instance.Returns(propertyBag);
            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var provider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var context = Substitute.For<ITypeDescriptorContext>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var handler = Substitute.For<IObservablePropertyChangeHandler>();

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 0.0),
                new Point3D(1.0, 0.0, 1.0)
            });
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                SurfaceLine = surfaceLine
            };
            var inputParametersContext = new MacroStabilityInwardsInputContext(input,
                                                                               calculation,
                                                                               new[]
                                                                               {
                                                                                   surfaceLine
                                                                               },
                                                                               Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                               failureMechanism,
                                                                               assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(inputParametersContext,
                                                                             AssessmentSectionTestHelper.GetTestAssessmentLevel,
                                                                             handler);

            var editor = new MacroStabilityInwardsInputContextSurfaceLineSelectionEditor();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.GetService(Arg.Any<Type>()).Returns(service);
            // service.DropDownControl(Arg.Any<Control>());
            context.Instance.Returns(propertyBag);
            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(surfaceLine, result);
        }

        private static MacroStabilityInwardsSurfaceLine ValidSurfaceLine()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 0.0),
                new Point3D(1.0, 0.0, 1.0)
            });
            return surfaceLine;
        }
    }
}