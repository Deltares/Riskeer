﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Design;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Forms.UITypeEditors;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class MacroStabilityInwardsInputContextStochasticSoilModelSelectionEditorTest
    {
        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();

            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var input = new MacroStabilityInwardsInput
            {
                StochasticSoilModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            };
            var inputContext = new MacroStabilityInwardsInputContext(input,
                                                                     calculationItem,
                                                                     Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                     new[]
                                                                     {
                                                                         MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
                                                                     },
                                                                     failureMechanism,
                                                                     assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(inputContext, handler);

            var editor = new MacroStabilityInwardsInputContextStochasticSoilModelSelectionEditor();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();

            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(1.0, soilProfile);
            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel("Model", new[]
            {
                new Point2D(0, 2),
                new Point2D(4, 2)
            }, new[]
            {
                stochasticSoilProfile
            });

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(2, 1, 0),
                new Point3D(2, 3, 0)
            });

            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var input = new MacroStabilityInwardsInput
            {
                SurfaceLine = surfaceLine,
                StochasticSoilModel = stochasticSoilModel,
                StochasticSoilProfile = stochasticSoilProfile
            };
            var inputParametersContext = new MacroStabilityInwardsInputContext(input,
                                                                               calculationItem,
                                                                               Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                               new[]
                                                                               {
                                                                                   stochasticSoilModel
                                                                               },
                                                                               failureMechanism,
                                                                               assessmentSection);

            var properties = new MacroStabilityInwardsInputContextProperties(inputParametersContext, handler);

            var editor = new MacroStabilityInwardsInputContextStochasticSoilModelSelectionEditor();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            object result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(stochasticSoilModel, result);

            mockRepository.VerifyAll();
        }
    }
}