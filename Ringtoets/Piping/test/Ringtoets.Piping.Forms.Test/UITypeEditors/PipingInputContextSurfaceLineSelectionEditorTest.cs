﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Design;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.UITypeEditors;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingInputContextSurfaceLineSelectionEditorTest
    {
        [Test]
        public void EditValue_NoCurrentItemInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();
            var assessmentSectionMock = mockRepository.StrictMock<AssessmentSectionBase>();

            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = ValidSurfaceLine()
            };
            var inputParametersContext = new PipingInputContext(pipingInput,
                                                                new[]
                                                                {
                                                                    new RingtoetsPipingSurfaceLine()
                                                                },
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                assessmentSectionMock);

            var properties = new PipingInputContextProperties
            {
                Data = inputParametersContext
            };

            var editor = new PipingInputContextSurfaceLineSelectionEditor();
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
            var assessmentSectionMock = mockRepository.StrictMock<AssessmentSectionBase>();

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 0.0), 
                new Point3D(1.0, 0.0, 1.0)
            });
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };
            var inputParametersContext = new PipingInputContext(pipingInput,
                                                                new[]
                                                                {
                                                                    surfaceLine
                                                                },
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                assessmentSectionMock);

            var properties = new PipingInputContextProperties
            {
                Data = inputParametersContext
            };

            var editor = new PipingInputContextSurfaceLineSelectionEditor();
            var someValue = new object();
            var propertyBag = new DynamicPropertyBag(properties);

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();
            context.Expect(c => c.Instance).Return(propertyBag);

            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(surfaceLine, result);

            mockRepository.VerifyAll();
        }

        private static RingtoetsPipingSurfaceLine ValidSurfaceLine()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 0.0),
                new Point3D(1.0, 0.0, 1.0)
            });
            return surfaceLine;
        }
    }
}