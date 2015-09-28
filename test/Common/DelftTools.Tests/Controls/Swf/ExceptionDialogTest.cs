using System;
using DelftTools.Controls.Swf;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    [TestFixture]
    public class ExceptionDialogTest
    {
        Exception exception;

        [SetUp]
        public void SetUp()
        {
            try
            {
                throw new DivideByZeroException("Divide by zero occured.");
            }
            catch (DivideByZeroException e)
            {
                exception = e;
            }
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void Show()
        {
            var dialog = new ExceptionDialog(exception);
            
            WindowsFormsTestHelper.ShowModal(dialog);
        }
    }
}