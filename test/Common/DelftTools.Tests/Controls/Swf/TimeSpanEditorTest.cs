using DelftTools.Controls.Swf;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    [TestFixture]
    [Category(TestCategory.WindowsForms)]
    public class TimeSpanEditorTest
    {
        [Test]
        public void Show()
        {
            WindowsFormsTestHelper.ShowModal(new TimeSpanEditor());
        }

        [Test]
        public void ShowWithOnlyDays()
        {
            WindowsFormsTestHelper.ShowModal(new TimeSpanEditor {IncludeDays = true});
        }

        [Test]
        public void ShowFull()
        {
            WindowsFormsTestHelper.ShowModal(new TimeSpanEditor
                {
                    IncludeDays = true,
                    IncludeTensOfSeconds = true
                });
        }
    }
}