using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Forms.UITypeEditors;

namespace Ringtoets.Piping.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingCalculationInputSelectionEditorTest
    {
        [Test]
        public void GetEditStyle_Always_ReturnDropDown()
        {
            // Setup
            var editor = new PipingCalculationInputSelectionEditor<object>();

            // Call
            var editStyle = editor.GetEditStyle();

            // Assert
            Assert.AreEqual(UITypeEditorEditStyle.DropDown, editStyle);
        }

        [Test]
        public void EditValue_NoProviderNoContext_ReturnsOriginalValue()
        {
            // Setup
            var editor = new PipingCalculationInputSelectionEditor<object>();
            var someValue = new object();

            // Call
            var result = editor.EditValue(null, null, someValue);

            // Assert
            Assert.AreSame(someValue, result);
        }

        [Test]
        public void EditValue_NoContext_ReturnsOriginalValue()
        {
            // Setup
            var editor = new PipingCalculationInputSelectionEditor<object>();
            var someValue = new object();
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);

            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(null, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_Always_ReturnsOriginalValue()
        {
            // Setup
            var editor = new PipingCalculationInputSelectionEditor<object>();
            var someValue = new object();
            var mockRepository = new MockRepository();
            var provider = mockRepository.DynamicMock<IServiceProvider>();
            var service = mockRepository.DynamicMock<IWindowsFormsEditorService>();
            var context = mockRepository.DynamicMock<ITypeDescriptorContext>();

            provider.Expect(p => p.GetService(null)).IgnoreArguments().Return(service);
            service.Expect(s => s.DropDownControl(null)).IgnoreArguments();

            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(context, provider, someValue);

            // Assert
            Assert.AreSame(someValue, result);

            mockRepository.VerifyAll();
        }
    }
}