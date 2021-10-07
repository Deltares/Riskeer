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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Design;
using Core.Gui.PropertyBag;
using Core.Gui.UITypeEditors;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.PresentationObjects;
using Riskeer.Revetment.Forms.UITypeEditors;

namespace Riskeer.Revetment.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class WaveConditionsInputContextTargetProbabilitySelectionEditorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var editor = new WaveConditionsInputContextTargetProbabilitySelectionEditor();

            // Assert
            Assert.IsInstanceOf<SelectionEditor<IHasTargetProbabilityProperty, SelectableTargetProbability>>(editor);
        }

        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var selectableTargetProbability = new SelectableTargetProbability(
                new AssessmentSectionStub(), Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                WaveConditionsInputWaterLevelType.None, 0.1);

            var properties = new ObjectPropertiesWithSelectableTargetProbability(
                selectableTargetProbability, Enumerable.Empty<SelectableTargetProbability>());

            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new WaveConditionsInputContextTargetProbabilitySelectionEditor();
            var someValue = new object();

            var mocks = new MockRepository();
            var serviceProvider = mocks.Stub<IServiceProvider>();
            var service = mocks.Stub<IWindowsFormsEditorService>();
            var descriptorContext = mocks.Stub<ITypeDescriptorContext>();
            serviceProvider.Stub(p => p.GetService(null)).IgnoreArguments().Return(service);
            descriptorContext.Stub(c => c.Instance).Return(propertyBag);
            mocks.ReplayAll();

            // Call
            object result = editor.EditValue(descriptorContext, serviceProvider, someValue);

            // Assert
            Assert.AreSame(someValue, result);
            mocks.VerifyAll();
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            const WaveConditionsInputWaterLevelType waterLevelType = WaveConditionsInputWaterLevelType.LowerLimit;
            const double targetProbability = 0.1;

            var selectableTargetProbability = new SelectableTargetProbability(assessmentSection, assessmentSection.WaterLevelCalculationsForLowerLimitNorm, waterLevelType, targetProbability);
            var properties = new ObjectPropertiesWithSelectableTargetProbability(
                selectableTargetProbability,
                new[]
                {
                    new SelectableTargetProbability(assessmentSection, assessmentSection.WaterLevelCalculationsForSignalingNorm, waterLevelType, targetProbability)
                });

            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new WaveConditionsInputContextTargetProbabilitySelectionEditor();
            var someValue = new object();

            var mocks = new MockRepository();
            var serviceProvider = mocks.Stub<IServiceProvider>();
            var service = mocks.Stub<IWindowsFormsEditorService>();
            var descriptorContext = mocks.Stub<ITypeDescriptorContext>();
            serviceProvider.Stub(p => p.GetService(null)).IgnoreArguments().Return(service);
            descriptorContext.Stub(c => c.Instance).Return(propertyBag);
            mocks.ReplayAll();

            // Call
            object result = editor.EditValue(descriptorContext, serviceProvider, someValue);

            // Assert
            Assert.AreEqual(selectableTargetProbability, result);
            mocks.VerifyAll();
        }

        private class ObjectPropertiesWithSelectableTargetProbability : ObjectProperties<object>, IHasTargetProbabilityProperty
        {
            private readonly IEnumerable<SelectableTargetProbability> selectableTargetProbabilities;

            public ObjectPropertiesWithSelectableTargetProbability(SelectableTargetProbability selectedTargetProbability,
                                                                   IEnumerable<SelectableTargetProbability> selectableTargetProbabilities)
            {
                SelectedTargetProbability = selectedTargetProbability;
                this.selectableTargetProbabilities = selectableTargetProbabilities;
            }

            public SelectableTargetProbability SelectedTargetProbability { get; }

            public IEnumerable<SelectableTargetProbability> GetSelectableTargetProbabilities()
            {
                return selectableTargetProbabilities;
            }
        }
    }
}