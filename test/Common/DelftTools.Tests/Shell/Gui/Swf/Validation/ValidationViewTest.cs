using System.Threading;
using DelftTools.Shell.Gui.Swf.Validation;
using DelftTools.TestUtils;
using DelftTools.Utils.Validation;
using NUnit.Framework;

namespace DelftTools.Tests.Shell.Gui.Swf.Validation
{
    [TestFixture]
    [Category(TestCategory.WindowsForms)]
    public class ValidationViewTest
    {
        [Test]
        public void ShowFast()
        {
            int count = 0;
            var view = new ValidationView
            {
                OnValidate = d =>
                {
                    return new ValidationReport((count++).ToString(), new ValidationIssue[0]);
                },
                Data = new object()
            };
            WindowsFormsTestHelper.ShowModal(view);
        }

        [Test]
        public void ShowMedium()
        {
            int count = 0;
            var view = new ValidationView
            {
                OnValidate = d =>
                {
                    Thread.Sleep(498);
                    return new ValidationReport((count++).ToString(), new ValidationIssue[0]);
                },
                Data = new object()
            };
            WindowsFormsTestHelper.ShowModal(view);
        }

        [Test]
        public void ShowSlow()
        {
            int count = 0;
            var view = new ValidationView
                {
                    OnValidate = d =>
                        {
                            Thread.Sleep(600);
                            return new ValidationReport((count++).ToString(), new ValidationIssue[0]);
                        },
                    Data = new object()
                };
            WindowsFormsTestHelper.ShowModal(view);
        }
    }
}