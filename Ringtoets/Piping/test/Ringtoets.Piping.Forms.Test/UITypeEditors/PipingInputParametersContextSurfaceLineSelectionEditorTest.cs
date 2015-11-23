using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Core.Common.Utils.PropertyBag.Dynamic;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.UITypeEditors;

namespace Ringtoets.Piping.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingInputParametersContextSurfaceLineSelectionEditorTest
    {
        [Test]
        public void EditValue_NoCurrentItemInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var inputParametersContext = new PipingInputParametersContext
            {
                AvailablePipingSurfaceLines = new[] { new RingtoetsPipingSurfaceLine() },
                WrappedPipingInputParameters = new PipingInputParameters
                {
                    SurfaceLine = new RingtoetsPipingSurfaceLine()
                }
            };

            var properties = new PipingInputParametersContextProperties
            {
                Data = inputParametersContext
            };

            var editor = new PipingInputParametersContextSurfaceLineSelectionEditor();
            var someValue = new object();
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();
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
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var inputParametersContext = new PipingInputParametersContext
            {
                AvailablePipingSurfaceLines = new[] { surfaceLine },
                WrappedPipingInputParameters = new PipingInputParameters
                {
                    SurfaceLine = surfaceLine
                }
            };

            var properties = new PipingInputParametersContextProperties
            {
                Data = inputParametersContext
            };

            var editor = new PipingInputParametersContextSurfaceLineSelectionEditor();
            var someValue = new object();
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();
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
    }
}