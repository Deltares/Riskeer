﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.UITypeEditors;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingInputContextStochasticSoilModelSelectionEditorTest
    {
        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                StochasticSoilModel = new StochasticSoilModel(0, "StochasticSoilModelName", "StochasticSoilModelSegmentName")
            };
            var pipingInputContext = new PipingInputContext(pipingInput,
                                                            calculationItem,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            new[]
                                                            {
                                                                new TestStochasticSoilModel()
                                                            },
                                                            failureMechanism,
                                                            assessmentSectionMock);

            var properties = new PipingInputContextProperties
            {
                Data = pipingInputContext
            };

            var editor = new PipingInputContextStochasticSoilModelSelectionEditor();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(context, provider, someValue);

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
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();

            var stochasticSoilProfile = new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            var stochasticSoilModel = new StochasticSoilModel(0, "Model", string.Empty)
            {
                Geometry =
                {
                    new Point2D(0, 2),
                    new Point2D(4, 2)
                },
                StochasticSoilProfiles =
                {
                    stochasticSoilProfile
                }
            };
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(2, 1, 0),
                new Point3D(2, 3, 0)
            });

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                StochasticSoilModel = stochasticSoilModel,
                StochasticSoilProfile = stochasticSoilProfile
            };
            var inputParametersContext = new PipingInputContext(pipingInput,
                                                                calculationItem,
                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                new[]
                                                                {
                                                                    stochasticSoilModel
                                                                },
                                                                failureMechanism,
                                                                assessmentSectionMock);

            var properties = new PipingInputContextProperties
            {
                Data = inputParametersContext
            };

            var editor = new PipingInputContextStochasticSoilModelSelectionEditor();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(stochasticSoilModel, result);

            mockRepository.VerifyAll();
        }
    }
}