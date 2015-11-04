using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Core.Common.Utils.PropertyBag.Dynamic;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.UITypeEditors;

namespace Ringtoets.Piping.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingCalculationInputSoilProfileSelectionEditorTest
    {
        [Test]
        public void EditValue_NoCurrentItemInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var pipingCalculationInputs = new PipingCalculationInputs
            {
                AvailablePipingSoilProfiles = new[] { new TestPipingSoilProfile()  },
                PipingData = new PipingData
                {
                    SoilProfile = new TestPipingSoilProfile()
                }
            };

            var properties = new PipingCalculationInputsProperties
            {
                Data = pipingCalculationInputs
            };

            var editor = new PipingCalculationInputsSoilProfileSelectionEditor();
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
            var soilProfile = new TestPipingSoilProfile();
            var pipingCalculationInputs = new PipingCalculationInputs
            {
                AvailablePipingSoilProfiles = new[] { soilProfile },
                PipingData = new PipingData
                {
                    SoilProfile = soilProfile
                }
            };

            var properties = new PipingCalculationInputsProperties
            {
                Data = pipingCalculationInputs
            };

            var editor = new PipingCalculationInputsSoilProfileSelectionEditor();
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
            Assert.AreSame(soilProfile, result);

            mockRepository.VerifyAll();
        }
    }
}